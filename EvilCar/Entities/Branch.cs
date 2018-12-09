using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvilCar
{
    // Nur wegen vollständigkeit hier
    // Keine Ahnung was das sein soll.
    // Achim und ich hatten uns erstmal drauf geeinigt, dass das wie eine Flotte ist...
    public class Branch
    {
        public string name;
        public List<Fleet> fleets = new List<Fleet>();

        public Branch(string name, List<Fleet> fleets)
        {
            this.name = name;
            if (fleets != null)
                this.fleets = fleets;
        }
    }
}
