using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace Tp1_Reseaux
{
    class Program
    {
        private static int x_axis = 10;
        private static int y_axis = 10;
        private static bool[,] gameGrid = new bool[x_axis, y_axis];
        private static List<Position> touchedPosition = new List<Position>();

        static void Main(string[] args)
        {
            Grid.StartGame();


            //AddBoat(new Position(1, 2), new Position(1, 4));
            //AddBoat(new Position(2, 5), new Position(5, 5));
            //AddTouchedPosition(6, 6);
            //ShowGameGrid();

            //AddBoats();

        }

        //private static void AddTouchedPosition(int c1, int c2)
        //{
        //    touchedPosition.Add(new Position(c1, c2));
        //}

        private static void AddBoats()
        {
            //List<Boat> boats = new List<Boat>();
            //Boats.PorteAvions = new Boat(Boat.BoatType.Torpilleur);

            //if (Boats.PorteAvions == Boats.PorteAvions)
            //{
            //    WriteLine("DHHSHSHSH");
            //}

            //WriteLine("----PLACEMENT DES BATEAUX----");

            //foreach (var item in )
            //{

            //}


        }

        private static void AddBoat(Position start, Position end)
        {
            if (start.X == end.X)
            {
                for (int i = start.Y; i <= end.Y; i++)
                {
                    gameGrid[start.X, i] = true;
                }
            }

            if (start.Y == end.Y)
            {
                for (int i = start.X; i <= end.X; i++)
                {
                    gameGrid[i, start.Y] = true;
                }
            }
        }

        private static void ShowGameGrid()
        {
            Write("  A B C D E F G H I J\n");
            for (int i = 0; i < x_axis; i++)
            {
                Write($"{i + 1} ");
                for (int j = 0; j < y_axis; j++)
                {
                    int[,] pos = new int[j, i];

                    if (touchedPosition.Any(p => p.X == j && p.Y == i))
                    {
                        Write("X ");
                    }
                    else if (gameGrid[j, i])
                    {
                        Write("O ");
                    }
                    else
                        Write(". ");
                }
                Write("\n");
            }
        }
    }
}








//List<bool> positions = new List<bool>();

//string test = "A1_A3";

//string[] coordonnées = test.Split('_');

//string x1 = coordonnées[0].Substring(0, 1);
//string y1 = coordonnées[0].Substring(1, 1);

//string x2 = coordonnées[1].Substring(0, 1);
//string y2 = coordonnées[1].Substring(1, 1);

//WriteLine(coordonnées[0]);

//WriteLine(x1);
//WriteLine(y1);
//WriteLine(x2);
//WriteLine(y2);