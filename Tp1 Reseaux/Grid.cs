using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;


namespace Tp1_Reseaux
{
    public class Grid
    {
        private static int x_axis = 10;
        private static int y_axis = 10;
        private static bool[,] gameGrid = new bool[10, 10];
        private static List<Boat> allBoats;
        private static List<Position> touchedPosition = new List<Position>();
        private static List<char> validChar = new List<char>() { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j' };

        private Grid()
        {

        }

        public static void StartGame()
        {
            AddAllBoats();
            ShowGrid();
        }

        private void AddTouchedPosition(Position position)
        {
            touchedPosition.Add(position);
        }

        private static void AddAllBoats()
        {
            allBoats = new List<Boat>() { new AircraftCarrier() /**, new Cruiser(),
                new CounterTorpedo(), new SubMarine(), new Torpedo() */ };

            foreach (var boat in allBoats)
            {
                WriteLine($"--- Coordonnées de votre {boat.Name} ---");

                bool positionStartValid = false;
                bool positionEndValid = false;
                string positionStart = "";
                string positionEnd = "";

                Position positionStartValidated = null;
                Position positionEndValidated = null;

                bool allValid = false;

                while (!allValid)
                {

                    while (!positionStartValid)
                    {
                        WriteLine("Veuillez entrer la position de début");
                        positionStart = ReadLine().ToLower();

                        positionStartValid = ValidatePosition(positionStart, out positionStartValidated/*, null, boat*/);
                    }

                    while (!positionEndValid)
                    {
                        WriteLine("Veuillez entrer la position de fin");
                        positionEnd = ReadLine().ToLower();

                        positionEndValid = ValidatePosition(positionEnd, out positionEndValidated/*, positionStart, boat*/);
                    }



                    if (positionStartValidated.X == positionEndValidated.X && positionStartValidated.Y != positionEndValidated.Y)
                    {
                        if (Math.Abs(positionStartValidated.Y - positionEndValidated.Y) + 1 != boat.Length)
                        {
                            positionStartValid = false;
                            positionEndValid = false;
                            WriteLine("Bateau de mauvaise longueur");
                            //continue;
                        }
                        else
                        {
                            allValid = true;

                        }


                        //else
                        //{
                        //    int cpt = 0;
                        //    for (int i = positionStartValidated.Y; i <= boat.Length; i++)
                        //    {
                        //        boat.Positions[cpt]= new Position(positionStartValidated.X, i);
                        //    }
                        //    allValid = true;
                        //}

                    }

                    if (positionStartValidated.Y == positionEndValidated.Y && positionStartValidated.X != positionEndValidated.X)
                    {
                        if (Math.Abs(positionStartValidated.X - positionEndValidated.X) + 1 != boat.Length)
                        {
                            positionStartValid = false;
                            positionEndValid = false;
                            WriteLine("Bateau de mauvaise longueur");
                            //continue;
                        }
                        else
                        {
                            allValid = true;

                        }

                        //else
                        //{
                        //    int cpt = 0;
                        //    for (int i = positionStartValidated.X; i <= boat.Length; i++)
                        //    {
                        //        boat.Positions[cpt] = new Position(positionStartValidated.Y, i);
                        //    }
                        //    allValid = true;
                        //}

                    }





                    //if (Math.Abs(positionStartValidated.X - positionEndValidated.X) + 1 != boat.Length ||
                    //    Math.Abs(positionStartValidated.Y - positionEndValidated.Y) + 1 != boat.Length)
                    //{
                    //    WriteLine("Bateau de mauvaise longeur");
                    //}

                    //else
                    //{

                    //    WriteLine("OK");
                    //}


                }

                WriteLine("OK");





                //if (start.X == end.X)
                //{
                //    for (int i = start.Y; i <= end.Y; i++)
                //    {
                //        gameGrid[start.X, i] = true;
                //    }
                //}

                //if (start.Y == end.Y)
                //{
                //    for (int i = start.X; i <= end.X; i++)
                //    {
                //        gameGrid[i, start.Y] = true;
                //    }
                //}


                AddBoat(positionStartValidated, positionEndValidated);
                //AddBoat(PositionBuilder(positionStart[0], positionStart[1].ToString()), 
                //    PositionBuilder(positionEnd[0], positionEnd[1].ToString()));

                boat.IsPlaced = true;


            }
        }

        private static Position PositionBuilder(char char1, int value)
        {
            //Position position = new Position(CharConverter(char1), value);

            int CharConverter(char character)
            {
                switch (character)
                {
                    case 'a': return 1;
                    case 'b': return 2;
                    case 'c': return 3;
                    case 'd': return 4;
                    case 'e': return 5;
                    case 'f': return 6;
                    case 'g': return 7;
                    case 'h': return 8;
                    case 'i': return 9;
                    case 'j': return 10;
                    default: return 0;
                }
            }

            return new Position(CharConverter(char1), value);
        }

        private static bool ValidatePosition(string position, out Position positionValidated /*, string position2, Boat boatToValidate*/)
        {

            positionValidated = null;

            if (!int.TryParse(position.Substring(1), out int secondPart))
            {
                return false;
            }

            if (!validChar.Contains(position[0]))
            {
                WriteLine("Veuillez respecter l'intervalle A - J");
                return false;
            }

            if (secondPart < 1 || secondPart > 10)
            {
                WriteLine("Veuillez respecter l'intervalle 1 - 10");
                return false;
            }

            //positionValidated = PositionBuilder(position[0], secondPart);

            positionValidated = new Position(validChar.IndexOf(position[0]) + 1, secondPart + 1);

            return true;
            //if (position2 != null)
            //{
            //    int value = int.Parse(position2.Substring(1));
            //    if (Math.Abs(secondPart - value) + 1 != boatToValidate.Length)
            //    {
            //        WriteLine("Bateau de mauvaise longeur");
            //        return false;
            //    }
            //    //else
            //    //{
            //    //    boatToValidate.Positions[0] = PositionBuilder(position2[0], position2.Substring(1));
            //    //    boatToValidate.Positions[0] = PositionBuilder(position2[0], position2.Substring(1));
            //    //    boatToValidate.Positions[0] = PositionBuilder(position2[0], position2.Substring(1));
            //    //}

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

        private static void ShowGrid()
        {
            //foreach (var b in allBoats)
            //{
            //    foreach (var p in b.Positions)
            //    {
            //        gameGrid[p.X, p.Y] = true;

            //    }


            //}

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
