using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using System;
using System.Xml;
using System.Collections.Generic;
using System.Drawing;
using GameState;
using GameMenu;
using GameGraphic;

namespace Platformer
{	
	public class EditorPlayer : Sprite {
		public Vector gridPos;
		Graphic graphic;
		public EditorPlayer (Vector _gridPos) : base()
		{
			gridPos = _gridPos;
			graphic = new Graphic (Color.Blue, Player.WIDTH, Player.HEIGHT);
		}
		
		public void Update (Camera c)
		{
			ApplyCamera (c);
		}
		
		public void Draw (Surface sfcGameWindow)
		{
			graphic.Draw (sfcGameWindow, (int)x, (int)y, 255, true);
		}
	}
	
	public class EditorGrid : Sprite {
		private Surface sfcGrid;
		public EditorGrid ()
		{
			sfcGrid = new Surface (new Size (Constants.Constants.MAP_WIDTH + Constants.Constants.MAP_WIDTH * 
				Tile.WIDTH, Constants.Constants.MAP_HEIGHT + Constants.Constants.MAP_HEIGHT * Tile.HEIGHT));
			sfcGrid.Fill (Color.Transparent);
			Graphic vertical = new Graphic (Color.Black, 1, Tile.HEIGHT * Constants.Constants.MAP_HEIGHT 
				+ Constants.Constants.MAP_HEIGHT * 2);
			Graphic horizont = new Graphic (Color.Black, Tile.WIDTH * Constants.Constants.MAP_WIDTH + 
				Constants.Constants.MAP_WIDTH * 2, 1);
			for (int x=0; x < Constants.Constants.MAP_WIDTH; x++) {
				vertical.Draw (sfcGrid, x * 2 + x * Tile.WIDTH, 0, 255, true);
			}
			for (int y=0; y < Constants.Constants.MAP_HEIGHT; y++) {
				horizont.Draw (sfcGrid, 0, y * 2 + y * Tile.HEIGHT, 255, true);
			}			
		}
		
		public void Update (Camera c)
		{
			ApplyCamera (c);
		}
		
		public void Draw (Surface sfcGameWindow)
		{
			sfcGameWindow.Blit (sfcGrid, new Rectangle (new Point ((int)x, (int)y), 
				new Size (Constants.Constants.WIDTH, Constants.Constants.HEIGHT)));
		}
		
	}
	public class LevelEditorState : GameState.GameState
	{
		private Menu menu;
		public MenuText menuQuit, menuSave;
		private Graphic menuBackground;
		private Graphic menuBackgroundBorder;		
		List<MenuObject > menuObjs;
		public EditorGrid grid;
		private Map map;
		private Menu nameMenu;
		private string name;
		private Vector vecPlayer;
		private EditorPlayer player;
		private Camera camera;
		public static float CAMERA_SPEED = 5f;
		public static int TEXT_SIZE = 25;
		private float overallCameraX, overallCameraY;
		
		public LevelEditorState (Surface _sfcGameWindow, string _name="untitled") : base(_sfcGameWindow)
		{
			camera = new Camera ();
			name = _name;
			map = new Map ();
			map.editor = true;	
			vecPlayer = new Vector (new Point (2, 3));
			player = new EditorPlayer (vecPlayer);
			if (name.EndsWith (".xml")) {
				XmlDocument doc = new XmlDocument ();
				doc.Load (Constants.Constants.GetResourcePath (name));
				map.FromXML (doc);
				XmlNode p = doc.SelectSingleNode ("//player");
				player.gridPos.X = Convert.ToInt16 (p.Attributes ["x"].InnerText);
				player.gridPos.Y = Convert.ToInt16 (p.Attributes ["y"].InnerText);
				name = name.Replace (".xml", "");				
			} else {			
				map.EmptyMap ();
			}
			
			Vector v = map.GetTilePos ((int)player.gridPos.X, (int)player.gridPos.Y);
			player.x = v.X;
			player.y = v.Y;	
			
			grid = new EditorGrid ();
			
			menuObjs = new List<MenuObject> ();
			foreach (TileType typ in Enum.GetValues(typeof(TileType))) {
				MenuText t = new MenuText (typ.ToString (), Color.Gold, Color.Black, TEXT_SIZE);
				menuObjs.Add (t);
			}
			MenuText menuPlayer = new MenuText ("Player", Color.Gold, Color.Black, TEXT_SIZE);
			menuObjs.Add (menuPlayer);
			menuSave = new MenuText ("Save", Color.Gold, Color.Red, TEXT_SIZE);	
			menuSave.selectedHandler += EventSave;
			menuObjs.Add (menuSave);
			menuQuit = new MenuText ("Exit", Color.Gold, Color.Red, TEXT_SIZE);
		}
		
		public void Run ()
		{
			menuObjs.Add (menuQuit);
			MenuText menuName = new MenuText (name, Color.Green, Color.Green, TEXT_SIZE);
			menuObjs.Add (menuName);			
			menu = new Menu (menuObjs, MenuLayout.Horizontal, 5, 700);	
			
			menuBackgroundBorder = new Graphic (Color.Black, menu.width - 2, Constants.Constants.HEIGHT - 700);
			menuBackground = new Graphic (Color.White, menu.width - 2, Constants.Constants.HEIGHT - 700 - 2);			
		}
		

