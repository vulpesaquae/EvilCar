using System;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace EvilCar
{
    // Gehört find ich auch eher mit in die UserType Klasse...

    // Wird auch in anderen Klassen benötigt...dann vielleicht doch hier??
    public enum UserRole
    {
        User, Manager, Admin
    }

    class Program
    {
        public const string PROFILES_FILENAME = "Profiles.xml";

        // Laut Domain Model in der Projektbeschreibung (S. 6) hätte ich mir Administrator und FleetManager Klasse sparen können.
        // Müssten da eh schauen, wie wir das mit der Authetifizierung machen wollen.
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Please enter your credentials for the login.");
                Console.WriteLine("Press CTRL + C to extit.");

                var profile = Login();
                if (profile != null)
                {
                    Console.WriteLine($"Hello {profile.Name}! You are now logged in.");

                    if (profile is User)
                    {
                        var user = (User)profile;
                    }
                    else if (profile is Manager)
                    {
                        var manager = (Manager)profile;
                    }
                    else if (profile is Admin)
                    {
                        var admin = (Admin)profile;
                        admin.AdminConsole();
                    }
                }
                else
                {
                    Console.WriteLine("There was an error with your credentials. Please try again.\n");
                }
            }
        }

        // Vielleicht mit in die UserType Klasse packen??
        // Da könnte am Anfang ein (leeres)UserType Objekt erstellt werden.
        // Im Konstruktor?? (oder extra Funktion aufrufen??) kann dann der Ablauf hier abgearbeitet werden.
        // Die Klasse wird dann als UserType zurückgegeben
        // Dadurch könnte auch UserRole mit in die UserType Klasse, nur gehören da eigentlich die XML und Base64 Operationen nicht rein...
        // Hat find ich beides seine Vorteile und daseins berechtigungn......
        /// <summary>
        /// Login for the user into his profile
        /// </summary>
        /// <returns>Specific UserType class for the role of the authorized user</returns>
        public static UserType Login()
        {
            string username, password;
            InsertCredentials(out username, out password);

            var xmlDoc = XDocument.Load(PROFILES_FILENAME);
            var profile = ProfilesXml_GetUser(xmlDoc, username);

            if (profile != null)
            {
                if (Base64Decode(profile.Element("Password").Value).Equals(password))
                {
                    switch ((UserRole)Enum.Parse(typeof(UserRole), profile.Element("Role").Value))
                    {
                        case UserRole.User: return new User(username);
                        case UserRole.Manager: return new Manager(username);
                        case UserRole.Admin: return new Admin(username);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get a XML Profile for a specified username from the file PROFILES_FILENAME
        /// </summary>
        /// <param name="username">Name to identifi the profile</param>
        /// <returns></returns>
        public static XElement ProfilesXml_GetUser(XDocument xmlDoc, string username) => xmlDoc.Descendants("Profile").SingleOrDefault(x => x.Element("Name").Value.ToLower().Equals(username.ToLower()));

        /// <summary>
        /// Let the user insert the credentials for a profile
        /// </summary>
        /// <param name="username">Name for the profile to work with</param>
        /// <param name="password">Password for the profile to work with</param>
        public static void InsertCredentials(out string username, out string password)
        {
            Console.Write("Username: ");
            username = Console.ReadLine();

            Console.Write("Password: ");
            password = Console.ReadLine();
        }

        /// <summary>
        /// Convert plain text to base64
        /// </summary>
        /// <param name="str">Text to convert to Base64</param>
        /// <returns></returns>
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

        /// <summary>
        /// Convert base64 to plain text
        /// </summary>
        /// <param name="str">Text to convert to plain text</param>
        /// <returns></returns>
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
