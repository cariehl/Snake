using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
	// A buffer to hold objects to be drawn on the screen
	class Buffer
	{
		// An icon to be drawn on the screen at position (x, y)
		// Character keeps track of the ASCII character to be drawn
		private struct Icon
		{
			public int X;
			public int Y;
			public char Character;
		};

		// A list of icons to draw to the screen
		private List<Icon> _icons;

		// Public constructor
		public Buffer()
		{
			// initializes the icons list
			this._icons = new List<Icon>();
		}

		// Adds a new icon at position (newX, newY) with representation of newCharacter
		public void AddIcon(int newX, int newY, char newCharacter)
		{
			_icons.Add(new Icon(){X = newX, Y = newY, Character = newCharacter});
		}

		// Renders this buffer to the console
		public void Render()
		{
			// Write each icon in the correct position on the console
			foreach (var icon in _icons) {
				Console.SetCursorPosition(icon.X, icon.Y);
				Console.Write(icon.Character);
			}
		}

		// Clears this buffer from the console
		public void Clear()
		{
			// Rewrite each icon as a space (which is the background icon)
			foreach (var icon in _icons) {
				Console.SetCursorPosition(icon.X, icon.Y);
				Console.Write(" ");
			}

			// Remove all elements from this buffer so we can start fresh on the next frame
			_icons.Clear();
		}
	}
}
