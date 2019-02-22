using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tp1_Reseaux
{
    public class Cruiser : Boat
    {
        public override string Name => "Croiseur";
        public override int Length => 4;
        public override Position[] Positions => new Position[Length];
    }
}
