using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tp1_Reseaux
{
    public class AircraftCarrier : Boat
    {
        public override string Name => "Porte-avions";
        public override int Length => 5;
        public override Position[] Positions => new Position[Length];
    }
}
