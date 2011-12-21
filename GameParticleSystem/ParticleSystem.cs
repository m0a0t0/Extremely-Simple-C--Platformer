using System;
using System.Drawing;
using System.Collections.Generic;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Primitives;
using GameGraphic;

namespace Platformer
{
	public class ParticleSystem
	{
		private List<Particle> particles;
		private ParticleOptions ops;
		private Random rand;
		private double timeElapsed, newParticlesTime;
		public bool finished = false;
		
		public ParticleSystem (ParticleOptions _ops)
		{
			particles = new List<Particle> ();
			rand = new Random ();
			ops = _ops;
			int n = rand.Next ((int)ops.numberRange.X, (int)ops.numberRange.Y);
			for (int i=0; i < n; i++) {
				newParticle ();
			}
		}
		
		public void newParticle ()
		{
			Graphic _graphic = ops.graphics [rand.Next ((int)ops.graphicRange.X, (int)ops.graphicRange.Y)].Clone ();
			float x = (float)RandomDouble (ops.xPosRange.X, ops.xPosRange.Y);
			float y = (float)RandomDouble (ops.yPosRange.X, ops.yPosRange.Y);
			float xStep = rand.Next ((int)ops.xSpeedRange.X, (int)ops.xSpeedRange.Y) * rand.Next ((int)ops.xDirRange.X, (int)ops.xDirRange.Y);
			float yStep = rand.Next ((int)ops.ySpeedRange.X, (int)ops.ySpeedRange.Y) * rand.Next ((int)ops.yDirRange.X, (int)ops.yDirRange.Y);
			int xStop = rand.Next ((int)ops.xStopRange.X, (int)ops.xStopRange.Y);
			int yStop = rand.Next ((int)ops.yStopRange.X, (int)ops.yStopRange.Y);
			Particle p = new Particle (_graphic, ops.alpha, ops.alphaStep, x, xStep, y, yStep, xStop, yStop);
			particles.Add (p);		
		}
		
		private double RandomDouble (double min, double max)
		{
			return rand.NextDouble () * (max - min) + min;
		}
		
		public void Update (float elapsed, Camera camera)
		{
			if (particles.Count == 0) {
				finished = true;
			}
			timeElapsed += elapsed;
			if (timeElapsed >= newParticlesTime && ops.repeat == true) {
				timeElapsed = 0;
				newParticlesTime = RandomDouble (ops.newParticlesTimeRange.X, ops.newParticlesTimeRange.Y);
				int n = rand.Next ((int)ops.newParticlesNumberRange.X, (int)ops.newParticlesNumberRange.Y);
				for (int i=0; i < n; i++) {
					particles.RemoveAt (rand.Next (0, particles.Count));
					newParticle ();
				}
			}
			ops.xPosRange.X += camera.x;
			ops.xPosRange.Y += camera.x;
			ops.yPosRange.X += camera.y;
			ops.yPosRange.Y += camera.y;
			List<Particle > removes = new List<Particle> ();
			int extras = 0;
			foreach (Particle p in particles) {
				if (p.dead) {
					removes.Add (p);
					if (ops.repeat) {
						extras += 1;
					}
				}
				p.Update (elapsed, camera);
			}
			
			if (extras != 0) {
				for (int i=1; i <= extras; i++) {
					newParticle ();
				}
			}
			foreach (Particle p in removes) {
				particles.Remove (p);
			}
		}
		
		public void Draw (Surface sfcGameWindow)
		{
			foreach (Particle p in particles) {
				p.Draw (sfcGameWindow);
			}
		}
	}
	
	public class ParticleOptions {
		public Vector xPosRange, yPosRange, xDirRange, yDirRange, xSpeedRange, ySpeedRange, numberRange, graphicRange, 
			xStopRange, yStopRange, newParticlesTimeRange, newParticlesNumberRange;
		public int alpha, alphaStep;
		public bool repeat;
		public Graphic[] graphics;
		
		public ParticleOptions (Vector _xPosRange, Vector _yPosRange, Vector _xDirRange, Vector _yDirRange, Vector _xSpeedRange,
			Vector _ySpeedRange, Vector _numberRange, Vector _xStopRange, Vector _yStopRange, Vector _newParticlesTimeRange, 
			Vector _newParticlesNumberRange, int _alpha, int _alphaStep, bool _repeat, Graphic[] _graphics)
		{
			xPosRange = _xPosRange;
			yPosRange = _yPosRange;
			xDirRange = _xDirRange;
			yDirRange = _yDirRange;
			xSpeedRange = _xSpeedRange;
			ySpeedRange = _ySpeedRange;
			numberRange = _numberRange;
			xStopRange = _xStopRange;
			yStopRange = _yStopRange;
			newParticlesTimeRange = _newParticlesTimeRange;
			newParticlesNumberRange = _newParticlesNumberRange;
			alpha = _alpha;
			alphaStep = _alphaStep;
			repeat = _repeat;
			graphics = _graphics;
			graphicRange = new Vector (new Point (0, graphics.Length));
		}
		
		public ParticleOptions Clone ()
		{
			return (ParticleOptions)MemberwiseClone ();
		}
	}
	
	public class Particle : Sprite 
	{
		private Graphic graphic;
		private float alpha, alphaStep;
		private float xStep, yStep;
		private int xStop, yStop;
		private float xTravelled, yTravelled;
		public bool dead = false;
		
		public Particle (Graphic _graphic, int _alpha, int _alphaStep, float _x, float _xStep, float _y, float _yStep, 
			int _xStop, int _yStop)
		{
			graphic = _graphic;
			alpha = _alpha;
			alphaStep = _alphaStep;
			x = _x;
			y = _y;
			xStep = _xStep;
			yStep = _yStep;
			xStop = _xStop;
			yStop = _yStop;
		}
		
		public void Update (float elapsed, Camera camera)
		{
			dead = (Math.Abs (xTravelled) >= xStop || Math.Abs (yTravelled) >= yStop || alpha+alphaStep <= 0);
			alpha += alphaStep * elapsed;
			float xChange = xStep * elapsed;
			float yChange = yStep * elapsed;
			x += xChange;
			y += yChange;
			xTravelled += xChange;
			yTravelled += yChange;
			ApplyCamera (camera);
		}
		
		public void Draw (Surface sfcGameWindow)
		{
			graphic.Draw (sfcGameWindow, x, y, (int)alpha);
		}
	}
	
}

