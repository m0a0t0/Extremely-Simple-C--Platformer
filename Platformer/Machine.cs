using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using GameState;
using Constants;
using GameMenu;

namespace Platformer
{
	public class Machine : GameStateMachine
	{
		public Machine ()
		{
			sfcGameWindow = Video.SetVideoMode (Constants.Constants.WIDTH, Constants.Constants.HEIGHT);
			Video.WindowCaption = "Platformer";
			MakeMenuState ();
		}
		
		public void MakeMenuState ()
		{
			state = new MenuState (sfcGameWindow);
			((MenuState)state).menuStart.selectedHandler += EventMenuStart;
			((MenuState)state).menuEditor.selectedHandler += EventMenuEditor;			
			((MenuState)state).Run ();		
		}
		
		public void MakeEditorState ()
		{
			List<MenuObject > objs = new List<MenuObject> ();
			objs.Add (new MenuText ("New", Color.Gold, Color.Black, 30));
			state = new SelectLevelScreenState (sfcGameWindow, objs);
			((SelectLevelScreenState)state).fileSelectedHandler += EventEditorFileSelected;
			((SelectLevelScreenState)state).menuExitHandler += EventToMenu;			
		}

		void EventEditorFileSelected (GameMenu.MenuObject obj)
		{
			MakeActualEditorState (((MenuText)obj).text);
		}
		
		void MakeActualEditorState (string text)
		{
			string t = text + ".xml";
			if (text == "New") {
				t = "untitled";
			}
			state = new LevelEditorState (sfcGameWindow, t);
			((LevelEditorState)state).backToMenuHandler += EventToMenu;	
		}
		
		void MakeRunGameState ()
		{
			state = new SelectLevelScreenState (sfcGameWindow, new List<MenuObject> ());
			((SelectLevelScreenState)state).fileSelectedHandler += EventRunFileSelected;
			((SelectLevelScreenState)state).menuExitHandler += EventToMenu;			
		
		}
		
		void EventRunFileSelected (GameMenu.MenuObject obj)
		{
			MakeActualRunGameState (((MenuText)obj).text);
		}
		
		void MakeActualRunGameState (string text)
		{
			string t = text + ".xml";			
			state = new RunGameState (sfcGameWindow, t);
			((RunGameState)state).eventDead += EventDead;				
		}
		
		void EventMenuStart (GameMenu.MenuObject obj)
		{
			MakeRunGameState ();
		}
		
		void EventMenuEditor (GameMenu.MenuObject obj)
		{
			MakeEditorState ();
		}
		
		void EventToMenu (GameMenu.MenuObject obj)
		{
			MakeMenuState ();
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

