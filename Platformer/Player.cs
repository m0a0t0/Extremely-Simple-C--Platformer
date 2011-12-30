using System;
using System.Drawing;
using System.Collections.Generic;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Primitives;
using GameGraphic;

namespace Platformer
{
	public class Player : GunSprite
	{
		public static int WIDTH = 16;
		public static int HEIGHT = 16;
		
		public Player (float _x, float _y)
		{
			colour = Color.Blue;
			falling = true;
			MOVE_SPEED = 200;
			FALL_SPEED = 4;
			FALL_CAP = (MOVE_SPEED + MOVE_SPEED / 100 * 50) / FALL_SPEED;
			x = _x;
			y = _y;
			gun = new Gun (GunType.HandGun, this);
			width = WIDTH;
			height = HEIGHT;
			graphic = new Graphic (colour, WIDTH, HEIGHT);
		}
		
		public override void Update (float elapsed, Camera camera, Player player, Map map)
		{
//			y += yDir * elapsed * FALL_SPEED;			
//			x += xDir * elapsed * MOVE_SPEED;			
			base.Update (elapsed, camera, player, map);
			//rect = new Rectangle (new Point ((int)x, (int)y), new Size (WIDTH, HEIGHT + 1));			
		}
		
		public override void Draw (Surface sfcGameWindow)
		{
			if (dead && system != null) {
				system.Draw (sfcGameWindow);
				return;
			}
			base.Draw (sfcGameWindow);
			graphic.Draw (sfcGameWindow, (int)x, (int)y);
			//sfcGameWindow.Draw (new Box (new Point ((int)x, (int)y), new Size (WIDTH, HEIGHT)), colour);
		}
		
	}
}

