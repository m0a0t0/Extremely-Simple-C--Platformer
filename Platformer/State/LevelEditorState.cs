using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Drawing;
using GameState;
using GameMenu;
using GameGraphic;

namespace Platformer
{	
	public class EditorSprite : Sprite {
		public Vector gridPos;
		public object data;
		
		public EditorSprite (Vector _gridPos, Sprite sprite) : base()
		{
			gridPos = _gridPos;
			graphic = sprite.graphic;
		}
		
		public void Update (Camera c)
		{
			ApplyCamera (c);
		}
		
		public override void Draw (Surface sfcGameWindow)
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
		
		public override void Draw (Surface sfcGameWindow)
		{
			sfcGameWindow.Blit (sfcGrid, new Rectangle (new Point ((int)x, (int)y), 
				new Size (Constants.Constants.WIDTH, Constants.Constants.HEIGHT)));
		}
		
	}
	public class LevelEditorState : GameState.GameState
	{
		private Menu menu;
		private Graphic menuBackground;
		private Graphic menuBackgroundBorder;		
		public EditorGrid grid;
		private Map map;
		private Menu nameMenu;
		private string name;
		private Vector vecPlayer;
		private EditorSprite player;
		private Camera camera;
		public static float CAMERA_SPEED = 5f;
		public static int TEXT_SIZE = 25;
		public event MenuSelectedHandler backToMenuHandler;
		private List<EditorSprite> enemies;
		
		public LevelEditorState (Surface _sfcGameWindow, string _name="untitled") : base(_sfcGameWindow)
		{
			camera = new Camera ();
			name = _name;
			map = new Map ();
			map.editor = true;	
			vecPlayer = new Vector (new Point (2, 3));
			player = new EditorSprite (vecPlayer, new Player (0, 0));
			enemies = new List<EditorSprite> ();
			if (name.EndsWith (".xml")) {
				XmlDocument doc = new XmlDocument ();
				doc.Load (Constants.Constants.GetResourcePath (name));
				map.FromXML (doc);
				XmlNode p = doc.SelectSingleNode ("//player");
				player.gridPos.X = Convert.ToInt16 (p.Attributes ["x"].InnerText);
				player.gridPos.Y = Convert.ToInt16 (p.Attributes ["y"].InnerText);
				name = name.Replace (".xml", "");		
				foreach (XmlNode node in doc.SelectNodes("//enemy")) {
					EnemyType typ = (EnemyType)Enum.Parse (typeof(EnemyType), node.Attributes ["type"].InnerText);
					int x = Convert.ToInt16 (node.Attributes ["x"].InnerText);
					int y = Convert.ToInt16 (node.Attributes ["y"].InnerText);
					Enemy enemy = Enemy.GetEnemy (typ, x, y);
					EditorSprite s = new EditorSprite (new Vector (new Point (x, y)), enemy);
					Vector vec = map.GetTilePos (x, y);
					s.x = vec.X;
					s.y = vec.Y;					
					s.data = (object)typ;
					enemies.Add (s);
				}
			} else {			
				map.EmptyMap ();
			}
			
			Vector v = map.GetTilePos ((int)player.gridPos.X, (int)player.gridPos.Y);
			player.x = v.X;
			player.y = v.Y;	
			
			grid = new EditorGrid ();
			
			List<MenuObject > menuObjs = new List<MenuObject> ();
			foreach (TileType typ in Enum.GetValues(typeof(TileType))) {
				MenuText t = new MenuText (typ.ToString (), Color.Gold, Color.Black, TEXT_SIZE);
				menuObjs.Add (t);
			}
			foreach (EnemyType typ in Enum.GetValues(typeof(EnemyType))) {
				MenuText t = new MenuText (typ.ToString (), Color.Gold, Color.Gray, TEXT_SIZE);
				menuObjs.Add (t);
			}			
			MenuText menuNoEnemy = new MenuText ("NoEnemy", Color.Gold, Color.Gray, TEXT_SIZE);
			menuObjs.Add (menuNoEnemy);
			MenuText menuPlayer = new MenuText ("Player", Color.Gold, Color.Blue, TEXT_SIZE);
			menuObjs.Add (menuPlayer);
			MenuText menuSave = new MenuText ("Save", Color.Gold, Color.Red, TEXT_SIZE);	
			menuSave.selectedHandler += EventSave;
			menuObjs.Add (menuSave);
			MenuText menuDelete = new MenuText ("Delete", Color.Gold, Color.Red, TEXT_SIZE);
			menuDelete.selectedHandler += EventDelete;
			menuObjs.Add (menuDelete);
			MenuText menuQuit = new MenuText ("Exit", Color.Gold, Color.Red, TEXT_SIZE);
			menuQuit.selectedHandler += delegate(MenuObject obj) {
				backToMenuHandler (obj);
			};
			menuObjs.Add (menuQuit);
			MenuText menuName = new MenuText (name, Color.Green, Color.Green, TEXT_SIZE);
			menuObjs.Add (menuName);			
			menu = new Menu (menuObjs, MenuLayout.Horizontal, 5, 700);	
			GenerateMenuBackground ();			
		}

