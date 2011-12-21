using System;

namespace Constants
{
	public class Constants
	{
		public static int WIDTH = 1024;
		public static int HEIGHT = 768;	
		public static int PLAYER_WIDTH = 16;						
		public static int PLAYER_HEIGHT = 16;
		public static string RESOURCE_PATH = "resources/";
	
		public static string GetResourcePath (string resource)
		{
			if (resource.EndsWith (".ttf")) {
				return RESOURCE_PATH + "fonts/" + resource;
			} else {
				return "";
			}
		}
	}
}

