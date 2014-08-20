using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Resources;
using org.flixel.data;

namespace org.flixel
{
	//@desc		This is a global helper class full of useful functions for audio, input, basic info, and the camera system
	public class FlxG
	{
		//[Embed(source="data/cursor.png")] static private var ImgDefaultCursor:Class;

        public delegate void EnterFrame();
        public delegate void CollisionFunction(FlxCore object1, FlxCore object2);
        public delegate void FlashCompleteFunction();
        public delegate void FadeCompleteFunction();
        public delegate void FocusChangedFunction();
        public delegate void KeyEvent(KeyboardEvent keyboardEvent);
        public delegate void MouseEvent();

        public static event EnterFrame OnEnterFrame;
        public static event KeyEvent OnKeyDown;
        public static event KeyEvent OnKeyUp;
        public static event FocusChangedFunction OnFocusLost;
        public static event FocusChangedFunction OnFocus;
        public static event MouseEvent OnMouseDown;
        public static event MouseEvent OnMouseUp;
		
		public const string LIBRARY_NAME = "flixel";
		public const uint LIBRARY_MAJOR_VERSION = 1;
		public const uint LIBRARY_MINOR_VERSION = 27;

        static public Panel container;

        static protected Random randomObject = new Random();
        static protected FlxGame _game;
		
		//@desc Represents the amount of time in seconds that passed since last frame
		static public double elapsed;
		//@desc A reference or pointer to the current FlxState object being used by the game
		static public FlxState state;
		//@desc The width of the screen in game pixels
		static public int width;
		//@desc The height of the screen in game pixels
		static public int height;
		//@desc Levels and scores are generic global variables that can be used for various cross-state stuff
		static public int level;
		//static public FlxArray levels;
		static public int score;
		static public FlxArray<string> scores;

		//@desc The current game coordinates of the mouse pointer (not necessarily the screen coordinates)
		static public FlxMouse mouse;
		static public FlxKeyboard keys;
		
		//audio
		static protected int _muted;
		static protected string _music;
		static protected MediaElement _musicChannel;
		static protected TimeSpan _musicPosition;
		static protected double _volume;
		static protected double _musicVolume;
		static protected double _masterVolume;
		
		//Ccmera system variables
		static public FlxCore followTarget;
		static public Point followLead;
		static public double followLerp;
		static public Point followMin;
		static public Point followMax;
		static protected Point _scrollTarget;
		
		//graphics stuff     
        static public int _zoom;
		static public Point scroll;
		static public WriteableBitmap buffer;
        static public WriteableBitmap frontBuffer;
		static protected Dictionary<string, WriteableBitmap> _cache;
		
		//Kongregate API object
		//static public var kong:FlxKong;

        static public double random()
        {
            double randomValue = 0;
            do
            {
                randomValue = randomObject.NextDouble();
            }
            while (randomValue == 1);

            return randomValue;

        }

        static public void enterFrame()
        {
            if (OnEnterFrame != null)
                OnEnterFrame();
        }

        static public void mouseDown()
        {
            if (OnMouseDown != null)
                OnMouseDown();
        }

        static public void mouseUp()
        {
            if (OnMouseUp != null)
                OnMouseUp();
        }

        static public void keyDown(KeyboardEvent keyboardEvent)
        {
            if (OnKeyDown != null)
                OnKeyDown(keyboardEvent);
        }

        static public void keyUp(KeyboardEvent keyboardEvent)
        {
            if (OnKeyUp != null)
                OnKeyUp(keyboardEvent);
        }

        static public void focusLost()
        {
            if (OnFocusLost != null)
                OnFocusLost();
        }

        static public void focus()
        {
            if (OnFocus != null)
                OnFocus();
        }
		
		static public void resetInput()
		{
			keys.reset();
			mouse.reset();
		}
		
		static public void setMusic(string Music)
        {
            setMusic(Music, 1, true);
        }
        
