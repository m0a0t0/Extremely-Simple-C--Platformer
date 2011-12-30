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
		private List<Enemy> enemies;		
		
		public RunGameState (Surface _sfcGameWindow, string name) : base(_sfcGameWindow)
		{
			map = new Map ();
			XmlDocument doc = new XmlDocument ();
			doc.Load (Constants.Constants.GetResourcePath (name));
			map.FromXML (doc);
			XmlNode p = doc.SelectSingleNode ("//player");
			player = new Player (Convert.ToInt32 (p.Attributes ["x"].InnerText) * Tile.WIDTH + 10, 
				Convert.ToInt32 (p.Attributes ["y"].InnerText) * Tile.WIDTH);
			enemies = new List<Enemy> ();
			foreach (XmlNode node in doc.SelectNodes("//enemy")) {
				EnemyType typ = (EnemyType)Enum.Parse (typeof(EnemyType), node.Attributes ["type"].InnerText);
				int x = Convert.ToInt16 (node.Attributes ["x"].InnerText);
				int y = Convert.ToInt16 (node.Attributes ["y"].InnerText);				
				Vector v = map.GetTilePos (x, y);
				enemies.Add (Enemy.GetEnemy (typ, v.X, v.Y));
			}
			camera = new Camera ();
		}
		
		private void HandleInput ()
		{
			if (Keyboard.IsKeyPressed (Key.D)) {
				player.xDir = 1;
			} else if (Keyboard.IsKeyPressed (Key.A)) {
				player.xDir = -1;
			} else {
				player.xDir = 0;
			}
			if (Keyboard.IsKeyPressed (Key.W) && !player.jumping && !player.falling) {
				player.jumping = true;
			}
			
			if (Keyboard.IsKeyPressed (Key.Space)) {
				player.gun.Fire ();
			}
		}
		
		public override void Update (float elapsed)
		{
			if (player.system != null && player.system.finished) {
				eventDead ();
			}
			player.Update (elapsed, camera, player, map);			
			if (player.dead) {
				return;
			}
			for (int i=0; i < enemies.Count; i++) {
				Enemy enemy = enemies [i];
				if (enemy.system != null && enemy.system.finished) {
					enemies.Remove (enemy);
					i -= 1;
				} else {
					enemy.Update (elapsed, camera, player, map);
					foreach (Bullet b in player.gun.bullets) {
						if (b.rect.IntersectsWith (enemy.rect)) {
							b.dead = true;
							enemy.health -= b.hitPoints;
						}
					}
				}
			}
			HandleInput ();			
			map.Update (elapsed, ref player, camera, ref enemies);
			camera.Update (player);						
			
		}
		
		public override void Draw ()
		{
			sfcGameWindow.Fill (Color.White);
			sfcGameWindow.Lock ();
			map.Draw (sfcGameWindow);
			player.Draw (sfcGameWindow);
			foreach (Enemy enemy in enemies) {
				enemy.Draw (sfcGameWindow);
			}
			sfcGameWindow.Unlock ();
			sfcGameWindow.Update ();
		}
	}
}

