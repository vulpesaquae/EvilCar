using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvilCar
{
    public class Fleet
    {
        List<Car> cars = new List<Car>();
        public string name;
        public string ManagerName;

        public Fleet(string name, string ManagerName)
        {
            this.name = name;
            this.ManagerName = ManagerName;
        }

        public Fleet(string name, string ManagerName, List<Car> cars)
        {
            this.name = name;
            this.ManagerName = ManagerName;
            this.cars = cars;
        }

        public void add(Car car) => cars.Add(car);
    }
}
