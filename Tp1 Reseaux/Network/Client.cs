using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Tp1_Reseaux
{
	public sealed class Client
	{
		public class NoPlayerAssignedException : Exception
		{
			public NoPlayerAssignedException() { }
			public NoPlayerAssignedException(string message) : base(message) { }
		}

		//Properties of the class
		public bool IsOpen => Tcp.Connected;
		public int PlayerAssigned { get; private set; }

		//Reader and writer objects used to communicate with the server
		private static StreamReader Reader = StreamReader.Null;
		private static StreamWriter Writer = StreamWriter.Null;

		//Tcp connexion with the server
		private static TcpClient Tcp;

		//Instance of the singleton class
		private static Client Instance = null;
		public static Client GetInstance() => (Instance is null) ? Instance = new Client() : Instance;

		private Client() {
			PlayerAssigned = default(int);
		}

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

		private void CloseClient()
		{
			Reader.Close();
			Writer.Close();
			Tcp.Close();
		}

		public void Send(Request request)
		{
			string message = request.ParseSendMessage(request.RawMessage);

			if (message is null)
				throw new RequestMalformedException();

			Writer.Write(message + "\r\n");
			Writer.Flush();
		}

		public string Read()
		{
			return Reader.ReadLine();
		}
	}
}
