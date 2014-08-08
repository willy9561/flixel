namespace org.flixel.data
{
	//@desc		Just a helper structure for the FlxSprite animation system
	public class FlxAnim
	{
		public string name;
		public double delay;
		public int[] frames;
		public bool looped;
		
		public FlxAnim(string Name, int[] Frames)
        {
            constructor(Name, Frames, 0, true);
        }
        
        //@desc		Constructor
		//@param	Name		What this animation should be called (e.g. "run")
		//@param	Frames		An array of numbers indicating what frames to play in what order (e.g. 1, 2, 3)
		//@param	FrameRate	The speed in frames per second that the animation should play at (e.g. 40 fps)
		//@param	Looped		Whether or not the animation is looped or just plays once
		public FlxAnim(string Name, int[] Frames, double FrameRate, bool Looped)
		{
			constructor(Name, Frames, FrameRate, Looped);
		}

        protected void constructor(string Name, int[] Frames, double FrameRate, bool Looped)
        {
            name = Name;
			delay = 1.0/FrameRate;
			frames = Frames;
			looped = Looped;
        }
	}
}