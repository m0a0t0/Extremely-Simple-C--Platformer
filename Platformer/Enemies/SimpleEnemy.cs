using System;
using System.Drawing;
using System.Collections.Generic;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Primitives;
using GameGraphic;

namespace Platformer
{
	public class SimpleEnemy : Enemy
	{
		public SimpleEnemy (float _x, float _y) : base (_x, _y)
		{
			health = 25;
			width = height = 27;
			y = y + (Tile.HEIGHT - height);
			colour = Color.Red;
			graphic = new Graphic (colour, width, height);
		}
		
		public override void DoAI (Camera camera, Player player, Map map)
		{
			base.DoAI (camera, player, map);
			Vector vecCurrentPos = new Vector (new Point ((int)((x + map.overallCamera.x) / Tile.WIDTH), 
				(int)Math.Round((y + map.overallCamera.y) / Tile.HEIGHT)));
			if (map.map [(int)vecCurrentPos.Y] [(int)vecCurrentPos.X + (int)xDir].tileType != TileType.Air) {
				xDir *= -1;
			} else if (map.map [(int)vecCurrentPos.Y + 1] [(int)vecCurrentPos.X].tileType != TileType.Brick) {
				xDir *= -1;
			}
			if (xDir == 0 && !falling && !jumping) {
				xDir = 1;
			} else if (falling || jumping) {
				xDir = 0;
			}
		}
		
		public override void Draw (Surface sfcGameWindow)
		{
			base.Draw (sfcGameWindow);
			if (!dead) {
				graphic.Draw (sfcGameWindow, (int)x, (int)y);
			}
		}
	}
}

