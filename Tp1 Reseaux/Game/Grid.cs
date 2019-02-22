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
		public enum GridCell
		{
			Empty,
			Boat,
			Shot
		}

        private static readonly int GridSize = 10;
        private static readonly char[] GridHorizontalScale = new char[]{ 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J'};

		private Dictionary<Position, GridCell> GridTable = new Dictionary<Position, GridCell>();

        public List<Boat> Boats { get; private set; }

		//Singleton Class
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
					GridTable.Add(new Position(x, y), GridCell.Empty);
				}
			}
		}

		//private void AddAllBoats()
		//{
		//    Boats = new List<Boat>() { new AircraftCarrier() /**, new Cruiser(),
		//        new CounterTorpedo(), new SubMarine(), new Torpedo() */ };

		//    foreach (var boat in allBoats)
		//    {
		//        WriteLine($"--- Coordonnées de votre {boat.Name} ---");

		//        bool positionStartValid = false;
		//        bool positionEndValid = false;
		//        string positionStart = "";
		//        string positionEnd = "";

		//        Position positionStartValidated = null;
		//        Position positionEndValidated = null;

		//        bool allValid = false;

		//        while (!allValid)
		//        {

		//            while (!positionStartValid)
		//            {
		//                WriteLine("Veuillez entrer la position de début");
		//                positionStart = ReadLine().ToLower();

		//                positionStartValid = ValidatePosition(positionStart, out positionStartValidated/*, null, boat*/);
		//            }

		//            while (!positionEndValid)
		//            {
		//                WriteLine("Veuillez entrer la position de fin");
		//                positionEnd = ReadLine().ToLower();

		//                positionEndValid = ValidatePosition(positionEnd, out positionEndValidated/*, positionStart, boat*/);
		//            }

		//            if (positionStartValidated.X == positionEndValidated.X && positionStartValidated.Y != positionEndValidated.Y)
		//            {
		//                if (Math.Abs(positionStartValidated.Y - positionEndValidated.Y) + 1 != boat.Length)
		//                {
		//                    positionStartValid = false;
		//                    positionEndValid = false;
		//                    WriteLine("Bateau de mauvaise longueur");
		//                }
		//                else
		//                    allValid = true;
		//            }

		//            if (positionStartValidated.Y == positionEndValidated.Y && positionStartValidated.X != positionEndValidated.X)
		//            {
		//                if (Math.Abs(positionStartValidated.X - positionEndValidated.X) + 1 != boat.Length)
		//                {
		//                    positionStartValid = false;
		//                    positionEndValid = false;
		//                    WriteLine("Bateau de mauvaise longueur");
		//                }
		//                else
		//                    allValid = true;
		//            }
		//        }
		//        AddBoat(positionStartValidated, positionEndValidated);
		//        boat.IsPlaced = true;
		//    }
		//}

		private void AddBoat(Position start, Position end)
		{
			Position key1 = GridTable.Keys.First(g => g.X == start.X && g.Y == start.Y);
			Position key2 = GridTable.Keys.First(g => g.X == end.X && g.Y == end.Y);
			GridTable[key1] = GridCell.Boat;
			GridTable[key2] = GridCell.Boat;
		}

		public override string ToString()
		{
			//Utiliser pour tester
			AddBoat(new Position(1, 1), new Position(3, 6));

			string gridRepresentation = "";
			int startingPoint = 0;
			foreach(KeyValuePair<Position, GridCell> keyValuePair in GridTable)
			{
				GridCell currentGridCell = keyValuePair.Value;
				Position currentPosition = keyValuePair.Key;

				if(currentPosition.Y != startingPoint)
				{
					gridRepresentation += "\n";
					startingPoint = currentPosition.Y;
				}

				if (currentGridCell == GridCell.Boat)
					gridRepresentation += "O";
				else if (currentGridCell == GridCell.Empty)
					gridRepresentation += ".";
				else if (currentGridCell == GridCell.Shot)
					gridRepresentation += "X";
			}
			return gridRepresentation;
		}
    }
}
