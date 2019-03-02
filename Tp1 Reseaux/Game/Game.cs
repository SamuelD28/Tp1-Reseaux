using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;
using System.IO;
using static Tp1_Reseaux.Draw;

namespace Tp1_Reseaux
{
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
		Waiting,
		Restart
	}

	public sealed class Game
	{
		//Default ip adress used if no ip adress is provided by the user
		public static readonly string DEFAULT_IP = "127.0.0.1";

		//Default port number used if no port number is provided by the user
		public static readonly int DEFAULT_PORT = 8888;

		//Contains the client instance used for communication with the server
		private Client Client = Client.GetInstance();

		//Contains the list of all the shot the player did so far.
		private List<Shot> MyShotHistory = new List<Shot>();

		//Contains the list of all the shot made by the opponent
		private List<Shot> OpponentShotHistory = new List<Shot>();

		//Contains the boats and shots made by the opponent
		private Grid GameGrid = new Grid();

		//Grid used to display the shot made by the player
		private Grid ShootingGrid = new Grid();

		//Contains the list of all the boats available to the player
		private static readonly List<Boat> Boats = new List<Boat>() {
				new Boat(BoatType.AircraftCarrier),
				new Boat(BoatType.Destroyer),
				new Boat(BoatType.Submarine),
				new Boat(BoatType.Torpedo),
				new Boat(BoatType.CounterTorpedo)
		};

		//USED ONLY FOR TESTING PURPOSE. Default position for the boats.
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

		//Used to skip the reading the server
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
			CurrentState = GameState.Setup;
			CurrentIp = DEFAULT_IP;
			CurrentPort = DEFAULT_PORT;
		}

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

			Draw.DrawLog(ConsoleColor.DarkGreen, $"Using IP [{CurrentIp}]");
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

			Draw.DrawLog(ConsoleColor.DarkGreen, $"Using Port [{CurrentPort}]");
		}

		/// <summary>
		/// Method used that initiate the setup for starting the game.
		/// </summary>
		public void Setup()
		{
			Clear();
			CurrentState = GameState.Setup;
			for (; ; )
			{
				GetIpAdress();
				GetPortNumber();
				try
				{
					Client.Connect(CurrentIp, CurrentPort);
					Draw.DrawLog(ConsoleColor.DarkGreen, "Connected to the server");
					CurrentPlayer = "J" + Client.Read();
					Draw.DrawLog(ConsoleColor.Blue, $"You are {CurrentPlayer}");
					CurrentState = GameState.Ready;
					break;
				}
				catch (Exception e)
				{
					Draw.DrawLog(ConsoleColor.Red, $"Cant connect to the server.\n{e.Message}");
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
				Draw.DrawLaunchMessage();
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
			while (Client.IsOpen || CurrentState == GameState.Restart)
			{
				Clear();
				DrawBoard(CurrentPlayer, GameGrid, ShootingGrid);
				try
				{
					switch (CurrentState)
					{
						case GameState.Placing_Boat: HandleBoatPlacement(); break;
						case GameState.My_Turn: HandleMyTurn(); break;
						case GameState.Waiting: HandleWaiting(); break;
						case GameState.Victory: HandleVictory(); break;
						case GameState.Defeat: HandleDefeat(); break;
						case GameState.Restart: HandleRestart(); break;
						default: HandleWaiting(); break;
					}
					if (!SkipServerResponse)
						ReadServerResponse();
					else
					{
						CurrentState = GameState.Waiting;
						SkipServerResponse = false;
					}
				}
				catch (ServerResponseException)
				{
					DrawLog(ConsoleColor.Red, "Error processing response from the server");
					ReadLine();
				}
				catch (RequestMalformedException)
				{
					DrawLog(ConsoleColor.Red, "Error processing the request to the server.");
					ReadLine();
				}
				catch (TimeoutException)
				{
					HandleServerIssue("Server could not respond in time");
				}
				catch (IOException)
				{
					HandleServerIssue("Lost Connection with the server");
				}
			}

			HandleClose();
		}

		/// <summary>
		/// Method for handling the closing of the game client
		/// </summary>
		private void HandleClose()
		{
			DrawLog(ConsoleColor.Yellow, $"\n[{DateTime.Now}] Game closed.");
		}

		/// <summary>
		/// method for handling an issue from the server.
		/// </summary>
		/// <param name="message"></param>
		private void HandleServerIssue(string message)
		{
			DrawLog(ConsoleColor.Red, $"[{DateTime.Now}] {message}");
			Write($"[{DateTime.Now}] Would you like to reconnect? [Y/N] : ");
			char answer = ReadKey().KeyChar;

			if (Char.ToUpper(answer) == 'Y')
				CurrentState = GameState.Restart;
		}

		/// <summary>
		/// Handler method for restrating the game. We can preserve the boats placement
		/// by not the shots.
		/// </summary>
		private void HandleRestart()
		{
			MyShotHistory.Clear();
			OpponentShotHistory.Clear();
			Setup();
			Start();
		}

		/// <summary>
		/// Handler method for player turn. Retrieve and send a shot to the server
		/// </summary>
		private void HandleMyTurn()
		{
			Position position = GetPlayerShot();
			Response response = SendShotToServer(position);
		}

		/// <summary>
		/// Method tha handle the defeat of the player.
		/// </summary>
		private void HandleDefeat()
		{
			DrawInformationBox("YOU LOST", "", CurrentState.ToString());
			Environment.Exit(0);
		}

		/// <summary>
		/// Method that handle the victory of the player
		/// </summary>
		private void HandleVictory()
		{
			DrawInformationBox("YOU WON", "", CurrentState.ToString());
			Environment.Exit(0);
		}

		/// <summary>
		/// Method that handle the placement of the boats
		/// </summary>
		private void HandleBoatPlacement()
		{
			GetPlayerBoats();
			SendBoatsToServer();
			SkipServerResponse = true;
		}

		/// <summary>
		/// Method that handle the waiting state of the game
		/// </summary>
		private void HandleWaiting()
		{
			//Do a better handling
			DrawInformationBox($"Wait your turn", "", CurrentState.ToString());
		}

		/// <summary>
		/// Method that send the complete list of boats to the server
		/// </summary>
		private void SendBoatsToServer()
		{
			foreach (IBoat boat in Boats)
			{
				try
				{
					//Send request to the server
					Request.SetHeader(RequestType.Boat, boat.GetPlacement());
					Client.Send(Request.Message);

					//Read response from the server
					Response response = Request.ParseResponse(Client.Read());
					//Do a better handling of the request
					if (!response.Success)
						Draw.DrawLog(ConsoleColor.Red, response.Error);
				}
				catch(BoatNotPlacedException b)
				{
					DrawLog(ConsoleColor.Red, $"The boat {b.GetType()} is not placed. Be sure to place all the boats.");
				}
			}
		}

		/// <summary>
		/// Method that send a shot to the server.
		/// </summary>
		/// <param name="position">Coordinate of the shot</param>
		/// <returns>Response object containing the parsed response from the server.</returns>
		private Response SendShotToServer(Position position)
		{
			//Send request to the server
			Request.SetHeader(RequestType.Shot, position.ToString());
			Client.Send(Request.Message);

			//Read the response from the server. 
			Response response = Request.ParseResponse(Client.Read());

			//Do a better handling of the response in case of an error.
			if (!response.Success)
				DrawLog(ConsoleColor.Red, response.Error);

			//Read the shot result and do the proper action
			if (response.ShotResult == ShotResult.Hit)
				AddShotToMyHistory(new Shot(position, ShotResult.Hit));
			else if (response.ShotResult == ShotResult.Missed)
			{
				SkipServerResponse = true;
				AddShotToMyHistory(new Shot(position, ShotResult.Missed));
			}
			else if (response.ShotResult == ShotResult.Sunk)
				AddShotToMyHistory(new Shot(position, ShotResult.Sunk));

			return response;
		}

		/// <summary>
		/// Method that retrieve the shot coordinate from the player
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		private Position GetShotPosition(string message)
		{
			Position position = null;
			while (position is null)
			{
				Write(message);
				position = Position.Create(ReadLine());

				if (position is null)
					WriteLine("Incorrect position. (Format [A-J][1-9][1-9])");

				//Check if the shot was already made
				if (MyShotHistory.FindIndex(s => s.Position.Equals(position)) != -1)
				{
					Draw.DrawLog(ConsoleColor.DarkRed, "You already shot at this position");
					position = null;
				}
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
				Clear();
				Draw.DrawBoard(CurrentPlayer.ToString(), GameGrid, ShootingGrid);

				Boat boat = Boats.Find(b => !b.IsPlaced);
				Draw.DrawInformationBox($"Placing : {boat.Type.ToString()}",
										$"Length : {boat.LifePoints}",
										 CurrentState.ToString());

				//----Used for testing purposes only-----//
				if (UseDefaultPlacements()) break;

				Position firstPosition = GetShotPosition("Enter First Coordinate : ");
				Position secondPosition = GetShotPosition("Enter Second Coordinate : ");

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

			Draw.DrawInformationBox(
				$"Your last shot : {(lastShotPlayerOne != null ? lastShotPlayerOne.ToString() : "No shot yet")}",
				$"Opponent last shot : {(lastShotPlayerTwo != null ? lastShotPlayerTwo.ToString() : "No shot yet")}",
				CurrentState.ToString());

			return GetShotPosition("Enter Shooting Coordinate : ");
		}

		/// <summary>
		/// Method that determine the server response and pass it to the proper handler.
		/// </summary>
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

		/// <summary>
		/// Method that read the turn send by the server
		/// </summary>
		/// <param name="serverResponse"></param>
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

		/// <summary>
		/// Method that read the player shot send by the server
		/// </summary>
		/// <param name="serverResponse"></param>
		private void GetServerShot(string serverResponse = null)
		{
			Request.SetHeader(RequestType.Shot);
			Response response = new Response();

			if (serverResponse is null)
				response = Request.ParseResponse(Client.Read());
			else
				response = Request.ParseResponse(serverResponse);

			Shot shot = (response.ShotResult != ShotResult.Missed)
				? new Shot(response.Position, ShotResult.Hit)
				: new Shot(response.Position);

			AddShotToOpponentHistory(shot);
		}

		/// <summary>
		/// Method that read the game result send by the server
		/// </summary>
		/// <param name="serverResponse"></param>
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

		/// <summary>
		/// Testing method for placing all the boat at default position
		/// </summary>
		/// <returns></returns>
		private bool UseDefaultPlacements()
		{
			Write("Would you like all boats to be placed in defaults positions? [Y/N]");
			string response = ReadLine();

			if (response.Length > 0 && char.ToUpper(response[0]) == 'Y')
			{
				Draw.DrawLog(ConsoleColor.DarkGreen, $"[{DateTime.Now}] Placing boats...");
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

		/// <summary>
		/// Add a shot made by the player to the history
		/// </summary>
		/// <param name="shot"></param>
		private void AddShotToMyHistory(Shot shot)
		{
			MyShotHistory.Add(shot);
			ShootingGrid.AddShot(shot);
		}

		/// <summary>
		/// Add a shot made by the opponent to the history
		/// </summary>
		/// <param name="shot"></param>
		private void AddShotToOpponentHistory(Shot shot)
		{
			OpponentShotHistory.Add(shot);
			GameGrid.AddShot(shot);
		}
	}
}
