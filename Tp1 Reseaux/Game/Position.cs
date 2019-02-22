using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tp1_Reseaux
{
	/// <summary>
	/// La classe position contiendras TOUTE la logique pour
	/// determiner si une position est valide ou non. Elle leveras
	/// les exception ou les erreur necessaire pour le signaler 
	/// a celui qui en fait appel. Cest ici que lon devrait gerer ou
	/// non la conversion de -1. A revoir
	/// </summary>
    public class Position
    {
        public int X { get; }
        public int Y { get; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

		//private static bool ValidatePosition(string position, out Position positionValidated)
		//{
			//positionValidated = null;

			//if (!int.TryParse(position.Substring(1), out int secondPart))
			//{
			//	return false;
			//}

			//if (!validChar.Contains(position[0]))
			//{
			//	WriteLine("Veuillez respecter l'intervalle A - J");
			//	return false;
			//}

			//if (secondPart < 1 || secondPart > 10)
			//{
			//	WriteLine("Veuillez respecter l'intervalle 1 - 10");
			//	return false;
			//}

			//positionValidated = new Position(validChar.IndexOf(position[0]) + 1, secondPart + 1);

			//return true;
		//}
	}
}
