using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using GameState;
using GameMenu;
using GameGraphic;

namespace Platformer
{	
	public class LevelEditorState : GameState.GameState
	{
		private Menu menu;
		public MenuText menuQuit;
		private Graphic menuBackground;
		private Graphic menuBackgroundBorder;		
		List<MenuObject > menuObjs;
		public Surface sfcGrid;
		private Map map;
		
		public LevelEditorState (Surface _sfcGameWindow) : base(_sfcGameWindow)
		{
			map = new Map ();
			map.EmptyMap ();
			sfcGrid = new Surface (new Size (Constants.Constants.MAP_WIDTH + Constants.Constants.MAP_WIDTH * 
				Tile.WIDTH, Constants.Constants.MAP_HEIGHT + Constants.Constants.MAP_HEIGHT * Tile.HEIGHT));
			sfcGrid.Fill (Color.Transparent);
			Graphic vertical = new Graphic (Color.Black, 1, Tile.HEIGHT * Constants.Constants.MAP_HEIGHT 
				+ Constants.Constants.MAP_HEIGHT*2);
			Graphic horizont = new Graphic (Color.Black, Tile.WIDTH * Constants.Constants.MAP_WIDTH + 
				Constants.Constants.MAP_WIDTH*2, 1);
			for (int x=0; x < Constants.Constants.MAP_WIDTH; x++) {
				vertical.Draw (sfcGrid, x*2 + x * Tile.WIDTH, 0, 255, true);
			}
			for (int y=0; y < Constants.Constants.MAP_HEIGHT; y++) {
				horizont.Draw (sfcGrid, 0, y*2 + y * Tile.HEIGHT, 255, true);
			}
			
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
			
			menuBackgroundBorder = new Graphic (Color.Black, menu.width - 2, Constants.Constants.HEIGHT - 700);
			menuBackground = new Graphic (Color.White, menu.width - 2, Constants.Constants.HEIGHT - 700 - 2);			
		}
		
		public override void Update (float elapsed)
		{
			menu.Update (elapsed);
			Player p = null;
			//map.Update (elapsed, ref p, new Camera ());
		}
		
		public override void Draw ()
		{
			sfcGameWindow.Fill (Color.White);

			sfcGameWindow.Blit (sfcGrid, new Rectangle (new Point (0, 0), new Size (Constants.Constants.WIDTH, Constants.Constants.HEIGHT)));			
			map.Draw (sfcGameWindow);			
			menuBackgroundBorder.Draw (sfcGameWindow, 5, 700, 255, true);			
			menuBackground.Draw (sfcGameWindow, 6, 700, 255, true);
			menu.Draw (sfcGameWindow);	
			sfcGameWindow.Update ();
		}
	}
}

