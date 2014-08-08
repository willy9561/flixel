using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace org.flixel
{
	
	//@desc		A simple particle system class
	public class FlxEmitter : FlxCore
	{
		public Point minVelocity;
		public Point maxVelocity;
		private double _minRotation;
		private double _maxRotation;
		private double _gravity;
		private double _drag;
		private double _delay;
		private double _timer;
		private FlxArray<FlxSprite> _sprites;
		private int _particle;
		
		//@desc		Constructor
		//@param	X				The X position of the emitter
		//@param	Y				The Y position of the emitter
		//@param	Width			The width of the emitter (particles are emitted from a random position inside this box)
		//@param	Height			The height of the emitter
		//@param	Sprites			A pre-configured FlxArray of FlxSprite objects for the emitter to use (optional)
		//@param	Delay			A negative number defines the lifespan of the particles that are launched all at once.  A positive number tells it how often to fire a new particle.
		//@param	MinVelocityX	The minimum X velocity of the particles
		//@param	MaxVelocityX	The maximum X velocity of the particles (every particle will have a random X velocity between these values)
		//@param	MinVelocityY	The minimum Y velocity of the particles
		//@param	MaxVelocityY	The maximum Y velocity of the particles (every particle will have a random Y velocity between these values)
		//@param	MinRotation		The minimum angular velocity of the particles
		//@param	MaxRotation		The maximum angular velocity of the particles (you guessed it)
		//@param	Gravity			How much gravity should affect the particles
		//@param	Drag			Sets both the X and Y "Drag" or deceleration on the particles
		//@param	Graphics		If you opted to not pre-configure an array of FlxSprite objects, you can simply pass in a particle image or sprite sheet (ignored if you pass in an array)
		//@param	Quantity		The number of particles to generate when using the "create from image" option (ignored if you pass in an array)
		//@param	Multiple		Whether the image in the Graphics param is a single particle or a bunch of particles (if it's a bunch, they need to be square!)
        public FlxEmitter(double X, double Y, int Width, int Height)
		{
            constructor(X, Y, Width, Height, null, -1, -100, 100, -100, 100, -360, 360, 500, 0, null, 0, false, null);
		}

        public FlxEmitter(double X, double Y, int Width, int Height, FlxArray<FlxSprite> Sprites, double Delay, double MinVelocityX, double MaxVelocityX, double MinVelocityY, double MaxVelocityY, double MinRotation, double MaxRotation, double Gravity, double Drag, string Graphics, int Quantity, bool Multiple, FlxLayer Parent)
        {
            constructor(X, Y, Width, Height, Sprites, Delay, MinVelocityX, MaxVelocityX, MinVelocityY, MaxVelocityY, MinRotation, MaxRotation, Gravity, Drag, Graphics, Quantity, Multiple, Parent);
        }

        protected void constructor(double X, double Y, int Width, int Height, FlxArray<FlxSprite> Sprites, double Delay, double MinVelocityX, double MaxVelocityX, double MinVelocityY, double MaxVelocityY, double MinRotation, double MaxRotation, double Gravity, double Drag, string Graphics, int Quantity, bool Multiple, FlxLayer Parent)
        {
			visible = false;
			x = X;
			y = Y;
			width = Width;
			height = Height;
			
			minVelocity = new Point(MinVelocityX,MinVelocityY);
			maxVelocity = new Point(MaxVelocityX,MaxVelocityY);
			_minRotation = MinRotation;
			_maxRotation = MaxRotation;
			_gravity = Gravity;
			_drag = Drag;
			_delay = Delay;
			
			int i;
			if(Graphics != null)
			{
				_sprites = new FlxArray<FlxSprite>();
				for(i = 0; i < Quantity; i++)
				{
					if(Multiple)
						_sprites.add(new FlxSprite(Graphics, 0, 0, true)).randomFrame();
					else
                        _sprites.add(new FlxSprite(Graphics, 0, 0, true));
				}
				for(i = 0; i < _sprites.Count; i++)
				{
					if(Parent == null)
						FlxG.state.add(_sprites[i]);
					else
						Parent.add(_sprites[i]);
				}
			}
			else
				_sprites = Sprites;
			
			kill();
			if(_delay > 0)
				restart();
        }
		
		//@desc		Called automatically by the game loop, decides when to launch particles and when to "die"
		override public void update()
		{
			_timer += FlxG.elapsed;
			if(_delay < 0)
			{
				if(_timer > -_delay) { kill(); return; }
				if(_sprites[0].exists) return;
                for (uint i = 0; i < _sprites.Count; i++) emit();
				return;
			}
			while(_timer > _delay) { _timer -= _delay; emit(); }
		}
		
		//@desc		Call this function to reset the emitter (if you used a negative delay, calling this function "Explodes" the emitter again)
        public void restart()
		{
			active = true;
			_timer = 0;
			_particle = 0;
		}
		
		//@desc		This function can be used both internally and externally to emit the next particle
		public void emit()
		{
            FlxSprite s = _sprites[_particle];
			s.exists = true;
			s.x = x - (s.width>>1);
            if (width != 0) s.x += FlxG.random() * width;
			s.y = y - (s.height>>1);
            if (height != 0) s.y += FlxG.random() * height;
			s.velocity.X = minVelocity.X;
            if (minVelocity.X != maxVelocity.X) s.velocity.X += FlxG.random() * (maxVelocity.X - minVelocity.Y);
			s.velocity.Y = minVelocity.Y;
            if (minVelocity.Y != maxVelocity.Y) s.velocity.Y += FlxG.random() * (maxVelocity.Y - minVelocity.Y);
			s.acceleration.Y = _gravity;
			s.angularVelocity = _minRotation;
            if (_minRotation != _maxRotation) s.angularVelocity += FlxG.random() * (_maxRotation - _minRotation);
            if (s.angularVelocity != 0) s.angle = FlxG.random() * 360 - 180;
			s.drag.X = _drag;
			s.drag.Y = _drag;
			_particle++;
			if(_particle >= _sprites.Count)
				_particle = 0;
			s.onEmit();
		}
		
		//@desc		Call this function to turn off all the particles and the emitter
		override public void kill()
		{
			active = false;
			for(int i = 0; i < _sprites.Count; i++)
				_sprites[i].exists = false;
		}
	}
}
