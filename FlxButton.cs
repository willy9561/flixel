using System;
using System.Windows;

namespace org.flixel
{
    //@desc		A simple button class that calls a function when mouse-clicked
	public class FlxButton : FlxCore
	{
		public delegate void CallbackFunction();
        
        private bool _onToggle;
		private FlxSprite _off;
		private FlxSprite _on;
		private FlxText _offT;
		private Point _offTO;
		private FlxText _onT;
		private Point _onTO;
		private CallbackFunction _callback;
		private bool _pressed;
		private bool _initialized;
		
		public FlxButton(int X, int Y, FlxSprite Image, CallbackFunction Callback, FlxSprite ImageOn, FlxText Text,FlxText TextOn)
        {
            constructor(X, Y, Image, Callback, Image, Text, TextOn);
        }
        
        //@desc		Constructor
		//@param	X			The X position of the button
		//@param	Y			The Y position of the button
		//@param	Image		A FlxSprite object to use for the button background
		//@param	Callback	The function to call whenever the button is clicked
		//@param	ImageOn		A FlxSprite object to use for the button background when highlighted (optional)
		//@param	Text		A FlxText object to use to display text on this button (optional)
		//@param	TextOn		A FlxText object that is used when the button is highlighted (optional)
		public FlxButton(int X, int Y, FlxSprite Image, CallbackFunction Callback)
		{
			constructor(X, Y, Image, Callback, Image, null, null);
		}

        protected void constructor(int X, int Y, FlxSprite Image, CallbackFunction Callback, FlxSprite ImageOn, FlxText Text, FlxText TextOn)
        {
			x = X;
			y = Y;
			_off = Image;
			if(ImageOn == null) _on = _off;
			else _on = ImageOn;
			width = _off.width;
			height = _off.height;
			if(Text != null) _offT = Text;
			if(TextOn == null) _onT = _offT;
			else _onT = TextOn;
			if(_offT != null) _offTO = new Point(_offT.x,_offT.y);
			if(_onT != null) _onTO = new Point(_onT.x,_onT.y);
			
			_off.scrollFactor = scrollFactor;
			_on.scrollFactor = scrollFactor;
			if(_offT != null)
			{
				_offT.scrollFactor = scrollFactor;
				_onT.scrollFactor = scrollFactor;
			}
			
			_callback = Callback;
			_onToggle = false;
			_pressed = false;
			
			updatePositions();
			
			_initialized = false;
        }
		
		//@desc		Called by the game loop automatically, handles mouseover and click detection
		override public void update()
		{
			if(!_initialized)
			{
                FlxG.OnMouseUp += new FlxG.MouseEvent(onMouseUp);
				_initialized = true;
			}
			
			base.update();

			if((_off != null) && _off.exists && _off.active) _off.update();
			if((_on != null) && _on.exists && _on.active) _on.update();
			if(_offT != null)
			{
				if((_offT != null) && _offT.exists && _offT.active) _offT.update();
				if((_onT != null) && _onT.exists && _onT.active) _onT.update();
			}

			visibility(false);
			if(_off.overlapsPoint(FlxG.mouse.x,FlxG.mouse.y))
			{
				if(!FlxG.mouse.pressed())
					_pressed = false;
				else if(!_pressed)
				{
					_pressed = true;
					if(!_initialized) _callback();
				}
				visibility(!_pressed);
			}
			if(_onToggle) visibility(_off.visible);
			updatePositions();
		}
		
		override public void render()
		{
			base.render();
			if((_off != null) && _off.exists && _off.visible) _off.render();
			if((_on != null) && _on.exists && _on.visible) _on.render();
			if(_offT != null)
			{
				if((_offT != null) && _offT.exists && _offT.visible) _offT.render();
				if((_onT != null) && _onT.exists && _onT.visible) _onT.render();
			}
		}
		
		//@desc		Call this function from your callback to toggle the button off, like a checkbox
		public void switchOff()
		{
			_onToggle = false;
		}
		
		//@desc		Call this function from your callback to toggle the button on, like a checkbox
		public void switchOn()
		{
			_onToggle = true;
		}
		
		//@desc		Check to see if the button is toggled on, like a checkbox
		//@return	Whether the button is toggled
		public bool on()
		{
			return _onToggle;
		}
		
		//@desc		Internal function for handling the visibility of the off and on graphics
		//@param	On		Whether the button should be on or off
		private void visibility(bool On)
		{
			if(On)
			{
				_off.visible = false;
				if(_offT != null) _offT.visible = false;
				_on.visible = true;
				if(_onT != null) _onT.visible = true;
			}
			else
			{
				_on.visible = false;
				if(_onT != null) _onT.visible = false;
				_off.visible = true;
				if(_offT != null) _offT.visible = true;
			}
		}
		
		//@desc		Internal function that just updates the X and Y position of the button's graphics
		private void updatePositions()
		{
			_off.x = x;
			_off.y = y;
			if(_offT != null)
			{
				_offT.x = _offTO.X+x;
				_offT.y = _offTO.Y+y;
			}
			_on.x = x;
			_on.y = y;
			if(_onT != null)
			{
				_onT.x = _onTO.X+x;
				_onT.y = _onTO.Y+y;
			}
		}
		
		//@desc		Internal function for handling the actual callback call (for UI thread dependent calls)
		private void onMouseUp()
		{
            if((!exists) || (!visible)) return;
			if(_off.overlapsPoint(FlxG.mouse.x+(1-scrollFactor.X)*FlxG.scroll.X,FlxG.mouse.y+(1-scrollFactor.Y)*FlxG.scroll.Y)) _callback();
		}
	}
}
