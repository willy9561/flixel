using org.flixel;

namespace org.flixel.data
{
	//@desc		This is a special effects utility class to help FlxGame do the 'fade' effect
	public class FlxFade : FlxSprite
	{
		protected double _delay;
		protected FlxG.FadeCompleteFunction _complete;
		protected double _helper;
		
		//@desc		Constructor initializes the fade object
		public FlxFade() :
            base(null, 0, 0, false, false, FlxG.width, FlxG.height, 0xFF000000, false)
		{
            scrollFactor.X = 0;
            scrollFactor.Y = 0;
            visible = false;
		}

        public void restart()
        {
            restart(0, 1, null, false);
        }
        
        //@desc		Reset and trigger this special effect
		//@param	Color			The color you want to use
		//@param	Duration		How long it should take to fade the screen out
		//@param	FadeComplete	A function you want to run when the fade finishes
		//@param	Force			Force the effect to reset
        public void restart(uint Color, double Duration, FlxG.FadeCompleteFunction FadeComplete, bool Force)
		{
			if(Duration == 0)
			{
				visible = false;
				return;
			}
			if(!Force && visible) return;
			draw(new FlxSprite(null, 0, 0, false, false, width, height, Color, false), 0, 0, false);
			_delay = Duration;
			_complete = FadeComplete;
			_helper = 0;
			alpha = 0;
			visible = true;
		}
		
		//@desc		Updates and/or animates this special effect
		override public void update()
		{
			if(visible && (alpha != 1))
			{
				_helper += FlxG.elapsed/_delay;
				alpha = _helper;

                if (alpha >= 1)
                {
                    alpha = 1;
                    if (_complete != null)
                        _complete();
                }
			}
		}

        public override void render()
        {
            FlxG.buffer.CopyPixels(_pixels, _pixels.GetRect(), new IntPoint(), 1, (1-alpha));
        }
	}
}
