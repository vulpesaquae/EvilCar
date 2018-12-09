using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvilCar
{
    public class Car
    {
        public bool IsBooked { get; set; } = false;
        public bool IsLimo { get; }
        public int Guid { get; }

        public Car(int guid, bool isLimo)
        {
            Guid = guid;
            IsLimo = isLimo;
        }
    }
}
