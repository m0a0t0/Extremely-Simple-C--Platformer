using System;
using System.Drawing;
using System.Collections.Generic;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Primitives;
using GameGraphic;

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
		private List<List<Tile>> map;
		private Dictionary<TileType,Graphic> lookup;
		
		public Map ()
		{
			lookup = new Dictionary<TileType, Graphic> ();
			lookup.Add (TileType.Air, new Graphic (Color.Transparent, Tile.WIDTH, Tile.HEIGHT));
			lookup.Add (TileType.Brick, new Graphic (Color.Brown, Tile.WIDTH, Tile.HEIGHT));
			lookup.Add (TileType.Fire, new Graphic (Color.Transparent, Tile.WIDTH, Tile.HEIGHT));
		}
			
		public void EmptyMap ()
		{
			for (int y=0; y < Constants.Constants.MAP_HEIGHT; y++) {
				List<Tile > row = new List<Tile> ();
				for (int x=0; x < Constants.Constants.MAP_WIDTH; x++) {
					Tile t = new Tile (x * Tile.WIDTH, y * Tile.HEIGHT, TileType.Air, lookup [TileType.Air]);
					row.Add (t);
				}
				map.Add (row);
			}
		}
		public void LoadFromString (string str)
		{
			EffectFire fire = new EffectFire ();
			map = new List<List<Tile>> ();
			List<Tile > row = new List<Tile> ();
			int x, y;
			x = y = 0;
			foreach (char c in str) {
				if (c == '\n') {
					map.Add (row);
					row = new List<Tile> ();
					x = 0;					
					y += Tile.HEIGHT;
				} else {
					TileType t = (TileType)Convert.ToInt16 (c) - 48;
					// if it wasn't cloned then each TileGraphic of each TileType would be the same
					Tile tile = new Tile (x, y, t, lookup [t].Clone ());
					if (t == TileType.Fire) {
						Vector xPos = new Vector (new Point ((int)x, (int)x + Tile.WIDTH));
						Vector yPos = new Vector (new Point ((int)y-EffectFire.HEIGHT, (int)y - EffectFire.HEIGHT));
						ParticleOptions p = fire.template.Clone();
						p.xPosRange = xPos;
						p.yPosRange = yPos;
						tile.system = new ParticleSystem (p);
					}			
					row.Add (tile);
					x += Tile.WIDTH;					
				}
			}
		}
		
		public void Draw (Surface sfcGameWindow)
		{
			foreach (List<Tile> r in map) {
				foreach (Tile c in r) {
					if (!c.outOfSight)
						c.Draw (sfcGameWindow);
				}
			}
		}
		
		public void Update (float elapsed, ref Player player, Camera camera)
		{
			float xSpeed = player.xSpeed * elapsed * Player.MOVE_SPEED;
			float ySpeed = player.ySpeed * elapsed * Player.FALL_SPEED;
			Rectangle rectLeft = new Rectangle (new Point ((int)(player.x + xSpeed), (int)(player.y + ySpeed)), 
				new Size (1, Player.HEIGHT));
			Rectangle rectRight = new Rectangle (new Point ((int)(player.x + xSpeed + Player.WIDTH), 
				(int)(player.y + ySpeed)), new Size (1, Player.HEIGHT));
			Rectangle rectTop = new Rectangle (new Point ((int)(player.x + xSpeed), ((int)(player.y + ySpeed))), 
				new Size (Player.WIDTH, 1));
			Rectangle rectBot = new Rectangle (new Point ((int)(player.x + xSpeed), 
				(int)(player.y + Player.HEIGHT + ySpeed + 1)), new Size (Player.WIDTH, 1));
			
			List<TileDirectionRelPlayer > lst = new List<TileDirectionRelPlayer> ();			
			foreach (List<Tile> r in map) {
				foreach (Tile c in r) {
					c.Update (elapsed, camera); // Save CPU by only using one foreach loop
					if (c.outOfSight) {
						continue;				
					}
					if (c.tileType == TileType.Air)
						continue;
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
				player.ySpeed = 0;
			}			
			else if (tile.left) {
				player.x = tile.tile.x + Tile.WIDTH;
			}
			else if (tile.right) {
				player.x = tile.tile.x - Player.WIDTH - 1;
			}
		}
	}
}

