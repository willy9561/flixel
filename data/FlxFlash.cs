using org.flixel;

namespace org.flixel.data
{
	//@desc		This is a special effects utility class to help FlxGame do the 'flash' effect
	public class FlxFlash : FlxSprite
	{
		protected double _delay;
		protected FlxG.FlashCompleteFunction _complete;
		protected double _helper;
		
		//@desc		Constructor for this special effect
		public FlxFlash() :
            base(null, 0, 0, false, false, FlxG.width, FlxG.height, 0xFF000000, false)
		{
            scrollFactor.X = 0;
            scrollFactor.Y = 0;
            visible = false;
            
		}
		
		public void restart(uint Color)
        {
            restart(Color, 1, null, false);
        }
        
        //@desc		Reset and trigger this special effect
		//@param	Color			The color you want to use
		//@param	Duration		How long it takes for the flash to fade
		//@param	FlashComplete	A function you want to run when the flash finishes
		//@param	Force			Force the effect to reset
        public void restart(uint Color, double Duration, FlxG.FlashCompleteFunction FlashComplete, bool Force)
		{
			if(Color == 0)
			{
				visible = false;
				return;
			}
			if(!Force && visible) return;
			draw(new FlxSprite(null, 0, 0, false, false, width, height, Color, false), 0, 0, false);
			_delay = Duration;
			_complete = FlashComplete;
			_helper = 1;
			alpha = 1;
			visible = true;
		}
		
		//@desc		Updates and/or animates this special effect
		override public void update()
		{
			if(visible)
			{
				_helper -= FlxG.elapsed/_delay;
				alpha = _helper;
				if(alpha <= 0)
				{
					visible = false;
					if(_complete != null)
						_complete();
				}
			}
		}

        public override void render()
        {
            FlxG.buffer.CopyPixels(_pixels, _pixels.GetRect(), new IntPoint(), 1, alpha);
        }
	}
}