        //@desc		Set up and autoplay a music track
		//@param	Music		The sound file you want to loop in the background
		//@param	Volume		How loud the sound should be, from 0 to 1
		//@param	Autoplay	Whether to automatically start the music or not (defaults to true)
		static public void setMusic(string Music, double Volume, bool Autoplay)
		{
			stopMusic();
			_music = Music;
			_musicVolume = Volume;
			if(Autoplay)
				playMusic();
		}
		
		static public void play(string SoundEffect)
        {
            play(SoundEffect, 1);
        }
        
        //@desc		Plays a sound effect once
		//@param	SoundEffect		The sound you want to play
		//@param	Volume			How loud to play it (0 to 1)
		static public void play(string SoundEffect, double Volume)
		{
            if (container != null)
            {
                MediaElement me = new MediaElement();
                me.CurrentStateChanged += new RoutedEventHandler(me_CurrentStateChanged);
                container.Children.Add(me);

                StreamResourceInfo sri = Application.GetResourceStream(new Uri(SoundEffect, UriKind.Relative));
                if (sri != null)
                {
                    Stream stream = sri.Stream;
                    me.Volume = Volume * _muted * _volume * _masterVolume;
                    me.AutoPlay = true;
                    me.SetSource(stream);
                }
            }
		}

        static void me_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            MediaElement me = sender as MediaElement;
            if (me != null && me.CurrentState == System.Windows.Media.MediaElementState.Paused)
                container.Children.Remove(me);
        }
		
		//@desc		Plays or resumes the music file set up using setMusic()
		static public void playMusic()
		{
            if (_music != null)
            {
                if (_musicPosition.TotalSeconds < 0)
                    return;
                if (_musicPosition.TotalSeconds == 0)
                {
                    if (_musicChannel == null && container != null)
                    {
                        _musicChannel = new MediaElement();
                        container.Children.Add(_musicChannel);
                        StreamResourceInfo sri = Application.GetResourceStream(new Uri(_music, UriKind.Relative));
                        if (sri != null)
                        {
                            Stream stream = sri.Stream;
                            _musicChannel.Volume = _muted * _volume * _musicVolume * _masterVolume;
                            _musicChannel.AutoPlay = true;
                            _musicChannel.CurrentStateChanged += delegate(object sender, RoutedEventArgs e) 
                                {
                                    MediaElement me = sender as MediaElement;
                                    if (me.CurrentState == System.Windows.Media.MediaElementState.Paused)
                                    {
                                        me.Position = new TimeSpan(0);
                                        me.Play();
                                    }
                                };
                            _musicChannel.SetSource(stream);
                        }
                    }
                }
                else
                {
                    _musicChannel.Position = _musicPosition;
                    _musicChannel.Play();
                }
            }
			_musicPosition = new TimeSpan();
		}
		
		//@desc		Pauses the current music track
		static public void pauseMusic()
		{
			if(_musicChannel == null)
			{
				_musicPosition = new TimeSpan();
				return;
			}
			_musicPosition = _musicChannel.Position;
			_musicChannel.Stop();
		}
		
		//@desc		Stops the current music track
		static public void stopMusic()
		{
			_musicPosition = new TimeSpan();
			if(_musicChannel != null)
			{
				_musicChannel.Stop();
				_musicChannel = null;
			}
		}
		
		//@desc		Mutes the sound
		//@param	SoundOff	Whether the sound should be off or on
		static public void setMute(bool SoundOff) { if(SoundOff) _muted = 0; else _muted = 1; adjustMusicVolume(); }
		
		//@desc		Check to see if the game is muted
		//@return	Whether the game is muted
		static public bool getMute() { if(_muted == 0) return true; return false; }
		
		//@desc		Change the volume of the game
		//@param	Volume		A number from 0 to 1
		static public void setVolume(double Volume) { _volume = Volume; adjustMusicVolume(); }
		
		//@desc		Find out how load the game is currently
		//@param	A number from 0 to 1
		static public double getVolume() { return _volume; }
		
		//@desc		Change the volume of just the music
		//@param	Volume		A number from 0 to 1
		static public void setMusicVolume(double Volume) { _musicVolume = Volume; adjustMusicVolume(); }
		
