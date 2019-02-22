using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tp1_Reseaux
{
	public sealed class Game
	{
		private static readonly string DEFAULT_IP = "127.0.0.1";
		private static readonly int DEFAULT_PORT = 8888;

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

		private Client Client = Client.GetInstance();

		public string CurrentPlayer => (Client.PlayerAssigned != 0)? "Joueur" + Client.PlayerAssigned: "Non-disponible";
		public string CurrentIp { get; private set; }
		public int CurrentPort { get; private set; }
		public GameState CurrentState { get; private set; }

		private static Game Instance = null;

		public static Game GetInstance() => (Instance is null) ? Instance = new Game() : Instance;

		private Game() {
			CurrentIp = String.Empty;
			CurrentPort = default(int);
		}

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

		private void PlaceBoat()
		{
			//Start the placement of boats
		}

		private void PlaceShot()
		{
			string shotPosition = Console.ReadLine();
			PlaceShot shotRequest = new PlaceShot(shotPosition);
		}
	}
}
