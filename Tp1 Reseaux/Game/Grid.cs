using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;


namespace Tp1_Reseaux
{
	/// <summary>
	/// On veut enlever tout les affichages dans la classe pour 
	/// la rendre independante du frontend utiliser (dans notre cas une console).
	/// Les positions devront etre conserver dans les bateaux et non
	/// une liste directement dans la grille. La liste representant la 
	/// grille est maintenant un dictionnaire, plus simple et pratique.
	/// Il faut voir la classe comme etant un object que l'on vas utiliser
	/// de lexterieur de celle-ci.
	/// </summary>
	public sealed class Grid
    {
        public static readonly int GridSize = 10;
        public static readonly char[] GridHorizontalScale = new char[]{ 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J'};

		private Dictionary<Position, object> GridTable = new Dictionary<Position, object>();

		public static Grid Instance = null;

		public static Grid GetInstance() => (Instance is null) ? new Grid() : Instance;

		private Grid(){
			Init();
		}

		private void Init()
		{
			for(int y = 0; y < GridSize; y++)
			{
				for(int x = 0; x < GridSize ; x++)
				{
					GridTable.Add(Position.Create(x, y), null);
				}
			}
		}

		private bool AddBoat(BoatType type,  Position start, Position end)
		{
			Boat boat = new Boat(type);

			int? difference = start.Difference(end);

			if (difference != boat.NbVies)
				return false;

			Position startingPoint = Position.GetClosestPosition(start, end);

			for(int i = 0; i < difference ; i++)
			{
				Position key = GridTable.Keys.First(p => p.Equals(startingPoint));
				GridTable[key] = boat;

				if (startingPoint.Flow == PositionFlow.Horizontal)
					startingPoint.Assign(startingPoint.X + 1, startingPoint.Y);
				else
					startingPoint.Assign(startingPoint.X, startingPoint.Y + 1);
			}

			return true;
		}

		public bool AddShot(Position shot)
		{
			Position key = GridTable.Keys.First(p => p.Equals(shot));

			object result;
			GridTable.TryGetValue(key , out result);

			if(result is Boat)
			{
				Boat boat = (Boat)result;
				boat.NbVies = boat.NbVies - 1;
			}

			//A repenser
			GridTable[key] = new Shot(key);

			return true;
		}

		public override string ToString()
		{
			//----Utiliser pour tester----// // 1 , 0
			AddBoat(BoatType.AircraftCarrier , Position.Create("B1"), Position.Create(6, 0));
			AddShot(Position.Create(2,0));

			string gridRepresentation = "";
			int startingPoint = 0;

			foreach(KeyValuePair<Position, object> keyValuePair in GridTable)
			{
				object currentGridCell = keyValuePair.Value;
				Position currentPosition = keyValuePair.Key;

				if(currentPosition.Y != startingPoint)
				{
					gridRepresentation += "\n";
					startingPoint = currentPosition.Y;
				}

				if (currentGridCell is null)
					gridRepresentation += ".";
				else if (currentGridCell is Boat)
					gridRepresentation += "O";
				else if(currentGridCell is Shot)
					gridRepresentation += "X";
			}
			return gridRepresentation;
		}
    }
}
