using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tp1_Reseaux
{
    public class Torpedo : Boat
    {
        public override string Name => "Torpilleur";
        public override int Length => 2;
        public override Position[] Positions => new Position[Length];
    }
}
