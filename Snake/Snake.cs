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
			// Find the position of the snake's tail
			var tail = _segments.Find(x => x.Id == _segments.Count);
			var xnew = tail.X;
			var ynew = tail.Y;

			// Offset the next segment based on which direction the tail is moving
			// i.e if the tail is moving down, we want the new segment to be one unit above the tail
			switch (tail.Direction) {
				case Program.SnakeDirection.Left:
					xnew = tail.X + 1;
					break;
				case Program.SnakeDirection.Right:
					xnew = tail.X - 1;
					break;
				case Program.SnakeDirection.Up:
					ynew = tail.Y + 1;
					break;
				case Program.SnakeDirection.Down:
					ynew = tail.Y - 1;
					break;
			}

			// Add the new segment one position behind the tail
			_segments.Add(new Segment(){X = xnew, Y = ynew, Direction = tail.Direction, Id = tail.Id + 1});
		}

		// Moves the Snake one unit, with the new head going in the specified direction
		public void Move(Program.SnakeDirection direction)
		{
			// Moving the snake is accomplished by removing the tail segment, adding a new segment one unit
			// in front of the current head and then updating all the other segments' ID so they don't lose
			// track of their "position" in the snake

			// Find the position of the snake's head
			var head = _segments.Find(x => x.Id == 1);
			var xnew = head.X;
			var ynew = head.Y;

			// Offset one segment in front of the head based on its direction
			// i.e if the head is moving down, we want the new segment to be one unit below the head
			// Overrides any attempts to move in the opposite direction
			// i.e if the snake is moving up and the player tries to move down, the snake will continue moving up
			switch (direction) {
				case Program.SnakeDirection.Left:
					if (head.Direction == Program.SnakeDirection.Right) {
						xnew = head.X + 1;
						direction = Program.SnakeDirection.Right;
					} else
						xnew = head.X - 1;
					break;
				case Program.SnakeDirection.Right:
					if (head.Direction == Program.SnakeDirection.Left) {
						xnew = head.X - 1;
						direction = Program.SnakeDirection.Left;
					} else
						xnew = head.X + 1;
					break;
				case Program.SnakeDirection.Up:
					if (head.Direction == Program.SnakeDirection.Down) {
						ynew = head.Y + 1;
						direction = Program.SnakeDirection.Down;
					} else
						ynew = head.Y - 1;
					break;
				case Program.SnakeDirection.Down:
					if (head.Direction == Program.SnakeDirection.Up) {
						ynew = head.Y - 1;
						direction = Program.SnakeDirection.Up;
					} else
						ynew = head.Y + 1;
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
			// Find the position of the head
			var head = _segments.Find(x => x.Id == 1);

			// First check to see if this snake is colliding with one of its own segments
			if (_segments.Any(seg => head.X == seg.X && head.Y == seg.Y && head.Id != seg.Id)) {
				return true;
			}

			// Then check to see if we are on a wall
			// TODO: Change this from "magic numbers" to use information from the caller
			return barriers.Any(bar => bar.CollisionAtPoint(head.X, head.Y));
		}

		// Returns true if this snake is colliding with the given food object
		public bool DetectFood(Food food)
		{
			// Find the position of the head
			var head = _segments.Find(x => x.Id == 1);

			// Return true if the head is occupying the same space as the food
			return food.ExistsAtPoint(head.X, head.Y);
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
