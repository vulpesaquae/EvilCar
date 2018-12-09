using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        /// <summary>
        /// Load the database
        /// </summary>
        /// <param name="doc">File to load the data from</param>
        /// <returns>Loaded data from the file</returns>
        public static Database buildFromXMl(XDocument doc)
        {
            Database db = new Database();

            try
            {
                // Create all Profiles as User's
                foreach(var profile in doc.Descendants("Profile"))
                {
                    var name = profile.Element("Name").Value;
                    var password = profile.Element("Password").Value;
                    var role = profile.Element("Role").Value;

                    db.allUsers.Add(new User(name, password, (Entities.UserRole)Enum.Parse(typeof(Entities.UserRole), role)));
                }

                // Create all branches
                foreach(var branch in doc.Descendants("Branch"))
                {
                    var branchname = branch.Element("Name").Value;
                    var fleets = new List<Fleet>();

                    foreach(var fleet in branch.Descendants("Fleet"))
                    {
                        var fleetname = fleet.Element("Name").Value;
                        fleets.Add(new Fleet(fleetname));
                    }

                    db.allBranches.Add(new Branch(branchname, fleets));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex.Message);
            }

            return db;
        }

        /// <summary>
        /// Write the database to a xml file
        /// </summary>
        /// <param name="path">Path to the xml file to write or to create a new one</param>
        public void safe(string path)
        {
            XDocument xmlDoc = new XDocument();
            var evilcars = new XElement("EvilCars");

            foreach (var user in allUsers)
            {
                var xuser = new XElement("Profile");
                xuser.Add(new XElement("Name", user.name));
                xuser.Add(new XElement("Password", user.password));
                xuser.Add(new XElement("Role", user.role));
                evilcars.Add(xuser);
            }

            // safe all branches
            foreach(var branch in allBranches)
            {
                var xbranch = new XElement("Branch");
                xbranch.Add(new XElement("Name", branch.name));

                // safe all fleets of the branch
                if (branch.fleets.Any())
                {
                    var xfleets = new XElement("Fleets");

                    foreach (var fleet in branch.fleets)
                    {
                        var xfleet = new XElement("Fleet");
                        xfleet.Add(new XElement("Name", fleet.name));

                        xfleets.Add(xfleet);
                    }

                    xbranch.Add(xfleets);
                }
                evilcars.Add(xbranch);
            }

            xmlDoc.Add(evilcars);
            xmlDoc.Save(path);
        }

        #region User Tasks

        /// <summary>
        /// Checks if a username is in the database and returns true if so
        /// </summary>
        /// <param name="username">the username you want to check</param>
        /// <returns>true if the username is in the database</returns>
        public bool checkUsername(string username) => allUsers.Any(x => x.name.ToLower() == username.ToLower());

        /// <summary>
        /// Check if the username and password are valid for a certain profile
        /// </summary>
        /// <param name="username">name of the profile</param>
        /// <param name="password">password of the profile</param>
        /// <returns>true if the credentials are correct</returns>
        public bool checkCredentials(string username, string password)
        {
            var user = getUser(username);

            if(user != null)
            {
                return password == Base64Decode(user.password);
            }
            return false;
        }

        /// <summary>
        /// Get a specified User object
        /// </summary>
        /// <param name="username">the name of the user you want to get</param>
        /// <returns>User Object that has the username or null if there is no User with that name</returns>
        public User getUser(string username) => allUsers.SingleOrDefault(x => x.name.ToLower() == username.ToLower());

        /// <summary>
        /// Get a list of users with a specified role
        /// </summary>
        /// <param name="role">Role the user must have</param>
        /// <returns>List of users with a specified role</returns>
        public IEnumerable<User> GetUsersFromRole(Entities.UserRole role) => allUsers.Where(x => x.role == role);

        /// <summary>
        /// Add a new User object to the database
        /// </summary>
        /// <param name="username">Name of the user</param>
        /// <param name="plainPassword">Password in plain text</param>
        /// <param name="role">Role of the user</param>
        public bool CreateUser(string username, string plainPassword, Entities.UserRole role)
        {
            // TODO: create manager with a fleet
            if (!checkUsername(username) && plainPassword.Any())
            {
                allUsers.Add(new User(username, Base64Encode(plainPassword), role));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove a specified User object from the database
        /// </summary>
        /// <param name="username">Name of the User object to remove</param>
        /// <returns>True if the user was removed or false if not</returns>
        public bool RemoveUser(string username)
        {
            var user = getUser(username);
            // there is a user AND user is not a manager OR there is more than one manager
            // delete the manager only, if there are 2 or more
            if (user != null && (user.role != Entities.UserRole.Manager || allUsers.Count(x => x.role == Entities.UserRole.Manager) > 1))
            {
                return allUsers.Remove(user);
            }
            return false;
        }

        /// <summary>
        /// Assign a new password for a user and send a async information
        /// </summary>
        /// <param name="username">Name of the user</param>
        /// <param name="newPlainPassword">New password as plain text</param>
        /// <returns>True if the password was changed successfuly, false otherwise</returns>
        public bool UpdateUser(string username, string newPlainPassword)
        {
            var user = getUser(username);
            if(user != null)
            {
                user.password = Base64Encode(newPlainPassword);

                var content = "*************************\n\n" + $"Hello {username}! Your password was changed to \"{newPlainPassword}\"." + "\n\n*************************";
                // fire and forget
                // to show it is async, the text will be displayed after 5 seconds
                // but we can continue working
                SendInformation(content);

                return true;
            }
            return false;
        }

        private async Task SendInformation(string content)
        {
            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(5000);
                Console.WriteLine(content);
            });
        }

        #endregion User Tasks

        #region Branch Tasks

        public bool CreateBranch(string name)
        {
            if(!allBranches.Any(x => x.name == name))
            {
                allBranches.Add(new Branch(name));
                return true;
            }
            return false;
        }

        public bool CreateFleet(string branchName, string fleetName, List<Car> cars = null)
        {
            var branch = allBranches.SingleOrDefault(x => x.name == branchName);
            if(branch != null && !branch.fleets.Any(x => x.name == fleetName))
            {
                var fleet = new Fleet(fleetName);
                branch.fleets.Add(fleet);

                return true;
            }
            return false;
        }

        #endregion Branch Tasks

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
