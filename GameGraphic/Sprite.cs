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
		
		protected virtual void ApplyCamera (Camera camera)
		{
			x += camera.x;
			y += camera.y;
		}
	}
}

