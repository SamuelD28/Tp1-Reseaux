using System;
using System.Collections.Generic;
using static System.Console;

namespace Tp1_Reseaux
{
    public static class Draw
    {
        /// <summary>
        /// Method that display the launch message in the console
        /// </summary>
        public static void DrawLaunchMessage()
        {
            string launchMessage = "Launching";
            Write(launchMessage);
            for (int i = 0; i < 8; i++)
            {
                Write(".");
                System.Threading.Thread.Sleep(250);
            }
            Clear();
        }

        /// <summary>
        /// Method that display the header of the game
        /// </summary>
        private static void DrawHeader(string player)
        {
            WriteLine("╔══════════════════════════════════════════════════════════════════════╗"); //72
            Console.ForegroundColor = ConsoleColor.Gray;
            Write("║");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Write("┴┬┴┬┴┬┴┬┴┬┴┬┴┬┴┬┴┬┴┬┴┬┴┬┴┬┴┬┤ ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Write("BATTLESHIP");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Write(" ├┬┴┬┴┬┴┬┴┬┴┬┴┬┴┬┴┬┴┬┴┬┴┬┴┬┴┬┴");
            Console.ForegroundColor = ConsoleColor.Gray;
            Write("║");
            WriteLine("\n╚═══════════════════════════════╗      ╔═══════════════════════════════╝");
            WriteLine($"                                ║  {player}  ║                               ");
            WriteLine("                                ╚══════╝                               ");
            WriteLine();
            Console.ResetColor();
        }

        /// <summary>
        /// Method that display the grids of the game
        /// </summary>
        /// <param name="gameGrid">Grids of your boat</param>
        /// <param name="shootingGrid">Grids of your previous shots</param>
        private static void DrawGrids(Grid gameGrid, Grid shootingGrid)
        {
            DrawGridHeader("The Boats");
            DrawGrid(gameGrid, 0, CursorTop);
            
            SetCursorPosition(49, 6);
            DrawGridHeader("The Shots");
            DrawGrid(shootingGrid, 49, 7);

            Console.ResetColor();
            WriteLine();
        }

        /// <summary>
        /// Method that display the header of the grids
        /// </summary>
        /// <param name="gridHeader">The text you need to display</param>
        public static void DrawGridHeader(string gridHeader)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Write(" ───── ");
            Console.ResetColor();
            Write(gridHeader);
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Write(" ──────\n");
            Console.ResetColor();
        }

        /// <summary>
        /// Method that display the footer of the grids
        /// </summary>
        public static void DrawGridFooter()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Write(" ──────────────────────");
            Console.ResetColor();
        }

        /// <summary>
        /// Method that display a single grid
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="indentLeft"></param>
        /// <param name="indentTop"></param>
        public static void DrawGrid(Grid grid, int indentLeft, int indentTop)
        {
            int temp = indentLeft;
            int startingPoint = 0;

            CursorTop = indentTop;
            SetCursorPosition(indentLeft, CursorTop);

            ForegroundColor = ConsoleColor.DarkGray;

            Write(" ▒  " + String.Join(" ", Grid.GridHorizontalScale));
            CursorTop = CursorTop + 1;
            SetCursorPosition(indentLeft, CursorTop);
            Write(" " + Grid.GridVerticalScale[startingPoint]);

            foreach (KeyValuePair<Position, object> keyValuePair in grid.GridTable)
            {
                ForegroundColor = ConsoleColor.DarkGray;
                object currentGridCell = keyValuePair.Value;
                Position currentPosition = keyValuePair.Key;

                if (currentPosition.Y != startingPoint)
                {
                    CursorTop = CursorTop + 1;
                    SetCursorPosition(indentLeft, CursorTop);

                    startingPoint = currentPosition.Y;
                    Write(" " + Grid.GridVerticalScale[currentPosition.Y]);
                }


                if (currentGridCell is null)
                    Write(" -");
                else if (currentGridCell is Boat)
                {
                    ForegroundColor = ConsoleColor.White;
                    Write($" {((Boat)currentGridCell).Representation}");
                }
                else if (currentGridCell is Shot)
                {
                    Shot shot = (Shot)currentGridCell;
                    if (shot.Result == ShotResult.Hit || shot.Result == ShotResult.Sunk)
                    {
                        ForegroundColor = ConsoleColor.DarkRed;
                        Write(" X");
                    }
                    else
                    {
                        ForegroundColor = ConsoleColor.White;
                        Write(" o");
                    }
                }
                else
                    throw new UnknownGridCellException();

            }

            CursorTop = CursorTop + 1;
            SetCursorPosition(indentLeft, CursorTop);
            DrawGridFooter();
        }

        /// <summary>
        /// Method that display header and grids
        /// </summary>
        /// <param name="player">Player to display</param>
        /// <param name="gameGrid">The grid with your boat</param>
        /// <param name="shootingGrid">The grid of your previous shots</param>
        public static void DrawBoard(string player, Grid gameGrid, Grid shootingGrid)
        {
            DrawHeader(player);
            DrawGrids(gameGrid, shootingGrid);
        }

        /// <summary>
        /// Method that display informations and the current state of the game
        /// </summary>
        /// <param name="header">Top information to display</param>
        /// <param name="body">Bottom information to display</param>
        /// <param name="state">State of the game</param>
        public static void DrawInformationBox(string header, string body, string state)
        {
            Write("\n");
            WriteLine($"╔".PadRight(72, '═') + "╗\n" +
                      $"║ Status : {state}".PadRight(72) + "║\n" +
                      $"║".PadRight(72) + "║\n" +
                      $"║ {header}".PadRight(72) + "║\n" +
                      $"║ {body}".PadRight(72) + "║\n" +
                      $"╚".PadRight(72, '═') + "╝\n");
        }

        /// <summary>
        /// Method that display logs of the game 
        /// </summary>
        /// <param name="color">Color of the logs in console</param>
        /// <param name="message">Message to display in console</param>
        public static void DrawLog(ConsoleColor color, string message)
        {
            ForegroundColor = color;
            WriteLine(message);
            ResetColor();
        }
    }
}
