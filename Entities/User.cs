using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvilCar
{
    public class User : UserType
    {
        public User(string name) : base(name) {}

        public Car RentedCar { get; private set; }
        public float Costs {
            get
            {
                // Kosten abhängig von der gebuchten Zeit machen??
                    // Auch die Services?? Können da allgemein den Preis des Ausleihens erhöhen
                    // Könnten Kosten pro Stunde angeben. Kunde muss dann jede Stunde den Leihpreis des Autos + die Kosten für die Services bezahlen
                    // Ansonsten nur die Kosten des Autos pro Stunde und Services einmalig??
                // BookedServices mit reinnehmen

                return RentedCar.Costs;
            }
        }
        public DateTime RentedSince { get; private set; }
        public List<Car.BookableServices> BookedServices { get; } = new List<Car.BookableServices>();
    }
}
