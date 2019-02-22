namespace Tp1_Reseaux
{
	/// <summary>
	/// Enumeration for the different boat possible. Contains
	/// the life points of the boat as well.
	/// </summary>
	public enum BoatType
	{
		Torpedo = 3,
		Submarine = 3,
		CounterTorpedo = 2,
		AircraftCarrier = 5,
		Destroyer= 4
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
			LifePoints = (int)type;
		}
	}
}
