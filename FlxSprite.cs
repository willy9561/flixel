using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using org.flixel.data;

namespace org.flixel
{
	
	//@desc		The main "game object" class, handles basic physics and animation
	public class FlxSprite : FlxCore
	{
		public delegate void AnimationCallback(string name, int currentFrame, int animation);
        
        public const bool LEFT = false;
		public const bool RIGHT = true;
		
		//@desc If you changed the size of your sprite object to shrink the bounding box, you might need to offset the new bounding box from the top-left corner of the sprite
		public Point offset;
		public Point velocity;
		public Point acceleration;
		//@desc	This isn't drag exactly, more like deceleration that is only applied when acceleration is not affecting the sprite
		public Point drag;
		public Point maxVelocity;
		//@desc WARNING: rotating sprites decreases rendering performance for this sprite by a factor of 10x!
		public double angle;
		public double angularVelocity;
		public double angularAcceleration;
		public double angularDrag;
		public double maxAngular;
		//@desc	If you want to do Asteroids style stuff, check out thrust (instead of directly accessing the object's velocity or acceleration)
		public double thrust;
		public double maxThrust;
		public double health;
		//@desc	Scale doesn't currently affect collisions automatically, you will need to adjust the width, height and offset manually.  WARNING: scaling sprites decreases rendering performance for this sprite by a factor of 10x!
		public Point scale;
		
		//@desc	Whether the current animation has finished its first (or only) loop
		public bool finished;
		private FlxArray<FlxAnim> _animations;
		private int _flipped;
		protected FlxAnim _curAnim;
		protected int _curFrame;
		private double _frameTimer;
		private AnimationCallback _callback;
		private bool _facing;
		
		//helpers
		protected int _bw;
		protected int _bh;
		private Rect _r;
        private Point _p;
		private Point _pZero;
		public WriteableBitmap pixels;
		protected WriteableBitmap _pixels;
		private double _alpha;
		private uint _color;
		//private var _ct:ColorTransform;
		
		//@desc		Constructor
		//@param	Graphic		The image you want to use
		//@param	X			The initial X position of the sprite
		//@param	Y			The initial Y position of the sprite
		//@param	Animated	Whether the Graphic parameter is a single sprite or a row of sprites
		//@param	Reverse		Whether you need this class to generate horizontally flipped versions of the animation frames
		//@param	Width		If you opt to NOT use an image and want to generate a colored block, or your sprite's frames are not square, you can specify a width here 
		//@param	Height		If you opt to NOT use an image you can specify the height of the colored block here (ignored if Graphic is not null)
		//@param	Color		Specifies the color of the generated block (ignored if Graphic is not null)
		//@param	Unique		Whether the graphic should be a unique instance in the graphics cache
        public FlxSprite(string Graphic, int X, int Y, bool Animated, bool Reverse, int Width, int Height, uint Color, bool Unique)
        {
            constructor(Graphic, X, Y, Animated, Reverse, Width, Height, Color, Unique);
        }

        public FlxSprite(string Graphic, int X, int Y, bool Animated)
		{
            constructor(Graphic, X, Y, Animated, false, 0, 0, 0xFF000000, false);
		}

        public FlxSprite(string Graphic)
        {
            constructor(Graphic, 0, 0, false, false, 0, 0, 0xFF000000, false);
        }

        protected void constructor(string Graphic,int X, int Y, bool Animated, bool Reverse, int Width, int Height, uint Color, bool Unique)
        {
			if(Graphic == null)
				pixels = FlxG.createBitmap(Width,Height,Color,Unique);
			else
				pixels = FlxG.addBitmap(Graphic,Reverse);
				
			last.X = x = X;
			last.Y = y = Y;
			if(Width == 0)
			{
				if(Animated)
					Width = pixels.PixelHeight;
				else
					Width = pixels.PixelWidth;
			}
			width = _bw = Width;
			height = _bh = pixels.PixelHeight;
			offset = new Point();
			
			velocity = new Point();
			acceleration = new Point();
			drag = new Point();
			maxVelocity = new Point(10000,10000);
			
			angle = 0;
			angularVelocity = 0;
			angularAcceleration = 0;
			angularDrag = 0;
			maxAngular = 10000;
			
			thrust = 0;
			
			scale = new Point(1,1);
			
			finished = false;
			_facing = true;
			_animations = new FlxArray<FlxAnim>();
			if(Reverse)
				_flipped = pixels.PixelWidth>>1;
			else
				_flipped = 0;
			_curAnim = null;
			_curFrame = 0;
			_frameTimer = 0;

            _p = new Point(x, y);
			_pZero = new Point();
			_r = new Rect(0,0,_bw,_bh);
			_pixels = new WriteableBitmap(width,height);

            _pixels.CopyPixels(pixels, new IntRect(0, _bw, _bh, 0), new IntPoint());
	
			health = 1;
			alpha = 1;
			color = 0x00ffffff;
			
			_callback = null;
        }
		
