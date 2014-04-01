using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
	interface IBufferable
	{
		void Draw(string[,] buffer);

		void Clear(string[,] buffer);
	}
}
