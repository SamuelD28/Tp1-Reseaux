using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

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
			Defeat,
			Error,
			My_Turn,
			Waiting
		}

		//Contains the client instance used for communication with the server
		private Client Client = Client.GetInstance();

		private List<Shot> MyShotHistory = new List<Shot>();

		private List<Shot> OpponentShotHistory = new List<Shot>();

		private Grid GameGrid = new Grid();

		private Grid ShootingGrid = new Grid();

		private static readonly List<Boat> Boats = new List<Boat>() {
				new Boat(BoatType.AircraftCarrier),
				new Boat(BoatType.Destroyer),
				new Boat(BoatType.Submarine),
				new Boat(BoatType.Torpedo),
				new Boat(BoatType.CounterTorpedo)
		};

		//USED ONLY FOR TESTING PURPOSE. Will be removed in production
		private static readonly Position[,] SEED_Positions = new Position[,] {
			{Position.Create("A1"), Position.Create("a5") },
			{Position.Create("b1"), Position.Create("b4") },
			{Position.Create("c1"), Position.Create("c3") },
			{Position.Create("d1"), Position.Create("d3") },
			{Position.Create("e1"), Position.Create("e2") }
		};

		//Return the player that was assigned by the server
		public string CurrentPlayer { get; private set; }

		//Contains the current ip used for the connection by the client
		public string CurrentIp { get; private set; }

		//Contains the current port used for the connection by the client
		public int CurrentPort { get; private set; }

		private bool SkipServerResponse { get; set; }

		//Contains the current state of the game
		public GameState CurrentState { get; private set; }

		private static Game Instance = null;

		/// <summary>
		/// Method used to obtain the instance of the game
		/// </summary>
		/// <returns></returns>
		public static Game GetInstance() => (Instance is null) ? Instance = new Game() : Instance;

		/// <summary>
		/// Private constructor for initiating a new game
		/// </summary>
		private Game()
		{
			CurrentIp = DEFAULT_IP;
			CurrentPort = DEFAULT_PORT;
		}

		//---Method for handling the game logic and flow--//

		/// <summary>
		/// Method used to retrieve the ip adress from the user
		/// </summary>
		private void GetIpAdress()
		{
			Write($"[{DateTime.Now}] Enter the IP Adress ({CurrentIp}) : ");
			string ipAdress = ReadLine();

			if (ipAdress.Trim().Length == 0)
				CurrentIp = DEFAULT_IP;
			else
				CurrentIp = ipAdress;

			DrawLog(ConsoleColor.DarkGreen, $"Using IP [{CurrentIp}]");
		}

		/// <summary>
		/// Method used to retireve the port number from the user
		/// </summary>
		private void GetPortNumber()
		{
			Write($"[{DateTime.Now}] Enter the Port Number ({CurrentPort}) : ");
			int portNumber;
			int.TryParse(ReadLine(), out portNumber);

			if (portNumber == default(int))
				CurrentPort = DEFAULT_PORT;
			else
				CurrentPort = portNumber;

			DrawLog(ConsoleColor.DarkGreen, $"Using Port [{CurrentPort}]");
		}

		/// <summary>
		/// Method used that initiate the setup for starting the game.
		/// </summary>
		public void Setup()
		{
			CurrentState = GameState.Setup;
			for (; ; )
			{
				GetIpAdress();
				GetPortNumber();
				try
				{
					Client.Connect(CurrentIp, CurrentPort);
					DrawLog(ConsoleColor.DarkGreen, "Connected to the server");
					CurrentPlayer = "J" + Client.Read();
					DrawLog(ConsoleColor.Blue, $"You are {CurrentPlayer}");
					CurrentState = GameState.Ready;
					break;
				}
				catch (Exception e)
				{
					DrawLog(ConsoleColor.Red, $"Cant connect to the server.\n{e.Message}");
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
				DrawLaunchMessage();
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
				Clear();
				DrawGrids();
				switch (CurrentState)
				{
					case GameState.Placing_Boat: HandleBoatPlacement(); break;
					case GameState.My_Turn: HandleMyTurn(); break;
					case GameState.Waiting: HandleWaiting(); break;
					case GameState.Victory: HandleVictory(); break;
					case GameState.Defeat: HandleDefeat(); break;
					case GameState.Error: break;
				}


				if (!SkipServerResponse)
					ReadServerResponse();
				else
				{
					CurrentState = GameState.Waiting;
					SkipServerResponse = false;
				}
			}
		}

		//---Methods that send information to the server---//

		private void SendBoatsToServer()
		{
			foreach (IBoat boat in Boats)
			{
				//Send request to the server
				Request.SetHeader(RequestType.Boat, boat.GetPlacement());
				Client.Send(Request.Message);

				//Read response from the server
				Response response = Request.ParseResponse(Client.Read());
				//Do a better handling of the request
				if (!response.Success)
					DrawLog(ConsoleColor.Red, response.Error);

			}
		}

		private Response SendShotToServer(Position position)
		{
			//Send request to the server
			Request.SetHeader(RequestType.Shot, position.ToString());
			Client.Send(Request.Message);

			//Read the response from the server. Have no choice otherwise the order is messed up
			Response response = Request.ParseResponse(Client.Read());
			//Do a better handling of the response in case of an error.
			if (!response.Success)
				DrawLog(ConsoleColor.Red, response.Error);

			if (response.ShotResult == ShotResult.Hit)
				AddShotToMyHistory(new Shot(position, true));
			else if (response.ShotResult == ShotResult.Missed)
			{
				SkipServerResponse = true;
				AddShotToMyHistory(new Shot(position, false));
			}
			else if (response.ShotResult == ShotResult.Sunk)
				AddShotToMyHistory(new Shot(position, true));

			return response;
		}

		//---Methods that get information from the player---//

		private Position GetPlayerPosition(string message)
		{
			Position position = null;
			while (position is null)
			{
				Write(message);
				position = Position.Create(ReadLine());

				if (position is null)
					WriteLine("Incorrect position. (Format [A-J][1-9][1-9])");
			}
			return position;
		}

		/// <summary>
		/// Method used to place boat for the game.
		/// </summary>
		private void GetPlayerBoats()
		{
			//Loop until all the boats are mark as placed
			while (Boats.Exists(b => !b.IsPlaced))
			{
				Boat boat = Boats.Find(b => !b.IsPlaced);
				DrawInformationBox($"Placing : {boat.Type.ToString()}", $"Length : {boat.LifePoints}");

				//----Used for testing purposes only-----//
				if (UseDefaultPlacements()) break;

				Position firstPosition = GetPlayerPosition("Enter First Coordinate : ");
				Position secondPosition = GetPlayerPosition("Enter Second Coordinate : ");

				if (GameGrid.AddBoat(boat, firstPosition, secondPosition))
					boat.AssignPlacement(firstPosition, secondPosition);
			}
		}

		/// <summary>
		/// Method used to handle the logic for sending shot.
		/// </summary>
		private Position GetPlayerShot()
		{
			Shot lastShotPlayerOne = (MyShotHistory.Count > 0) ? MyShotHistory.Last() : null;
			Shot lastShotPlayerTwo = (OpponentShotHistory.Count > 0) ? OpponentShotHistory.Last() : null;

			DrawInformationBox(
				$"Payer One Last Shot : {(lastShotPlayerOne != null ? lastShotPlayerOne.ToString() : "No shot yet")}",
				$"Player Two Last Shot : {(lastShotPlayerTwo != null ? lastShotPlayerTwo.ToString() : "No shot yet")}");

			return GetPlayerPosition("Enter Shooting Coordinate : ");
		}

		//---God Method that handle different game state--//

		private void HandleMyTurn()
		{
			Position position = GetPlayerShot();
			Response response = SendShotToServer(position);

			//Le serveur est juste inconsistent.
			//Il renvoie le tour du joueur si son tir est un succes. Ce qui
			//fait aucun sens honnetement. 
			//if (response.ShotResult != ShotResult.Missed)
			//	GetServerTurn(Client.Read());
		}

		private void HandleDefeat()
		{
			//Could do seomthing better
			DrawInformationBox("YOU LOST", "YOU LOST");
			WriteLine("Press a key to continue...");
		}

		private void HandleVictory()
		{
			//Could do something better
			DrawInformationBox("YOU WON", "YOU WON");
			WriteLine("Press a key to continue...");
		}

		private void HandleBoatPlacement()
		{
			GetPlayerBoats();
			SendBoatsToServer();
			SkipServerResponse = true; //Nasty shit
		}

		private void HandleWaiting()
		{
			//Do a better handling
			DrawInformationBox($"Player 1 : Ready", "Player 2 : Placing Boats");
		}

		//---Methods that retrieve information from the server--//

		private void ReadServerResponse()
		{
			string serverResponse = Client.Read();
			switch (Request.DetermineReceivedType(serverResponse))
			{
				case RequestType.Shot: GetServerShot(serverResponse); break;
				case RequestType.Turn: GetServerTurn(serverResponse); break;
				case RequestType.Won: GetServerGameResult(serverResponse); break;
				default: CurrentState = GameState.Waiting; break;
			}
		}

		private void GetServerTurn(string serverResponse = null)
		{
			Request.SetHeader(RequestType.Turn);

			Response response = new Response();

			if (serverResponse is null)
				response = Request.ParseResponse(Client.Read());
			else
				response = Request.ParseResponse(serverResponse);

			if (response.Player == CurrentPlayer)
				CurrentState = GameState.My_Turn;
			else
				CurrentState = GameState.Waiting;
		}

		private void GetServerShot(string serverResponse = null)
		{
			Request.SetHeader(RequestType.Shot);
			Response response = new Response();

			if (serverResponse is null)
				response = Request.ParseResponse(Client.Read());
			else
				response = Request.ParseResponse(serverResponse);

			AddShotToOpponentHistory(new Shot(response.Position));
		}

		private void GetServerGameResult(string serverResponse = null)
		{
			Request.SetHeader(RequestType.Won);
			Response response = new Response();

			if (serverResponse is null)
				response = Request.ParseResponse(Client.Read());
			else
				response = Request.ParseResponse(serverResponse);

			if (response.Player == CurrentPlayer)
				CurrentState = GameState.Victory;
			else
				CurrentState = GameState.Defeat;
		}

		private bool UseDefaultPlacements()
		{
			Write("Would you like all boats to be placed in defaults positions? [Y/N]");
			string response = ReadLine();

			if (response.Length > 0 && char.ToUpper(response[0]) == 'Y')
			{
				DrawLog(ConsoleColor.DarkGreen, "Placing boats...");
				for (int i = 0; i < Boats.Count; i++)
				{
					GameGrid.AddBoat(Boats[i], SEED_Positions[i, 0], SEED_Positions[i, 1]);
					Boats[i].AssignPlacement(SEED_Positions[i, 0], SEED_Positions[i, 1]);
				}
				return true;
			}
			else
				return false;
		}

		//---Methods that draws in the console. Refactor in static class---//

		/// <summary>
		/// Method that display the launch message in the console
		/// </summary>
		private void DrawLaunchMessage()
		{
			CurrentState = GameState.Launching;
			string launchMessage = "Launching";
			Write(launchMessage);
			for (int i = 0; i < 8; i++)
			{
				Write(".");
				System.Threading.Thread.Sleep(250);
			}
			Clear();
		}

		private void DrawGrids()
		{
			//Find a way to make them display next to each other
			WriteLine(GameGrid.ToString());
			WriteLine(ShootingGrid.ToString());
		}

		private void DrawInformationBox(string header, string body)
		{
			Write("\n");
			WriteLine($"┌".PadRight(50, '─') + "┐\n" +
					  $"|{CurrentState}".PadRight(50) + "|\n" +
					  $"|".PadRight(50) + "|\n" +
					  $"|{header}".PadRight(50) + "|\n" +
					  $"|{body}".PadRight(50) + "|\n" +
					  $"└".PadRight(50, '─') + "┘\n");
		}

		private void DrawLog(ConsoleColor color, string message)
		{
			ForegroundColor = color;
			WriteLine(message);
			ResetColor();
		}

		//---Various other methods used by the class---//

		private void AddShotToMyHistory(Shot shot)
		{
			MyShotHistory.Add(shot);
			ShootingGrid.AddShot(shot.Position);
		}

		private void AddShotToOpponentHistory(Shot shot)
		{
			OpponentShotHistory.Add(shot);
			GameGrid.AddShot(shot.Position);
		}

	}
}
