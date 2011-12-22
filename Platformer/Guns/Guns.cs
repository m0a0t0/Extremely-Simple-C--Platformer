using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Primitives;
using SdlDotNet.Input;
using System;
using System.Xml;
using System.Collections.Generic;
using System.Drawing;
using GameGraphic;

namespace Platformer
{
	public enum GunType {
		HandGun,
	}
	
	public class Gun : Sprite
	{
		GunType gunType;
		public bool left = false;
		Graphic graphic;
		GunSprite parent;
		
		public Gun (GunType _gunType, GunSprite _parent)
		{
			parent = _parent;
			gunType = _gunType;
			width = Tile.WIDTH;
			height = Tile.HEIGHT;		
			Surface sfc = new Surface (100, 100);
			if (gunType == GunType.HandGun) {
				width = (int)(width / 4);
				height = (int)(height / 10);
				sfc = new Surface (new Size (width, height));		
				sfc.Fill (Color.White);	
				sfc.Draw (new Box (new Point (0, 0), new Size (width, height)), 
					Color.Black, true, true);
			
			}
			sfc.Update ();
			graphic = new Graphic (sfc);	
		}
		
		public void Draw (Surface sfcGameWindow)
		{
			graphic.Draw (sfcGameWindow, x, y);
		}
		
	}
}

