﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public PositionFlow Flow { get; set; }

        private Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Position Create(int x, int y)
        {
            if (!PositionWithinBound(x, y))
            {
                return null;
            }

            return new Position(x, y);
        }

        public static Position Create(string position)
        {
            int[] values = ParseTextualToXY(position.ToUpper());

            if (!PositionWithinBound(values[0], values[1]))
            {
                return null;
            }

            return new Position(values[0], values[1]);
        }

        public void Assign(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int? Difference(Position second)
        {
            if (X == second.X && Y != second.Y)
            {
                Flow = PositionFlow.Vertical;
                second.Flow = PositionFlow.Vertical;
                return Math.Abs(Y - second.Y);
            }
            else if (Y == second.Y && X != second.X)
            {
                Flow = PositionFlow.Horizontal;
                second.Flow = PositionFlow.Horizontal;
                return Math.Abs(X - second.X);
            }
            else
                return null;
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
            int x = Grid.GridHorizontalScale.ToList().IndexOf(position[0]);

            int y = int.Parse(position.Substring(1)) - 1;

            return new int[2] { x, y };
        }

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

        public static Position GetClosestPosition(Position start, Position end)
        {
            int distanceStart = start.X + start.Y;
            int distanceEnd = end.X + end.Y;

            if (distanceStart < distanceEnd)
                return start;
            else
                return end;
        }
    }
}
