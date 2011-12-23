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
	}
	
	public abstract class GunSprite : KillableSprite
	{
		public Gun gun;
		
		public virtual void Update (float elapsed, Camera camera, Player player)
		{
			ApplyCamera (camera);
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
		
		public override void Draw (Surface sfcGameWindow)
		{
			gun.Draw (sfcGameWindow);
		}
	}
}

