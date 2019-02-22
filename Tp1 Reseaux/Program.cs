using System;
using System.Timers;

namespace Tp1_Reseaux
{
	class Program
	{

		static void Main()
		{
			new Program().Principal();
		}

		private void Principal()
		{
			Game game = Game.GetInstance();

			game.Setup();

			if (!game.Start())
				Console.WriteLine("Cant start the game.");
			
		}
	}
}
