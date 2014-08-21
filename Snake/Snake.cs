using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
	// A snake that moves across the screen and is controlled by the player.
	class Snake
	{
		// A struct containing information about one of the snake's parts
		// Each segment is a single "@" in the console
		private struct Segment
		{
			public int X;
			public int Y;
			public int Id;
			public Program.SnakeDirection Direction;
		};
		
		// List of all the segments that are part of our snake
		private List<Segment> _segments;

		// Shortcuts to get the head and tail
		Segment _head { get { return _segments.Find(x => x.Id == 1); } }
		Segment _tail { get { return _segments.Find(x => x.Id == _segments.Count); } }

		// Public constructor
		public Snake(int startX, int startY)
		{
			// Initialize our list of segments
			_segments = new List<Segment>();

			// Creates the "head" of the snake (first segment)
			this._segments.Add(new Segment(){X = startX, Y = startY, Id = 1, Direction = Program.SnakeDirection.Right});
		}

		// Adds a segment to the end of the snake
		public void AddSegment()
		{
			var xnew = _tail.X;
			var ynew = _tail.Y;

			// Offset the next segment based on which direction the tail is moving
			// i.e if the tail is moving down, we want the new segment to be one unit above the tail
			switch (_tail.Direction) {
				case Program.SnakeDirection.Left:
					xnew = _tail.X + 1;
					break;
				case Program.SnakeDirection.Right:
					xnew = _tail.X - 1;
					break;
				case Program.SnakeDirection.Up:
					ynew = _tail.Y + 1;
					break;
				case Program.SnakeDirection.Down:
					ynew = _tail.Y - 1;
					break;
			}

			// Add the new segment one position behind the tail
			_segments.Add(new Segment(){X = xnew, Y = ynew, Direction = _tail.Direction, Id = _tail.Id + 1});
		}

		// Moves the Snake one unit, with the new head going in the specified direction
		public void Move(Program.SnakeDirection direction)
		{
			// Moving the snake is accomplished by removing the tail segment, adding a new segment one unit
			// in front of the current head and then updating all the other segments' ID so they don't lose
			// track of their "position" in the snake

			var xnew = _head.X;
			var ynew = _head.Y;

			// Offset one segment in front of the head based on its direction
			// i.e if the head is moving down, we want the new segment to be one unit below the head
			// Overrides any attempts to move in the opposite direction
			// i.e if the snake is moving up and the player tries to move down, the snake will continue moving up
			switch (direction) {
				case Program.SnakeDirection.Left:
					if (_head.Direction == Program.SnakeDirection.Right) {
						xnew = _head.X + 1;
						direction = Program.SnakeDirection.Right;
					} else
						xnew = _head.X - 1;
					break;
				case Program.SnakeDirection.Right:
					if (_head.Direction == Program.SnakeDirection.Left) {
						xnew = _head.X - 1;
						direction = Program.SnakeDirection.Left;
					} else
						xnew = _head.X + 1;
					break;
				case Program.SnakeDirection.Up:
					if (_head.Direction == Program.SnakeDirection.Down) {
						ynew = _head.Y + 1;
						direction = Program.SnakeDirection.Down;
					} else
						ynew = _head.Y - 1;
					break;
				case Program.SnakeDirection.Down:
					if (_head.Direction == Program.SnakeDirection.Up) {
						ynew = _head.Y - 1;
						direction = Program.SnakeDirection.Up;
					} else
						ynew = _head.Y + 1;
					break;
			}

			// Clear the tail from the console
			var toClear = _segments.Find(x => x.Id == _segments.Count);
			Console.SetCursorPosition(toClear.X, toClear.Y);
			Console.Write(' ');

			// Remove the tail of the snake
			_segments.RemoveAt(_segments.FindIndex(x => x.Id == _segments.Count));

			// Add a new segment one unit in front of the head
			_segments.Add(new Segment(){X = xnew, Y = ynew, Direction = direction, Id = 0});

			// Write the new segment to the console
			Console.SetCursorPosition(xnew, ynew);
			Console.Write('@');

			// Increment each segment's ID by one
			// There is probably a better way to do this...
			for (var i = 0; i < _segments.Count; i++) {
				var seg = _segments[i];
				seg.Id++;
				_segments[i] = seg;
			}
		}

		// Returns true if this snake is colliding with a barrier object or wall
		public bool DetectCollision(IEnumerable<Barrier> barriers)
		{
			// First check to see if this snake is colliding with one of its own segments
			if (_segments.Any(seg => _head.X == seg.X && _head.Y == seg.Y && _head.Id != seg.Id)) {
				return true;
			}

			// Then check to see if we are on a wall
			// TODO: Change this from "magic numbers" to use information from the caller
			return barriers.Any(bar => bar.CollisionAtPoint(_head.X, _head.Y));
		}

		// Returns true if this snake is colliding with the given food object
		public bool DetectFood(Food food)
		{
			// Return true if the head is occupying the same space as the food
			return food.ExistsAtPoint(_head.X, _head.Y);
		}

		// Returns true if a snake segment exists at the given coords
		public bool ExistsAtPoint(int x, int y)
		{
			return _segments.Any(seg => seg.X == x && seg.Y == y);
		}

		// Returns the number of segments in the snake
		public int GetNumSegments()
		{
			return _segments.Count;
		}
	}
}
