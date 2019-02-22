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
        static void Main(string[] args)
        {
            Game game = Game.GetInstance();

            game.Setup();

            game.Start();

            Grid GameGrid = Grid.GetInstance();

            Console.Write(GameGrid.ToString());
        }
    }
}