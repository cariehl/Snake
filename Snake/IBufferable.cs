using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
	// An object that is able to be drawn to a buffer
	interface IBufferable
	{
		// Draws this object to the given buffer
		void Draw(Buffer buffer);
	}
}
