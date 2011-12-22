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
		public int xDir, yDir;		
		
		protected virtual void ApplyCamera (Camera camera)
		{
			x += camera.x;
			y += camera.y;
			outOfSight = x < 0 || x > Constants.Constants.WIDTH || y < 0 || 
				y > Constants.Constants.HEIGHT;
		}
	}
}

