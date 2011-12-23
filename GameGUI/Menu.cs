using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using Constants;
using GameGraphic;

namespace GameMenu
{
	public enum MenuLayout {
		Horizontal,
		Vertical,
		Grid,
	}
	
	public delegate void MenuSelectedHandler(MenuObject obj);
	
	public class Menu
	{	
		public List<MenuObject> objects;
		public int selected;
		private MenuLayout layout; 	
		public float lastSelectChange = -0.2f;
		public float selectChangeTime = 0.2f;
		private int startX, startY;
		private bool xOverflow = false;		
		private bool yOverflow = false;
		private Camera camera;
		public int width, height;

		public Menu (List<MenuObject> _objects, MenuLayout _layout, int _startX = 0, int _startY=50)
		{
			camera = new Camera ();
			startX = _startX;
			startY = _startY;
			layout = _layout;
			objects = _objects;
			if (layout == MenuLayout.Vertical) {
				int y = startY;
				foreach (MenuObject obj in objects) {
					int x = Constants.Constants.WIDTH / 2 - obj.width / 2;
					if (startX != 0) {
						x = startX;
					}
					obj.x = x;
					obj.y = y;
					y += obj.height + 20;
				}
				width = objects [0].width;
				height = y;
				if (y >= Constants.Constants.HEIGHT - objects [0].height) {
					yOverflow = true;
					height = Constants.Constants.HEIGHT;
				}
			} else if (layout == MenuLayout.Horizontal) {
				int y = startY;
				int x = startX;
				foreach (MenuObject obj in objects) {
					obj.x = x;
					obj.y = y;
					x += obj.width + 10;
				}
				width = x;
				if (x >= Constants.Constants.WIDTH - objects [0].width) {
					xOverflow = true;
					x = Constants.Constants.WIDTH;
				}
			}
			objects [0].selected = true;
		}
		
		public void GenerateWidth ()
		{
			int x = startX;
			foreach (MenuObject obj in objects) {
				x += obj.width + 10;
			}
			width = x;			
		}
		
		private void UnSelect ()
		{
			foreach (MenuObject obj in objects) {
				obj.selected = false;
			}
		}
		
		public void Update (float elapsed, Camera _camera=null)
		{
			if (_camera != null) {
				camera.x += _camera.x;
				camera.y += _camera.y;
			}
			lastSelectChange += elapsed;			
			HandleInput ();
			objects [selected].selected = true;
			foreach (MenuObject m in objects) {
				m.Update (elapsed, camera);
			}
		}
		
		public void HandleInput ()
		{
			bool change = lastSelectChange >= selectChangeTime;		
			bool next, prev;
			next = prev = false;
			if (layout == MenuLayout.Vertical) {
				if (Keyboard.IsKeyPressed (Key.UpArrow) && change) {
					prev = true;
				} else if (Keyboard.IsKeyPressed (Key.DownArrow) && change) {
					next = true;
				}
			} else if (layout == MenuLayout.Horizontal) {
				if (Keyboard.IsKeyPressed (Key.LeftArrow) && change) {
					prev = true;
					if (xOverflow) {
						camera.x -= objects [0].width / 2;
					}
				} else if (Keyboard.IsKeyPressed (Key.RightArrow) && change) {
					next = true;
					if (xOverflow) {
						camera.x += objects [0].width / 2;
					}
				}
			}
			
			if (prev) {
				UnSelect ();
				lastSelectChange = 0.0f;
				selected -= 1;
				if (selected < 0) {
					selected = objects.Count - 1;
				}				
			} else if (next) {
				UnSelect ();				
				lastSelectChange = 0.0f;				
				selected += 1;
				if (selected >= objects.Count) {
					selected = 0;
				}				
			}
			if (Keyboard.IsKeyPressed (Key.Return) && change) {
				objects [selected].Fire ();
			}			
			
			if (change) {
				if (objects [selected].HandleInput (change)) {
					lastSelectChange = 0.0f;
				}
			}
		}
		
		public void Draw (Surface sfcGameWindow)
		{
			foreach (MenuObject m in objects) {
				m.Draw (sfcGameWindow);
			}
		}
		
	}
	
	public abstract class MenuObject : Sprite {
		//public int width, height;
		public bool selected = false;
		//public int x,y;
		public event MenuSelectedHandler selectedHandler;
		
		public virtual bool HandleInput (bool change)
		{
			return false;
		}
		
		public virtual void Update (float elapsed, Camera c)
		{
			ApplyCamera (c);
		}
		
		//public override void Draw (Surface sfcGameWindow);
		
		public virtual void Fire ()
		{
			if (selectedHandler != null) {
				selectedHandler (this);
			}
		}
	}
	
	public class MenuTextEntry : MenuObject {
		public string text;
		public int textSize;
		private Color colour;
		private SdlDotNet.Graphics.Font font;
		public event MenuSelectedHandler escapeHandler;
		
		public MenuTextEntry ( int _textSize, Color _colour, string _text="untitled")
		{
			text = _text;
			textSize = _textSize;
			colour = _colour;
			font = new SdlDotNet.Graphics.Font (Constants.Constants.GetResourcePath ("arial.ttf"), textSize);
		}
		
		public override bool HandleInput (bool change)
		{
			if (Keyboard.IsKeyPressed (Key.Backspace)) {
				if (text.Length > 0) {
					text = text.Remove (text.Length - 1);
					return true;
				}
			} else if (Keyboard.IsKeyPressed (Key.Escape)) {
				escapeHandler (this);
			} else {
				foreach (char c in "qwertyuiopasdfghjklzxcvbnm") {
					Key k = (Key)Enum.Parse (typeof(Key), c.ToString (), true);
					if (Keyboard.IsKeyPressed (k) && ((text == null) || (text == "") || (c != text [text.Length - 1]) || (change))) {
						text += c;
						return true;
					}
				}
			}
			return false;
		}
		
		public override void Draw (Surface sfcGameWindow)
		{
			if (text != null) {
				sfcGameWindow.Blit (font.Render (text, colour), new Point ((int)x, (int)y));
			}
		}
	}
	
	public class MenuText : MenuObject {
		public string text;
		public Color colourSelected, colourNotSelected;
		private Color colour;
		public int textSize;
		public SdlDotNet.Graphics.Font font;
		private Surface sfcText;
		private bool lastSelected;
		
		public MenuText (string _text, Color _selected, Color _notSelected, int _textSize)
		{
			text = _text;
			colourSelected = _selected;
			colourNotSelected = _notSelected;
			textSize = _textSize;
			font = new SdlDotNet.Graphics.Font (Constants.Constants.GetResourcePath("arial.ttf"), textSize);
			colour = colourNotSelected;
			lastSelected = selected;
			RenderText ();
			width = sfcText.Width;
			height = sfcText.Height;
		}
		
		public void RenderText ()
		{
			if (selected) {
				colour = colourSelected;
			} else {
				colour = colourNotSelected;
			}			
			sfcText = font.Render (text, colour);
			width = sfcText.Width;
			height = sfcText.Height;			
		}
		
		public override void Update (float elapsed, Camera camera)
		{
			base.Update (elapsed, camera);
			if (lastSelected != selected) {
				RenderText ();
				lastSelected = selected;				
			}
		}
		
		public override void Draw (Surface sfcGameWindow)
		{
			sfcGameWindow.Blit (sfcText, new Point ((int)x, (int)y));
		}
	}
}

