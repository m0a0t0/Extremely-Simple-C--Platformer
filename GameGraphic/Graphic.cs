using System;
using System.Drawing;
using System.Collections.Generic;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Primitives;

namespace GameGraphic
{
	public class Graphic
	{
		public Color colour;
		public Surface sfc;
		public Box box;
		public int width, height;
		
		public Graphic (Color c, int _width, int _height)
		{
			colour = c;
			width = _width;
			height = _height;
		}
		
		public Graphic (Surface s)
		{
			sfc = s;
		}
		
		public void Draw (Surface sfcGameWindow, float x, float y, int alpha=255, bool fill=false)
		{
			if (sfc != null) {
				sfcGameWindow.Blit (sfc, new Point ((int)x, (int)y));
			} else {
				box = new Box (new Point ((int)x, (int)y), new Size (width, height));	
				int r = colour.R;
				int g = colour.G;
				int b = colour.B;
				Color colour2 = Color.FromArgb (alpha, r, g, b);
				if (colour == Color.Transparent) {
					colour2 = colour;
				}
				sfcGameWindow.Draw (box, colour2, true, fill);
			}
		}
		
		public Graphic Clone ()
		{
			return (Graphic)this.MemberwiseClone ();
		}
	}
}

