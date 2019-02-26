namespace Tp1_Reseaux
{
	public class Response
	{
		public Response()
		{
			Success = false;
			Wait = false;
			Position = null;
			Player = string.Empty;
			Won = false;
			ShotResult = ShotResult.Missed;
			Error = string.Empty;
		}
		public bool Success { get; set; }
		public bool Wait { get; set; }
		public Position Position { get; set; }
		public string Player { get; set; } //Should do an enum to simplify
		public bool Won { get; set; }
		public ShotResult ShotResult { get; set; }
		public string Error { get; set; }
	}
}
