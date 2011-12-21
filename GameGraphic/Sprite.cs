using System;
using System.Drawing;
using System.Collections.Generic;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Primitives;

namespace GameGraphic
{
	public class Sprite
	{
		public float x, y;
		public Rectangle rect;
		public bool outOfSight;
		public int width, height;
		
		protected virtual void ApplyCamera (Camera camera)
		{
			x += camera.x;
			y += camera.y;
			outOfSight = x - width < 0 || x > Constants.Constants.WIDTH || y - height < 0 || 
				y > Constants.Constants.HEIGHT;
		}
	}
}

