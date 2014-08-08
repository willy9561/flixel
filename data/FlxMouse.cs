namespace org.flixel.data
{
	
	public class FlxMouse
	{
		public int x;
		public int y;
		protected int _current;
		protected int _last;
		
		public FlxMouse()
		{
			x = 0;
			y = 0;
			_current = 0;
			_last = 0;
		}

		public void update(int X, int Y)
		{
			x = X;
			y = Y;
			if((_last == -1) && (_current == -1))
				_current = 0;
			else if((_last == 2) && (_last == 2))
				_current = 1;
			_last = _current;
		}
		
		public void reset()
		{
			_current = 0;
			_last = 0;
		}
		
		//@desc		Check to see if this key is pressed
		//@param	Key		One of the key constants listed above (e.g. LEFT or A)
		//@return	Whether the key is pressed
		public bool pressed() { return _current > 0; }
		
		//@desc		Check to see if this key was JUST pressed
		//@param	Key		One of the key constants listed above (e.g. LEFT or A)
		//@return	Whether the key was just pressed
		public bool justPressed() { return _current == 2; }
		
		//@desc		Check to see if this key is NOT pressed
		//@param	Key		One of the key constants listed above (e.g. LEFT or A)
		//@return	Whether the key is not pressed
		public bool justReleased() { return _current == -1; }
		
	    public void handleMouseDown()
		{
			if(_current > 0) _current = 1;
			else _current = 2;
		}
		
		public void handleMouseUp()
		{
			if(_current > 0) _current = -1;
			else _current = 0;
		}
	}
}