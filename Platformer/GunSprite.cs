using System;
using System.Drawing;
using System.Collections.Generic;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Primitives;
using GameGraphic;

namespace Platformer
{
	public class GunSprite : Sprite
	{
		public Gun gun;
		
		public virtual void Update (float elapsed, Camera c)
		{
			ApplyCamera (c);
			gun.left = xSpeed < 0;
			if (gun.left) {
				gun.x = x - gun.width;
			} else if (!gun.left) {
				gun.x = x + width;// + gun.width;
			}
			gun.y = y+height/3;
		}
		
		public virtual void Draw (Surface sfcGameWindow)
		{
			gun.Draw (sfcGameWindow);
		}
	}
}

