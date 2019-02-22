using System;

namespace Tp1_Reseaux
{
	public abstract class Request
	{
		public Request(string rawMessage)
		{
			RawMessage = rawMessage;
		}
		public string RawMessage { get; set; }
		public abstract void ParseReceivedMessage(string message);
		public abstract string ParseSendMessage(string message);
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
