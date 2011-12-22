using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using System;
using System.Xml;
using System.Collections.Generic;
using System.Drawing;
using GameState;
using Constants;
using GameGraphic;

namespace Platformer
{
	public delegate void DeadHandler ();
	public class RunGameState : GameState.GameState
	{
		Map map;
		Player player;
		Camera camera;
		public event DeadHandler eventDead;
		
		public RunGameState (Surface _sfcGameWindow, string name) : base(_sfcGameWindow)
		{
			map = new Map ();
			XmlDocument doc = new XmlDocument ();
			doc.Load (Constants.Constants.GetResourcePath (name));
			map.FromXML (doc);
			XmlNode p = doc.SelectSingleNode ("//player");
			player = new Player (Convert.ToInt32 (p.Attributes ["x"].InnerText) * Tile.WIDTH + 10, 
				Convert.ToInt32 (p.Attributes ["y"].InnerText) * Tile.WIDTH);
			camera = new Camera ();
		}
		
//		private void EventTick (object sender, TickEventArgs e)
//		{
//			Update (e.SecondsElapsed);
//			Draw ();
//		}
//                
//		private void EventQuit (object sender, QuitEventArgs e)
//		{
//			Events.QuitApplication ();
//		}	
		
		private void HandleInput ()
		{
			if (Keyboard.IsKeyPressed (Key.D)) {
				player.xSpeed = 1;
			} else if (Keyboard.IsKeyPressed (Key.A)) {
				player.xSpeed = -1;
			} else {
				player.xSpeed = 0;
			}
			
			if (Keyboard.IsKeyPressed (Key.W) && !player.jumping && !player.falling) {
				player.jumping = true;
			}
		}
		
		public override void Update (float elapsed)
		{
			if (player.system != null && player.system.finished) {
				eventDead ();
			}
			player.Update (elapsed, camera);			
			if (player.dead) {
				return;
			}
			HandleInput ();			
			map.Update (elapsed, ref player, camera);
			camera.Update (player);						
			
		}
		
		public override void Draw ()
		{
			sfcGameWindow.Fill (Color.White);
			sfcGameWindow.Lock ();
			map.Draw (sfcGameWindow);
			player.Draw (sfcGameWindow);
			sfcGameWindow.Unlock ();
			sfcGameWindow.Update ();
		}
	}
}

