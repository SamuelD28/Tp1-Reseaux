using System;
using System.Linq;

namespace Tp1_Reseaux
{
	public enum PositionFlow
    {
        Horizontal,
        Vertical
    }

    /// <summary>
    /// La classe position contiendras TOUTE la logique pour
    /// determiner si une position est valide ou non. Elle leveras
    /// les exception ou les erreur necessaire pour le signaler 
    /// a celui qui en fait appel. Cest ici que lon devrait gerer ou
    /// non la conversion de -1. A revoir
    /// </summary>
    public class Position
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public PositionFlow Flow { get; private set; }

        private Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Position Create(int x, int y)
        {
            if (!PositionWithinBound(x, y))
                return null;

            return new Position(x, y);
        }

        public static Position Create(string position)
        {
            int[] values = ParseTextualToXY(position.ToUpper());

            if (!PositionWithinBound(values[0], values[1]))
                return null;

            return new Position(values[0], values[1]);
        }

        public bool Assign(int x, int y)
        {
			if (PositionWithinBound(x, y))
			{
				X = x;
				Y = y;
				return false;
			}
			else
				return false;
        }

        public int? Difference(Position second)
        {
            if (X == second.X && Y != second.Y)
                return Math.Abs(Y - second.Y) + 1; //We add one to account for the first position not beeing included
            else if (Y == second.Y && X != second.X)
                return Math.Abs(X - second.X) + 1; //We add one to account for the first position not beeing included
            else
                return null;
        }

		public static bool AssignFlow(Position firstPoint, Position secondPoint)
		{
			if (firstPoint.X == secondPoint.X && firstPoint.Y != secondPoint.Y)
			{
				firstPoint.Flow = PositionFlow.Vertical;
				secondPoint.Flow = PositionFlow.Vertical;
				return true;
			}
			else if (firstPoint.Y == secondPoint.Y && firstPoint.X != secondPoint.X)
			{
				firstPoint.Flow = PositionFlow.Horizontal;
				secondPoint.Flow = PositionFlow.Horizontal;
				return true;
			}
			else
				return false;
		}

		public Position Clone()
		{
			Position clone = new Position(X, Y);
			clone.Flow = Flow;
			return clone;
		}

        public static bool PositionWithinBound(int x, int y)
        {
            if (x > Grid.GridSize || x < 0 || y > Grid.GridSize || y < 0)
            {
                return false;
            }
            return true;
        }

        public static int[] ParseTextualToXY(string position)
        {
			string trimmed = position.Replace(" ", String.Empty);

			if (trimmed.Length < 2 || trimmed.Length > 3)
				return new int[] { -1, -1 };

            int x = Grid.GridHorizontalScale.ToList().IndexOf(position[0]);

			int y;
            int.TryParse(position.Substring(1), out y);
			y = y - 1;

            return new int[2] { x, y};
        }

        public static Position GetClosestPosition(Position start, Position end)
        {
            int distanceStart = start.X + start.Y;
            int distanceEnd = end.X + end.Y;

            if (distanceStart < distanceEnd)
                return start.Clone();
            else
                return end.Clone();
        }

		public override string ToString() => $"{Grid.GridHorizontalScale[X]}{Y + 1}"; 

        public override int GetHashCode()
        {
            return X * 31 + Y * 31;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Position))
                return false;

            Position position = (Position)obj;

            return Y == position.Y && X == position.X;
        }
	}
}
