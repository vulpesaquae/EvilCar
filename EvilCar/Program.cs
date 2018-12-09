using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace EvilCar
{
    class Program
    {
        public const string PROFILES_FILENAME = "Profiles.xml";

        // so können wir alle commands bei help relativ einfach anzeigen lassen
        // auf die rollen können auch einfach überprüft werden
        private static Dictionary<Entities.CommandNames, Entities.CommandDescription> Commands = new Dictionary<Entities.CommandNames, Entities.CommandDescription>()
        {
            { Entities.CommandNames.quit, new Entities.CommandDescription("Close the program", Entities.UserRole.User)},
            { Entities.CommandNames.listadmins, new Entities.CommandDescription("List all existing admin's", Entities.UserRole.Admin) },
            { Entities.CommandNames.readadmin, new Entities.CommandDescription("Read data of a admin", Entities.UserRole.Admin, "[name]") },
            { Entities.CommandNames.listmanagers, new Entities.CommandDescription("List all existing fleet manager's", Entities.UserRole.Admin) },
            { Entities.CommandNames.readmanager, new Entities.CommandDescription("Read data of a fleet manager", Entities.UserRole.Admin,"[name]") },
            { Entities.CommandNames.createadmin, new Entities.CommandDescription("Create a new admin", Entities.UserRole.Admin, "[name] [password]") },
            { Entities.CommandNames.createmanager, new Entities.CommandDescription("Create a new fleet manager", Entities.UserRole.Admin, "[name] [password] [fleetname, ...]") },
            { Entities.CommandNames.deletemanager, new Entities.CommandDescription("Delete a fleet manager", Entities.UserRole.Admin, "[name]") },
            { Entities.CommandNames.updatemanager, new Entities.CommandDescription("Update the password of a fleet manager", Entities.UserRole.Admin, "[name] [new password] [repeat password]") },
            { Entities.CommandNames.updateprofile, new Entities.CommandDescription("Update the password of your own profile", Entities.UserRole.Undefined, "[new password] [repeat password]") },
            { Entities.CommandNames.readuser, new Entities.CommandDescription("Read data of a user", Entities.UserRole.Manager, "[name]") },
            { Entities.CommandNames.createuser, new Entities.CommandDescription("Create a new profile for a customer", Entities.UserRole.Manager, "[name] [password]") },
            { Entities.CommandNames.updateuser, new Entities.CommandDescription("Update the password of a user", Entities.UserRole.Manager, "[name] [new password] [repeat password]") },
            { Entities.CommandNames.createbranch, new Entities.CommandDescription("Create a new branch", Entities.UserRole.Admin, "[name]") },
            { Entities.CommandNames.createfleet, new Entities.CommandDescription("Create a new branch", Entities.UserRole.Manager, "[fleetname] [branchname]") },
            { Entities.CommandNames.deletefleet, new Entities.CommandDescription("Delete a fleet and remove from the branch", Entities.UserRole.Manager, "[name] [branchname]") }
        };

        static void Main(string[] args)
        {
            var xmlDoc = XDocument.Load("Profiles.xml");

            Database db = Database.buildFromXMl(xmlDoc);

            while (true)
            {
                Console.WriteLine("Welcome to the EvilCar console.");
                Console.WriteLine("You need to authenticate yourself to get access.");
                Console.WriteLine("Please enter your username...");

                string username = Console.ReadLine();

                if (!db.checkUsername(username))
                {
                    Console.WriteLine("There is no such user...");
                    continue;
                }
                else
                {
                    Console.WriteLine("Please insert your password...");
                }
                string password = Console.ReadLine();

                // Login
                if (db.checkCredentials(username, password))
                {
                    User profile = db.getUser(username);
                    Console.WriteLine($"\nHello {profile.name}! You are now logged in.");

                    //the main menue
                    while (true)
                    {
                        Console.WriteLine("Enter the command you want to execute. If you don't know them, enter help");

                        //TODO Commands Zeug
                        // TODO: die einzelnen case zweige in jeweils eine eigene funktion auslagern??
                        string[] command_args = Console.ReadLine().Split(' ');
                        if(command_args.Length > 0)
                        {
                            Console.WriteLine("\n**********");
                            switch (command_args[0].ToLower())
                            {
                                // help
                                case nameof(Entities.CommandNames.help):
                                    foreach(var key in Commands.Keys)
                                    {
                                        if (profile.role == Commands[key].role || Commands[key].role == Entities.UserRole.Undefined)
                                        {
                                            Console.WriteLine($"\n{key} {Commands[key].arguments}\n{Commands[key].description}");
                                        }
                                    }
                                    break;
                                // quit
                                case nameof(Entities.CommandNames.quit):
                                    db.safe("Profiles.xml");
                                    return;
                                // list admins
                                case nameof(Entities.CommandNames.listadmins):
                                    foreach(var user in db.GetUsersFromRole(Entities.UserRole.Admin))
                                    {
                                        Console.WriteLine(user.name);
                                    }
                                    break;
                                // read admin
                                case nameof(Entities.CommandNames.readadmin):
                                    if(CheckCommandAccessibility(profile, Entities.CommandNames.readadmin) && CheckCommandArguments(command_args, 1))
                                    {
                                        var user = db.getUser(command_args[1]);
                                        if(user != null && user.role == Entities.UserRole.Admin)
                                            Console.WriteLine($"Name: {user.name}");
                                        else
                                            Console.WriteLine($"User \"{user.name}\" does not exist");
                                    }
                                    break;
                                // list manager
                                case nameof(Entities.CommandNames.listmanagers):
                                    foreach (var user in db.GetUsersFromRole(Entities.UserRole.Manager))
                                    {
                                        Console.WriteLine(user.name);
                                    }
                                    break;
                                // read manager
                                case nameof(Entities.CommandNames.readmanager):
                                    if (CheckCommandAccessibility(profile, Entities.CommandNames.readmanager) && CheckCommandArguments(command_args, 1))
                                    {
                                        var user = db.getUser(command_args[1]);
                                        if (user != null && user.role == Entities.UserRole.Manager)
                                            Console.WriteLine($"Name: {user.name}");
                                        else
                                            Console.WriteLine($"User \"{user.name}\" does not exist");
                                    }
                                    break;
                                // create admin
                                case nameof(Entities.CommandNames.createadmin):
                                    if(CheckCommandAccessibility(profile, Entities.CommandNames.createadmin) && CheckCommandArguments(command_args, 2))
                                    {
                                        if (db.CreateUser(command_args[1], command_args[2], Entities.UserRole.Admin))
                                            Console.WriteLine($"Your created \"{command_args[1]}\"");
                                        else
                                            Console.WriteLine($"Cannot create \"{command_args[1]}\"");
                                    }
                                    break;
                                // create manager
                                case nameof(Entities.CommandNames.createmanager):
                                    if(CheckCommandAccessibility(profile, Entities.CommandNames.createmanager) && CheckCommandArguments(command_args, 3))
                                    {
                                        if(db.CreateUser(command_args[1], command_args[2], Entities.UserRole.Manager))
                                            Console.WriteLine($"Your created \"{command_args[1]}\"");
                                        else
                                            Console.WriteLine($"Cannot create \"{command_args[1]}\"");
                                    }
                                    break;
                                // delete manager
                                case nameof(Entities.CommandNames.deletemanager):
                                    if(CheckCommandAccessibility(profile, Entities.CommandNames.deletemanager) && CheckCommandArguments(command_args, 1))
                                    {
                                        if (db.RemoveUser(command_args[1]))
                                            Console.WriteLine($"You removed \"{command_args[1]}\"");
                                        else
                                            Console.WriteLine($"Cannot remove \"{command_args[1]}\"");
                                    }
                                    break;
                                // update manager
                                case nameof(Entities.CommandNames.updatemanager):
                                    if(CheckCommandAccessibility(profile, Entities.CommandNames.updatemanager) && CheckCommandArguments(command_args, 3))
                                    {
                                        if (db.getUser(command_args[1]).role == Entities.UserRole.Manager && command_args[2] == command_args[3] && db.UpdateUser(command_args[1], command_args[2]))
                                            Console.WriteLine($"Successfully changed the password of \"{command_args[1]}\". He will be informed.");
                                        else
                                            Console.WriteLine($"Cannot change password of \"{command_args[1]}\"");
                                    }
                                    break;
                                // update profile
                                case nameof(Entities.CommandNames.updateprofile):
                                    if (CheckCommandAccessibility(profile, Entities.CommandNames.updatemanager) && CheckCommandArguments(command_args, 2))
                                    {
                                        if (command_args[1] == command_args[2] && db.UpdateUser(profile.name, command_args[1]))
                                            Console.WriteLine($"Successfully changed the password of \"{command_args[1]}\". He will be informed.");
                                        else
                                            Console.WriteLine($"Cannot change password of \"{profile.name}\"");
                                    }
                                    break;
                                // create branch
                                case nameof(Entities.CommandNames.createbranch):
                                    if(CheckCommandAccessibility(profile, Entities.CommandNames.createbranch) && CheckCommandArguments(command_args, 1))
                                    {
                                        if (db.CreateBranch(command_args[1]))
                                            Console.WriteLine($"Created branch \"{command_args[1]}\"");
                                        else
                                            Console.WriteLine($"Cannot create branch \"{command_args[1]}\"");
                                    }
                                    break;
                                // create fleet
                                case nameof(Entities.CommandNames.createfleet):
                                    if (CheckCommandAccessibility(profile, Entities.CommandNames.createfleet) && CheckCommandArguments(command_args, 2))
                                    {
                                        if (db.CreateFleet(command_args[2], command_args[1], profile.name))
                                            Console.WriteLine($"Created fleet {command_args[1]}");
                                        else
                                            Console.WriteLine($"Cannot create fleet {command_args[1]}");
                                    }
                                    break;
                                // delete fleet
                                case nameof(Entities.CommandNames.deletefleet):
                                    if (CheckCommandAccessibility(profile, Entities.CommandNames.deletefleet) && CheckCommandArguments(command_args, 2))
                                    {
                                        if (db.DeleteFleet(command_args[2], command_args[1], profile.name))
                                            Console.WriteLine($"Deleted fleet {command_args[1]}");
                                        else
                                            Console.WriteLine($"Cannot delete fleet {command_args[1]}");
                                    }
                                    break;
                                //
                                default:
                                    Console.WriteLine("There is no such command!");
                                    break;
                            }
                            Console.WriteLine("**********\n");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("There was an error with your credentials. Please try again.\n");
                }
            }
        }

        #region Check Command

        // check if the user is allowed to execute the command
        private static bool CheckCommandAccessibility(User profile, Entities.CommandNames commandName)
        {
            if (!(profile.role == Commands[commandName].role) || Commands[commandName].role == Entities.UserRole.Undefined)
            {
                Console.WriteLine("You have not the rights to execute this command.");
                return false;
            }
            return true;
        }

        // check if all arguments are valid
        // valid means, there is any character
        private static bool CheckCommandArguments(string[] commandArgs, int numberArguments)
        {
            // numberArguments + 1, because the first index is the command itself
            // all additional arguments can be ignored
            if (commandArgs.Length >= numberArguments+1)
            {
                for (int i = 0; i < numberArguments+1; i++)
                {
                    // .Any to check if there is any character --> equal to .Length > 0 or .Count() > 0
                    // but shorter and (maybe) faster, because it return true for the first element
                    // the other two option count all elements
                    if (!commandArgs[i].Any())
                    {
                        Console.WriteLine("Some of the arguments are missing");
                        return false;
                    }
                }
            }
            else
            {
                Console.WriteLine("Some of the arguments are missing");
                return false;
            }
            return true;
        }

        #endregion Check Command

    }
}
