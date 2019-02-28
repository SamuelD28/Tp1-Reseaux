using System;

namespace Tp1_Reseaux
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(
                Math.Min(75, Console.LargestWindowWidth),
                Math.Min(50, Console.LargestWindowHeight));

            Game.GetInstance().Setup();
            Game.GetInstance().Start();

		}
    }
}