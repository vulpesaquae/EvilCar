using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace EvilCar
{
    class Database
    {
        List<User> allUsers = new List<User>();
        List<Branch> allBranches = new List<Branch>();

        private Database()
        {

        }

        public static Database buildFromXMl(XDocument doc)
        {
            Database db = new Database();

            try
            {
                foreach(var profile in doc.Descendants("Profile"))
                {
                    var name = profile.Element("Name").Value;
                    var password = profile.Element("Password").Value;
                    var role = profile.Element("Role").Value;

                    db.allUsers.Add(new User(name, password, (Entities.UserRole)Enum.Parse(typeof(Entities.UserRole), role)));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex.Message);
            }

            return db;
        }

        public void safe(string path)
        {
            throw new NotImplementedException("encode db to xml");
        }

        /// <summary>
        /// checks if a username is in the database and returns true if so
        /// </summary>
        /// <param name="username">the username you want to check</param>
        /// <returns>true if the username is in the database</returns>
        public bool checkUsername(string username)
        {
            foreach(User user in allUsers)
            {
                if(user.name == username)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="username">credentials to check</param>
        /// <param name="password">credentials to check</param>
        /// <returns>true if the credentials are correct</returns>
        public bool checkCredentials(string username, string password)
        {
            foreach (User user in allUsers)
            {
                if (user.name == username)
                {
                    if(Base64Decode(user.password) == password)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="username">the name of the user you want to get</param>
        /// <returns>the User Object that has the username or null if there is no User with that name</returns>
        public User getUser(string username)
        {
            foreach(User user in allUsers)
            {
                if(user.name == username)
                {
                    return user;
                }
            }
            return null;
        }

        #region Base64

        /// <summary>
        /// Convert plain text to base64
        /// </summary>
        /// <param name="str">Text to convert to Base64</param>
        /// <returns></returns>
        public static string Base64Encode(string str)
        {
            if (str.Length > 0)
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
            if (str.Length > 0)
            {
                var byte64text = Convert.FromBase64String(str);
                return Encoding.UTF8.GetString(byte64text);
            }
            else
            {
                return "";
            }
        }

        #endregion Base64
    }
}
