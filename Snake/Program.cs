using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
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
		public const int Width = 80, Height = 24;

		// Direction of the snake, starts off moving to the right
		static SnakeDirection _direction = SnakeDirection.Right;

		// Is the game still going?
		static bool _playing = true;

		// Speed of the game (wait time in ms between frames)
		const int GameSpeed = 100;

		// The snake
		static Snake _snake = new Snake(10, 10);

		// The food object
		static Food _food;

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

		// Keeps track of the position of the food object
		public struct Food
		{
			public int x;
			public int y;
		};

		// container method to draw the food on the screen
		// replace this when Food becomes a separate class
		static void DrawFood(Food food)
		{
			Console.SetCursorPosition(food.x, food.y);
			Console.Write("$");
		}

		// container method to clear food from the screen
		// replace this when Food becomes a separate class
		static void ClearFood(Food food)
		{
			Console.SetCursorPosition(food.x, food.y);
			Console.Write(" ");
		}

		// method to draw static objects on the screen before the game starts
		// these include barriers and walls
		static void PreRender()
		{
			// draw the upper wall
			Console.SetCursorPosition(0, 0);
			for (var i = 0; i < Width; i++) {
				Console.Write("#");
			}

			// draw the left and right walls
			for (var i = 1; i < Height - 1; i++) {
				Console.SetCursorPosition(0, i);
				Console.Write("#");
				Console.SetCursorPosition(Width - 1, i);
				Console.Write("#");
			}

			// draw the bottom wall
			Console.SetCursorPosition(0, Height - 1);
			for (var i = 0; i < Width; i++) {
				Console.Write("#");
			}
		}

		// main game loop
		static void GameLoop()
		{
			// Initialize the food position
			_food.x = _rand.Next(Width - 2) + 1;
			_food.y = _rand.Next(Height - 2) + 1;

			// Initialize two screen buffers
			var buffers = new Buffer[2];
			for (int i = 0; i < 2; i++) {
				buffers[i] = new Buffer();
			}
			var curBuffer = 0;

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
					_food.x = _rand.Next(Width - 2) + 1;
					_food.y = _rand.Next(Height - 2) + 1;
				}

				// If the snake runs into a wall...
				if (_snake.DetectCollision()) {

					// ...end the game
					_playing = false;
				} else {
					// Draw the scene to the current buffer
					try {
						buffers[curBuffer].AddIcon(_food.x, _food.y, '$');
					} catch (Exception e) {
						Console.Write(curBuffer);
					}
					_snake.Draw(buffers[curBuffer]);

					// Clear the old buffer from the screen
					buffers[(curBuffer + 1) % 2].Clear();

					// Render the current buffer to the console
					buffers[curBuffer].Render();
				}

				// Cycle the current buffer
				curBuffer = (curBuffer + 1) % 2;

				// Stop the timer and get the elapsed time in ms
				timer.Stop();
				var elapsedTime = (int)timer.Elapsed.TotalMilliseconds;

				// Sleep until the next frame
				Thread.Sleep(GameSpeed - elapsedTime);
			}
		}

		// asynchronous user input loop
		static void GetUserInput()
		{
			while (true) {
				// Read a key from the console
				var key = Console.ReadKey().Key;

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
			// initialize all our threads
			var gameLoop = new Thread(new ThreadStart(GameLoop));
			var userInputLoop = new Thread(new ThreadStart(GetUserInput));
			
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