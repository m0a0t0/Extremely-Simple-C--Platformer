using System;
using System.Drawing;
using System.Collections.Generic;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Primitives;
using GameGraphic;

namespace Platformer
{	
	public abstract class KillableSprite : Sprite {
		public float health = 100.0f;
		public bool dead = false;
		public ParticleSystem system;	
		protected Color colour;
		
		public virtual void Update (float elapsed, Camera camera, Player player)
		{
			rect = new Rectangle (new Point ((int)x, (int)y), new Size (width, height));
			if (health <= 0) {
				dead = true;
			}
			if (dead) {
				if (system == null) {
					ParticleOptions ops = (new EffectDie (colour, (KillableSprite)this)).template.Clone();
					ops.xPosRange = new Vector (new Point ((int)x, (int)x + width));
					ops.yPosRange = new Vector (new Point ((int)y, (int)y + height));
					system = new ParticleSystem (ops);
				}
				system.Update (elapsed, camera);
				return;				
			}			
		}
	}
	
	public abstract class GunSprite : KillableSprite
	{
		public Gun gun;
		public bool falling;
		private bool _jumping = false;
		public int FALL_CAP;
		public int FALL_SPEED;
		public int MOVE_SPEED;
		public List<TileDirectionRelSprite> tileDirRelSprites;
		public Rectangle rectBot;
		
		public bool jumping {
			get { return _jumping; }
			set {
				if (yDir == 0 && value && !falling) {
					yDir = -FALL_CAP;
				}
				_jumping = value;
			}
		}
		
		public override void Update (float elapsed, Camera camera, Player player)
		{
			base.Update (elapsed, camera, player);
			if (!dead) {
				tileDirRelSprites = new List<TileDirectionRelSprite> ();
				ApplyCamera (camera);
				x += xDir * MOVE_SPEED * elapsed;
				y += yDir * FALL_SPEED * elapsed;
				if (jumping) {
					if (yDir >= 0) {
						jumping = false;
						falling = true;
					} else {
						yDir += FALL_SPEED;
					}
				} else if (falling) {
					if (yDir < FALL_CAP)
						yDir += FALL_SPEED;
				}
				gun.Update (elapsed, camera);
				if (xDir != 0) {
					gun.left = xDir < 0;

				}
				if (gun.left) {
					gun.x = x - gun.width;
				} else if (!gun.left) {
					gun.x = x + width;
				}
				gun.y = y + height / 3;
			}
		}
		
		public virtual void Collision (bool left)
		{
			
		}
		
		public override void Draw (Surface sfcGameWindow)
		{
			if (dead) {
				if (system != null) {
					system.Draw (sfcGameWindow);
					return;
				}
			}
			if (gun.gunType != GunType.NullGun) {
				gun.Draw (sfcGameWindow);
			}
		}
	}
}

