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
		NullGun,
	}
	
	public class Gun : Sprite
	{
		GunType gunType;
		public bool left = false;
		GunSprite parent;
		public List<Bullet> bullets;
		private float lastFireTime = 0.0f;
		private float fireInterval;
		
		public Gun (GunType _gunType, GunSprite _parent)
		{
			bullets = new List<Bullet> ();
			parent = _parent;
			gunType = _gunType;
			width = Tile.WIDTH;
			height = Tile.HEIGHT;
			ChangeGun ();
		}
		
		public void ChangeGun ()
		{
			Surface sfc = new Surface (100, 100);
			if (gunType == GunType.HandGun) {
				width = (int)(width / 4);
				height = (int)(height / 10);
				sfc = new Surface (new Size (width, height));		
				sfc.Fill (Color.White);	
				sfc.Draw (new Box (new Point (0, 0), new Size (width, height)), 
					Color.Black, true, true);
				fireInterval = 0.25f;
			} else if (gunType == GunType.NullGun) {
				width = height = 0;
				fireInterval = float.MaxValue;
			}
			sfc.Update ();
			graphic = new Graphic (sfc);				
		}
		
		public void Update (float elapsed, Camera camera)
		{
			lastFireTime += elapsed;
			List<Bullet > removes = new List<Bullet> ();
			foreach (Bullet b in bullets) {
				b.Update (elapsed, camera);
				if (b.dead) {
					removes.Add (b);
				}
			}
			
			foreach (Bullet b in removes) {
				bullets.Remove (b);
			}
		}
		
		public override void Draw (Surface sfcGameWindow)
		{
			graphic.Draw (sfcGameWindow, x, y);
			foreach (Bullet b in bullets) {
				b.Draw (sfcGameWindow);
			}
		}
		
		public void Fire ()
		{
			if (lastFireTime >= fireInterval) {
				lastFireTime = 0.0f;
				int _xDir = 1;
				if (left) {
					_xDir = -1;
				}
				Bullet b = new Bullet (x, y, gunType, _xDir);
				bullets.Add (b);
			}
		}		
	}
	
	public class Bullet : Sprite {
		//public static int HG_SPEED = 300;
		private int speed;
		private int alphaSpeed = -50;
		private float alpha = 255;
		private float hitPoints;
		private float hitPointSpeed;
		public bool dead = false;
		
		public Bullet (float _x, float _y, GunType type, int _xDir, int _yDir=0)
		{
			x = _x;
			y = _y;
			xDir = _xDir;
			yDir = _yDir;
			if (type == GunType.HandGun) {
				speed = 300;
				hitPoints = 10;
				hitPointSpeed = -0.7f;
				width = 5;
				height = 2;
				graphic = new Graphic (Color.Black, width, height);
			}
		}
		
		public void Update (float elapsed, Camera camera)
		{
			if (alpha <= 0 || dead) {
				dead = true;
				return;
			}
			ApplyCamera (camera);
			x += xDir * elapsed * speed;
			y += yDir * elapsed * speed;
			alpha += alphaSpeed * elapsed;
			hitPoints += hitPointSpeed * elapsed;
			rect = new Rectangle (new Point ((int)x, (int)y), new Size (width, height));
		}
		
		public override void Draw (Surface sfcGameWindow)
		{
			graphic.Draw (sfcGameWindow, (int)x, (int)y, (int)alpha);
		}
	}
}

