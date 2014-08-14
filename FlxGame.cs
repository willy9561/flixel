using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media.Imaging;
using org.flixel.data;

namespace org.flixel
{
	//@desc		FlxGame is the heart of all flixel games, and contains a bunch of basic game loops and things.  It is a long and sloppy file that you shouldn't have to worry about too much!
	public class FlxGame
	{
		/*[Embed(source="data/nokiafc22.ttf",fontFamily="system")] private var junk:String;
		[Embed(source="data/poweredby.png")] private var ImgPoweredBy:Class;
		[Embed(source="data/beep.mp3")] private var SndBeep:Class;
		[Embed(source="data/flixel.mp3")] private var SndFlixel:Class;*/

        internal string SndFlixel = "Flixel;component/data/flixel.mp3";
        internal string ImgPoweredBy = "Flixel;component/data/poweredby.png";

		internal const double MAX_ELAPSED = 0.0333F;
        internal const int  FPS_120 = 8; //milliseconds
        internal const double TICKS_TO_MILLI = 10000; //convert ticks to milliseconds
		
		//startup
		internal Type _iState;
		internal bool _created;
		
		//basic display stuff
		internal WriteableBitmap _bmpBack;
		internal WriteableBitmap _bmpFront;
		internal bool _flipped;		
		internal int _gameXOffset;
		internal int _gameYOffset;
		internal uint _bgColor;
		internal string _frame;
		internal FlxState _curState;
		//internal string _cursor;
		
		//basic update stuff
		internal double _elapsed;
		internal long _total;
		internal bool _paused;
		
		//Pause screen, sound tray, support panel, dev console, and special effects objects
		internal FlxPause _pausePopup;
		internal List<string> _helpStrings;
		//internal var _soundTray:Sprite;
		//internal double _soundTrayTimer;
		//internal var _soundTrayBars:Array;
		internal FlxPanel _panel;
		//internal FlxConsole _console;
		internal FlxQuake _quake;
		internal FlxFlash _flash;
		internal FlxFade _fade;
		internal bool _defaultSoundControls;
        //internal int _zoom;
		
		//logo stuff
		internal FlxArray<FlxSprite> _f;
		internal uint _fc;
		internal bool _logoComplete;
		internal double _logoTimer;
		internal string _poweredBy;
		internal string _logoFade;
		internal string _fSound;
		internal bool _showLogo;
		
		//@desc		Constructor
		//@param	GameSizeX		The width of your game in pixels (e.g. 320)
		//@param	GameSizeY		The height of your game in pixels (e.g. 240)
		//@param	InitialState	The class name of the state you want to create and switch to first (e.g. MenuState)
		//@param	Zoom			The level of zoom (e.g. 2 means all pixels are now rendered twice as big)
		//@param	BGColor			The color of the Flash app's background
		//@param	FlixelColor		The color of the great big 'f' in the flixel logo
		//@param	FlixelSound		The sound that is played over the flixel 'f' logo
		//@param	Frame			If you want you can add a little graphical frame to the outside edges of your game
		//@param	ScreenOffsetX	If you use a frame, you're probably going to want to scoot your game down to fit properly inside it
		//@param	ScreenOffsetY	These variables do exactly that :)	
		public FlxGame(int GameSizeX, int GameSizeY, Type InitialState, int Zoom, uint BGColor, bool ShowFlixelLogo, uint FlixelColor, string FlixelSound, string Frame, int ScreenOffsetX, int ScreenOffsetY)
		{
			constructor(GameSizeX, GameSizeY, InitialState, Zoom, BGColor, ShowFlixelLogo, FlixelColor, FlixelSound, Frame, ScreenOffsetX, ScreenOffsetY);
		}

        public FlxGame(int GameSizeX, int GameSizeY, Type InitialState, int Zoom)
        {
            constructor(GameSizeX, GameSizeY, InitialState, Zoom, 0xFF000000, true, 0xFFFFFFFF, null, null, 0, 0);
        }

