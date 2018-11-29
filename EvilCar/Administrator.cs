﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EvilCar
{
    class Administrator : UserType
    {
        public Administrator(string name) : base(name) {}

        public void AdminConsole()
        {
            ConsoleKeyInfo inputKey;
            do
            {
                Console.WriteLine("Please choose one of the options: ");
                Console.WriteLine("\t<Q> Exit");
                Console.WriteLine("\t<C> Create a new administrator");

                inputKey = Console.ReadKey(false);

                switch (inputKey.Key)
                {
                    case ConsoleKey.C: Admin_Create(); break;
                }
            } while (inputKey.Key != ConsoleKey.Q);
        }

        public bool Admin_Create()
        {
            Console.WriteLine("\n\nCreate a new administartor!");

            Console.Write("Username: ");
            var username = Console.ReadLine();

            Console.Write("Password: ");
            var password = Console.ReadLine();

            Console.Write($"\nCreate new admin \"{username}\"? yes - <y>, no - <n>: ");
            if(Console.ReadKey(false).Key == ConsoleKey.Y)
            {
                XDocument xmlDoc = XDocument.Load("Profiles.xml");

                // check if name already exists
                if(xmlDoc.Descendants("Profile").SingleOrDefault(x => x.Element("Name").Value.Equals(username)) == null)
                {
                    // create the node for the admin
                    var newNode = new XElement("Profile");
                    newNode.Add(new XElement("Name", username));
                    newNode.Add(new XElement("Password", Program.Base64Encode(password)));
                    newNode.Add(new XElement("Role", UserRole.Admin.ToString()));

                    // add the node to the main node
                    xmlDoc.Element("Profiles").Add(newNode);
                    xmlDoc.Save("Profiles.xml");

                    Console.WriteLine($"\nNew Admin {username} was created.");

                    return true;
                }
                else
                {
                    Console.WriteLine("\nThe user already exists.");
                }

            }
            else
            {
                Console.WriteLine("\nCanceled to create new administrator.");
            }

            return false;
        }

        public void Admin_Read(string name)
        {
            throw new NotImplementedException();
        }

        public bool FleetManager_Create(string name, string password, string branch)
        {
            throw new NotImplementedException();
        }

        // Delete fleet manager if there are more than one fleet manager for a branch
        public bool FleetManager_Delete(string name)
        {
            throw new NotImplementedException();
        }

        // An asynchronous e-mail with the new password will be sent
        // E-mail will be mocked by an asynchronous Console.WriteLine task that lasts about 5-10 seconds
        public bool FleetManager_UpdatePassword(string name, string password)
        {
            throw new NotImplementedException();
        }

        public void FleetManager_Read(string name)
        {
            throw new NotImplementedException();
        }

        public void UpdateProfile()
        {
            throw new NotImplementedException();
        }

        public bool Branch_Create(string name)
        {
            throw new NotImplementedException();
        }
    }
}
