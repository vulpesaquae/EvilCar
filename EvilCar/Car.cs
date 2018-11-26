using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvilCar
{
    class Car
    {
        // müssen die noch mit dem Preis verbinden.
        public enum BookableServices
        {
            Spotify, Parker, Navigation, Massage
        }

        public bool IsBooked { get;  set; } = false;
    }
}