		void EventDelete (MenuObject obj)
		{
			try {
				File.Delete (Constants.Constants.GetResourcePath (name + ".xml"));
			} catch (Exception ex) {
				Console.WriteLine (ex.ToString ());
			}
			backToMenuHandler (obj);
		}
		
		public void GenerateMenuBackground () {
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
			menu.GenerateWidth ();
			GenerateMenuBackground ();						
			menu.lastSelectChange = -0.2f;
			nameMenu = null;
			map.ToXML (name, vecPlayer, enemies);
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
			List<Enemy > _enemies = null;
			map.Update (elapsed, ref p, camera, ref _enemies);
			grid.Update (camera);
			player.Update (camera);	
			foreach (EditorSprite s in enemies) {
				s.Update (camera);
			}
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
					if (Mouse.MousePosition.X + map.overallCamera.x < x * Tile.WIDTH + x * 2 + 1) {
						tileX = x - 1;
						tileXA = true;
						break;
					}
				}
				for (int y=0; y < Constants.Constants.MAP_HEIGHT; y++) {
					if (Mouse.MousePosition.Y + map.overallCamera.y < y * Tile.WIDTH + y * 2 + 1) {
						tileY = y - 1;
						tileYA = true;
						break;
					}
				}	
				if (tileXA && tileYA) {
					MenuText menuText = (MenuText)menu.objects [menu.selected];
					Vector v = map.GetTilePos (tileX, tileY);					
					if (menuText.text == "Player") {
						player.gridPos = new Vector (new Point (tileX, tileY));
						player.x = v.X;
						player.y = v.Y;		
					} else if (menuText.text == "NoEnemy") {
						EditorSprite removeSprite = null;
						foreach (EditorSprite s in enemies) {
							if (s.gridPos.X == tileX && s.gridPos.Y == tileY) {
								removeSprite = s;
							}
						}
						if (removeSprite != null) {
							enemies.Remove (removeSprite);
						}
					} else if (menuText.text.EndsWith ("Enemy")) {
						EnemyType typ = (EnemyType)Enum.Parse (typeof(EnemyType), menuText.text);
						Enemy e = Enemy.GetEnemy (typ, 0, 0);
						EditorSprite s = new EditorSprite (new Vector (new Point (tileX, tileY)), e);	
						s.data = (object)typ;	
						for (int i=0; i < enemies.Count; i++) {
							EditorSprite sprite = enemies [i];
							if (sprite.gridPos.X == s.gridPos.X && sprite.gridPos.Y == s.gridPos.Y) {
								enemies.Remove (sprite);
								i--;
							}
						}
						s.x = v.X;
						s.y = v.Y;
						enemies.Add (s);
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
			foreach (EditorSprite s in enemies) {
				s.Draw (sfcGameWindow);
			}
			menu.Draw (sfcGameWindow);	
			sfcGameWindow.Update ();
		}
	}
}

