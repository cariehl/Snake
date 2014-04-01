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
		static private SnakeDirection _direction = SnakeDirection.Right;
		private static bool _playing = true;
		private const int GameSpeed = 40;

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

		static void Buffer(Snake snake, Food food, string[,] buffer)
		{
			// Buffer Borders
			for (var row = 0; row < Height; row++) {
				for (var col = 0; col < Width; col++) {
					if (row == 0 || row == Height - 1 || col == 0 || col == Width - 1) {
						buffer[col, row] = "#";
					}
				}
			}

			// Buffer Snake
			snake.Draw(buffer);

			// Buffer Food
			buffer[food.x, food.y] = "$";
		}

		static void Clear(Snake snake, Food food, string[,] buffer)
		{
			// Clear Snake
			snake.Clear(buffer);

			// Clear Food
			buffer[food.x, food.y] = " ";
		}

		static void InitializeBuffer(string[,] buffer)
		{
			// Fill with blank spaces
			for (var row = 0; row < Height; row++) {
				for (var col = 0; col < Width; col++) {
					buffer[col, row] = " ";
				}
			}
		}

		static void GameLoop()
		{
			var rand = new Random();
			var curBuffer = 0;
			var buffers = new string[2][,];

			for (var i = 0; i < 2; i++) {
				buffers[i] = new string[Width, Height];
				InitializeBuffer(buffers[i]);
			}

			var snake = new Snake(10, 10);
			Food food;

			food.x = rand.Next(Width - 2) + 1;
			food.y = rand.Next(Height - 2) + 1;

			// main game loop
			while (_playing) {
				Clear(snake, food, buffers[curBuffer]);
				curBuffer = (curBuffer + 1) % 2;

				snake.Move(_direction);

				if (snake.DetectFood(food)) {
					snake.AddSegment();
					food.x = rand.Next(Width - 2) + 1;
					food.y = rand.Next(Height - 2) + 1;
				}

				if (snake.DetectCollision()) {
					_playing = false;
					Console.Write("\bYou are a loser!");
				} else {
					Buffer(snake, food, buffers[curBuffer]);

					// Write to console
					for (var i = 0; i < Height; i++) {
						Console.SetCursorPosition(0, i);
						for (var j = 0; j < Width; j++) {
							Console.Write(buffers[curBuffer][j, i]);
						}
					}
				}
				Thread.Sleep(GameSpeed);
			}
		}

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

		static void Main(string[] args)
		{
			var gameLoop = new Thread(new ThreadStart(GameLoop));
			var userInput = new Thread(new ThreadStart(GetUserInput));
			gameLoop.Start();
			userInput.Start();

			while (_playing) {
				userInput.Join(GameSpeed);
				gameLoop.Join();
			}
			userInput.Abort();
			gameLoop.Abort();
		}
	}
}
