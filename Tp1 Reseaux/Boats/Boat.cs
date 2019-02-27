using System;

namespace Tp1_Reseaux
{
	public class BoatNotPlacedException : Exception
	{
		public BoatNotPlacedException() { }
		public BoatNotPlacedException(string message) : base(message) { }
	}

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
	/// Class used to handle the logic related to
	/// a boat that is used for playing the game
	/// </summary>
	/// <exception cref="BoatNotPlacedException">Requested the placement of a boat that wasnt placed</exception>
    public class Boat : IBoat
	{
		//Contains the life points of the boat
		public int LifePoints { get; private set; }

		//Indicate if the boat has been placed inside the playable grid
		public bool IsPlaced => Placement[0] != null && Placement[1] != null;

		//Indicate that the boat is sunk.
		public bool Sunk => LifePoints <= 0;

		public char Representation { get; private set; }

		//Type of the current boat
		public BoatType Type { get; private set; }

		private Position[] Placement { get; set; }

		/// <summary>
		/// Constructor for the boat that require the boat type to instantiate
		/// </summary>
		/// <param name="type"></param>
		public Boat(BoatType type)
		{
			Placement = new Position[2];
            Type = type;
			switch (type)
			{
				case BoatType.Torpedo:
				case BoatType.Submarine:
					LifePoints = 3;
				break;
				case BoatType.CounterTorpedo:
					LifePoints = 2;
				break;
				case BoatType.AircraftCarrier:
					LifePoints = 5;
				break;
				case BoatType.Destroyer:
					LifePoints = 4;
				break;
			}
			Representation = '■';
		}

		public void AssignPlacement(Position first, Position second)
		{
			//We make sure that first position is place at index 0 and the furthest at index 1
			Position closestPoint = Position.GetClosestPosition(first, second);
			Position furthestPoint = (closestPoint.Equals(first)) ? second : first;

			Placement[0] = closestPoint;
			Placement[1] = furthestPoint;
		}

		public void SubstractLifePoints(int damage)
		{
			if (LifePoints - damage < 0)
				LifePoints = 0;
			else
				LifePoints = LifePoints - damage;
		}

		/// <summary>
		/// Method that return the placement of the boat inside the grid.
		/// </summary>
		/// <returns>The string representation of the boat placement</returns>
		/// <exception cref="BoatNotPlacedException">The boat wasnt place inside the grid</exception>
		public string GetPlacement()
		{
			if (Placement[0] is null || Placement[1] is null)
				throw new BoatNotPlacedException();

			return Placement[0].ToString() + Placement[1].ToString();
		}
	}
}
