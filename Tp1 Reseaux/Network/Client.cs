using System;
using System.IO;
using System.Net.Sockets;

namespace Tp1_Reseaux
{
	/// <summary>
	/// Exception thrown when the client cant retrieve the
	/// player assigned by the server.
	/// </summary>
	public class NoPlayerAssignedException : Exception
	{
		public NoPlayerAssignedException() { }
		public NoPlayerAssignedException(string message) : base(message) { }
	}

	/// <summary>
	/// Class that handle the client for communicating
	/// with the server
	/// </summary>
	public sealed class Client
	{
		//Indicate that the connection is open or close
		public bool IsOpen => Tcp.Connected;

		//Retrieve the player assigned by the server. SHOULD EXTRACT
		public int PlayerAssigned { get; private set; }

		//Reader used for reading the server stream
		private static StreamReader Reader = StreamReader.Null;

		//Writer used for sending message to the server
		private static StreamWriter Writer = StreamWriter.Null;

		//Tcp connexion with the server
		private static TcpClient Tcp = new TcpClient();

		//Instance of the singleton class
		private static Client Instance = null;

		/// <summary>
		/// Method that return the instance of the singleton class
		/// </summary>
		/// <returns>Instance of the class</returns>
		public static Client GetInstance() => (Instance is null) ? Instance = new Client() : Instance;

		/// <summary>
		/// Private constructor for the Client
		/// </summary>
		private Client() {
			PlayerAssigned = default(int);
		}

		/// <summary>
		/// Method used to connect the client to the server.
		/// </summary>
		/// <param name="server">Ip adress of the server to connect to</param>
		/// <param name="port">Port number of the server to connect to</param>
		/// <returns>True if the connection succeded</returns>
		public bool Connect(string server, int port)
		{
			Tcp = new TcpClient();
			if (Tcp.ConnectAsync(server, port).Wait(2000))
				InitClient();
			else
			{
				CloseClient();
				throw new TimeoutException();
			}

			return IsOpen;
		}

		/// <summary>
		/// Method used to initiate the connexion with the server. Retrieve
		/// the reader and writer for communication
		/// </summary>
		private void InitClient()
		{
			Reader = new StreamReader(Tcp.GetStream());
			Writer = new StreamWriter(Tcp.GetStream());

			int playerAssignedByServer;
			int.TryParse(Read(), out playerAssignedByServer);

			if (playerAssignedByServer == default(int))
				throw new NoPlayerAssignedException("No player was assigned by the server");

			PlayerAssigned = playerAssignedByServer;
		}

		/// <summary>
		/// Method used to close the connexion with the Server
		/// </summary>
		private void CloseClient()
		{
			Reader.Close();
			Writer.Close();
			Tcp.Close();
		}

		/// <summary>
		/// Method used to send request to the server
		/// </summary>
		/// <param name="request">Type of request to send</param>
		public void Send(Request request)
		{
			string message = request.ParseSendMessage(request.RawMessage);

			if (message is null)
				throw new RequestMalformedException();

			Writer.Write(message + "\r\n");
			Writer.Flush();
		}

		/// <summary>
		/// Method for readign the stream from the server
		/// </summary>
		/// <returns>Stream from the server</returns>
		public string Read()
		{
			return Reader.ReadLine();
		}
	}
}
