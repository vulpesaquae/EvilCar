using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvilCar
{
    public class Car
    {

        // müssen die noch mit dem Preis verbinden.
        public enum BookableServices
        {
            Spotify, Parker, Navigation, Massage
        }

        public Car(string name, bool isLimo)
        {
            this.Name = name;
            this.IsLimo = isLimo;
        }

        public bool IsBooked { get;  set; } = false;
        public bool IsLimo { get; }
        public string Name { get; }
    }
}
