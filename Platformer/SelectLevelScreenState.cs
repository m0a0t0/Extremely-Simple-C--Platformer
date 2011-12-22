using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using GameState;
using Constants;
using GameGraphic;
using GameMenu;

namespace Platformer
{
	public class SelectLevelScreenState : GameState.GameState
	{
		private Menu menu;
		private List<MenuObject> objs;
		public event MenuSelectedHandler fileSelectedHandler;
		public event MenuSelectedHandler menuExitHandler;
		
		public SelectLevelScreenState (Surface _sfcGameWindow, List<MenuObject> extraObjs) : base (_sfcGameWindow)
		{
			objs = new List<MenuObject> ();
			foreach (MenuObject obj in extraObjs) {
				obj.selectedHandler += EventSelected;				
				objs.Add (obj);
			}
			
			string[] files = Directory.GetFiles (Constants.Constants.LEVEL_PATH);
			foreach (string f in files) {
				string file = Path.GetFileName (f);
				MenuObject obj = new MenuText (file.Replace (".xml", ""), Color.Gold, Color.Black, 30);
				obj.selectedHandler += EventSelected;
				objs.Add (obj);
			}
			MenuText t = new MenuText ("Exit", Color.Gold, Color.Red, 30);
			t.selectedHandler += delegate(MenuObject obj) {
				menuExitHandler (obj);
			};
			objs.Add (t);
			menu = new Menu (objs, MenuLayout.Vertical);
		}

		void EventSelected (MenuObject obj)
		{
			fileSelectedHandler (obj);
		}
		
		public override void Update (float elapsed)
		{
			menu.Update (elapsed, new Camera());
		}
		
		public override void Draw ()
		{
			sfcGameWindow.Fill (Color.White);
			menu.Draw (sfcGameWindow);
			sfcGameWindow.Update ();
		}
	}
}

