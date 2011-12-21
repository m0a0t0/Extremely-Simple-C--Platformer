using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using GameState;
using GameMenu;

namespace Platformer
{	
	public class LevelEditorState : GameState.GameState
	{
		private Menu menu;
		public MenuText menuQuit;
		List<MenuObject > menuObjs;
		
		public LevelEditorState (Surface _sfcGameWindow) : base(_sfcGameWindow)
		{
			menuObjs = new List<MenuObject> ();
			foreach (TileType typ in Enum.GetValues(typeof(TileType))) {
				MenuText t = new MenuText (typ.ToString (), Color.Gold, Color.Black, 25);
				menuObjs.Add (t);
			}
			menuQuit = new MenuText ("Exit", Color.Gold, Color.Red, 25);
		}
		
		public void Run ()
		{
			menuObjs.Add (menuQuit);
			menu = new Menu (menuObjs, MenuLayout.Horizontal, 5, 700);			
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

