using System;
using System.Drawing;
using System.Collections.Generic;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Primitives;
using GameGraphic;

namespace Platformer
{
	public class SimpleEnemy : Enemy
	{
		public SimpleEnemy (float _x, float _y) : base (_x, _y)
		{
			width = height = 27;
			y = y + (Tile.HEIGHT - height);
			graphic = new Graphic (Color.Red, width, height);
		}
		
		public override void DoAI (Player player)
		{
			
		}
		
		public override void Draw (Surface sfcGameWindow)
		{
			base.Draw (sfcGameWindow);
			if (!dead) {
				graphic.Draw (sfcGameWindow, (int)x, (int)y);
			}
		}
	}
}

