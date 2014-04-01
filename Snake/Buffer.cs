using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
	internal struct Icon
	{
		public int X;
		public int Y;
		public char Character;
	};

	class Buffer
	{
		private List<Icon> _icons;

		public Buffer()
		{
			// initializes the icons array to potentially contain each interior tile of the game board
			this._icons = new List<Icon>();
		}

		// Adds a new icon at position (newX, newY) with representation of newCharacter
		public void AddIcon(int newX, int newY, char newCharacter)
		{
			_icons.Add(new Icon(){X = newX, Y = newY, Character = newCharacter});
		}

		public void Draw()
		{
			// Render the buffer to the console
			foreach (var icon in _icons) {
				Console.SetCursorPosition(icon.X, icon.Y);
				Console.Write(icon.Character);
			}
		}

		public void Clear()
		{
			// Clear the buffer from the console
			foreach (var icon in _icons) {
				Console.SetCursorPosition(icon.X, icon.Y);
				Console.Write(" ");
			}

			// Remove all elements from the buffer so we can start fresh on the next frame
			_icons.Clear();
		}
	}
}
