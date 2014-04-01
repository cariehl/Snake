using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
	class Snake : IBufferable
	{
		private struct Segment
		{
			public int X;
			public int Y;
			public int Id;
			public Program.SnakeDirection direction;
		};
		
		private List<Segment> _segments =  new List<Segment>();

		public Snake(int startX, int startY)
		{
			// Initialize the snake "head"
			this._segments.Add(new Segment(){X = startX, Y = startY, Id = 1, direction = Program.SnakeDirection.Right});
		}

		// Adds a segment to the end of the snake
		public void AddSegment()
		{
			var tail = _segments.Find(x => x.Id == _segments.Count);
			var xnew = tail.X;
			var ynew = tail.Y;
			switch (tail.direction) {
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
			_segments.Add(new Segment(){X = xnew, Y = ynew, direction = tail.direction, Id = tail.Id + 1});
		}

		// Moves the Snake one unit, with the new head going in the specified direction
		public void Move(Program.SnakeDirection direction)
		{
			// Move the tail of the snake to the position of the head + 1
			var head = _segments.Find(x => x.Id == 1);
			var xnew = head.X;
			var ynew = head.Y;
			switch (direction) {
				case Program.SnakeDirection.Left:
					if (head.direction == Program.SnakeDirection.Right) {
						xnew = head.X + 1;
						direction = Program.SnakeDirection.Right;
					} else
						xnew = head.X - 1;
					break;
				case Program.SnakeDirection.Right:
					if (head.direction == Program.SnakeDirection.Left) {
						xnew = head.X - 1;
						direction = Program.SnakeDirection.Left;
					} else
						xnew = head.X + 1;
					break;
				case Program.SnakeDirection.Up:
					if (head.direction == Program.SnakeDirection.Down) {
						ynew = head.Y + 1;
						direction = Program.SnakeDirection.Down;
					} else
						ynew = head.Y - 1;
					break;
				case Program.SnakeDirection.Down:
					if (head.direction == Program.SnakeDirection.Up) {
						ynew = head.Y - 1;
						direction = Program.SnakeDirection.Up;
					} else
						ynew = head.Y + 1;
					break;
			}

			_segments.RemoveAt(_segments.FindIndex(x => x.Id == _segments.Count));
			_segments.Add(new Segment(){X = xnew, Y = ynew, direction = direction, Id = 0});

			// Increment the ID by one
			// There has to be a better way to do this...
			for (int i = 0; i < _segments.Count; i++) {
				_segments[i] = new Segment() {
					X = _segments[i].X,
					Y = _segments[i].Y,
					direction = _segments[i].direction,
					Id = _segments[i].Id + 1
				};
			}
		}

		public bool DetectCollision()
		{
			var head = _segments.Find(x => x.Id == 1);
			// First check to see if this snake is colliding with its own segments
			if (this._segments.Any(seg => head.X == seg.X && head.Y == seg.Y && head.Id != seg.Id)) {
				return true;
			}
			// Finally check to see if we are on a wall
			return head.Y == 0 || head.Y == Program.Height - 1 || head.X == 0 ||
			       head.X == Program.Width - 1;
		}

		public bool DetectFood(Program.Food food)
		{
			var head = _segments.Find(x => x.Id == 1);
			return head.X == food.x && head.Y == food.y;
		}

		// Draws this Snake onto the specified buffer
		public void Draw()
		{
			foreach (var seg in _segments) {
				Console.SetCursorPosition(seg.X, seg.Y);
				Console.Write("@");
			}
		}

		// Clear this Snake from the specified buffer
		public void Clear()
		{
			foreach (var seg in _segments) {
				Console.SetCursorPosition(seg.X, seg.Y);
				Console.Write(" ");
			}
		}

		public int GetNumSegments()
		{
			return _segments.Count;
		}
	}
}
