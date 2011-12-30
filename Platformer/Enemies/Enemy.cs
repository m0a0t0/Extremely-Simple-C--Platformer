using System;
using System.Xml;
using System.Drawing;
using System.Collections.Generic;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Primitives;
using GameGraphic;

namespace Platformer
{
	public enum EnemyType {
		SimpleEnemy,
	};
	
	public abstract class Enemy : GunSprite
	{
		public Enemy (float _x, float _y)
		{
			x = _x;
			y = _y;
			gun = new Gun (GunType.NullGun, this);
			MOVE_SPEED = 200;
			FALL_SPEED = 4;
			FALL_CAP = (MOVE_SPEED + MOVE_SPEED / 100 * 50) / FALL_SPEED;
		}
		
		public override void Update (float elapsed, Camera camera, Player player, Map map)
		{
			base.Update (elapsed, camera, player, map);
			if (!dead) {
				DoAI (camera, player,map);
			}
		}
		
		public virtual void DoAI (Camera camera, Player player, Map map)
		{
			if (player.rect.IntersectsWith (rect)) {
				player.dead = true;
			}
		}
		
		public static Enemy GetEnemy (EnemyType type, float x, float y)
		{
			if (type == EnemyType.SimpleEnemy) {
				return (Enemy)new SimpleEnemy (x, y);
			}
			return null;
		}
		
		public override void Draw (Surface sfcGameWindow)
		{
			base.Draw (sfcGameWindow);
		}
	}
}

