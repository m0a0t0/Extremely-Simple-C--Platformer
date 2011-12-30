using System;
using System.Drawing;
using System.Collections.Generic;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Primitives;
using GameGraphic;
using System.Xml;
using System.IO;

namespace Platformer
{
	public enum TileType {
		Air = 0,
		Brick = 1,
		Ladder = 2,
		Fire = 3,
	};
	
	public class Map
	{
		public List<List<Tile>> map;
		public Dictionary<TileType,Graphic> lookup;
		public bool editor = false;
		public Camera overallCamera;
		
		public Map ()
		{
			overallCamera = new Camera ();
			map = new List<List<Tile>> ();
			lookup = new Dictionary<TileType, Graphic> ();
			lookup.Add (TileType.Air, new Graphic (Color.Transparent, Tile.WIDTH, Tile.HEIGHT));
			lookup.Add (TileType.Brick, new Graphic (Color.Brown, Tile.WIDTH, Tile.HEIGHT));
			lookup.Add (TileType.Fire, new Graphic (Color.Transparent, Tile.WIDTH, Tile.HEIGHT));
		}
		
		public void FromXML (XmlDocument doc)
		{			
			EmptyMap ();
			EffectFire fire = new EffectFire ();
			XmlNodeList tiles = doc.SelectNodes ("//tile");
			foreach (XmlNode node in tiles) {
				int x = Convert.ToInt16 (node.Attributes ["x"].InnerText);
				int y = Convert.ToInt16 (node.Attributes ["y"].InnerText);
				Vector v = GetTilePos (x, y);
				TileType typ = (TileType)Enum.Parse (typeof(TileType), node.Attributes ["type"].InnerText, true);
				if (typ == TileType.Fire && editor) {
					map [y] [x].tileGraphic = new Graphic (Color.Orange, Tile.WIDTH, Tile.HEIGHT);
				} else {
					map [y] [x].tileGraphic = lookup [typ].Clone ();					
				}
				if (typ == TileType.Fire && !editor) {
					Vector xPos = new Vector (new Point ((int)v.X, (int)v.X + Tile.WIDTH));
					Vector yPos = new Vector (new Point ((int)v.Y - EffectFire.HEIGHT, (int)v.Y - 
							EffectFire.HEIGHT));
					ParticleOptions p = fire.template.Clone ();
					p.xPosRange = xPos;
					p.yPosRange = yPos;
					map [y] [x].system = new ParticleSystem (p);
				}									
				map [y] [x].tileType = typ;
				map [y] [x].x = (int)v.X;
				map [y] [x].y = (int)v.Y;
			}
		}
			
		public void ToXML (String name, Vector playerPos, List<EditorSprite> enemies)
		{
			XmlWriter writer = new XmlTextWriter (Constants.Constants.GetResourcePath (name + ".xml"), null);
			writer.WriteStartDocument ();
			writer.WriteStartElement ("map");
			writer.WriteStartElement ("tiles");
			foreach (List<Tile> r in map) {
				foreach (Tile c in r) {
					writer.WriteStartElement ("tile");
					writer.WriteAttributeString ("x", r.IndexOf (c).ToString ());
					writer.WriteAttributeString ("y", map.IndexOf (r).ToString ());
					writer.WriteAttributeString ("type", c.tileType.ToString ());
					writer.WriteEndElement ();
				}
			}
			writer.WriteEndElement ();	
			writer.WriteStartElement ("player");
			writer.WriteAttributeString ("x", playerPos.X.ToString ());
			writer.WriteAttributeString ("y", playerPos.Y.ToString ());
			writer.WriteEndElement ();	
			writer.WriteStartElement ("enemies");
			foreach (EditorSprite sprite in enemies) {
				writer.WriteStartElement ("enemy");
				EnemyType typ = (EnemyType)sprite.data;
				writer.WriteAttributeString ("x", sprite.gridPos.X.ToString ());
				writer.WriteAttributeString ("y", sprite.gridPos.Y.ToString ());				
				writer.WriteAttributeString ("type", typ.ToString ());
				writer.WriteEndElement ();
			}
			writer.WriteEndElement ();	
			writer.Close ();
		}
		
		public Vector GetTilePos (int x, int y)
		{
			Vector v = new Vector (new Point (x * Tile.WIDTH, y * Tile.HEIGHT));
			if (editor) {
				v.X = x * Tile.WIDTH + x * 2 + 1 + -1*overallCamera.x;
				v.Y = y * Tile.HEIGHT + y * 2 + 1 + -1*overallCamera.y;
			}
			return v;
		}
		public void EmptyMap ()
		{
			for (int y=0; y < Constants.Constants.MAP_HEIGHT; y++) {
				List<Tile > row = new List<Tile> ();
				for (int x=0; x < Constants.Constants.MAP_WIDTH; x++) {
					Vector v = GetTilePos (x, y);
					Tile t = new Tile ((int)v.X, (int)v.Y, TileType.Air, lookup [TileType.Air]);
					row.Add (t);
				}
				map.Add (row);
			}
		}
		
		public void Draw (Surface sfcGameWindow)
		{
			foreach (List<Tile> r in map) {
				foreach (Tile c in r) {
					if (!c.outOfSight)
						c.Draw2 (sfcGameWindow, editor);
				}
			}
		}

