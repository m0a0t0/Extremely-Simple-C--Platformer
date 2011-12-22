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
		
		public Map ()
		{
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
			
		public void ToXML (String name, Vector playerPos)
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
			writer.WriteEndElement ();	
			writer.Close ();
		}
		
		public Vector GetTilePos (int x, int y)
		{
			Vector v = new Vector (new Point (x * Tile.WIDTH, y * Tile.HEIGHT));
			if (editor) {
				v.X = x * Tile.WIDTH + x * 2 + 1;
				v.Y = y * Tile.HEIGHT + y * 2 + 1;
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
						c.Draw (sfcGameWindow, editor);
				}
			}
		}

		public void Update (float elapsed, ref Player player, Camera _camera, ref List<Bullet> playerBullets)
		{
			Rectangle rectLeft, rectRight, rectTop, rectBot;
			if (player != null) {
				float xSpeed = player.xDir * elapsed * Player.MOVE_SPEED;
				float ySpeed = player.yDir * elapsed * Player.FALL_SPEED;				
				rectLeft = new Rectangle (new Point ((int)(player.x + xSpeed), (int)(player.y + ySpeed)), 
					new Size (1, Player.HEIGHT));
				rectRight = new Rectangle (new Point ((int)(player.x + xSpeed + Player.WIDTH), (int)(player.y + 
					ySpeed)), new Size (1, Player.HEIGHT));
				rectTop = new Rectangle (new Point ((int)(player.x + xSpeed), ((int)(player.y + ySpeed))), 
					new Size (Player.WIDTH, 1));
				rectBot = new Rectangle (new Point ((int)(player.x + xSpeed), (int)(player.y + Player.HEIGHT + 
					ySpeed + 1)), new Size (Player.WIDTH, 1));
				if (player.gun.left) {
					rectLeft.X -= player.gun.width;
				} else {
					rectRight.X += player.gun.width;
				}
			}
			
			List<TileDirectionRelPlayer > lst = new List<TileDirectionRelPlayer> ();			
			foreach (List<Tile> r in map) {
				foreach (Tile c in r) {
					c.Update (elapsed, _camera);
					if (c.tileType == TileType.Air) {
						continue;
					}
					if (playerBullets != null) {
						foreach (Bullet b in playerBullets) {
							if (c.rect.IntersectsWith (b.rect)) {
								playerBullets [playerBullets.IndexOf (b)].dead = true;
							}
						}
					}
				
					if (c.outOfSight || player == null) {
						continue;				
					}
					TileDirectionRelPlayer t = new TileDirectionRelPlayer ();
					t.tile = c;
					t.below = c.rect.IntersectsWith (rectBot);
					t.above = c.rect.IntersectsWith (rectTop);
					t.left = c.rect.IntersectsWith (rectLeft);
					t.right = c.rect.IntersectsWith (rectRight);
					if (t.below || t.above || t.left || t.right) {
						if (t.tile.tileType == TileType.Fire) {
							player.dead = true;
						}
						lst.Add (t);
					}
				}
			}
			if (player == null) {
				return;
			}
			if (lst.Count == 0 && !player.jumping) {
				player.falling = true;
			} else {
				foreach (TileDirectionRelPlayer t in lst) {
					MovePlayer (ref player, t, new Rectangle[] {rectLeft, rectRight, rectTop, rectBot});
				}
			}
		}
		
		private void MovePlayer (ref Player player, TileDirectionRelPlayer tile, Rectangle[] rectangles)
		{
			if (tile.below && !player.jumping && rectangles [3].IntersectsWith (new Rectangle (new Point ((int)tile.tile.x, (int)tile.tile.y), new Size (Tile.WIDTH, 7)))) {
				player.y = tile.tile.y - Player.HEIGHT - 1;
				player.falling = false;
				player.yDir = 0;
			} else if (tile.left) {
				player.x = tile.tile.x + Tile.WIDTH;
				if (player.gun.left) {
					player.x += player.gun.width + 1;
				}
			} else if (tile.right) {
				player.x = tile.tile.x - Player.WIDTH - 1;
				if (!player.gun.left) {
					player.x -= player.gun.width;
				}				
			}
		}
	}
}

