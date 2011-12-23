using System;
using System.Drawing;
using System.Collections.Generic;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Primitives;
using GameGraphic;

namespace Platformer
{
	public class Tile : Sprite
	{
		public TileType tileType;
		public Graphic tileGraphic;
		public static int WIDTH = 32;
		public static int HEIGHT = 32;
		public ParticleSystem system;
		
		public Tile (int _x, int _y, TileType _tileType, Graphic _tileGraphic)
		{
			width = WIDTH;
			height = HEIGHT;
			x = _x;
			y = _y;
			tileType = _tileType;
			tileGraphic = _tileGraphic;
			rect = new Rectangle (new Point ((int)x, (int)y), new Size (Tile.WIDTH, Tile.HEIGHT));
			system = null;
		}
		
		public override void Draw (Surface sfcGameWindow)
		{
		}
		
		public void Draw2 (Surface sfcGameWindow, bool editor)
		{
			tileGraphic.Draw (sfcGameWindow, x, y, 255, editor);
			if (system != null) {
				system.Draw (sfcGameWindow);
			}			
		}
		
		public void Update (float elapsed, Camera camera)
		{
			ApplyCamera (camera);
			if (system != null) {
				system.Update (elapsed, camera);
			}
		}
		
		protected override void ApplyCamera (Camera camera)
		{
			base.ApplyCamera (camera);
			rect = new Rectangle (new Point ((int)x, (int)y), new Size (Tile.WIDTH, Tile.HEIGHT));	
		}
	}
	
	public class TileDirectionRelPlayer
	{
		public Tile tile;
		public bool above, below, left, right;

		public TileDirectionRelPlayer ()
		{
			above = below = left = right = false;
		}
	}
}

