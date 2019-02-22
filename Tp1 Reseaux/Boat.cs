using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tp1_Reseaux
{
    public abstract class Boat 
    {
        public abstract string Name { get; }
        public abstract int Length { get; }
        public abstract Position[] Positions { get; } 
        public bool IsPlaced { get; set; }
        bool IsDrowned { get; set; }
    }
}
