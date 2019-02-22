namespace Tp1_Reseaux
{
	/// <summary>
	/// Enumeration for the different boat possible. Contains
	/// the life points of the boat as well.
	/// </summary>
	public enum BoatType
	{
		Torpedo,
		Submarine,
		CounterTorpedo,
		AircraftCarrier,
		Destroyer
	}

	/// <summary>
	/// Class used to handle the boats for the player
	/// </summary>
    public class Boat 
    {
		//Contains the life points of the boat
		public int LifePoints { get; set; }

		//Indicate if the boat has been placed inside the playable grid
		public bool IsPlaced { get; set; }

		//Indicate that the boat is sunk.
		public bool Sunk => LifePoints <= 0;

		//Type of the current boat
		public BoatType Type { get; private set; }

		/// <summary>
		/// Constructor for the boat that require the boat type to instantiate
		/// </summary>
		/// <param name="type"></param>
		public Boat(BoatType type)
		{
			IsPlaced = false;
            Type = type;
			switch (type)
			{
				case BoatType.Torpedo: LifePoints = 3; break;
				case BoatType.Submarine: LifePoints = 3; break;
				case BoatType.CounterTorpedo: LifePoints = 2; break;
				case BoatType.AircraftCarrier: LifePoints = 5; break;
				case BoatType.Destroyer: LifePoints = 4; break;
			}
		}
	}
}