		//@desc		Find out how loud the music is
		//@return	A number from 0 to 1
		static public double getMusicVolume() { return _musicVolume; }
		
		//@desc		An internal function that adjust the volume levels and the music channel after a change
		static private void adjustMusicVolume()
		{
			if(_muted < 0)
				_muted = 0;
			else if(_muted > 1)
				_muted = 1;
			if(_volume < 0)
				_volume = 0;
			else if(_volume > 1)
				_volume = 1;
			if(_musicVolume < 0)
				_musicVolume = 0;
			else if(_musicVolume > 1)
				_musicVolume = 1;
			if(_masterVolume < 0)
				_masterVolume = 0;
			else if(_masterVolume > 1)
				_masterVolume = 1;
			if(_musicChannel != null)
				_musicChannel.Volume = (_muted*_volume*_musicVolume*_masterVolume);
		}
		
		//@desc		Generates a new BitmapData object (basically a colored square :P) and caches it
		//@param	Width	How wide the square should be
		//@param	Height	How high the square should be
		//@param	Color	What color the square should be
		//@return	This object is used during the sprite blitting process
		static public WriteableBitmap createBitmap(int Width, int Height, uint Color, bool Unique)
		{
			string key = Width+"x"+Height+":"+Color;

            WriteableBitmap wb = new WriteableBitmap(Width, Height);
            wb.SetColor(unchecked((int)Color));

			if((!_cache.ContainsKey(key)) || (_cache[key] == null))
            {
                if (!_cache.ContainsKey(key))
                    _cache.Add(key, wb);
                else
                    _cache[key] = wb;              
            }
			else if(Unique)
			{
				uint inc = 0;
				string ukey;
				do { ukey = key + inc++; } while((!_cache.ContainsKey(ukey)) || (_cache[ukey] == null));
			    
                key = ukey;
                
                if (!_cache.ContainsKey(key))
                    _cache.Add(key, wb);
                else
                    _cache[key] = wb;    
			}
			
            return _cache[key];
		}
		
		static public WriteableBitmap addBitmap(string Graphic)
        {
            return addBitmap(Graphic, false);
        }
        
        //@desc		Loads a bitmap from a file, caches it, and generates a horizontally flipped version if necessary
		//@param	Graphic		The image file that you want to load
		//@param	Reverse		Whether to generate a flipped version
		static public WriteableBitmap addBitmap(string Graphic, bool Reverse)
		{
			bool needReverse = false;
			string key = Graphic;

			if((!_cache.ContainsKey(key)) || (_cache[key] == null))
			{
                BitmapImage bi = new BitmapImage();
                StreamResourceInfo sri = Application.GetResourceStream(new Uri(Graphic, UriKind.Relative));
                Stream stream = sri.Stream;
                bi.SetSource(stream);
                WriteableBitmap wb = new WriteableBitmap(bi);
                
                _cache[key] = wb;
				if(Reverse) needReverse = true;
			}

			WriteableBitmap pixels = _cache[key];
			if(!needReverse && Reverse)
				needReverse = true;

			if(needReverse)
			{
				WriteableBitmap newPixels = new WriteableBitmap(pixels.PixelWidth, pixels.PixelHeight);
                newPixels.CopyPixelsXReverse(pixels);

                pixels = newPixels;
			}
			return pixels;
		}
		
		//@desc		Rotates a point in 2D space around another point by the given angle
		//@param	X		The X coordinate of the point you want to rotate
		//@param	Y		The Y coordinate of the point you want to rotate
		//@param	PivotX	The X coordinate of the point you want to rotate around
		//@param	PivotY	The Y coordinate of the point you want to rotate around
		//@param	Angle	Rotate the point by this many degrees
		//@return	A Flash Point object containing the coordinates of the rotated point
		static public Point rotatePoint(double X, double Y, double PivotX, double PivotY, double Angle)
		{
			double radians = -Angle / 180 * Math.PI;
			double dx = X-PivotX;
			double dy = PivotY-Y;
			return new Point(PivotX + Math.Cos(radians)*dx - Math.Sin(radians)*dy, PivotY - (Math.Sin(radians)*dx + Math.Cos(radians)*dy));
		}
		
