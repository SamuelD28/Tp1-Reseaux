namespace Tp1_Reseaux
{
	/// <summary>
	/// Enumeration of all the possible shot result
	/// </summary>
	public enum ShotResult
	{
		Hit,
		Missed,
		Sunk
	}

	/// <summary>
	/// Class used to store a shot made by  player.
	/// </summary>
	public class Shot
	{
		//Contains the position of the shot
		public Position Position { get; private set; }

		//Contains true if the shot hit a boat
		public ShotResult Result { get; private set; }

		/// <summary>
		/// Constructor that takes a position object as a parameter.
		/// </summary>
		/// <param name="position">Position of the shot made</param>
		public Shot(Position position)
		{
			Position = position;
			Result = ShotResult.Missed;
		}

		/// <summary>
		/// Constructor that takes a position and hit result as
		/// parameters
		/// </summary>
		/// <param name="position">Position of the shot</param>
		/// <param name="hit">Shot result</param>
		public Shot(Position position, ShotResult result)
		{
			Position = position;
			Result = result;
		}

		/// <summary>
		/// Method that return the position and result of the shot as a string
		/// </summary>
		/// <returns>Shot position and result</returns>
		public override string ToString() =>
			$"{Position.ToString()} : " +
			$"{(Result == ShotResult.Hit ? "Hit" : Result == ShotResult.Sunk ? "Sunk" : "Missed")}";
	}
}