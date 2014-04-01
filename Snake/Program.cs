using System;
using System.Collections.Generic;
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
	static class Program
	{
		public const int Width = 80, Height = 24;
		static SnakeDirection _direction = SnakeDirection.Right;
		static bool _playing = true;
		const int GameSpeed = 100;
		static Snake _snake = new Snake(10, 10);
		static Food _food;
		static Random _rand = new Random();

		public enum SnakeDirection
		{
			Up,
			Down,
			Left,
			Right,
		};

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
			_food.x = _rand.Next(Width - 2) + 1;
			_food.y = _rand.Next(Height - 2) + 1;

			// main game loop
			while (_playing) {
				// Clear food and snake from the console
				ClearFood(_food);
				_snake.Clear();

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
					// Redraw food and snake to the console
					DrawFood(_food);
					_snake.Draw();
				}

				// Sleep until the next frame
				Thread.Sleep(GameSpeed);
			}
		}

		// asynchronous user input loop
		static void GetUserInput()
		{
			while (true) {
				var key = Console.ReadKey().Key;

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
			userInputLoop.Abort();

			Console.SetCursorPosition(0, Height);
			Console.Write("\bYou are a loser!");

			Console.ReadKey();
		}
	}
}