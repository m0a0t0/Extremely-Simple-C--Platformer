using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using Constants;

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
		private List<MenuObject> objects;
		private int selected;
		private MenuLayout layout;
		private float lastSelectChange = 0.0f;
		private float selectChangeTime = 0.2f;

		public Menu (List<MenuObject> _objects, MenuLayout _layout)
		{
			layout = _layout;
			objects = _objects;
			if (layout == MenuLayout.Vertical) {
				int y = 50;
				foreach (MenuObject obj in objects) {
					obj.x = Constants.Constants.WIDTH / 2 - obj.width / 2;
					obj.y = y;
					y += obj.height + 20;
				}
			}
			objects [0].selected = true;
		}
		
		private void UnSelect ()
		{
			foreach (MenuObject obj in objects) {
				obj.selected = false;
			}
		}
		
		public void Update (float elapsed)
		{
			lastSelectChange += elapsed;
			bool change = lastSelectChange >= selectChangeTime;
			if (Keyboard.IsKeyPressed (Key.UpArrow) && change) {
				UnSelect ();
				lastSelectChange = 0.0f;
				selected -= 1;
				if (selected < 0) {
					selected = objects.Count - 1;
				}
			} else if (Keyboard.IsKeyPressed (Key.DownArrow) && change) {
				UnSelect ();				
				lastSelectChange = 0.0f;				
				selected += 1;
				if (selected >= objects.Count) {
					selected = 0;
				}
			} else if (Keyboard.IsKeyPressed (Key.Space)) {
				objects [selected].Fire ();
			}
			objects [selected].selected = true;
			foreach (MenuObject m in objects) {
				m.Update (elapsed);
			}
		}
		
		public void Draw (Surface sfcGameWindow)
		{
			sfcGameWindow.Fill (Color.White);
			foreach (MenuObject m in objects) {
				m.Draw (sfcGameWindow);
			}
			sfcGameWindow.Update ();
		}
		
	}
	
	public abstract class MenuObject {
		public int width, height;
		public bool selected = false;
		public int x,y;
		public event MenuSelectedHandler selectedHandler;
		
		public abstract void Update (float elapsed);
		public abstract void Draw (Surface sfcGameWindow);
		
		public virtual void Fire ()
		{
			selectedHandler (this);
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
			sfcText = font.Render (text, colour);			
		}
		
		public override void Update (float elapsed)
		{
			if (lastSelected != selected) {
				if (selected) {
					colour = colourSelected;
				} else {
					colour = colourNotSelected;
				}
				RenderText ();
				lastSelected = selected;				
			}
		}
		
		public override void Draw (Surface sfcGameWindow)
		{
			sfcGameWindow.Blit (sfcText, new Point (x, y));
		}
	}
}

