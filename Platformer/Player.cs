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
		public bool falling;		
		private bool _jumping = false;
		public bool jumping {
			get { return _jumping; }
			set {
				if (ySpeed == 0 && value && !falling) {
					ySpeed = -FALL_CAP;
				}
				_jumping = value;
			}
		}
		
		public static int WIDTH = 16;
		public static int HEIGHT = 16;
		public static Color COLOUR = Color.Blue;
		public static int FALL_SPEED = 4;
		public static int MOVE_SPEED = 200;
		public static int FALL_CAP = (MOVE_SPEED+MOVE_SPEED/100*50)/FALL_SPEED;
		public bool dead = false;
		public ParticleSystem system;
		
		public Player (float _x, float _y)
		{
			x = _x;
			y = _y;
			gun = new Gun (GunType.HandGun, this);
			width = WIDTH;
			height = HEIGHT;
//			gun.x = x + width;
//			gun.y = y;
		}
		
		public override void Update (float elapsed, Camera camera)
		{
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
			y += ySpeed * elapsed * FALL_SPEED;			
			x += xSpeed * elapsed * MOVE_SPEED;			
			base.Update (elapsed, camera);
			if (jumping) {
				if (ySpeed >= 0) {
					jumping = false;
					falling = true;
				} else {
					ySpeed += FALL_SPEED;
				}
			} else if (falling) {
				if (ySpeed < FALL_CAP)
					ySpeed += FALL_SPEED;
			}
			rect = new Rectangle (new Point ((int)x, (int)y), new Size (WIDTH, HEIGHT + 1));			
		}
		
		public override void Draw (Surface sfcGameWindow)
		{
			if (dead && system != null) {
				system.Draw (sfcGameWindow);
				return;
			}
			base.Draw (sfcGameWindow);
			sfcGameWindow.Draw (new Box (new Point ((int)x, (int)y), new Size (WIDTH, HEIGHT)), COLOUR);
		}
		
	}
}

