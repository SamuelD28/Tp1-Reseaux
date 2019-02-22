using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tp1_Reseaux
{
    public class SubMarine : Boat
    {
        public override string Name => "Sous-marin";
        public override int Length => 3;
        public override Position[] Positions => new Position[Length];
    }
}
