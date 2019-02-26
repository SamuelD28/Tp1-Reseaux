namespace Tp1_Reseaux
{
	public enum ShotResult
	{
		Hit,
		Missed,
		Sunk
	}
	
	public class Shot
	{
		public Position Position { get; private set; }
		public bool Hit { get; private set; }

		public Shot(Position position)
		{
			Position = position;
			Hit = false;
		}

		public Shot(Position position, bool hit)
		{
			Position = position;
			Hit = hit;
		}

		public override string ToString() => $"{Position.ToString()} : {(Hit ? "Hit" : "Missed")}";  
	}
}