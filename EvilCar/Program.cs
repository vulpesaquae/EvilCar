using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace EvilCar
{
    class Program
    {

        // Gehört find ich auch eher mit in die UserType Klasse...
        public enum UserRole
        {
            User, Manager, Admin
        }

        // Laut Domain Model in der Projektbeschreibung (S. 6) hätte ich mir Administrator und FleetManager Klasse sparen können.
        // Müssten da eh schauen, wie wir das mit der Authetifizierung machen wollen.
        static void Main(string[] args)
        {
            var profile = Login();
            if(profile != null)
            {
                Console.WriteLine($"Hello {profile.Name}! You are now logged in.");

                if(profile is User)
                {
                    var user = (User)profile;
                }
                else if(profile is FleetManager)
                {
                    var manager = (FleetManager)profile;
                }
                else if(profile is Administrator)
                {
                    var admin = (Administrator)profile;
                }
            }
            else
            {
                Console.WriteLine("There was an error.");
            }

            Console.Read();
        }

        // Vielleicht mit in die UserType Klasse packen??
        // Da könnte am Anfang ein (leeres)UserType Objekt erstellt werden.
        // Im Konstruktor?? (oder extra Funktion aufrufen??) kann dann der Ablauf hier abgearbeitet werden.
        // Die Klasse wird dann als UserType zurückgegeben
        // Dadurch könnte auch UserRole mit in die UserType Klasse, nur gehören da eigentlich die XML und Base64 Operationen nicht rein...
        // Hat find ich beides seine Vorteile und daseins berechtigungn......
        public static UserType Login()
        {
            Console.WriteLine("Enter Username: ");
            var username = Console.ReadLine();

            Console.WriteLine("Enter Password: ");
            var password = Console.ReadLine();

            XDocument xmlDoc = XDocument.Load("Profiles.xml");
            var profile = xmlDoc.Descendants("Profile").SingleOrDefault(x => x.Element("Name").Value.Equals(username));

            if (profile != null)
            {
                if (Base64Decode(profile.Element("Password").Value).Equals(password))
                {
                    switch ((UserRole)Enum.Parse(typeof(UserRole), profile.Element("Role").Value))
                    {
                        case UserRole.User: return new User(username);
                        case UserRole.Manager: return new FleetManager(username);
                        case UserRole.Admin: return new Administrator(username);
                    }
                }
            }

            return null;
        }


        public static string Base64Encode(string str)
        {
            if(str.Length > 0)
            {
                var plainTextBytes = Encoding.UTF8.GetBytes(str);
                return Convert.ToBase64String(plainTextBytes);
            }
            else
            {
                return "";
            }
        }

        public static string Base64Decode(string str)
        {
            if(str.Length > 0)
            {
                var byte64text = Convert.FromBase64String(str);
                return Encoding.UTF8.GetString(byte64text);
            }
            else
            {
                return "";
            }
        }
    }
}
