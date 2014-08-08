using System;
using org.flixel;

namespace org.flixel.data
{
	//@desc		This is a special effects utility class to help FlxGame do the 'quake' or screenshake effect
	public class FlxQuake
	{
		protected int _zoom;
		protected double _intensity;
		protected double _length;
		protected double _timer;
		
		public int x;
		public int y;
		
		public FlxQuake(int Zoom)
		{
			_zoom = Zoom;
			reset(0);
		}
		
		public void reset(double Intensity)
        {
            reset(Intensity, 0.5);
        }
        
        //@desc		Reset and trigger this special effect
		//@param	Intensity	Percentage of screen size representing the maximum distance that the screen can move during the 'quake'
		//@param	Duration	The length in seconds that the "quake" should last
		public void reset(double Intensity, double Duration)
		{
			x = 0;
			y = 0;
			_intensity = Intensity;
			if(_intensity == 0)
			{
				_length = 0;
				_timer = 0;
				return;
			}
			_length = Duration;
			_timer = 0.01;
		}
		
		//@desc		Updates and/or animates this special effect
		public void update()
		{
			if(_timer > 0)
			{
				_timer += FlxG.elapsed;
				if(_timer > _length)
				{
					_timer = 0;
					x = 0;
					y = 0;
				}
				else
				{

                    x = (int)(FlxG.random() * _intensity * FlxG.width * 2 - _intensity * FlxG.width) * _zoom;
                    y = (int)(FlxG.random() * _intensity * FlxG.height * 2 - _intensity * FlxG.height) * _zoom;
				}
			}
		}
	}
}