using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvilCar
{
    public class User
    {
        public string Password { get; set; }
        public string Name { get; }
        public Entities.UserRole Role { get; }
        public Car RentedCar { get; private set; }
        public DateTime RentedSince { get; private set; }
        public List<Car.BookableServices> BookedServices { get; } = new List<Car.BookableServices>();

        public User(string name, string password, Entities.UserRole role) {
            Name = name;
            Password = password;
            Role = role;
        }
    }
}
