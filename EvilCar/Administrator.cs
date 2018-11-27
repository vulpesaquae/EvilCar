using System;
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

        public void Console()
        {
            System.Console.WriteLine("Please choose one of the options:");
            System.Console.WriteLine("<1> Create a new administrator");

            switch(System.Console.ReadKey().KeyChar)
            {
                case '1': Admin_Create(); break;
            }
        }

        public bool Admin_Create()
        {
            System.Console.WriteLine("\n\nCreate a new administartor!");

            System.Console.Write("Username: ");
            var username = System.Console.ReadLine();

            System.Console.Write("Password: ");
            var password = System.Console.ReadLine();

            System.Console.Write($"Create new admin \"{username}\"? yes - <y>, no - <n>: ");
            if(System.Console.ReadKey().Key == ConsoleKey.Y)
            {
                XDocument xmlDoc = XDocument.Load("Profiles.xml");

                if(xmlDoc.Descendants("Profile").SingleOrDefault(x => x.Element("Name").Value.Equals(username)) == null)
                {
                    System.Console.WriteLine("\nWurde noch nicht erstellt. Muss mir erstmal noch XmlReader und XmlWriter anschauen. Da kann man wohl auch schon direkt mit Base64 arbeite. Würden wir die extra funktionen nicht benötigen und das Login könnte auch in die UserType Klasse.");
                }
                else
                {
                    System.Console.WriteLine("\nThe user already exists.");
                }

            }
            else
            {
                System.Console.WriteLine("\nCanceled to create new administrator.");
            }

            throw new NotImplementedException();
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
