using System;
using System.Text.RegularExpressions;

namespace Tp1_Reseaux
{
	public class PlaceBoat : Request
	{
		public bool Success => ErrorMessage == String.Empty;
		public string ErrorMessage = String.Empty;

		public PlaceBoat(string rawMessage) : base(rawMessage) { }

		public override void ParseReceivedMessage(string serverResponse)
		{
			if (serverResponse != "SUCCES")
				ErrorMessage = serverResponse;
			else
				ErrorMessage = String.Empty;
		}

		public override string ParseSendMessage(string boatPosition)
		{
			string trimmed = boatPosition.Replace(" ", string.Empty);

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
	}
}