		public void Update (float elapsed, ref Player player, Camera _camera, ref List<Enemy> enemies)
		{	
			overallCamera.x += -1 * _camera.x;
			overallCamera.y += -1 * _camera.y;
			
			foreach (List<Tile> r in map) {
				foreach (Tile c in r) {
					c.Update (elapsed, _camera);
					if (c.tileType == TileType.Air || player == null) {
						continue;
					}
					GunSprite gS = (GunSprite)player;
					CollisionBullets (c, ref gS);
					Collision (ref gS, c, elapsed);					
					
					foreach (Enemy enemy in enemies) {
						gS = (GunSprite)enemy;
						CollisionBullets (c, ref gS);
						Collision (ref gS, c, elapsed);
					}
				}
			}
			
			if (player == null || enemies == null) {
				return;
			} 
			if (player.tileDirRelSprites.Count == 0) {
				player.falling = true;
			} else {
				foreach (TileDirectionRelSprite t in player.tileDirRelSprites) {
					GunSprite gS = (GunSprite)player;						
					MoveSprite (ref gS, t, gS.rectBot);
				}
			}
			
			foreach (Enemy enemy in enemies) {
				if (enemy.tileDirRelSprites.Count == 0) {
					enemy.falling = true;
				} else {
					foreach (TileDirectionRelSprite t in enemy.tileDirRelSprites) {
						GunSprite gS = (GunSprite)enemy;						
						MoveSprite (ref gS, t, gS.rectBot);
					}
				}				
			}
		}
		
		private void CollisionBullets (Tile c, ref GunSprite gS)
		{
			if (gS.gun.bullets != null) {
				foreach (Bullet b in gS.gun.bullets) {
					if (c.rect.IntersectsWith (b.rect)) {
						gS.gun.bullets [gS.gun.bullets.IndexOf (b)].dead = true;
					}
				}
			}			
		}
		
		private void Collision (ref GunSprite gS, Tile tile, float elapsed)
		{		
//			Tile mBLeft, mBRight;
//			mBLeft = map [(int)((gS.y + overallCamera.y) / Tile.HEIGHT) + 1] [(int)((gS.x + overallCamera.x) / 
//				Tile.WIDTH)];
//			mBRight = map [(int)((gS.y + overallCamera.y) / Tile.HEIGHT) + 1] [(int)((gS.width + gS.x + 
//				overallCamera.x) / Tile.WIDTH)];
//			
//			if (mBLeft.tileType != TileType.Air || mBRight.tileType != TileType.Air) {
//				gS.falling = false;
//			} else if (gS.falling == false && !gS.jumping) {
//				gS.falling = true;						
//			}	
			
			Rectangle rectLeft, rectRight, rectTop, rectBot;
			if (gS != null) {
				float xSpeed = gS.xDir * elapsed * gS.MOVE_SPEED;
				float ySpeed = gS.yDir * elapsed * gS.FALL_SPEED;				
				rectLeft = new Rectangle (new Point ((int)(gS.x + xSpeed), (int)(gS.y + ySpeed)), 
					new Size (1, gS.height));
				rectRight = new Rectangle (new Point ((int)(gS.x + xSpeed + gS.width), (int)(gS.y + 
					ySpeed)), new Size (1, gS.height));
				rectTop = new Rectangle (new Point ((int)(gS.x + xSpeed), ((int)(gS.y + ySpeed))), 
					new Size (Player.WIDTH, 1));
				rectBot = new Rectangle (new Point ((int)(gS.x + xSpeed), (int)(gS.y + gS.height + 
					ySpeed + 1)), new Size (gS.width, 1));
				gS.rectBot = rectBot;
				if (gS.gun.left) {
					rectLeft.X -= gS.gun.width;
				} else {
					rectRight.X += gS.gun.width;
				}
			}
			
			if (tile.outOfSight || gS == null) {
				return;			
			}
			TileDirectionRelSprite t = new TileDirectionRelSprite ();
			t.tile = tile;
			t.below = tile.rect.IntersectsWith (rectBot);
			t.above = tile.rect.IntersectsWith (rectTop);
			t.left = tile.rect.IntersectsWith (rectLeft);
			t.right = tile.rect.IntersectsWith (rectRight);
			if (t.below || t.above || t.left || t.right) {
				if (t.tile.tileType == TileType.Fire) {
					gS.dead = true;
				}
				gS.tileDirRelSprites.Add (t);
			}			
		}
		private void MoveSprite (ref GunSprite gS, TileDirectionRelSprite tile, Rectangle rectangle)
		{
			if (tile.below && !gS.jumping && rectangle.IntersectsWith (new Rectangle (new Point ((int)tile.tile.x, (int)tile.tile.y), new Size (Tile.WIDTH, 7)))) {
				gS.y = tile.tile.y - gS.height - 1;
				gS.falling = false;
				gS.yDir = 0;
			} else if (tile.left) {
				gS.x = tile.tile.x + Tile.WIDTH;
				if (gS.gun.left) {
					gS.x += gS.gun.width + 1;
				}
			} else if (tile.right) {
				gS.x = tile.tile.x - gS.width - 1;
				if (!gS.gun.left) {
					gS.x -= gS.gun.width;
				}	
			}
		}
	}
}