		//@desc		Calculates the angle between a point and the origin (0,0)
		//@param	X		The X coordinate of the point
		//@param	Y		The Y coordinate of the point
		//@return	The angle in degrees
		static public double getAngle(double X, double Y)
		{
			return Math.Atan2(Y,X) * 180 / Math.PI;
		}

		static public void follow(FlxCore Target)
        {
            follow(Target, 1);
        }
        
        //@desc		Tells the camera subsystem what FlxCore object to follow
		//@param	Target		The object to follow
		//@param	Lerp		How much lag the camera should have (can help smooth out the camera movement)
		static public void follow(FlxCore Target, double Lerp)
		{
			followTarget = Target;
			followLerp = Lerp;
			
			scroll.X = _scrollTarget.X = (width>>1)-followTarget.x-(followTarget.width>>1);
			scroll.Y = _scrollTarget.Y = (height>>1)-followTarget.y-(followTarget.height>>1);
		}
		
		static public void followAdjust()
        {
            followAdjust(0, 0);
        }
        
        //@desc		Specify an additional camera component - the velocity-based "lead", or amount the camera should track in front of a sprite
		//@param	LeadX		Percentage of X velocity to add to the camera's motion
		//@param	LeadY		Percentage of Y velocity to add to the camera's motion
		static public void followAdjust(double LeadX, double LeadY)
		{
			followLead = new Point(LeadX,LeadY);
		}
		
		static public void followBounds()
        {
            followBounds(0, 0, 0, 0);
        }
        
        //@desc		Specify an additional camera component - the boundaries of the level or where the camera is allowed to move
		//@param	MinX	The smallest X value of your level (usually 0)
		//@param	MinY	The smallest Y value of your level (usually 0)
		//@param	MaxX	The largest X value of your level (usually the level width)
		//@param	MaxY	The largest Y value of your level (usually the level height)
		static public void followBounds(int MinX, int MinY, int MaxX, int MaxY)
		{
			followMin = new Point(-MinX,-MinY);
			followMax = new Point(-MaxX+width,-MaxY+height);
			if(followMax.X > followMin.X)
				followMax.X = followMin.X;
			if(followMax.Y > followMin.Y)
				followMax.Y = followMin.Y;
		}
		
		static public double computeVelocity(double Velocity)
        {
            return computeVelocity(Velocity, 0, 0, 10000);
        }
        
        //@desc		A fairly stupid tween-like function that takes a starting velocity and some other factors and returns an altered velocity
		//@param	Velocity		Any component of velocity (e.g. 20)
		//@param	Acceleration	Rate at which the velocity is changing
		//@param	Drag			Really kind of a deceleration, this is how much the velocity changes if Acceleration is not set
		//@param	Max				An absolute value cap for the velocity
		static public double computeVelocity(double Velocity, double Acceleration, double Drag, double Max)
		{
			if(Acceleration != 0)
				Velocity += Acceleration*FlxG.elapsed;
			else if(Drag != 0)
			{
				double d = Drag*FlxG.elapsed;
				if(Velocity - d > 0)
					Velocity -= d;
				else if(Velocity + d < 0)
					Velocity += d;
				else
					Velocity = 0;
			}
			if((Velocity != 0) && (Max != 10000))
			{
				if(Velocity > Max)
					Velocity = Max;
				else if(Velocity < -Max)
					Velocity = -Max;
			}
			return Velocity;
		}

        static public double computeDistance(double Velocity, double Acceleration, double Drag = 0)
        {
	        double acc = 0;
	        if (Acceleration != 0)
		        acc = Acceleration;
	        else if(Drag != 0)
	        {
		        if(Velocity - Drag > 0)
			        acc = -Drag;
		        else if(Velocity + Drag < 0)
			        acc = Drag;
		        else
			        acc = -Velocity;
	        }
	        return Velocity * FlxG.elapsed + (acc * FlxG.elapsed * FlxG.elapsed) / 2;
        }

