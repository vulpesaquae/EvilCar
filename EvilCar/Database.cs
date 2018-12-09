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
                foreach(var profile in doc.Descendants(nameof(User)))
                {
                    var name = profile.Element(nameof(User.Name)).Value;
                    var password = profile.Element(nameof(User.Password)).Value;
                    var role = profile.Element(nameof(User.Role)).Value;

                    db.allUsers.Add(new User(name, password, (Entities.UserRole)Enum.Parse(typeof(Entities.UserRole), role)));
                }

                // Create all branches
                foreach(var branch in doc.Descendants(nameof(Branch)))
                {
                    var branchname = branch.Element(nameof(Branch.Name)).Value;
                    var fleets = new List<Fleet>();

                    foreach(var fleet in branch.Descendants(nameof(Fleet)))
                    {
                        var fleetname = fleet.Element(nameof(Fleet.Name)).Value;
                        var managername = fleet.Element(nameof(Fleet.Manager)).Value;
                        Fleet newFleet = new Fleet(fleetname, managername, branchname);

                        foreach(var car in fleet.Descendants(nameof(Car)))
                        {
                            var carname = car.Element(nameof(Car.Name)).Value;
                            var isLimo_s = car.Element(nameof(Car.IsLimo)).Value;
                            var isLimo = false;
                            if (isLimo_s.ToLower() == "true" || isLimo_s == "1")
                                isLimo = true;
                            newFleet.Cars.Add(new Car(carname, isLimo));
                        }

                        fleets.Add(newFleet);
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
            var evilcars = new XElement(nameof(EvilCar));

            foreach (var user in allUsers)
            {
                var xuser = new XElement(nameof(User));
                xuser.Add(new XElement(nameof(User.Name), user.Name));
                xuser.Add(new XElement(nameof(User.Password), user.Password));
                xuser.Add(new XElement(nameof(User.Role), user.Role));
                evilcars.Add(xuser);
            }

            // safe all branches
            foreach(var branch in allBranches)
            {
                var xbranch = new XElement(nameof(Branch));
                xbranch.Add(new XElement(nameof(Branch.Name), branch.Name));

                // safe all fleets of the branch
                if (branch.Fleets.Any())
                {
                    var xfleets = new XElement(nameof(Branch.Fleets));

                    foreach (var fleet in branch.Fleets)
                    {
                        var xfleet = new XElement(nameof(Fleet));
                        xfleet.Add(new XElement(nameof(Fleet.Name), fleet.Name));
                        xfleet.Add(new XElement(nameof(Fleet.Manager), fleet.Manager));

                        if (fleet.Cars.Any())
                        {
                            var xcars = new XElement(nameof(Fleet.Cars));

                            foreach (var car in fleet.Cars)
                            {
                                var xcar = new XElement(nameof(Car));
                                xcar.Add(new XElement(nameof(Car.Name), car.Name));
                                xcar.Add(new XElement(nameof(Car.IsLimo), car.IsLimo));

                                xcars.Add(xcar);
                            }

                            xfleet.Add(xcars);
                        }
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
        public bool checkUsername(string username) => allUsers.Any(x => x.Name.ToLower() == username.ToLower());

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
                return password == Base64Decode(user.Password);
            }
            return false;
        }

        /// <summary>
        /// Get a specified User object
        /// </summary>
        /// <param name="username">the name of the user you want to get</param>
        /// <returns>User Object that has the username or null if there is no User with that name</returns>
        public User getUser(string username) => allUsers.SingleOrDefault(x => x.Name.ToLower() == username.ToLower());

        /// <summary>
        /// Get a list of users with a specified role
        /// </summary>
        /// <param name="role">Role the user must have</param>
        /// <returns>List of users with a specified role</returns>
        public IEnumerable<User> GetUsersFromRole(Entities.UserRole role) => allUsers.Where(x => x.Role == role);

        /// <summary>
        /// Add a new User object to the database
        /// </summary>
        /// <param name="username">Name of the user</param>
        /// <param name="plainPassword">Password in plain text</param>
        /// <param name="role">Role of the user</param>
        public bool CreateUser(string username, string plainPassword, Entities.UserRole role)
        {
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
            if (user != null && (user.Role != Entities.UserRole.Manager || allUsers.Count(x => x.Role == Entities.UserRole.Manager) > 1))
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
                user.Password = Base64Encode(newPlainPassword);

                var content = "*************************\n\n" + $"Hello {username}! Your password was changed to \"{newPlainPassword}\"." + "\n\n*************************";
                // fire and forget
                // to show it is async, the text will be displayed after 5 seconds
                // but we can continue working
                SendInformation(content);

                return true;
            }
            return false;
        }

        // the "E-Mail"
        // wait 5 seconds and than print something to the console
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

        private Branch GetBranch(string name) => allBranches.SingleOrDefault(x => x.Name.ToLower() == name.ToLower());

        /// <summary>
        /// Create a new branch
        /// </summary>
        /// <param name="name">Name of the branch to create</param>
        /// <returns>True if branch was created, false if not</returns>
        public bool CreateBranch(string name)
        {
            if(!allBranches.Any(x => x.Name.ToLower() == name.ToLower()))
            {
                allBranches.Add(new Branch(name));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Create a new fleet for a branch
        /// </summary>
        /// <param name="branchName">Name of the branch to add the fleet to</param>
        /// <param name="fleetName">Name of the fleet to create</param>
        /// <param name="managerName">Name of the manager assigend to this new fleet</param>
        /// <param name="cars">Cars the fleet contains</param>
        /// <returns>True if the fleet was created, false if not</returns>
        public bool CreateFleet(string branchName, string fleetName, string managerName, List<Car> cars = null)
        {
            var branch = GetBranch(branchName);
            if(branch != null && !branch.Fleets.Any(x => x.Name.ToLower() == fleetName.ToLower()))
            {
                var fleet = new Fleet(fleetName, managerName, branch.Name);
                branch.Fleets.Add(fleet);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Delete a fleet from a branch
        /// </summary>
        /// <param name="branchName">Name of the branch to delete the fleet from</param>
        /// <param name="fleetName">Name of the fleet to delete</param>
        /// <param name="managerName">Name of the manager assigned to the fleet</param>
        /// <returns></returns>
        public bool DeleteFleet(string branchName, string fleetName, string managerName)
        {
            var branch = GetBranch(branchName);
            if(branch != null)
            {
                var fleet = branch.Fleets.SingleOrDefault(x => x.Name.ToLower() == fleetName.ToLower());
                if(fleet != null && fleet.Manager == managerName)
                {
                    branch.Fleets.Remove(fleet);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get all fleets assigned to a specific manager
        /// </summary>
        /// <param name="managerName">Name of the manager assigned to the fleets</param>
        /// <returns></returns>
        public IEnumerable<Fleet> GetFleets(string managerName) => allBranches.SelectMany(x => x.Fleets.Where(y => y.Manager.ToLower() == managerName.ToLower()));

        /// <summary>
        /// Get a fleet by its name
        /// </summary>
        /// <param name="managername">Name of the manager assigned to the fleet</param>
        /// <param name="branchname">Name of the branch the fleet is part of</param>
        /// <param name="fleetname">Fleets name you looking for</param>
        /// <returns>Fleet if it exists, otherwise null</returns>
        private Fleet GetFleet(string managername, string branchname, string fleetname) => allBranches.SingleOrDefault(x => x.Name.ToLower() == branchname.ToLower())
            ?.Fleets.SingleOrDefault(x => x.Name.ToLower() == fleetname.ToLower() && x.Manager.ToLower() == managername.ToLower());

        /// <summary>
        /// Get all cars of a specific fleet
        /// </summary>
        /// <param name="managername">Name of the manager assigned to the fleet</param>
        /// <param name="branchname">Name of the branch the fleet is part of</param>
        /// <param name="fleetname">Fleets name you want to get the cars from</param>
        /// <returns></returns>
        public IEnumerable<Car> GetCarsFromFleet(string managername, string branchname, string fleetname) => GetFleet(managername, branchname, fleetname)?.Cars;

        /// <summary>
        /// Add a new to to a fleet
        /// </summary>
        /// <param name="managername">Name of the manager assigned to the fleet</param>
        /// <param name="branchname">Name of the branch the fleet is part of</param>
        /// <param name="fleetname">Name of the fleet the car should be added</param>
        /// <param name="newCar">Car object representing the new car</param>
        /// <returns>True if the new car was added, false if not</returns>
        public bool AddCarToFleet(string managername, string branchname, string fleetname, Car newCar)
        {
            var fleet = GetFleet(managername, branchname, fleetname);
            if (fleet != null && !fleet.Cars.Any(x => x.Name.ToLower() == newCar.Name.ToLower()))
            {
                fleet.Cars.Add(newCar);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Delete a car from a fleet
        /// </summary>
        /// <param name="managername">Name of the manager assigned to the fleet</param>
        /// <param name="branchname">Name of the branch the fleet is part of</param>
        /// <param name="fleetname">Name of the fleet the car should be added</param>
        /// <param name="carname">Name of the car to delete</param>
        /// <returns></returns>
        public bool DeleteCarFromFleet(string managername, string branchname, string fleetname, string carname)
        {
            var fleet = GetFleet(managername, branchname, fleetname);
            if(fleet != null)
            {
                var car = fleet.Cars.SingleOrDefault(x => x.Name.ToLower() == carname.ToLower());
                if(car != null)
                {
                    fleet.Cars.Remove(car);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get a specific car
        /// </summary>
        /// <param name="managername">Name of the manager assigned to the fleet</param>
        /// <param name="branchname">Name of the branch the fleet is part of</param>
        /// <param name="fleetname">Name of the fleet the car should be added</param>
        /// <param name="carname">Name of the car to get</param>
        /// <returns></returns>
        public Car GetCar(string managername, string branchname, string fleetname, string carname) => GetFleet(managername, branchname, fleetname).Cars.SingleOrDefault(x => x.Name.ToLower() == carname.ToLower());

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