		//@desc		Called by game loop, handles animation and physics
		override public void update()
		{
			base.update();
			
			if(!active) return;
			
			//animation
			if((_curAnim != null) && (_curAnim.delay > 0) && (_curAnim.looped || !finished))
			{
				_frameTimer += FlxG.elapsed;
				if(_frameTimer > _curAnim.delay)
				{
					_frameTimer -= _curAnim.delay;
					if(_curFrame == _curAnim.frames.Length-1)
					{
						if(_curAnim.looped) _curFrame = 0;
						finished = true;
					}
					else
						_curFrame++;
					calcFrame();
				}
			}
			
			//motion + physics
			angle += (angularVelocity = FlxG.computeVelocity(angularVelocity,angularAcceleration,angularDrag,maxAngular))*FlxG.elapsed;
			Point thrustComponents;
			if(thrust != 0)
			{
				thrustComponents = FlxG.rotatePoint(-thrust,0,0,0,angle);
				Point maxComponents = FlxG.rotatePoint(-maxThrust,0,0,0,angle);
				double max = Math.Abs(maxComponents.X);
				if(max > Math.Abs(maxComponents.Y))
					maxComponents.Y = max;
				else
					max = Math.Abs(maxComponents.Y);
				maxVelocity.X = Math.Abs(max);
				maxVelocity.Y = Math.Abs(max);
			}
			else
				thrustComponents = _pZero;

            // distance calculation
            double distX = FlxG.computeDistance(velocity.X, acceleration.X + thrustComponents.X, drag.X);
            double distY = FlxG.computeDistance(velocity.Y, acceleration.Y + thrustComponents.Y, drag.Y);
			
            // max velocity checking
            double maxV = maxVelocity.X * FlxG.elapsed;
            if (distX > maxV) {
	            distX = maxV;
            } else if (distX < -maxV) {
	            distX = -maxV;
            }
            maxV = maxVelocity.Y * FlxG.elapsed;
            if (distY > maxV) {
	            distY = maxV;
            } else if (distX < -maxV) {
	            distY = -maxV;
            }
			
            x += distX;
            y += distY;
			
            // velocity calculation
            velocity.X = FlxG.computeVelocity(velocity.X,acceleration.X + thrustComponents.X,drag.X,maxVelocity.X);
            velocity.Y = FlxG.computeVelocity(velocity.Y, acceleration.Y + thrustComponents.Y, drag.Y, maxVelocity.Y);


			//x += (velocity.X = FlxG.computeVelocity(velocity.X,acceleration.X+thrustComponents.X,drag.X,maxVelocity.X))*FlxG.elapsed;
			//y += (velocity.Y = FlxG.computeVelocity(velocity.Y,acceleration.Y+thrustComponents.Y,drag.Y,maxVelocity.Y))*FlxG.elapsed;
		}
		
		//@desc		Called by game loop, blits current frame of animation to the screen (and handles rotation)
		override public void render()
		{
			if(!visible)
				return;

            Point _p = new Point(x, y);
            getScreenXY(out _p);

            if ((angle != 0) || (scale.X != 1) || (scale.Y != 1))
            {
                TransformGroup tg = new TransformGroup();

                /*ScaleTransform st = new ScaleTransform();
                st.ScaleX = scale.X;
                st.ScaleY = scale.Y;*/

                RotateTransform rt = new RotateTransform();
                rt.Angle = Math.PI * 2 * (angle / 360);
                rt.CenterX = _bw >> 1;
                rt.CenterX = _bh >> 1;

                tg.Children.Add(rt);

                TranslateTransform tt = new TranslateTransform();
                tt.X = _p.X;
                tt.Y = _p.Y;

                tg.Children.Add(tt);

                FlxG.buffer.CopyPixels(_pixels, _pixels.GetRect(), tg);
            }
            else
            {
                FlxG.buffer.CopyPixels(_pixels, _pixels.GetRect(), new IntPoint(_p));
            }
		}
		
