using System;
using System.Text.RegularExpressions;

namespace Tp1_Reseaux
{
	public enum RequestType
	{
		Shot,
		Boat,
		Turn
	}

	/// <summary>
	/// Class for handling the protocol with the server.
	/// Parse the response and header send to the server.
	/// </summary>
	/// <exception cref="ServerResponseException">Unknown response by the server</exception>
	/// <exception cref="RequestMalformedException">Request format was invalid and could not be parsed</exception>
	public static class Request
	{
		public static string Message{ get; private set; }

		public static RequestType Type { get; private set; }

		public static RequestType? DetermineReceivedType(string message)
		{
			//THIS METHOD NEEDS TO BE IMPROVED. TOO CHEESY
			if (message.Contains("-"))
				return RequestType.Shot;
			else if (message.EndsWith("TURN"))
				return RequestType.Turn;
			else
				return null;
		}

		public static Response ParseResponse(string rawResponse)
		{
			Response response = new Response();
			switch (Type)
			{
				case RequestType.Shot: ParseShotResponse(rawResponse, ref response); break;
				case RequestType.Boat: ParseBoatResponse(rawResponse, ref response); break;
				case RequestType.Turn: ParseTurnResponse(rawResponse, ref response); break;
				default:break;
			}
			return response;
		}

		public static bool SetHeader(RequestType type)
		{
			Type = type;
			return true;
		}

		public static bool SetHeader(RequestType type, string message)
		{
			Type = type;
			switch (type)
			{
				case RequestType.Shot: Message = ParseShotRequest(message); break;
				case RequestType.Boat: Message = ParseBoatRequest(message); break;
				case RequestType.Turn: Message = ""; break;
				default:break;
			}
			return true;
		}

		/// <summary>
		/// Method used to parse a message that will interpreted as a shot request
		/// by the server
		/// </summary>
		/// <param name="message">Message to parse</param>
		/// <returns>Parsed message</returns>
		/// <exception cref="RequestMalformedException">Unable to parse the message</exception>
		private static string ParseShotRequest(string message)
		{
			string trimmed = message.Replace(" ", String.Empty);

			if (trimmed.Length < 2 || trimmed.Length > 3)
				throw new RequestMalformedException();

			return trimmed.ToUpper();
		}

		/// <summary>
		/// Method used to parse a message that will be interpreted as a
		/// boat placement by the server
		/// </summary>
		/// <param name="message">Message to parse</param>
		/// <returns>Parsed message</returns>
		/// <exception cref="RequestMalformedException">Unable to parse the message</exception>
		private static string ParseBoatRequest(string message)
		{
			string trimmed = message.Replace(" ", string.Empty);

			if (trimmed.Length < 4 && trimmed.Length > 6)
				throw new RequestMalformedException();

			Regex regex = new Regex("^([a-zA-Z][0-9]{1,2})([a-zA-Z][0-9]{1,2})$");
			Match matches = regex.Match(trimmed);

			if (matches.Groups.Count != 3)
				throw new RequestMalformedException();

			string firstPosition = matches.Groups[1].Value;
			string secondPosition = matches.Groups[2].Value;

			return (firstPosition + "_" + secondPosition).ToUpper();
		}

		private static void ParseShotResponse(string message, ref Response response)
		{
			Regex regex = new Regex("^(J[1-z])-([A-J][0-9]{1,2})-(FIRE|MISS|SUNK)$");
			Match matches = regex.Match(message);

			if (matches.Groups.Count != 4)
				throw new ServerResponseException();

			string player = matches.Groups[1].Value;
			string shotPosition = matches.Groups[2].Value;
			string shotResult = matches.Groups[3].Value;

			response.Player = player;
			response.Position = Position.Create(shotPosition);

			switch (shotResult.ToUpper())
			{
				case "FIRE": response.ShotResult = ShotResult.Hit; break;
				case "MISS": response.ShotResult = ShotResult.Missed; break;
				case "SUNK": response.ShotResult = ShotResult.Sunk; break;
				default: throw new ServerResponseException();
			}
		}

		//Could we ignore this part when sending shot request? Since the first message send by
		//server contains all the information we need to decide wether its our turn or not.
		//We could used this Parser only when in waiting mode. To clarify.
		private static void ParseTurnResponse(string message, ref Response response)
		{
			Regex regex = new Regex("^.*(J[1-2]).*(TURN).*$");
			Match matches = regex.Match(message);

			if (matches.Groups.Count != 3)
				throw new ServerResponseException();

			string player = matches.Groups[1].Value;
			response.Player = player;
		}

		private static void ParseBoatResponse(string message, ref Response response)
		{
			if (message.ToUpper() == "SUCCES")
				response.Success = true;
			else
			{
				Regex regex = new Regex("^([A-Z][1-9])_([A-Z][1-9]) (.*)$");
				Match matches = regex.Match(message);

				if (matches.Groups.Count != 4)
					throw new ServerResponseException();

				string error = matches.Groups[0].Value;
				response.Error = error;
			}
		}
	}

	public class ServerResponseException : Exception
	{
		public ServerResponseException() { }
		public ServerResponseException(string message) : base(message) { }
	}

	public class RequestMalformedException : Exception
	{
		public RequestMalformedException() { }
		public RequestMalformedException(string message) : base(message) { }
	}
}
