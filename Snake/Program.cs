using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Messaging;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace Snake
{
	// Let's play some Snake!
	static class Program
	{
		// Console width and height
		private const int Width = 80, Height = 24;

		// The number of barriers to generate for the game (excluding the walls)
		private const int NumBarriers = 5;

		// The maximum number of barrier segments for each barrier (excluding the walls)
		private const int MaxBarrierSegments = 10;

		// Direction of the snake, starts off moving to the right
		static SnakeDirection _direction = SnakeDirection.Right;

		// Is the game still going?
		static bool _playing = true;

		// Speed of the game (wait time in ms between frames)
		const int GameSpeed = 100;

		// The snake
		static Snake _snake = new Snake(10, 10);

		// The food object
		static Food _food = new Food();

		// List of barriers in the game
		static List<Barrier> _barriers = new List<Barrier>();

		// Random number generator
		static Random _rand = new Random();

		// Keeps track of the direction the snake is moving
		public enum SnakeDirection
		{
			Up,
			Down,
			Left,
			Right,
		};

		// Prints the title screen to the console
		static void PrintTitle()
		{
			Console.SetCursorPosition(27, 7);
			Console.Write("*~* WormSnake *~*");

			Console.SetCursorPosition(28, 9);
			Console.Write("by Cooper Riehl");

			Console.SetCursorPosition(29, 11);
			Console.Write("Instructions:");

			Console.SetCursorPosition(24, 13);
			Console.Write("You are the WormSnake  @");

			Console.SetCursorPosition(24, 14);
			Console.Write("Collect the Food       $");

			Console.SetCursorPosition(24, 15);
			Console.Write("Avoid the Walls        #");

			Console.SetCursorPosition(22, 17);
			Console.Write("Press any key to continue...");
		}

		// Moves the food to a new position, taking into account other objects on the screen
		static void SetFoodPos()
		{
			int randX;
			int randY;
			int barCount = 0;

			// Get random coordinates until we find one that is NOT occupied already
			// AND where the food is NOT surrounded on 3 or more sides by barriers
			// AND where the barrier is NOT within the first 10 spaces of the snake
			do {
				randX = _rand.Next(Width - 2) + 1;
				randY = _rand.Next(Height - 2) + 1;
				// Count the number of barriers surrounding the food object
				barCount =
					_barriers.FindAll(bar => bar.CollisionAtPoint(randX + 1, randY) ||
					                         bar.CollisionAtPoint(randX - 1, randY) ||
					                         bar.CollisionAtPoint(randX, randY + 1) ||
					                         bar.CollisionAtPoint(randX, randY - 1))
											 .Count;
			} while (_barriers.Exists(bar => bar.CollisionAtPoint(randX, randY)) ||
			         _snake.ExistsAtPoint(randX, randY) ||
					 barCount >= 3);

			// Set the food's position to the found coordinates
			_food.SetPosition(randX, randY);
		}

		// Adds the four walls to the list of barriers
		static void CreateOuterWalls()
		{
			// Generate top wall
			var topWall = new Barrier();
			for (var i = 0; i < Width; i++) {
				topWall.AddSegment(i, 0);
			}
			_barriers.Add(topWall);

			// Generate bottom wall
			var bottomWall = new Barrier();
			for (var i = 0; i < Width; i++) {
				bottomWall.AddSegment(i, Height - 1);
			}
			_barriers.Add(bottomWall);

			// Generate left wall
			var leftWall = new Barrier();
			for (var i = 0; i < Height; i++) {
				leftWall.AddSegment(0, i);
			}
			_barriers.Add(leftWall);

			// Generate right wall
			var rightWall = new Barrier();
			for (var i = 0; i < Height; i++) {
				rightWall.AddSegment(Width - 1, i);
			}
			_barriers.Add(rightWall);
		}

		// Adds a number of randomized barriers to the list of barriers
		static void CreateRandomBarriers(int num)
		{
			// Generate "NumBarriers" random barriers
			for (var i = 0; i < num; i++) {
				var thisBarrier = new Barrier();

				// Each barrier has a number of segments between 3 and maximum
				var thisNumSegments = _rand.Next(3, MaxBarrierSegments + 1);

				// Generate a starting point for the barrier
				var startX = _rand.Next(1, Width - 2);
				var startY = _rand.Next(1, Height - 2);

				// Add a segment at the starting point
				thisBarrier.AddSegment(startX, startY);

				// Generate "thisNumSegments" where each segment is touching
				// one of the existing segments
				for (var j = 0; j < thisNumSegments - 1; j++) {
					int randX, randY;

					// Keep generating points until we get one that doesn't conflict with
					// an existing segment
					do {
						// Get a random segment from the current barrier (starts with the starting point)
						// where coords[0] = x and coords[1] = y
						var coords = thisBarrier.GetRandomSegment(_rand);

						// Get a random number -1 to 1
						randX = _rand.Next(3) - 1;
						randY = _rand.Next(3) - 1;

						// Add that random number to each of the base coords
						randX += coords[0];
						randY += coords[1];
					} while (_barriers.Any(bar => bar.CollisionAtPoint(randX, randY)));
					

					// Add a new segment at the generated point
					thisBarrier.AddSegment(randX, randY);
				}

				// Finally add our new barrier to the list of barriers
				_barriers.Add(thisBarrier);
			}
		}

		// method to draw static objects on the screen before the game starts
		// these include barriers and walls
		static void PreRender()
		{
			foreach (var bar in _barriers) {
				bar.Render();
			}
		}

		// main game loop
		static void GameLoop()
		{
			// Initialize the food position to a random position
			SetFoodPos();

			// Game logic loop
			// This runs continuously while the game is going on
			while (_playing) {
				// Keep track of time used in game logic
				var timer =  new Stopwatch();
				timer.Start();

				// Move the snake one unit in the direction it is facing
				_snake.Move(_direction);

				// If the snake runs over a food piece...
				if (_snake.DetectFood(_food)) {
					// ...add a new segment...
					_snake.AddSegment();

					// ...and move the food to a new position
					SetFoodPos();
				}

				// If the snake runs into a wall...
				if (_snake.DetectCollision(_barriers)) {
					// ...end the game
					_playing = false;
				}

				// Stop the timer and get the elapsed time in ms
				timer.Stop();
				var elapsedTime = (int)timer.Elapsed.TotalMilliseconds;

				// Sleep until the next frame
				Thread.Sleep(GameSpeed - elapsedTime > 0 ? GameSpeed - elapsedTime : 0);
			}
		}

		// asynchronous user input loop
		static void GetUserInput()
		{
			while (true) {
				// Read a key from the console
				var key = Console.ReadKey(true).Key;

				// Set the snake's direction based on which arrow key was pressed
				switch (key) {
					case ConsoleKey.UpArrow:
						_direction = SnakeDirection.Up;
						break;
					case ConsoleKey.DownArrow:
						_direction = SnakeDirection.Down;
						break;
					case ConsoleKey.LeftArrow:
						_direction = SnakeDirection.Left;
						break;
					case ConsoleKey.RightArrow:
						_direction = SnakeDirection.Right;
						break;
				}
			}
		}

		// game start function
		static void Main(string[] args)
		{
			// Show the title screen
			PrintTitle();
			Console.ReadKey();

			// Clear the title
			Console.Clear();

			// initialize all our threads
			var gameLoop = new Thread(new ThreadStart(GameLoop));
			var userInputLoop = new Thread(new ThreadStart(GetUserInput));
			
			// Create walls and barriers for the game
			CreateOuterWalls();
			CreateRandomBarriers(NumBarriers);

			// draw static objects including walls and barriers
			PreRender();

			// start the threads
			gameLoop.Start();
			userInputLoop.Start();

			// wait for the game loop to end
			gameLoop.Join();

			// Stop the user input loop
			userInputLoop.Abort();

			// Print the ending message
			Console.SetCursorPosition(0, Height);
			Console.Write("\bYou are a loser!");

			// Wait for the user to quit
			Console.ReadKey();
		}
	}
}