        public FlxGame(int GameSizeX, int GameSizeY, Type InitialState)
        {
            constructor(GameSizeX, GameSizeY, InitialState, 2, 0xFF000000, true, 0xFFFFFFFF, null, null, 0, 0);
        }

        protected void constructor(int GameSizeX, int GameSizeY, Type InitialState, int Zoom, uint BGColor, bool ShowFlixelLogo, uint FlixelColor, string FlixelSound, string Frame, int ScreenOffsetX, int ScreenOffsetY)
        {
            FlxG.setGameData(this, GameSizeX, GameSizeY);
            FlxG._zoom = Zoom;

            _bmpBack = new WriteableBitmap(FlxG.width, FlxG.height);
            _bmpFront = new WriteableBitmap(FlxG.width * FlxG._zoom, FlxG.width * FlxG._zoom);
            FlxG.buffer = _bmpBack;
            FlxG.frontBuffer = _bmpFront;
            _flipped = false;
            _created = true;
            _logoTimer = 0;
                        
			_gameXOffset = ScreenOffsetX;
			_gameYOffset = ScreenOffsetY;
			_bgColor = BGColor;
			_fc = FlixelColor;
			
			_created = false;
            FlxG.OnKeyDown += new FlxG.KeyEvent(onKeyDown);
            FlxG.OnKeyUp += new FlxG.KeyEvent(onKeyUp);
            FlxG.OnEnterFrame += new FlxG.EnterFrame(onEnterFrame);
            FlxG.OnFocusLost += new FlxG.FocusChangedFunction(onFocusLost);
            FlxG.OnFocus += new FlxG.FocusChangedFunction(onFocus);
            FlxG.OnMouseDown += new FlxG.MouseEvent(onMouseDown);
            FlxG.OnMouseUp += new FlxG.MouseEvent(onMouseUp);
			_elapsed = 0;
			_total = 0;
			//flash.ui.Mouse.hide();
			_logoComplete = false;
			_f = null;
            _quake = new FlxQuake(FlxG._zoom);
			_flash = new FlxFlash();
			_fade = new FlxFade();
			if(FlixelSound == null)
                _fSound = SndFlixel;
			else
				_fSound = FlixelSound;
			_curState = null;
			_frame = Frame;
			_iState = InitialState;
			_paused = false;
			_helpStrings = new List<string>();
			_helpStrings.Add("Button 1");
			_helpStrings.Add("Button 2");
			_helpStrings.Add("Mouse");
			_helpStrings.Add("Move");
			_showLogo = ShowFlixelLogo;
			_panel = new FlxPanel();
            _pausePopup = new FlxPause(_gameXOffset, _gameYOffset, FlxG._zoom, _helpStrings);
        }
		
		protected void help()
        {
            help(null, null, null, null);
        }

        protected void help(string X, string C, string Mouse)
        {
            help(X, C, Mouse, null);
        }
        
        //@desc		Sets up the strings that are displayed on the left side of the pause game popup
		//@param	X		What to display next to the X button
		//@param	C		What to display next to the C button
		//@param	Mouse	What to display next to the mouse icon
		//@param	Arrows	What to display next to the arrows icon
		protected void help(string X, string C, string Mouse, string Arrows)
		{
			if(X != null)
				_helpStrings[0] = X;
			if(C != null)
				_helpStrings[1] = C;
			if(Mouse != null)
				_helpStrings[2] = Mouse;
			if(Arrows != null)
				_helpStrings[3] = Arrows;
		}
		
		protected void useDefaultVolumeControls(bool YesPlz)
		{
			_defaultSoundControls = YesPlz;
		}
		
		//@desc		This function is only used by the FlxGame class to do important internal management stuff
		private void onKeyUp(KeyboardEvent keyboardEvent)
		{
			if(keyboardEvent.keyCode == 192)
			{
				//_console.toggle();
				return;
			}
			if(_defaultSoundControls)
			{
				int c = keyboardEvent.keyCode;
				switch(c)
				{
					case 48:
						FlxG.setMute(!FlxG.getMute());
			    		showSoundTray();
						return;
					case 189:
						FlxG.setMute(false);
			    		FlxG.setMasterVolume(FlxG.getMasterVolume() - 0.1);
			    		showSoundTray();
						return;
					case 187:
						FlxG.setMute(false);
			    		FlxG.setMasterVolume(FlxG.getMasterVolume() + 0.1);
			    		showSoundTray();
						return;
					default: break;
				}
			}
			FlxG.keys.handleKeyUp(keyboardEvent);
		}
		
