using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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
                Console.WriteLine("\n\nPlease choose one of the options: ");
                Console.WriteLine("\t<Q> Exit");
                Console.WriteLine($"\t<A> Create a new \"{UserRole.Admin.ToString()}\"");
                Console.WriteLine($"\t<R> Read a \"{UserRole.Admin.ToString()}\"");
                Console.WriteLine($"\t<M> Create a new \"{UserRole.Manager.ToString()}\"");
                Console.WriteLine($"\t<L> Read a \"{UserRole.Manager.ToString()}\"");
                Console.WriteLine($"\t<D> Delete a \"{UserRole.Manager.ToString()}\"");
                Console.WriteLine($"\t<F> Update the password of a \"{UserRole.Manager.ToString()}\"");
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

        // Create a account with a specified user role
        public void CreateAccount(UserRole role)
        {
            Console.WriteLine($"\n\nCreate a new {role.ToString()}!");

            string username, password;
            Program.InsertCredentials(out username, out password);

            Console.Write($"\nCreate new {role.ToString()} \"{username}\"? yes - <y>, no - <n>: ");
            if (Console.ReadKey(false).Key == ConsoleKey.Y)
            {
                XDocument xmlDoc = XDocument.Load("Profiles.xml");

                // check if name already exists
                if (xmlDoc.Descendants("Profile").SingleOrDefault(x => x.Element("Name").Value.Equals(username)) == null)
                {
                    // create the node for the admin
                    var newNode = new XElement("Profile");
                    newNode.Add(new XElement("Name", username));
                    newNode.Add(new XElement("Password", Program.Base64Encode(password)));
                    newNode.Add(new XElement("Role", UserRole.Admin.ToString()));

                    // add the node to the main node
                    xmlDoc.Element("Profiles").Add(newNode);
                    xmlDoc.Save("Profiles.xml");

                    Console.WriteLine($"\nNew {role.ToString()} \"{username}\" was created.");
                }
                else
                {
                    Console.WriteLine("\nThe user already exists.");
                }
            }
            else
            {
                Console.WriteLine($"\nCanceled to create new {role.ToString()}.");
            }
        }

        // Create a new Admin
        public void Admin_Create() => CreateAccount(UserRole.Admin);

        // Read an admin
        public void Admin_Read()
        {
            using (var reader = XmlReader.Create("Profiles.xml"))
            {
                while(reader.Read())
                {
                    Console.WriteLine(reader.Name);
                }
            }
        }

        // Create a new Fleet Manager
        public void FleetManager_Create() => CreateAccount(UserRole.Manager);

        // Delete fleet manager if there are more than one fleet manager for a branch
        public void FleetManager_Delete()
        {
            throw new NotImplementedException();
        }

        // An asynchronous e-mail with the new password will be sent
        // E-mail will be mocked by an asynchronous Console.WriteLine task that lasts about 5-10 seconds
        public void FleetManager_UpdatePassword()
        {
            throw new NotImplementedException();
        }

        public void FleetManager_Read()
        {
            throw new NotImplementedException();
        }

        public void UpdateProfile()
        {
            throw new NotImplementedException();
        }

        public void Branch_Create()
        {
            throw new NotImplementedException();
        }
    }
}
