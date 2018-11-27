using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace EvilCar
{
    class Program
    {
        // Laut Domain Model in der Projektbeschreibung (S. 6) hätte ich mir Administrator und FleetManager Klasse sparen können.
        // Müssten da eh schauen, wie wir das mit der Authetifizierung machen wollen.
        static void Main(string[] args)
        {
            var user = Login();
            if(user != null)
            {
                Console.WriteLine("You are logged in.");
            }
            else
            {
                Console.WriteLine("There was an error.");
            }

            Console.Read();
        }

        public static User Login()
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
                    return new User();
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
