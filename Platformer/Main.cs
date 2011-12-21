using System;
using GameState;

namespace Platformer
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Machine machine = new Machine ();
			machine.Run();
		}
	}
}