		override public bool overlapsPoint(double X, double Y)
        {
            return overlapsPoint(X, Y, false);
        }
        
        //@desc		Checks to see if a point in 2D space overlaps this FlxCore object
		//@param	X			The X coordinate of the point
		//@param	Y			The Y coordinate of the point
		//@param	PerPixel	Whether or not to use per pixel collision checking
		//@return	Whether or not the point overlaps this object
		override public bool overlapsPoint(double X, double Y, bool PerPixel)
		{
			double tx = x;
			double ty = y;
			if((scrollFactor.X != 1) || (scrollFactor.Y != 1))
			{
				tx -= Math.Floor(FlxG.scroll.X*scrollFactor.X);
				ty -= Math.Floor(FlxG.scroll.Y*scrollFactor.Y);
			}
			/*if(PerPixel)
				return _pixels.hitTest(new Point(0,0),0xFF,new Point(X-tx,Y-ty));
			else*/ if((X <= tx) || (X >= tx+width) || (Y <= ty) || (Y >= ty+height))
				return false;
			return true;
		}
		
		//@desc		Called when this object collides with a FlxBlock on one of its sides
		//@return	Whether you wish the FlxBlock to collide with it or not
		override public bool hitWall() { velocity.X = 0; return true; }
		
		//@desc		Called when this object collides with the top of a FlxBlock
		//@return	Whether you wish the FlxBlock to collide with it or not
		override public bool hitFloor() { velocity.Y = 0; return true; }
		
		//@desc		Called when this object collides with the bottom of a FlxBlock
		//@return	Whether you wish the FlxBlock to collide with it or not
		override public bool hitCeiling() { velocity.Y = 0; return true; }
		
		//@desc		Call this function to "damage" (or give health bonus) to this sprite
		//@param	Damage		How much health to take away (use a negative number to give a health bonus)
		virtual public void hurt(double Damage)
		{
			if((health -= Damage) <= 0)
				kill();
		}
		
		//@desc		Called if/when this sprite is launched by a FlxEmitter
		virtual public void onEmit() { }
		
		public void addAnimation(string Name, int[] Frames)
        {
            addAnimation(Name, Frames, 0, true);
        }
        
        //@desc		Adds a new animation to the sprite
		//@param	Name		What this animation should be called (e.g. "run")
		//@param	Frames		An array of numbers indicating what frames to play in what order (e.g. 1, 2, 3)
		//@param	FrameRate	The speed in frames per second that the animation should play at (e.g. 40 fps)
		//@param	Looped		Whether or not the animation is looped or just plays once
		public void addAnimation(string Name, int[] Frames, double FrameRate, bool Looped)
		{
			_animations.add(new FlxAnim(Name,Frames,FrameRate,Looped));
		}
		
		//@desc		Pass in a function to be called whenever this sprite's animation changes
		//@param	AnimationCallback		A function that has 3 parameters: a string name, a uint frame number, and a uint frame index
		public void addAnimationCallback(AnimationCallback Function)
		{
			_callback = Function;
		}
		
		public void play(string AnimName)
        {
            play(AnimName, false);
        }
        
        //@desc		Plays an existing animation (e.g. "run") - if you call an animation that is already playing it will be ignored
		//@param	AnimName	The string name of the animation you want to play
		//@param	Force		Whether to force the animation to restart
		public void play(string AnimName, bool Force)
		{
			if(!Force && (_curAnim != null) && (AnimName == _curAnim.name)) return;
			_curFrame = 0;
			_frameTimer = 0;
			for(int i = 0; i < _animations.Count; i++)
			{
				if(_animations[i].name == AnimName)
				{
					finished = false;
					_curAnim = _animations[i];
					calcFrame();
					return;
				}
			}
		}
		
		//@desc		Tell the sprite which way to face (you can just set 'facing' but this function also updates the animation instantly)
		//@param	Direction		True is Right, False is Left (see static const members RIGHT and LEFT)		
		public bool facing
		{
			set
            {
                bool c = _facing != value;
			    _facing = value;
			    if(c) calcFrame();
            }
            get
            {
                return _facing;
            }
		}
		