        static public void overlapArray<T>(FlxArray<T> Array, FlxCore Core)
            where T : FlxCore
        {
            overlapArray(Array, Core, null);
        }

		static public void collideArrayX<T>(FlxArray<T> Array, FlxSprite Core)
            where T: FlxCore
		{
			if((Core == null) || !Core.exists || Core.dead) return;
			FlxCore core;
			for(int i = 0; i < Array.length; i++)
			{
				core = Array[i];
				if((core == Core) || (core == null) || !core.exists || core.dead) continue;
				core.collideX(Core);
			}
		}
 
		//@desc		Collides a FlxSprite against the FlxCores in the array on the Y axis ONLY
		//@param	Array		An array of FlxCore objects
		//@param	Sprite		A FlxSprite object
		static public void collideArrayY<T>(FlxArray<T> Array, FlxSprite Core)
            where T: FlxCore
		{
			if((Core == null) || !Core.exists || Core.dead) return;
			FlxCore core;
			for(int i = 0; i < Array.length; i++)
			{
				core = Array[i];
				if((core == Core) || (core == null) || !core.exists || core.dead) continue;
				core.collideY(Core);
			}
		}
        
        //@desc		Checks to see if a FlxCore overlaps any of the FlxCores in the array, and calls a function when they do
		//@param	Array		An array of FlxCore objects
		//@param	Core		A FlxCore object
		//@param	Collide		A function that takes two sprites as parameters (first the one from Array, then Sprite)
		static public void overlapArray<T>(FlxArray<T> Array, FlxCore Core, CollisionFunction Collide)
            where T: FlxCore
		{
			if((Core == null) || !Core.exists || Core.dead) return;
			FlxCore c = null;
			for(int i = 0; i < Array.Count; i++)
			{
				c = Array[i];
				if((c == Core) || (c == null) || !c.exists || c.dead) continue;
				if(c.overlaps(Core))
				{
					if(Collide != null)
						Collide(c,Core);
					else
					{
						c.kill();
						Core.kill();
					}
				}
			}
		}

        static public void overlapArrays<T, U>(FlxArray<T> Array1, FlxArray<U> Array2)
            where T: FlxCore 
            where U: FlxCore
        {
            overlapArrays(Array1, Array2, null);
        }
        
        //@desc		Checks to see if any FlxCore in Array1 overlaps any FlxCore in Array2, and calls Collide when they do
		//@param	Array1		An array of FlxCore objects
		//@param	Array2		Another array of FlxCore objects
		//@param	Collide		A function that takes two FlxCore objects as parameters (first the one from Array1, then the one from Array2)
		static public void overlapArrays<T, U> (FlxArray<T> Array1, FlxArray<U> Array2, CollisionFunction Collide) 
            where T: FlxCore 
            where U: FlxCore
		{
			int i;
			int j;
			FlxCore core1 = null;
			FlxCore core2 = null;
			
			for(i = 0; i < Array1.Count; i++)
			{
				core1 = Array1[i];
				if((core1 == null) || !core1.exists || core1.dead) continue;
				for(j = 0; j < Array2.Count; j++)
				{
					core2 = Array2[j];
					if((core1 == core2) || (core2 == null) || !core2.exists || core2.dead) continue;
					if(core1.overlaps(core2))
					{
						if(Collide != null)
							Collide(core1,core2);
						else
						{
							core1.kill();
							core2.kill();
						}
					}
				}
			}
		}
		
		//@desc		Collides a FlxSprite against the FlxCores in the array 
		//@param	Array		An array of FlxCore objects
		//@param	Sprite		A FlxSprite object
        static public void collideArray<T>(FlxArray<T> Cores, FlxSprite Sprite)
            where T: FlxCore
		{
			if((Sprite == null) || !Sprite.exists || Sprite.dead) return;
			FlxCore core = null;
			for(int i = 0; i < Cores.Count; i++)
			{
				core = Cores[i];
				if((core == Sprite) || (core == null) || !core.exists || core.dead) continue;
				core.collide(Sprite);
			}
		}
		
