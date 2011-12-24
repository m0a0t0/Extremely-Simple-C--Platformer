using System;
using System.Drawing;
using System.Collections.Generic;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Primitives;
using GameGraphic;

namespace Platformer
{
	public class Effect {
		public Vector xDirRange, yDirRange, xSpeedRange, ySpeedRange, numberRange, xStopRange, yStopRange, newParticlesTimeRange,
			newParticlesNumberRange;
		public int alpha, alphaStep;
		public bool repeat;
		public ParticleOptions template;
		public Graphic[] graphics;
	}
	
	public class EffectFire :Effect {
		public static int WIDTH = 6;
		public static int HEIGHT = 6;
		public EffectFire ()
		{
			xDirRange = new Vector (new PointF (-0.5f, 0.5f));
			yDirRange = new Vector (new Point (-1, -1));
			xSpeedRange = new Vector (new Point (0, 0));
			ySpeedRange = new Vector (new Point (100, 600));	
			numberRange = new Vector (new Point (8, 10));
			xStopRange = new Vector (new Point (Tile.WIDTH, Tile.WIDTH));
			yStopRange = new Vector (new Point (1, 20));//(Tile.HEIGHT - 5, Tile.HEIGHT-2));
			newParticlesNumberRange = new Vector (new Point (1, 6));
			newParticlesTimeRange = new Vector (new PointF (0.01f, 0.34f));
			alpha = 255;
			alphaStep = -200;
			repeat = true;
			graphics = new Graphic[]{new Graphic(Color.Orange, WIDTH, HEIGHT), new Graphic(Color.Red, WIDTH, HEIGHT), 
				new Graphic(Color.Yellow, WIDTH,HEIGHT)};
			Vector blank = new Vector (0);
			template = new ParticleOptions (blank, blank, xDirRange, yDirRange, xSpeedRange, ySpeedRange, numberRange, 
				xStopRange, yStopRange, newParticlesTimeRange, newParticlesNumberRange, alpha, alphaStep, repeat, graphics);
		}
	}
	
	public class EffectDie : Effect {
		public static int WIDTH = 2;
		public static int HEIGHT = 2;
		public EffectDie (Color colour, KillableSprite kS)
		{
			xDirRange = new Vector (new PointF (-1.0f, 2.0f));
			yDirRange = new Vector (new PointF (-1.0f, -1.0f));
			xSpeedRange = new Vector (new Point (150, 200));
			ySpeedRange = new Vector (new Point (150, 200));	
			numberRange = new Vector (new Point (100, 200));
			xStopRange = new Vector (new Point ((int)(kS.width*1.1), kS.width*2));
			yStopRange = new Vector (new Point ((int)(kS.height*1.1), kS.height*2));//(Tile.HEIGHT - 5, Tile.HEIGHT-2));
			newParticlesNumberRange = new Vector (new Point (0, 0));
			newParticlesTimeRange = new Vector (new PointF (0, 0));
			alpha = 255;
			alphaStep = -200;
			repeat = false;
			graphics = new Graphic[]{new Graphic(colour, WIDTH, HEIGHT), new Graphic(Color.Red, WIDTH, HEIGHT)};
			Vector blank = new Vector (0);
			template = new ParticleOptions (blank, blank, xDirRange, yDirRange, xSpeedRange, ySpeedRange, numberRange, 
				xStopRange,yStopRange, newParticlesTimeRange, newParticlesNumberRange, alpha, alphaStep, repeat, graphics);
		}
	}	
}

