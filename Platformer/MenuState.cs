using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using GameState;
using Constants;
using GameGraphic;
using GameMenu;

namespace Platformer
{
	public class MenuState : GameState.GameState
	{
		private Menu menu;
		public MenuText menuStart;
		public MenuText menuQuit;
		private List<MenuObject> objs;
		
		public MenuState (Surface _sfcGameWindow) : base (_sfcGameWindow)
		{
			objs = new List<MenuObject> ();
			menuStart = new MenuText ("Start", Color.Gold, Color.Black, 42);
			menuQuit = new MenuText ("Exit", Color.Gold, Color.Black, 42);
			menuQuit.selectedHandler += EventMenuQuit;
		}

		void EventMenuQuit (MenuObject obj)
		{
			Events.QuitApplication ();
		}
		
		public void Run ()
		{
			objs.Add (menuStart);
			objs.Add (menuQuit);
			menu = new Menu (objs, MenuLayout.Vertical);			
		}
		
		public override void Update (float elapsed)
		{
			menu.Update (elapsed);
		}
		
		public override void Draw ()
		{
			menu.Draw (sfcGameWindow);
		}
	}
}