		//@desc		Tell the sprite to change to a random frame of animation (useful for instantiating particles or other weird things)
		public void randomFrame()
		{
			Random random = new Random();
            int startFrame = random.Next(pixels.PixelWidth / _bw);
            
            specificFrame(startFrame);
            
            //_pixels.copyPixels(pixels,new Rectangle(Math.floor(Math.random()*(pixels.width/_bw))*_bw,0,_bw,_bh),_pZero);
		}
		
		//@desc		Tell the sprite to change to a specific frame of animation (useful for instantiating particles)
		//@param	Frame	The frame you want to display
		public void specificFrame(int Frame)
		{
			int startX = Frame*_bw;
            _pixels.CopyPixels(pixels, new IntRect(startX, startX + _bw, pixels.PixelHeight, 0 ), new IntPoint(0, 0));            
            //_pixels.copyPixels(pixels,new Rectangle(Frame*_bw,0,_bw,_bh),_pZero);
		}
		
		//@desc		Call this function to figure out the post-scrolling "screen" position of the object
		//@param	P	Takes a Flash Point object and assigns the post-scrolled X and Y values of this object to it
		override protected void getScreenXY(out Point P)
		{
            P = new Point();
            P.X = Math.Floor(x-offset.X)+Math.Floor(FlxG.scroll.X*scrollFactor.X);
			P.Y = Math.Floor(y-offset.Y)+Math.Floor(FlxG.scroll.Y*scrollFactor.Y);
		}
		
		//@desc		Internal function to update the current animation frame
		protected void calcFrame()
		{
			int rx = 0;
			if (_curAnim != null)
				rx = _curAnim.frames[_curFrame];
			if(!_facing && (_flipped > 0))
                rx = (_flipped << 1) - rx - 1;
            specificFrame(rx);
			//if(_ct != null) _pixels.colorTransform(_r,_ct);
			if(_callback != null) _callback(_curAnim.name,_curFrame,_curAnim.frames[_curFrame]);
		}
		
		//@desc		The setter for alpha
		//@param	Alpha	The new opacity value of the sprite (between 0 and 1)
		public double alpha
		{
            set
            {
			    if(value > 1) value = 1;
			    if(value < 0) value = 0;
			    _alpha = value;
			    //if((_alpha != 1) || (_color != 0x00ffffff)) _ct = new ColorTransform(Number(_color>>16)/255,Number(_color>>8&0xff)/255,Number(_color&0xff)/255,_alpha);
			    //else _ct = null;
			    calcFrame();
            }
            get
            {
                return _alpha;
            }
		}
		
		//@desc		The setter for color - tints the whole sprite this color (similar to Photoshop multiply)
		//@param	Color	The new color value of the sprite (0xRRGGBB) - ignores alpha
		public uint color
		{
            set
            {
			    _color = value & 0x00ffffff;
			    //if((_alpha != 1) || (_color != 0x00ffffff)) _ct = new ColorTransform(Number(_color>>16)/255,Number(_color>>8&0xff)/255,Number(_color&0xff)/255,_alpha);
			    //else _ct = null;
			    calcFrame();
            }
            get
            {
                return _color;
            }
		}
		
		public void draw(FlxSprite Brush)
        {
            draw(Brush, 0, 0, false);
        }
        
        //@desc		This function draws or stamps one FlxSprite onto another (not intended to replace render()!)
		//@param	Brush		The image you want to use as a brush or stamp or pen or whatever
		//@param	X			The X coordinate of the brush's top left corner on this sprite
		//@param	Y			They Y coordinate of the brush's top left corner on this sprite
		//@param	ForceAlpha	Whether or not to use the alpha of the brush as an eraser
		public void draw(FlxSprite Brush, int X, int Y, bool ForceAlpha)
		{
            WriteableBitmap wb = Brush._pixels;

            TransformGroup tg = new TransformGroup();                      

            /*ScaleTransform st = new ScaleTransform();
            st.ScaleX = Brush.scale.X;
            st.ScaleY = Brush.scale.Y;*/

            RotateTransform rt = new RotateTransform();
            rt.Angle = Math.PI * 2 * (Brush.angle / 360);
            rt.CenterX = Brush._bw >> 1;
            rt.CenterY = Brush._bh >> 1;

            tg.Children.Add(rt);

            TranslateTransform tt = new TranslateTransform();
            tt.X = X;
            tt.Y = Y;

            tg.Children.Add(tt);

            pixels.CopyPixels(wb, wb.GetRect(), tg);

            calcFrame();
		}
	}
}