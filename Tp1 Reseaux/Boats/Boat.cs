using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tp1_Reseaux
{
	public enum BoatType
	{
		Torpedo,
		Submarine,
		CounterTorpedo,
		AircraftCarrier,
		Destroyer
	}

	/// <summary>
	/// La classe bateau pourrait possiblement etre une interface.
	/// Ceci enleverait le besoin de overrider toute les propriete
	/// qui ne fait pas vraiment de sens du point de vue structurelle.
	/// A revoir
	/// </summary>
    public class Boat 
    {
		public int NbVies { get; set; }

		public bool Sunk => NbVies <= 0;

		public BoatType Type { get; private set; }

		public Boat(BoatType type)
		{
			switch (type)
			{
				case BoatType.Torpedo: NbVies = 2; break;
				case BoatType.Submarine:NbVies = 3; break;
				case BoatType.CounterTorpedo: NbVies = 3; break;
				case BoatType.AircraftCarrier:NbVies = 5; break;
				case BoatType.Destroyer: NbVies = 4; break;
				default: throw new Exception(); 
			}
		}
	}
}
