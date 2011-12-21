using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GameState
{
	public abstract class GameState
	{
		protected Surface sfcGameWindow;
		public GameState (Surface _sfcGameWindow)
		{
			sfcGameWindow = _sfcGameWindow;
		}
		
		public abstract void Update (float elapsed);
		public abstract void Draw ();			
	}
	
	public abstract class GameStateMachine
	{
		protected Surface sfcGameWindow;		
		protected GameState state;
		
		public virtual void Run ()
		{
			Events.Tick += new EventHandler<TickEventArgs> (EventTick);
			Events.Quit += new EventHandler<QuitEventArgs> (EventQuit);
			Events.Run ();	
		}
		
		protected virtual void EventTick (object sender, TickEventArgs e)
		{
			Update (e.SecondsElapsed);
			Draw ();			
		}

		protected virtual void EventQuit (object sender, QuitEventArgs e)
		{
			Events.QuitApplication ();			
		}
		
		protected abstract void Update (float elapsed);
		protected abstract void Draw ();		
	}
}

