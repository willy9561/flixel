using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace org.flixel
{
	
	//@desc		This is the basic "environment object" class, used to create walls and floors
	public class FlxBlock : FlxCore
	{
		protected WriteableBitmap _pixels;
        protected FlxArray<IntRect> _rects;
        protected int _tileSize;
		protected Point _p;

        public FlxBlock(int X, int Y, int Width, int Height, string TileGraphic)
        {
            constructor(X, Y, Width, Height, TileGraphic, 0);
        }
        
        //@desc		Constructor
		//@param	X			The X position of the block
		//@param	Y			The Y position of the block
		//@param	Width		The width of the block
		//@param	Height		The height of the block
		//@param	TileGraphic The graphic class that contains the tiles that should fill this block
		//@param	Empties		The number of "empty" tiles to add to the auto-fill algorithm (e.g. 8 tiles + 4 empties = 1/3 of block will be open holes)
        public FlxBlock(int X, int Y, int Width, int Height, string TileGraphic, uint Empties)
		{
			constructor(X, Y, Width, Height, TileGraphic, Empties);
		}

        protected void constructor(int X, int Y, int Width, int Height, string TileGraphic, uint Empties)
        {
			x = X;
			y = Y;
			width = Width;
			height = Height;
            isFixed = true;
			if(TileGraphic == null)
				return;

			_pixels = FlxG.addBitmap(TileGraphic);
			_rects = new FlxArray<IntRect>();
			_p = new Point();
			_tileSize = _pixels.PixelHeight;
			int widthInTiles = (int)Math.Ceiling(width/_tileSize);
            int heightInTiles = (int)Math.Ceiling(height / _tileSize);
			width = widthInTiles*_tileSize;
			height = heightInTiles*_tileSize;
            int numTiles = widthInTiles * heightInTiles;
            int numGraphics = _pixels.PixelWidth / _tileSize;
			
            for(uint i = 0; i < numTiles; i++)
			{
                if (FlxG.random() * (numGraphics + Empties) > Empties)
                {
                    double left = _tileSize * Math.Floor(FlxG.random() * numGraphics);
                    double top = _tileSize;
                    double right = left + _tileSize;
                    double bottom = 0;
                    _rects.Add(new IntRect(left, right, top, bottom));
                }
				else
                    _rects.Add(default(IntRect));
			}
        }
		
		//@desc		Draws this block
		override public void render()
		{
            base.render();
			getScreenXY(out _p);
			int opx = (int)_p.X;
			for(int i = 0; i < _rects.Count; i++)
			{
                if (_rects[i] != null)
                {
                    FlxG.buffer.CopyPixels(_pixels, _rects[i], new IntPoint(_p));
                }
				_p.X += _tileSize;
				if(_p.X >= opx + width)
				{
					_p.X = opx;
					_p.Y += _tileSize;
				}
			}
		}
	}
}