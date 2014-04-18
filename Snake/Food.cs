using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
	// A piece of food that extends the length of the snake when eaten
	class Food
	{
		// Keeps track of the position of the food object
		private struct FoodObj
		{
			public int X;
			public int Y;
		};

		private FoodObj _food;

		// Public constructor
		public Food()
		{
			// Initialize the food at position (startX, startY)
			_food = new FoodObj();
		}

		// Sets the position of the food to a new (x, y) coord
		public void SetPosition(int x, int y)
		{
			_food.X = x;
			_food.Y = y;

			Console.SetCursorPosition(x, y);
			Console.Write('$');
		}

		// Returns true if this food object exists at the given point
		public bool ExistsAtPoint(int x, int y)
		{
			return _food.X == x && _food.Y == y;
		}
	}
}
