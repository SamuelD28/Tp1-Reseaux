using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tp1_Reseaux
{
	/// <summary>
	/// La classe bateau pourrait possiblement etre une interface.
	/// Ceci enleverait le besoin de overrider toute les propriete
	/// qui ne fait pas vraiment de sens du point de vue structurelle.
	/// A revoir
	/// </summary>
    public abstract class Boat 
    {
        public abstract string Name { get; }
        public abstract int Length { get; }
        public abstract Position[] Positions { get; } 
        public bool IsPlaced { get; set; }
        bool IsDrowned { get; set; }
    }
}
