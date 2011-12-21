using System;
using Constants;

namespace GameGraphic
{
	public class Camera
	{
		public float x,y;
		
		public Camera ()
		{
			x = y = 0;
		}
		
		public void Update (Sprite player)
		{
			x = (Constants.Constants.WIDTH - player.x) - Constants.Constants.WIDTH / 2 - Constants.Constants.PLAYER_WIDTH / 2;
			y = (Constants.Constants.HEIGHT - player.y) - Constants.Constants.HEIGHT / 2 - 
				Constants.Constants.PLAYER_HEIGHT / 2;
		}
	}
}

