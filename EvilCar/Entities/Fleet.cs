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

        public Fleet(string name)
        {
            this.name = name;
        }

        public Fleet(string name, List<Car> cars)
        {
            this.name = name;
            this.cars = cars;
        }

        public void add(Car car) => cars.Add(car);
    }
}
