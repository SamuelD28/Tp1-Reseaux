using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Tp1_Reseaux
{
	//Should extract logic related to the player as it does not belong in this class

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
	/// <exception cref="TimeoutException">The server could not connect with the client in time</exception>
	public sealed class Client
	{
		//Indicate that the connection is open or close
		public bool IsOpen => Tcp.Connected;

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
		private Client() { }

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
		public void Send(string message)
		{
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
			//return Reader.ReadLine();
			//if (Tcp.Available == 0)
			//	return "";

			//var tokenSource = new CancellationTokenSource();
			//CancellationToken ct = tokenSource.Token;
			//string result = null;
			//Task readBuffer = Task.Factory.StartNew(() =>
			//{
			//	ct.ThrowIfCancellationRequested();

			//	result = Reader.ReadLine();

			//	if (ct.IsCancellationRequested)
			//		ct.ThrowIfCancellationRequested();

			//}, ct);
			//while (result is null)
			//{
			//	if (!readBuffer.Wait(5000))
			//	{
			//		tokenSource.Cancel();
			//		result = "";
			//	}
			//}
			//return result;
		}

	}
}
