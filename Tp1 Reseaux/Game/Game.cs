using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tp1_Reseaux
{
	public sealed class Game
	{
		//Default ip adress used if no ip adress is provided by the user
		public static readonly string DEFAULT_IP = "127.0.0.1";

		//Default port number used if no port number is provided by the user
		public static readonly int DEFAULT_PORT = 8888;

		/// <summary>
		/// Enumeration for the different a state can be in
		/// </summary>
		public enum GameState
		{
			Setup,
			Ready,
			Launching,
			Placing_Boat,
			Victory,
			Error,
			My_Turn,
			Not_My_Turn,
			Waiting
		}

		//Contains the client instance used for communication with the server
		private Client Client = Client.GetInstance();

		//Return the player that was assigned by the server
		public string CurrentPlayer => (Client.PlayerAssigned != 0)? "Joueur" + Client.PlayerAssigned: "Non-disponible";

		//Contains the current ip used for the connection by the client
		public string CurrentIp { get; private set; }

		//Contains the current port used for the connection by the client
		public int CurrentPort { get; private set; }

		//Contains the current state of the game
		public GameState CurrentState { get; private set; }

		//Contains the current instance of the class
		private static Game Instance = null;

		/// <summary>
		/// Method used to obtain the instance of the game
		/// </summary>
		/// <returns></returns>
		public static Game GetInstance() => (Instance is null) ? Instance = new Game() : Instance;

		/// <summary>
		/// Private constructor for initiating a new game
		/// </summary>
		private Game() {
			CurrentIp = string.Empty;
			CurrentPort = default(int);
		}

		/// <summary>
		/// Method used to retrieve the ip adress from the user
		/// </summary>
		private void GetIpAdress()
		{
			Console.Write("Enter the IP Adress : ");
			CurrentIp = Console.ReadLine();

			if (CurrentIp.Trim().Length == 0)
			{
				Console.WriteLine($"Using Default ({DEFAULT_IP})");
				CurrentIp = DEFAULT_IP;
			}
		}

		/// <summary>
		/// Method used to retireve the port number from the user
		/// </summary>
		private void GetPortNumber()
		{
			Console.Write("Enter the Port Number: ");
			int portNumber;
			int.TryParse(Console.ReadLine(), out portNumber);

			if (portNumber == default(int))
			{
				Console.WriteLine($"Using default ({DEFAULT_PORT})");
				CurrentPort = DEFAULT_PORT;
			}
			else
				CurrentPort = portNumber;
		}

		/// <summary>
		/// Method that display the launch message in the console
		/// </summary>
		private void DisplayLaunchMessage()
		{
			CurrentState = GameState.Launching;
			string launchMessage = "Launching";
			Console.Write(launchMessage);
			for (int i = 0; i < 8; i++)
			{
				Console.Write(".");
				System.Threading.Thread.Sleep(250);
			}
			Console.Clear();
		}

		/// <summary>
		/// Method used that initiate the setup for starting the game.
		/// </summary>
		public void Setup()
		{
			CurrentState = GameState.Setup;
			for (; ;)
			{
				GetIpAdress();
				GetPortNumber();
				try
				{
					Client.Connect(CurrentIp, CurrentPort);
					Console.WriteLine("Connected to the server");
					CurrentState = GameState.Ready;
					break;
				}
				catch (Exception e)
				{
					Console.WriteLine($"Cant connect to the server.\n{e.Message}");
				}
			}
		}

		/// <summary>
		/// Method that start the game.
		/// </summary>
		/// <returns>True if the game successfully started</returns>
		public bool Start()
		{
			if (Client.IsOpen && CurrentState == GameState.Ready)
			{
				DisplayLaunchMessage();
				CurrentState = GameState.Placing_Boat;
				Play();
				return true;
			}

			return false;
		}

		/// <summary>
		/// Method used for handling the logic for
		/// playing the game.
		/// </summary>
		private void Play()
		{
			while (Client.IsOpen)
			{
				switch (CurrentState)
				{
					case GameState.Placing_Boat: PlaceBoat(); break;
					case GameState.My_Turn: PlaceShot(); break;
					case GameState.Not_My_Turn: break;
					case GameState.Error: break;
					case GameState.Victory: break;
				}
			}
		}

		/// <summary>
		/// Method used to place boat for the game.
		/// </summary>
		private void PlaceBoat()
		{
			Grid grid = Grid.GetInstance();

			Console.Write(grid.ToString());

			Console.ReadLine();
		}

		/// <summary>
		/// Method used to handle the logic for sending shot.
		/// </summary>
		private void PlaceShot()
		{
			string shotPosition = Console.ReadLine();
			PlaceShot shotRequest = new PlaceShot(shotPosition);
		}
	}
}
