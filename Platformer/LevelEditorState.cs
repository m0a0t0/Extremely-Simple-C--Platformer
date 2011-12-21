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
	public delegate void EditorExitHandler(TileType t);
	
	public class LevelEditorState : GameState.GameState
	{
		private Menu menu;
		public event EditorExitHandler exitHandler;
		
		public LevelEditorState (Surface _sfcGameWindow) : base(_sfcGameWindow)
		{
			List<MenuObject > menuObjs = new List<MenuObject> ();
			foreach (TileType typ in Enum.GetValues(typeof(TileType))) {
				MenuText t = new MenuText (typ.ToString (), Color.Gold, Color.Black, 25);
				menuObjs.Add (t);
			}
		}
		
		public override void Update (float elapsed)
		{
		}
		
		public override void Draw ()
		{
		}
	}
}

