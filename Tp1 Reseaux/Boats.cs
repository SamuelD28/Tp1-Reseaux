using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tp1_Reseaux
{
    public static class Boats
    {
        //public bool AreTheyPlaced => All.

        public static IEnumerable<Boat> All => new List<Boat>() { PorteAvions, Croiseur, ContreTorpilleur, SousMarin, Torpilleur };

        public static Boat PorteAvions => new Boat(Boat.BoatType.PorteAvions);
        public static Boat Croiseur => new Boat(Boat.BoatType.Croiseur);
        public static Boat ContreTorpilleur => new Boat(Boat.BoatType.ContreTorpilleur);
        public static Boat SousMarin => new Boat(Boat.BoatType.SousMarin);
        public static Boat Torpilleur => new Boat(Boat.BoatType.Torpilleur);
    }
}
