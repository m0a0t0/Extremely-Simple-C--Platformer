using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using System;
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
		
		public RunGameState (Surface _sfcGameWindow) : base(_sfcGameWindow)
		{
			string str = "0000000000000000000000\n" +
						 "0010000000000000000000\n" +
						 "0110000000000000000000\n" +
						 "1111111111000111111111\n" +
						 "0000000000000000000000\n" +
						 "0000000000000000000000\n" +
						 "3333333333333333333333\n";
			map = new Map ();
			map.LoadFromString (str);
			player = new Player (0 * Tile.WIDTH + 10, 0 * Tile.WIDTH);
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
			camera.Update (player);			
			map.Update (elapsed, ref player, camera);
			
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

