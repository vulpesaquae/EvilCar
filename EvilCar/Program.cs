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
            { Entities.CommandNames.createbranch, new Entities.CommandDescription("Create a new branch", Entities.UserRole.Admin) },
            { Entities.CommandNames.listfleets, new Entities.CommandDescription("List all of your fleets", Entities.UserRole.Manager, "[fleetname]") },
            { Entities.CommandNames.createfleet, new Entities.CommandDescription("Create a new branch", Entities.UserRole.Manager, "[fleetname] [branchname]") },
            { Entities.CommandNames.deletefleet, new Entities.CommandDescription("Delete a fleet and remove from the branch", Entities.UserRole.Manager, "[name] [branchname]") },
            { Entities.CommandNames.addcar, new Entities.CommandDescription("Add a car to a fleet", Entities.UserRole.Manager, "[carname] [fleetname] [branchname] [isLimo = true/false]" )},
            { Entities.CommandNames.deletecar, new Entities.CommandDescription("Delete a car from a fleet", Entities.UserRole.Manager, "[carname] [fleetname] [branchname]" )},
            { Entities.CommandNames.listcars, new Entities.CommandDescription("List all cars of a fleet", Entities.UserRole.Manager, "[branchname] [fleetname]" )},
            { Entities.CommandNames.calculatecosts, new Entities.CommandDescription("Calculate the costs for a car. You can book spotify, navigation, parker, massage (limo only)", Entities.UserRole.Manager, "[carname] [branchname] [fleetname] [hours] [services, ...]")}
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
                    Console.WriteLine($"\nHello {profile.Name}! You are now logged in.");

                    //the main menue
                    while (true)
                    {
                        Console.WriteLine("Enter the command you want to execute. If you don't know them, enter help");

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
                                        if (profile.Role == Commands[key].role || Commands[key].role == Entities.UserRole.Undefined)
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
                                        Console.WriteLine(user.Name);
                                    }
                                    break;
                                // read admin
                                case nameof(Entities.CommandNames.readadmin):
                                    if(CheckCommandAccessibility(profile, Entities.CommandNames.readadmin) && CheckCommandArguments(command_args, 1))
                                    {
                                        var user = db.getUser(command_args[1]);
                                        if(user != null && user.Role == Entities.UserRole.Admin)
                                            Console.WriteLine($"Name: {user.Name}");
                                        else
                                            Console.WriteLine($"User \"{user.Name}\" does not exist");
                                    }
                                    break;
                                // list manager
                                case nameof(Entities.CommandNames.listmanagers):
                                    foreach (var user in db.GetUsersFromRole(Entities.UserRole.Manager))
                                    {
                                        Console.WriteLine(user.Name);
                                    }
                                    break;
                                // read manager
                                case nameof(Entities.CommandNames.readmanager):
                                    if (CheckCommandAccessibility(profile, Entities.CommandNames.readmanager) && CheckCommandArguments(command_args, 1))
                                    {
                                        var user = db.getUser(command_args[1]);
                                        if (user != null && user.Role == Entities.UserRole.Manager)
                                            Console.WriteLine($"Name: {user.Name}");
                                        else
                                            Console.WriteLine($"User \"{user.Name}\" does not exist");
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
                                        if (db.getUser(command_args[1]).Role == Entities.UserRole.Manager && command_args[2] == command_args[3] && db.UpdateUser(command_args[1], command_args[2]))
                                            Console.WriteLine($"Successfully changed the password of \"{command_args[1]}\". He will be informed.");
                                        else
                                            Console.WriteLine($"Cannot change password of \"{command_args[1]}\"");
                                    }
                                    break;
                                // update profile
                                case nameof(Entities.CommandNames.updateprofile):
                                    if (CheckCommandAccessibility(profile, Entities.CommandNames.updatemanager) && CheckCommandArguments(command_args, 2))
                                    {
                                        if (command_args[1] == command_args[2] && db.UpdateUser(profile.Name, command_args[1]))
                                            Console.WriteLine($"Successfully changed the password of \"{command_args[1]}\". He will be informed.");
                                        else
                                            Console.WriteLine($"Cannot change password of \"{profile.Name}\"");
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
                                // list fleets
                                case nameof(Entities.CommandNames.listfleets):
                                    if (CheckCommandAccessibility(profile, Entities.CommandNames.listfleets))
                                    {
                                        foreach(var fleet in db.GetFleets(profile.Name))
                                        {
                                            Console.WriteLine($"Name: {fleet.Name}, Branch: {fleet.Branch}");
                                        }
                                    }
                                    break;
                                // create fleet
                                case nameof(Entities.CommandNames.createfleet):
                                    if (CheckCommandAccessibility(profile, Entities.CommandNames.createfleet) && CheckCommandArguments(command_args, 2))
                                    {
                                        if (db.CreateFleet(command_args[2], command_args[1], profile.Name))
                                            Console.WriteLine($"Created fleet {command_args[1]}");
                                        else
                                            Console.WriteLine($"Cannot create fleet {command_args[1]}");
                                    }
                                    break;
                                // delete fleet
                                case nameof(Entities.CommandNames.deletefleet):
                                    if (CheckCommandAccessibility(profile, Entities.CommandNames.deletefleet) && CheckCommandArguments(command_args, 2))
                                    {
                                        if (db.DeleteFleet(command_args[2], command_args[1], profile.Name))
                                            Console.WriteLine($"Deleted fleet {command_args[1]}");
                                        else
                                            Console.WriteLine($"Cannot delete fleet {command_args[1]}");
                                    }
                                    break;
                                // add a car to a fleet
                                case nameof(Entities.CommandNames.addcar):
                                    if (CheckCommandAccessibility(profile, Entities.CommandNames.addcar) && CheckCommandArguments(command_args, 4))
                                    {
                                        bool isLimo = false;
                                        if (command_args[4].ToLower() == "1" || command_args[4].ToLower() == "true")
                                            isLimo = true;

                                        Car newCar = new Car(command_args[1], isLimo);
                                        if(db.AddCarToFleet(profile.Name, command_args[3], command_args[2], newCar))
                                            Console.WriteLine($"Added the car \"{newCar.Name}\" to the fleet \"{command_args[2]}\"");
                                        else
                                            Console.WriteLine($"Cannot add car \"{newCar.Name}\" to the fleet \"{command_args[2]}\"");
                                    }
                                    break;
                                // delete a car from a fleet
                                case nameof(Entities.CommandNames.deletecar):
                                    if (CheckCommandAccessibility(profile, Entities.CommandNames.deletecar) && CheckCommandArguments(command_args, 2))
                                    {
                                        if (db.DeleteCarFromFleet(profile.Name, command_args[3], command_args[2], command_args[1]))
                                            Console.WriteLine($"The Car \"{command_args[1]}\" was successfully deleted from the fleet \"{command_args[2]}\"");
                                        else
                                            Console.WriteLine($"Cannot delete \"{command_args[1]}\"");
                                    }
                                    break;
                                // list all the cars from a fleet
                                case nameof(Entities.CommandNames.listcars):
                                    if(CheckCommandAccessibility(profile, Entities.CommandNames.listcars) && CheckCommandArguments(command_args, 2))
                                    {
                                        IEnumerable<Car> cars = db.GetCarsFromFleet(profile.Name, command_args[1], command_args[2]);
                                        if(cars != null)
                                        {
                                            foreach(Car c in cars)
                                            {
                                                if (c.IsLimo)
                                                {
                                                    if (c.IsBooked)
                                                        Console.WriteLine($"limousine {c.Name}, booked");
                                                    else
                                                        Console.WriteLine($"limousine {c.Name}, free");
                                                }
                                                else
                                                {
                                                    if (c.IsBooked)
                                                        Console.WriteLine($"Car {c.Name}, booked");
                                                    else
                                                        Console.WriteLine($"Car {c.Name}, free");
                                                }
                                            }
                                            if(cars.Count() == 0)
                                            {
                                                Console.WriteLine($"There are no cars in the fleet \"{command_args[2]}\"");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine($"there went something wrong with your fleet- or branchname");
                                        }
                                    }
                                    break;
                                //calculate the costs of a car rent
                                case nameof(Entities.CommandNames.calculatecosts):
                                    if(CheckCommandAccessibility(profile, Entities.CommandNames.calculatecosts) && CheckCommandArguments(command_args, 5))
                                    {
                                        string carname = command_args[1];
                                        string branchname = command_args[2];
                                        string fleetname = command_args[3];
                                        float hours = 0;
                                        try
                                        {
                                            hours = float.Parse(command_args[4]);
                                        }
                                        catch (FormatException e)
                                        {
                                            Console.WriteLine($"{command_args[4]} is not a number...");
                                            continue;
                                        }
                                        bool parker = false, spotify = false, massage = false, navigation = false;
                                        for(int i = 5; i<command_args.Length; i++)
                                        {
                                            switch (command_args[i])
                                            {
                                                case "parker": parker = true; break;
                                                case "spotify": spotify = true; break;
                                                case "massage": massage = true; break;
                                                case "navigation": navigation = true; break;
                                            }
                                        }
                                        Car car = db.GetCar(profile.Name, branchname, fleetname, carname);
                                        if(car != null)
                                        {
                                            float costs = CalculateCost(car, hours, spotify, massage, parker, navigation);
                                            if(!car.IsLimo)
                                                Console.Write($"You want to book {carname}\nwith");
                                            else
                                                Console.Write($"You want to book limousine {carname} for {hours}h\nwith ");

                                            bool isFirst = true;
                                            if (parker)
                                            {
                                                Console.Write("parker");
                                                isFirst = false;
                                            }
                                            if (spotify)
                                            {
                                                if (!isFirst)
                                                    Console.Write(", ");
                                                Console.Write("spotify");
                                                isFirst = false;
                                            }
                                            if (massage && car.IsLimo)
                                            {
                                                if (!isFirst)
                                                    Console.Write(", ");
                                                Console.Write("massage");
                                                isFirst = false;
                                            }
                                            if (navigation)
                                            {
                                                if (!isFirst)
                                                    Console.Write(", ");
                                                Console.Write("navigation");
                                                isFirst = false;
                                            }
                                            Console.Write("\n");
                                            Console.WriteLine($"That would cost {costs} EUROs");
                                        }
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
            if (!(profile.Role == Commands[commandName].role) || Commands[commandName].role == Entities.UserRole.Undefined)
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

        public static float CalculateCost(Car car, float hours, bool spotify, bool massage, bool parker, bool navigation)
        {
            float perHour = 9.95F;

            if (spotify)
                perHour += 2;
            if (parker)
                perHour += 2;
            if (navigation)
                perHour += 4;
            if (car.IsLimo)
            {
                perHour += 5;
                if (massage)
                    perHour += 5;
            }
            return hours * perHour;
        }
    }
}