		//@desc		This function is only used by the FlxGame class to do important internal management stuff
		private void onKeyDown(KeyboardEvent keyboardEvent)
		{
			FlxG.keys.handleKeyDown(keyboardEvent);
		}
		
		//@desc		This function is only used by the FlxGame class to do important internal management stuff
		private void onMouseUp()
		{
			FlxG.mouse.handleMouseUp();
		}
		
		//@desc		This function is only used by the FlxGame class to do important internal management stuff
		private void onMouseDown()
		{
			FlxG.mouse.handleMouseDown();
		}
		
		//@desc		This function is only used by the FlxGame class to do important internal management stuff
		private void onFocus()
		{
			//if(!_panel.visible) flash.ui.Mouse.hide();
			_pausePopup.visible = false;
			FlxG.resetInput();
			_paused = false;
			FlxG.playMusic();
			//stage.frameRate = 90;
		}
		
		//@desc		This function is only used by the FlxGame class to do important internal management stuff
		private void onFocusLost()
		{
			/*if((x != 0) || (y != 0))
			{
				x = 0;
				y = 0;
			}*/
			//flash.ui.Mouse.show();
			_pausePopup.visible = true;
			_paused = true;
			FlxG.pauseMusic();
			//stage.frameRate = 10;
		}
		
		//@desc		This is the main game loop, but only once creation and logo playback is finished
		private void onEnterFrame()
		{
			int i;
			
			//Frame timing
			long t = DateTime.Now.Ticks;
            _elapsed = (t - _total) / TICKS_TO_MILLI; //change to milliseconds
            double realDelta = _elapsed;
			_total = t;
            if (_elapsed > FPS_120)
                _elapsed = MAX_ELAPSED; //don't know what the game is calibrated to
            FlxG.elapsed = _elapsed;


            //Clear buffer
            _bmpBack.SetColor((int)_bgColor);
			
			if(_logoComplete)
			{
				//Animate flixel HUD elements
				_panel.update();
                _pausePopup.update();
				//_console.update();

				/*if(_soundTrayTimer > 0)
					_soundTrayTimer -= _elapsed;
				else if(_soundTray.y > -_soundTray.height)
				{
					_soundTray.y -= _elapsed*FlxG.height*2;
					if(_soundTray.y < -_soundTray.height)
						_soundTray.visible = false;
				}*/                
				
				//State updating
				if(!_paused)
				{
					FlxG.updateInput();
					/*if(_cursor != null)
					{
						_cursor.x = FlxG.mouse.x+FlxG.scroll.x;
						_cursor.y = FlxG.mouse.y+FlxG.scroll.y;
					}*/
					FlxG.doFollow();
					_curState.update();
					
					//Update the various special effects
					_flash.update();
					_fade.update();
					_quake.update();
                    
					//_buffer.x = _quake.x;
					//_buffer.y = _quake.y;					
					
					

                    //if(_flipped)
                    //{
                    //    //for (int j = 0; j < _bmpFront.Pixels.Length; ++j) _bmpFront.Pixels[j] = (int)_bgColor;
						
                    //    _bmpFront.CopyPixels(_bmpBack, _bmpBack.GetRect(), new IntPoint(), FlxG._zoom);
                    //    FlxG.buffer = _bmpFront;
                    //}
                    //else
                    //{
                    //    //for (int j = 0; j < _bmpBack.Pixels.Length; ++j) _bmpBack.Pixels[j] = (int)_bgColor;
						
                    //    _bmpBack.CopyPixels(_bmpFront, _bmpFront.GetRect(), new IntPoint(), FlxG._zoom);
                    //    FlxG.buffer = _bmpBack;
                    //}
					
					//Render game content, special fx, and overlays
					_curState.render();
					_flash.render();
					_fade.render();
					_panel.render();
                    
					
					//Swap buffers
					//_bmpBack.Visible = !(_bmpFront.Visible = _flipped);
					//_flipped = !_flipped;                    
				}

                _pausePopup.render();

                long endTime = DateTime.Now.Ticks;
                realDelta = (endTime - t) / TICKS_TO_MILLI; //convert ticks to milliseconds

                if (realDelta < FPS_120)
                    Thread.Sleep((int)(FPS_120 - realDelta)); // fix rate at 60 fps or so

			}
			else
			{
				if(!_showLogo)
				{
					_logoComplete = true;
					FlxG.switchState(_iState);
				}
				else
				{
					if(_f == null)
					{
                        _f = new FlxArray<FlxSprite>();
						int scale = 1;
						//if(FlxG.height > 200)
						//	scale = 2;
						int pixelSize = 32*scale;
                        int top = (int)(FlxG.height / 2 - pixelSize * 2);
                        int left = (int)(FlxG.width / 2 - pixelSize);
						
						_f.Add(new FlxLogoPixel(left+pixelSize,top,pixelSize,0,_fc));
						_f.Add(new FlxLogoPixel(left,top+pixelSize,pixelSize,1,_fc));
						_f.Add(new FlxLogoPixel(left,top+pixelSize*2,pixelSize,2,_fc));
						_f.Add(new FlxLogoPixel(left+pixelSize,top+pixelSize*2,pixelSize,3,_fc));
						_f.Add(new FlxLogoPixel(left,top+pixelSize*3,pixelSize,4,_fc));

                        _f.Add(new FlxSprite(ImgPoweredBy, left, top + pixelSize * 4 + 16, false));
												
						//_logoFade = addChild(new Bitmap(new BitmapData(FlxG.width*_zoom,FlxG.height*_zoom,true,0xFF000000))) as Bitmap;
						//_logoFade.x = _gameXOffset*_zoom;
						//_logoFade.y = _gameYOffset*_zoom;

                        if (_fSound != null)
                            FlxG.play(_fSound);
					}
					
					_logoTimer += _elapsed;
                    for (i = 0; i < _f.length; i++)
                    {
                        _f[i].update();
                        _f[i].render();
                    }
					//if(_logoFade.alpha > 0)
						//_logoFade.alpha -= _elapsed*0.5;
						
					if(_logoTimer > 4)
					{
						_f.clear();
						//removeChild(_logoFade);
						FlxG.switchState(_iState);
						_logoComplete = true;
					}
				}
			}

            _bmpFront.CopyPixels(_bmpBack, _bmpBack.GetRect(), new IntPoint(), FlxG._zoom);
        }
		
		//@desc		This function is only used by the FlxGame class to do important internal management stuff
		private void showSoundTray()
		{
			/*FlxG.play(SndBeep);
			_soundTrayTimer = 1;
			_soundTray.y = _gameYOffset*_zoom;
			_soundTray.visible = true;
			int gv = (int)Math.Round(FlxG.getMasterVolume()*10);
			if(FlxG.getMute())
				gv = 0;
			for (int i = 0; i < _soundTrayBars.length; i++)
			{
				if(i < gv) _soundTrayBars[i].alpha = 1;
				else _soundTrayBars[i].alpha = 0.5;
			}*/
		}
		
		//@desc		Set up the support panel thingy with donation and aggregation info
		//@param	PayPalID		Your paypal username, usually your email address (leave it blank to disable donations)
		//@param	PayPalAmount	The default amount of the donation
		//@param	GameTitle		The text that you would like to appear in the aggregation services (usually just the name of your game)
		//@param	GameURL			The URL you would like people to use when trying to find your game
		protected void setupSupportPanel(string PayPalID, double PayPalAmount, string GameTitle, string GameURL, string Caption)
		{
			_panel.init(PayPalID,PayPalAmount,GameTitle,GameURL,Caption);
		}
	}
}