		//@desc		Collides an array of FlxSprites against a FlxCore object
		//@param	Sprites		An array of FlxSprites
		//@param	Core		A FlxCore object
        static public void collideArray2<T>(FlxCore Core, FlxArray<T> Sprites)
            where T: FlxSprite
		{
			if((Core == null) || !Core.exists || Core.dead) return;
			FlxSprite sprite = null;
			for(int i = 0; i < Sprites.Count; i++)
			{
				sprite = Sprites[i];
				if((Core == sprite) || (sprite == null) || !sprite.exists || sprite.dead) continue;
				Core.collide(sprite);
			}
		}
		
		//@desc		Collides the array of FlxSprites against the array of FlxCores
		//@param	Cores		An array of FlxCore objects
		//@param	Sprites		An array of  objects
        static public void collideArrays<T, U>(FlxArray<T> Cores, FlxArray<U> Sprites)
            where T: FlxCore
            where U: FlxSprite
		{
			int i;
			int j;
			FlxCore core = null;
			FlxSprite sprite = null;

			for(i = 0; i < Cores.Count; i++)
			{
				core = Cores[i];
				if((core == null) || !core.exists || core.dead) continue;
				for(j = 0; j < Sprites.Count; j++)
				{
					sprite = Sprites[j];
					if((core == sprite) || (sprite == null) || !sprite.exists || sprite.dead) continue;
					core.collide(sprite);
				}
			}
		}
		
		//@desc		Switch from one FlxState to another
		//@param	State		The class name of the state you want (e.g. PlayState)
		static public void switchState(Type State)
		{ 
			_game._panel.hide();
			FlxG.unfollow();
			FlxG.keys.reset();
			FlxG.mouse.reset();
			_game._quake.reset(0);
			//_game._buffer.x = 0;
			//_game._buffer.y = 0;

			//if(_game._cursor != null)
			//{
			//	_game._buffer.removeChild(_game._cursor);
			//	_game._cursor = null;
			//}

			FlxState newState = (FlxState) Activator.CreateInstance(State);
			//_game._buffer.addChild(newState);
			if(_game._curState != null)
			{
				//_game._buffer.swapChildren(newState,_game._curState);
				//_game._buffer.removeChild(_game._curState);
				_game._curState.destroy();
			}
			_game._fade.visible = false;
			_game._curState = newState;
		}
		
		//@desc		Log data to the developer console
		//@param	Data		The data (in string format) that you wanted to write to the console
		static public void log(string Data)
		{
			//_game._console.log(Data);
		}
		
		static public void quake(double Intensity)
        {
            quake(Intensity, 0.5);
        }
        
        //@desc		Shake the screen
		//@param	Intensity	Percentage of screen size representing the maximum distance that the screen can move during the 'quake'
		//@param	Duration	The length in seconds that the "quake" should last
		static public void quake(double Intensity, double Duration)
		{
			//_game._quake.reset(Intensity,Duration);
		}
		
		static public void flash(uint Color)
        {
            flash(Color, 1, null, false);
        }
        
        //@desc		Temporarily fill the screen with a certain color, then fade it out
		//@param	Color			The color you want to use
		//@param	Duration		How long it takes for the flash to fade
		//@param	FlashComplete	A function you want to run when the flash finishes
		//@param	Force			Force the effect to reset
		static public void flash(uint Color, double Duration, FlashCompleteFunction FlashComplete, bool Force)
		{
            _game._flash.restart(Color, Duration, FlashComplete, Force);
		}
		
		static public void fade(uint Color)
        {
            fade(Color, 1, null, false);
        }
        
        //@desc		Fade the screen out to this color
		//@param	Color			The color you want to use
		//@param	Duration		How long it should take to fade the screen out
		//@param	FadeComplete	A function you want to run when the fade finishes
		//@param	Force			Force the effect to reset
		static public void fade(uint Color, double Duration, FadeCompleteFunction FadeComplete, bool Force)
		{
            _game._fade.restart(Color, Duration, FadeComplete, Force);
		}
		
		static public void showCursor()
        {
            showCursor(null);
        }
        
