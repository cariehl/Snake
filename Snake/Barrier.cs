using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
	// A barrier that will kill the snake if it comes in contact
	class Barrier
	{
		// A struct containing information about one of the barrier's parts
		// Each segment is a single "#" in the console
		private struct Segment
		{
			public int X;
			public int Y;
			public int Id;
		};

		// List of all the segments that are part of our barrier
		private List<Segment> _segments;

		// Public constructor
		public Barrier()
		{
			// Initialize our list of segments
			_segments = new List<Segment>();
		}

		// Add a segment to this barrier
		public void AddSegment(int x, int y)
		{
			_segments.Add(new Segment(){X = x, Y = y, Id = _segments.Count});
		}

		// Returns true if a segment exists at the given (x, y) coordinates
		public bool CollisionAtPoint(int x, int y)
		{
			return _segments.Any(seg => seg.X == x && seg.Y == y);
		}

		// Returns a random segment's coordinates from the existing segments
		public int[] GetRandomSegment(Random rand)
		{
			// Get a random segment Id from 0 to the number of segments
			var randSegId = rand.Next(1, _segments.Count + 1) - 1;

			// Find the matching segment object
			var randSeg = _segments.Find(seg => seg.Id == randSegId);

			// Return the coordinates of the segment
			return new int[] {randSeg.X, randSeg.Y};
		}

		// Render this barrier onto the Console
		public void Render()
		{
			// Render each segment onto the Console with the icon '#'
			foreach (var seg in _segments) {
				Console.SetCursorPosition(seg.X, seg.Y);
				Console.Write('#');
			}
		}
	}
}
