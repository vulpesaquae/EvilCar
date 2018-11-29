using System;
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

        // Create a profile with a specified user role
        // List the names of this profiles
        private void Profile_Create(UserRole role)
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

        // Read a profile with a specified user role
        private void Profile_Read(UserRole role)
        {
            var xmldoc = XDocument.Load("Profiles.xml");
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
            throw new NotImplementedException();
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