        //@desc		Set the mouse cursor to some graphic file
		//@param	CursorGraphic	The image you want to use for the cursor
		static public void showCursor(string CursorGraphic)
		{
			/*if(CursorGraphic == null)
				_game._cursor = _game._buffer.addChild(new ImgDefaultCursor) as Bitmap;
			else
				_game._cursor = _game._buffer.addChild(new CursorGraphic) as Bitmap;*/
		}
		
		//@desc		Hides the mouse cursor
		static public void hideCursor()
		{
			/*if(_game._cursor == null) return;
			_game._buffer.removeChild(_game._cursor);
			_game._cursor = null;*/
		}
		
		//@desc		Switch to a different web page
		static public void openURL(string URL)
		{
			System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(URL));
		}

        static public void showSupportPanel()
        {
            showSupportPanel(true);
        }
		
        //@desc		Tell the support panel to slide onto the screen
		//@param	Top		Whether to slide on from the top or the bottom
		static public void showSupportPanel(bool Top)
		{
			_game._panel.show(Top);
		}
		
		//@desc		Conceals the support panel
		static public void hideSupportPanel()
		{
			_game._panel.hide();
		}

		//@desc		This function is only used by the FlxGame class to do important internal management stuff
		static internal void setGameData(FlxGame Game, int Width, int Height)
		{
			_game = Game;
			_cache  = new Dictionary<string,WriteableBitmap>();
			width = Width;
			height = Height;
			_muted = 1;
			_volume = 1.0;
			_musicVolume = 1.0;
			_masterVolume = 0.5;
			_musicPosition = new TimeSpan();
			mouse = new FlxMouse();
			keys = new FlxKeyboard();
			unfollow();
			//FlxG.levels = new FlxArray();
			FlxG.scores = new FlxArray<string>();
			level = 0;
			score = 0;
			//kong = null;
		}
		
		//@desc		This function is only used by the FlxGame class to do important internal management stuff
		static internal void setMasterVolume(double Volume) { _masterVolume = Volume; adjustMusicVolume(); }
		
		//@desc		This function is only used by the FlxGame class to do important internal management stuff
		static internal double getMasterVolume() { return _masterVolume; }
		
		//@desc		This function is only used by the FlxGame class to do important internal management stuff
		static internal void doFollow()
		{
			if(followTarget != null)
			{
				if(followTarget.exists && !followTarget.dead)
				{
					_scrollTarget.X = (width>>1)-followTarget.x-(followTarget.width>>1);
					_scrollTarget.Y = (height>>1)-followTarget.y-(followTarget.height>>1);
					if((followLead != null) && (followTarget is FlxSprite))
					{
						_scrollTarget.X -= (followTarget as FlxSprite).velocity.X*followLead.X;
						_scrollTarget.Y -= (followTarget as FlxSprite).velocity.Y*followLead.Y;
					}
				}
				scroll.X += (_scrollTarget.X-scroll.X)*followLerp*FlxG.elapsed;
				scroll.Y += (_scrollTarget.Y-scroll.Y)*followLerp*FlxG.elapsed;
				
				if(followMin != null)
				{
					if(scroll.X > followMin.X)
						scroll.X = followMin.X;
					if(scroll.Y > followMin.Y)
						scroll.Y = followMin.Y;
				}
				
				if(followMax != null)
				{
					if(scroll.X < followMax.X)
						scroll.X = followMax.X;
					if(scroll.Y < followMax.Y)
						scroll.Y = followMax.Y;
				}
			}
		}
		
		//@desc		This function is only used by the FlxGame class to do important internal management stuff
		static internal void unfollow()
		{
			followTarget = null;
			followLead = new Point();
			followLerp = 1;
			followMin = new Point();
			followMax = new Point();
			scroll = new Point();
			_scrollTarget = new Point();
		}
		
		//@desc		This function is only used by the FlxGame class to do important internal management stuff
		static internal void updateInput()
		{
			keys.update();			
		}

        static public void updateMouse(int x, int y)
        {
            mouse.update(x / _zoom, y / _zoom);
        }
	}
}
