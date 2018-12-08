using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
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
            { Entities.CommandNames.createadmin, new Entities.CommandDescription("Create a new admin", Entities.UserRole.Admin, "[name] [password]") },
            { Entities.CommandNames.createmanager, new Entities.CommandDescription("Create a new fleet manager", Entities.UserRole.Admin, "[name] [password] [fleetname, ...]") },
            { Entities.CommandNames.deletemanager, new Entities.CommandDescription("Delete a fleet manager", Entities.UserRole.Admin, "[name]") },
            { Entities.CommandNames.readadmin, new Entities.CommandDescription("Read data of a admin", Entities.UserRole.Admin, "[name]") },
            { Entities.CommandNames.readmanager, new Entities.CommandDescription("Read data of a fleet manager", Entities.UserRole.Admin, "[name]") },
            { Entities.CommandNames.updatemanager, new Entities.CommandDescription("Update the password of a fleet manager", Entities.UserRole.Admin, "[name] [your password] [new password]") },
            { Entities.CommandNames.updateprofile, new Entities.CommandDescription("Update the password of your own profile", Entities.UserRole.User, "[name] [your password] [new password]") },
            { Entities.CommandNames.createuser, new Entities.CommandDescription("Create a new profile for a customer", Entities.UserRole.Manager, "[name] [password]") },
            { Entities.CommandNames.readuser, new Entities.CommandDescription("Read data of a user", Entities.UserRole.Manager, "[name]") },
            { Entities.CommandNames.updateuser, new Entities.CommandDescription("Update the password of a user", Entities.UserRole.Manager, "[name] [your password] [new password]") }
        };

        // in der Main soll die ganze Menünavigation laufen. Ob mit tasten oder mit befehlen ist mir egal...
        static void Main(string[] args)
        {
            // hab das über Xml.Linq gemacht
            // kam mir einfacher vor
            // sollte das über XmlDocument einfacher sein, einfach abändern
            // so kann ich aber noch weiter arbeiten

            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml(new StreamReader(PROFILES_FILENAME).ReadToEnd());

            var xmlDoc = XDocument.Load("Profiles.xml");

            Database db = Database.buildFromXMl(xmlDoc);

            while (true)
            {
                Console.WriteLine("Welcome to the EvilCar console.");
                Console.WriteLine("You need to authenticate yourself to get access.");
                Console.WriteLine("Please enter your username...");

                string username = Console.ReadLine();

                // müssen wir den usernamen vorher wirklich beim einloggen überprüfen??
                // wird ja bei CheckCredentials eh nochmal gemacht
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
                        Console.WriteLine("\nEnter the command you want to execute. If you don't know them, enter help");

                        //TODO Commands Zeug
                        // TODO: die einzelnen case zweige in jeweils eine eigene funktion auslagern??
                        string[] command_args = Console.ReadLine().Split(' ');
                        if(command_args.Length > 0)
                        {
                            switch (command_args[0].ToLower())
                            {
                                // help
                                case nameof(Entities.CommandNames.help):
                                    Console.WriteLine("\n**********");
                                    foreach(var key in Commands.Keys)
                                    {
                                        if (profile.role == Commands[key].role)
                                        {
                                            Console.WriteLine($"\n{key} {Commands[key].arguments}\n{Commands[key].description}");
                                        }
                                    }
                                    Console.WriteLine("\n**********");
                                    break;
                                // quit
                                case nameof(Entities.CommandNames.quit):
                                    db.safe("Profiles.xml");
                                    return;
                                // create admin
                                case nameof(Entities.CommandNames.createadmin):
                                    if(CheckCommandAccessibility(profile, Entities.CommandNames.createadmin) && CheckCommandArguments(command_args, 2))
                                    {
                                        if (db.CreateUser(command_args[1], command_args[2], Entities.UserRole.Admin))
                                            Console.WriteLine($"Your created \"{command_args[1]}\"\n");
                                        else
                                            Console.WriteLine($"Cannot create \"{command_args[1]}\"\n");
                                    }
                                    break;
                                // create manager
                                case nameof(Entities.CommandNames.createmanager):
                                    if(CheckCommandAccessibility(profile, Entities.CommandNames.createmanager) && CheckCommandArguments(command_args, 3))
                                    {
                                        if(db.CreateUser(command_args[1], command_args[2], Entities.UserRole.Manager))
                                            Console.WriteLine($"Your created \"{command_args[1]}\"\n");
                                        else
                                            Console.WriteLine($"Cannot create \"{command_args[1]}\"\n");
                                    }
                                    break;
                                // delete manager
                                case nameof(Entities.CommandNames.deletemanager):
                                    if(CheckCommandAccessibility(profile, Entities.CommandNames.deletemanager) && CheckCommandArguments(command_args, 1))
                                    {
                                        if (db.RemoveUser(command_args[1]))
                                            Console.WriteLine($"You removed \"{command_args[1]}\"\n");
                                        else
                                            Console.WriteLine($"Cannot remove \"{command_args[1]}\"\n");
                                    }
                                    break;
                                //
                                default:
                                    Console.WriteLine("There is no such command!");
                                    break;
                            }
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
            if (!(profile.role == Commands[commandName].role))
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
