﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace EvilCar
{
    class Admin : UserType
    {
        public Admin(string name) : base(name) {}

        /// <summary>
        /// Print all options to the user
        /// Start the choosen function
        /// </summary>
        public void AdminConsole()
        {
            ConsoleKeyInfo inputKey;
            do
            {
                // Bin noch am überlegen ob es nicht besser ist, einfach von A-Z bei der Nummerierung zu gehen
                // Hab versucht dem soweit eine gewisse Logik zu geben, funktioniert aber nicht überall
                // Bei Zahlen haben wir nur 10 Möglichkeiten, ansonsten muss die eingabe noch geparsed, sollten also bei Buchstaben bleiben
                // Andererseits könnte man das auch über Commands machen
                    // also "createadmin", "readadmin" usw. um die Funktionen aufzurufen
                Console.WriteLine("\nPlease choose one of the options: ");
                Console.WriteLine("\t<Q> Go back to login screen");
                Console.WriteLine($"\t<A> Create a new \"{UserRole.Admin}\"");
                Console.WriteLine($"\t<R> Read a \"{UserRole.Admin}\"");
                Console.WriteLine($"\t<M> Create a new \"{UserRole.Manager}\"");
                Console.WriteLine($"\t<L> Read a \"{UserRole.Manager}\"");
                Console.WriteLine($"\t<D> Delete a \"{UserRole.Manager}\"");
                Console.WriteLine($"\t<F> Update the password of a \"{UserRole.Manager}\"");
                Console.WriteLine($"\t<U> Update your profile");
                Console.WriteLine($"\t<B> Create a new Branch");

                inputKey = Console.ReadKey(false);

                switch (inputKey.Key)
                {
                    case ConsoleKey.A: Admin_Create(); break;
                    case ConsoleKey.R: Admin_Read(); break;
                    case ConsoleKey.M: FleetManager_Create(); break;
                    case ConsoleKey.L: FleetManager_Read(); break;
                    case ConsoleKey.D: FleetManager_Delete(); break;
                    case ConsoleKey.F: FleetManager_UpdatePassword(); break;
                    case ConsoleKey.U: UpdateProfile(); break;
                    case ConsoleKey.B: Branch_Create(); break;
                }
            } while (inputKey.Key != ConsoleKey.Q);


        }

        // Create a profile with a specified user role
        // List the names of this profiles
        private void Profile_Create(UserRole role)
        {
            Console.WriteLine($"\n\nCreate a new {role}!");

            string username, password;
            Program.InsertCredentials(out username, out password);

            Console.Write($"\nCreate new {role} \"{username}\"? yes - <y>, no - <n>: ");
            if (Console.ReadKey(false).Key == ConsoleKey.Y)
            {
                XDocument xmlDoc = XDocument.Load(Program.PROFILES_FILENAME);

                // check if name already exists
                if (xmlDoc.Descendants("Profile").SingleOrDefault(x => x.Element("Name").Value.Equals(username)) == null)
                {
                    // create the node for the admin
                    var newNode = new XElement("Profile");
                    newNode.Add(new XElement("Name", username));
                    newNode.Add(new XElement("Password", Program.Base64Encode(password)));
                    newNode.Add(new XElement("Role", role));

                    // add the node to the main node
                    xmlDoc.Element("Profiles").Add(newNode);
                    xmlDoc.Save(Program.PROFILES_FILENAME);

                    Console.WriteLine($"\nNew {role} \"{username}\" was created.");
                }
                else
                {
                    Console.WriteLine("\nThe user already exists.");
                }
            }
            else
            {
                Console.WriteLine($"\nCanceled to create new {role}.");
            }
        }

        // Read a profile with a specified user role
        private void Profile_Read(UserRole role)
        {
            var xmldoc = XDocument.Load(Program.PROFILES_FILENAME);
            var profiles = xmldoc.Descendants("Profile");

            Console.WriteLine($"\nThese are the current {role.ToString()}'s:");

            foreach (var profile in profiles.Where(x => x.Element("Role").Value.Equals(role.ToString())))
            {
                Console.WriteLine($"\t{profile.Element("Name").Value}");
            }
        }

        // Create a new admin
        private void Admin_Create() => Profile_Create(UserRole.Admin);

        // Read the admin
        private void Admin_Read() => Profile_Read(UserRole.Admin);

        // Create a new manager
        private void FleetManager_Create() => Profile_Create(UserRole.Manager);

        // Delete fleet manager only, if there are more than one fleet manager for a branch
        private void FleetManager_Delete()
        {
            var xmlDoc = XDocument.Load(Program.PROFILES_FILENAME);

            if (xmlDoc.Descendants("Role").Count(x => x.Value.Equals(UserRole.Manager.ToString())) > 1)
            {
                Console.WriteLine($"\nWhich {UserRole.Manager} do you want to delete?");
                FleetManager_Read();

                Console.Write("Username: ");
                var username = Console.ReadLine();

                var profile = Program.ProfilesXml_GetUser(xmlDoc, username);
                if (profile != null)
                {
                    Console.WriteLine($"Are you sure you want to delete \"{username}\" as {UserRole.Manager}? <y> - yes, <n> - no: ");

                    if (Console.ReadKey(false).Key == ConsoleKey.Y)
                    {
                        profile.Remove();
                        xmlDoc.Save(Program.PROFILES_FILENAME);

                        Console.WriteLine($"\nDeleted \"{username}\"");
                    }
                    else
                    {
                        Console.WriteLine($"Canceled to delete \"{username}\"");
                    }
                }
                else
                {
                    Console.WriteLine($"\nCannot find the {UserRole.Manager} \"{username}\"");
                }
            }
            else
            {
                Console.WriteLine($"\nThere is only one {UserRole.Manager}.");
            }
        }

        private void FleetManager_Read() => Profile_Read(UserRole.Manager);

        // An asynchronous e-mail with the new password will be sent
        // E-mail will be mocked by an asynchronous Console.WriteLine task that lasts about 5-10 seconds
        private void FleetManager_UpdatePassword()
        {
            throw new NotImplementedException();
        }

        private void UpdateProfile()
        {
            throw new NotImplementedException();
        }

        private void Branch_Create()
        {
            throw new NotImplementedException();
        }
    }
}
