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
		public static Color COLOUR = Color.Blue;
		public ParticleSystem system;
		
		public Player (float _x, float _y)
		{
			falling = true;
			MOVE_SPEED = 200;
			FALL_SPEED = 4;
			FALL_CAP = (MOVE_SPEED + MOVE_SPEED / 100 * 50) / FALL_SPEED;
			x = _x;
			y = _y;
			gun = new Gun (GunType.HandGun, this);
			width = WIDTH;
			height = HEIGHT;
			graphic = new Graphic (COLOUR, WIDTH, HEIGHT);
		}
		
		public override void Update (float elapsed, Camera camera, Player player)
		{
			if (health <= 0) {
				dead = true;
			}
			if (dead) {
				if (system == null) {
					ParticleOptions ops = (new EffectDie ()).template;
					ops.xPosRange = new Vector (new Point ((int)x, (int)x + WIDTH));
					ops.yPosRange = new Vector (new Point ((int)y, (int)y + HEIGHT));
					system = new ParticleSystem (ops);
				}
				system.Update (elapsed, camera);
				return;				
			}
//			y += yDir * elapsed * FALL_SPEED;			
//			x += xDir * elapsed * MOVE_SPEED;			
			base.Update (elapsed, camera, player);
			rect = new Rectangle (new Point ((int)x, (int)y), new Size (WIDTH, HEIGHT + 1));			
		}
		
		public override void Draw (Surface sfcGameWindow)
		{
			if (dead && system != null) {
				system.Draw (sfcGameWindow);
				return;
			}
			base.Draw (sfcGameWindow);
			graphic.Draw (sfcGameWindow, (int)x, (int)y);
			sfcGameWindow.Draw (new Box (new Point ((int)x, (int)y), new Size (WIDTH, HEIGHT)), COLOUR);
		}
		
	}
}

