using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvilCar
{
    public class Fleet
    {
        public List<Car> cars = new List<Car>();
        public string Name;
        public string Manager;
        public string Branch;

        public Fleet(string name, string managerName, string branchName)
        {
            Name = name;
            Manager = managerName;
            Branch = branchName;
        }

        public Fleet(string name, string managerName, string branchName, List<Car> cars)
        {
            Name = name;
            Manager = managerName;
            Branch = branchName;
            this.cars = cars;
        }
    }
}
