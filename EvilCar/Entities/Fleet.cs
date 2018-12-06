using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvilCar
{
    public class Fleet
    {
        User fleet_manager;
        List<Car> cars = new List<Car>();
        string name;

        public Fleet(User manager, string name)
        {
            fleet_manager = manager;
            this.name = name;
        }

        public void add(Car car) => cars.Add(car);
    }
}
