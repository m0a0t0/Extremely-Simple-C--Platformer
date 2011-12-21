using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using GameState;
using Constants;

namespace Platformer
{
	public class Machine : GameStateMachine
	{
		public Machine ()
		{
			sfcGameWindow = Video.SetVideoMode (Constants.Constants.WIDTH, Constants.Constants.HEIGHT);
			Video.WindowCaption = "Platformer";
			//state = new RunGameState (sfcGameWindow);
			MakeMenuState ();
		}
		
		public void MakeMenuState ()
		{
			state = new MenuState (sfcGameWindow);
			((MenuState)state).menuStart.selectedHandler += EventMenuStart;
			((MenuState)state).Run ();			
		}
		
		public void MakeRunGameState ()
		{
			state = new RunGameState (sfcGameWindow);
			((RunGameState)state).eventDead += EventDead;			
		}
		
		void EventMenuStart (GameMenu.MenuObject obj)
		{
			MakeRunGameState ();
		}
		
		void EventDead ()
		{
			MakeMenuState ();	
		}
		protected override void Update (float elapsed)
		{
			state.Update (elapsed);
		}
		protected override void Draw ()
		{
			state.Draw ();
		}
	}
}

