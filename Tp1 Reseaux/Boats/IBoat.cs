namespace Tp1_Reseaux
{
	public interface IBoat
	{
		bool IsPlaced { get; }
		int LifePoints { get; }
		char Representation { get; }
		bool Sunk { get; }
		BoatType Type { get; }

		void AssignPlacement(Position first, Position second);
		void SubstractLifePoints(int dommage);
		string GetPlacement();
	}
}