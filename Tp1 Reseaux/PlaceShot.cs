using System;
using System.Text.RegularExpressions;

namespace Tp1_Reseaux
{
	public class PlaceShot : Request
	{
		public enum ShotResult
		{
			Missed, 
			Sunk,
			Hit
		}

		public ShotResult MyShotResult { get; private set; }

		public string PlayerTurn { get; private set; }

		public PlaceShot(string message) : base(message) { } 

		public override void ParseReceivedMessage(string message)
		{
			if (message.ToUpper().EndsWith("TURN"))
				ParseReceivedPlayerTurn(message);
			else
				ParseReceivedPlayerShot(message);
		}

		public void ParseReceivedPlayerTurn(string message)
		{
			Regex regex = new Regex("^.*(J[1-2]).*(TURN).*$");
			Match matches = regex.Match(message);

			if (matches.Length != 3)
				throw new ServerResponseException();

			PlayerTurn = matches.Groups[1].Value.ToUpper();
		}

		public void ParseReceivedPlayerShot(string message)
		{
			Regex regex = new Regex("^(J[1-z])-([A-J][0-9]{1,2})-(FIRE|MISS|SUNK)$");
			Match matches = regex.Match(message);

			if (matches.Length != 4)
				throw new ServerResponseException();

			string shotPosition = matches.Groups[2].Value;
			string shotResult = matches.Groups[3].Value;

			switch (shotResult.ToUpper())
			{
				case "FIRE": MyShotResult = ShotResult.Hit; break;
				case "MISS": MyShotResult = ShotResult.Missed; break;
				case "SUNK": MyShotResult = ShotResult.Sunk; break;
				default: throw new ServerResponseException();
			}
		}

		public override string ParseSendMessage(string message)
		{
			string trimmed = message.Replace("", String.Empty);

			if (trimmed.Length < 2 || trimmed.Length > 3)
				throw new RequestMalformedException();

			return trimmed.ToUpper();
		}
	}
}