		void EventSave (MenuObject obj)
		{
			List<MenuObject > objs = new List<MenuObject> ();
			MenuTextEntry nameMenuTextEntry = new MenuTextEntry (42, Color.Black, name);
			nameMenuTextEntry.escapeHandler += EventNameEscape;
			nameMenuTextEntry.selectedHandler += EventNameSave;
			objs.Add (nameMenuTextEntry);
			nameMenu = new Menu (objs, MenuLayout.Vertical, 10, 
				Constants.Constants.HEIGHT / 2);
			//nameMenu.selectChangeTime = 0.5f;
		}
		
		void EventNameSave (MenuObject obj)
		{
			MenuTextEntry tE = (MenuTextEntry)obj;
			name = tE.text;
			MenuText t = (MenuText)menu.objects [menu.objects.Count - 1];
			t.text = name;
			t.colourSelected = Color.Green;
			t.colourNotSelected = Color.Green;
			t.RenderText ();			
			menu.lastSelectChange = -0.2f;
			nameMenu = null;
			map.ToXML (name, vecPlayer);			
		}
		
		
		void EventNameEscape (MenuObject obj)
		{
			nameMenu = null;
		}
		
		public override void Update (float elapsed)
		{
			if (nameMenu != null) {
				nameMenu.Update (elapsed, new Camera ());
				return;
			}
			HandleMouse ();
			HandleInput ();
			
			menu.Update (elapsed);
			Player p = null;
			map.Update (elapsed, ref p, camera);
			grid.Update (camera);
			player.Update (camera);	
		}
		
		void HandleMouse ()
		{
			if (Mouse.IsButtonPressed (MouseButton.PrimaryButton)) {
				MenuText mT = (MenuText)menu.objects [menu.objects.Count - 1];
				if (mT.colourSelected != Color.Red) {
					mT.colourSelected = Color.Red;
					mT.colourNotSelected = Color.Red;
					mT.RenderText ();
				}
				int tileX, tileY;
				tileX = tileY = 0;
				bool tileXA = false;
				bool tileYA = false;
				for (int x=0; x < Constants.Constants.MAP_WIDTH; x++) {
					if (Mouse.MousePosition.X+overallCameraX < x * Tile.WIDTH + x * 2 + 1) {
						tileX = x - 1;
						tileXA = true;
						break;
					}
				}
				for (int y=0; y < Constants.Constants.MAP_HEIGHT; y++) {
					if (Mouse.MousePosition.Y+overallCameraY < y * Tile.WIDTH + y * 2 + 1) {
						tileY = y - 1;
						tileYA = true;
						break;
					}
				}	
				if (tileXA && tileYA) {
					MenuText menuText = (MenuText)menu.objects [menu.selected];
					if (menuText.text == "Player") {
						player.gridPos = new Vector (new Point (tileX, tileY));
						Vector v = map.GetTilePos ((int)player.gridPos.X, (int)player.gridPos.Y);
						player.x = v.X;
						player.y = v.Y;								
					} else if (menuText.text != "Exit" && menuText.text != "Save" && menuText.text != name) {
						TileType typ = (TileType)Enum.Parse (typeof(TileType), menuText.text, true);
						Graphic t = map.lookup [typ].Clone ();
						if (typ == TileType.Fire) {
							t.colour = Color.Orange;
						}
						map.map [tileY] [tileX].tileType = typ;
						map.map [tileY] [tileX].tileGraphic = t;						
					}
				}
			}			
		}
		
		void HandleInput ()
		{
			if (Keyboard.IsKeyPressed (Key.A)) {
				camera.x = CAMERA_SPEED;
			} else if (Keyboard.IsKeyPressed (Key.D)) {
				camera.x = -CAMERA_SPEED;
			} else {
				camera.x = 0;
			}
			if (Keyboard.IsKeyPressed (Key.W)) {
				camera.y = CAMERA_SPEED;
			} else if (Keyboard.IsKeyPressed (Key.S)) {
				camera.y = -CAMERA_SPEED;
			} else {
				camera.y = 0;
			}
			overallCameraX += -1 * camera.x;
			overallCameraY += -1 * camera.y;
		}
		public override void Draw ()
		{
			sfcGameWindow.Fill (Color.White);
			
			if (nameMenu != null) {
				nameMenu.Draw (sfcGameWindow);
				sfcGameWindow.Update ();
				return;
			}
			//sfcGameWindow.Blit (sfcGrid, new Rectangle (new Point (0, 0), new Size (Constants.Constants.WIDTH, Constants.Constants.HEIGHT)));			
			grid.Draw (sfcGameWindow);
			map.Draw (sfcGameWindow);			
			menuBackgroundBorder.Draw (sfcGameWindow, 5, 700, 255, true);			
			menuBackground.Draw (sfcGameWindow, 6, 700, 255, true);
			player.Draw (sfcGameWindow);
			menu.Draw (sfcGameWindow);	
			sfcGameWindow.Update ();
		}
	}
}

