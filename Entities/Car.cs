﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvilCar
{
    public class Car
    {
        public float Costs { get; }

        // müssen die noch mit dem Preis verbinden.
        public enum BookableServices
        {
            Spotify, Parker, Navigation, Massage
        }

        public bool IsBooked { get;  set; } = false;
    }
}
