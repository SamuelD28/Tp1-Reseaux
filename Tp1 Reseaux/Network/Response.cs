namespace Tp1_Reseaux
{
	/// <summary>
	/// Class used to parse response information sent by the server
	/// into an object.
	/// </summary>
	public class Response
	{
		public Response()
		{
			Success = false;
			Position = null;
			Player = string.Empty;
			ShotResult = ShotResult.Missed;
			Error = string.Empty;
		}
		public bool Success { get; set; }
		public Position Position { get; set; } 
		public string Player { get; set; } 
		public ShotResult ShotResult { get; set; }
		public string Error { get; set; }
	}
}
