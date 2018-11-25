using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvilCar
{
    class Car
    {
        // Ist wohl besser die Autokonfigurationen in einer Datei zu speichern und dann auszulesen
        // Dadurch ist man flexibler was Änderungen angeht und es können auch manuell neu hinzugefügt werden
        // Ist dann auch einfacher individuell was für Automarken zu machen...
        // Oder man macht das halt über eine Basisklassse die schon gewisse Grundfunktionen beinhaltet.
        // Alles andere wird dann hardcoded...also kosten, Services und Preise...
        public float Costs;

        // Bin mir nicht sicher, ob das hier der richtige Platz für ist.
        // Müssen das pro "Autoart" individuell anpassen können, also wohl eher nicht. (S. 11, Projektbeschreibung, letzter Satz)
        // Bin aber gerade zu faul mir deswegen groß Gedanken zu machen...vielleicht später nocht...
        public enum BookableServices
        {
            Spotify, Parker, Navigation, Massage
        }

        public Dictionary<BookableServices, float> CostsPerService = new Dictionary<BookableServices, float>()
        {
            { BookableServices.Spotify, 5.00f },
            { BookableServices.Parker, 15.00f },
            { BookableServices.Navigation, 2.50f },
            { BookableServices.Massage, 10.00f }
        };

        public List<BookableServices> BookedServices = new List<BookableServices>();

        public bool IsBooked { get; set; } = false;
    }
}
