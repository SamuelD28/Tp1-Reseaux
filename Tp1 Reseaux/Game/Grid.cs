using System;
using System.Collections.Generic;
using System.Linq;

namespace Tp1_Reseaux
{
	/// <summary>
	/// Exception used to signify that content of the grid cell 
	/// is unknown.
	/// </summary>
	public class UnknownGridCellException : Exception
	{
		public UnknownGridCellException() { }
	}

	/// <summary>
	/// Class used to handle the grid for the game
	/// </summary>
	public sealed class Grid
	{
		//Size of the grid
		public const int GridSize = 10;

		//Horizontal scale of the grid. Used for parsing and displaying
		public static readonly char[] GridHorizontalScale = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
		public static readonly string[] GridVerticalScale = new string[] {"1 ","2 ","3 ","4 ","5 ","6 ","7 ","8 ", "9 ", "10"};

		//Contains the all the grid data for the class
		private Dictionary<Position, object> GridTable = new Dictionary<Position, object>();

		//Contain the instance of the singleton class
		private static Grid Instance = null;

		/// <summary>
		/// Private Constructor that initiate a grid
		/// </summary>
		private Grid()
		{
			Init();
		}

		/// <summary>
		/// Method used to obtain the instance of the singleton class
		/// </summary>
		/// <returns></returns>
		public static Grid GetInstance() => (Instance is null) ? new Grid() : Instance;

		/// <summary>
		/// Method used to iniate a grid with null value 
		/// that represent an empty grid cell.
		/// </summary>
		private void Init()
		{
			for (int y = 0; y < GridSize; y++)
			{
				for (int x = 0; x < GridSize; x++)
				{
					GridTable.Add(Position.Create(x, y), null);
				}
			}
		}

		/// <summary>
		/// Method used to add a boat inside the playable grid
		/// </summary>
		/// <param name="boat">Boat to add</param>
		/// <param name="start">Starting position of the boat</param>
		/// <param name="end">Ending position of the boat</param>
		/// <returns></returns>
		public bool AddBoat(IBoat boat, Position start, Position end)
		{
			//Sortir une interface au lieu dutiliser directement Boat
			int? difference = start.Difference(end);

			if (difference != boat.LifePoints)
				return false;

			Position.AssignFlow(start, end);
			Position startingPoint = Position.GetClosestPosition(start, end);

			for (int i = 0; i < difference; i++)
			{
				Position key = GridTable.Keys.First(p => p.Equals(startingPoint));

				if (GridTable[key] is Boat)
					return false;

				GridTable[key] = boat;

				if (startingPoint.Flow == PositionFlow.Horizontal)
					startingPoint.Assign(startingPoint.X + 1, startingPoint.Y);
				else
					startingPoint.Assign(startingPoint.X, startingPoint.Y + 1);
			}
			return true;
		}

		/// <summary>
		/// Method used to make a shot in the playable grid.
		/// </summary>
		/// <param name="shot">Position of the shot</param>
		/// <returns></returns>
		public void AddShot(Position shot)
		{
			Position key = GridTable.Keys.First(p => p.Equals(shot));

			object result;
			GridTable.TryGetValue(key, out result);

			if (result is Boat)
			{
				Boat boat = (Boat)result;
				boat.SubstractLifePoints(1);
			}

			GridTable[key] = new Shot(key);
		}

		/// <summary>
		/// Method used to get the string representation of the current Grid
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			int startingPoint = 0;

			string gridRepresentation = "  " + new string(GridHorizontalScale) + "\n" + GridVerticalScale[startingPoint];

			foreach (KeyValuePair<Position, object> keyValuePair in GridTable)
			{
				object currentGridCell = keyValuePair.Value;
				Position currentPosition = keyValuePair.Key;

				if (currentPosition.Y != startingPoint)
				{
					gridRepresentation += "\n";
					startingPoint = currentPosition.Y;
					gridRepresentation += GridVerticalScale[currentPosition.Y];
				}

				if (currentGridCell is null)
					gridRepresentation += "-";
				else if (currentGridCell is Boat)
					gridRepresentation += $"{((Boat)currentGridCell).Representation}";
				else if (currentGridCell is Shot)
					gridRepresentation += "X";
				else
					throw new UnknownGridCellException();
			}
			return gridRepresentation;
		}
	}
}
