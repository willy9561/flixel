using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace org.flixel
{
	//@desc		This is a traditional tilemap display and collision class
	public class FlxTilemap : FlxCore
	{
		private int widthInTiles;
		private int heightInTiles;
		private WriteableBitmap _pixels;
		private FlxArray<int> _data;
		private FlxArray<IntRect> _rects;
		private int _tileSize;
		private Point _p;
		private FlxBlock _block;
		private int _ci;
		
		private int _screenRows;
		private int _screenCols;
		
		public FlxTilemap(string MapData, string TileGraphic)
        {
            constructor(MapData, TileGraphic, 1, 1);
        }
        
        //@desc		Constructor
		//@param	MapData			A string of comma and line-return delineated indices indicating what order the tiles should go in
		//@param	TileGraphic		All the tiles you want to use, arranged in a strip corresponding to the numbers in MapData
		//@param	CollisionIndex	The index of the first tile that should be treated as a hard surface
		//@param	DrawIndex		The index of the first tile that should actually be drawn
		public FlxTilemap(string MapData, string TileGraphic, int CollisionIndex, int DrawIndex)
		{
			constructor(MapData, TileGraphic, CollisionIndex, DrawIndex);
		}

        protected void constructor(string MapData, string TileGraphic, int CollisionIndex, int DrawIndex)
        {
			_ci = CollisionIndex;
			widthInTiles = 0;
			heightInTiles = 0;
			_data = new FlxArray<int>();
			int c;
			string[] cols;
			string[] rows = MapData.Split('\n');
			heightInTiles = rows.Length;
			for(int r = 0; r < heightInTiles; r++)
			{
				cols = rows[r].Split(',');
				if(cols.Length <= 1)
				{
					heightInTiles--;
					continue;
				}
				if(widthInTiles == 0)
					widthInTiles = cols.Length;
				for(c = 0; c < widthInTiles; c++)
                {
					int dataValue = 0;
                    try
                    {
                        dataValue = int.Parse(cols[c]);
                    }
                    catch(Exception ex)
                    {

                    }
                    
                    _data.Add(dataValue);
                }
			}

			_pixels = FlxG.addBitmap(TileGraphic);
			_rects = new FlxArray<IntRect>();
			_p = new Point();
			_tileSize = _pixels.PixelHeight;
			width = widthInTiles*_tileSize;
			height = heightInTiles*_tileSize;
			int numTiles = widthInTiles*heightInTiles;
			for(int i = 0; i < numTiles; i++)
			{
				if(_data[i] >= DrawIndex)
                    _rects.Add(new IntRect(_tileSize * _data[i], 0, _tileSize, _tileSize));
				else
                    _rects.Add(default(IntRect));
			}
			
			_block = new FlxBlock(0,0,_tileSize,_tileSize,null);
			
			_screenRows = (int)Math.Ceiling(FlxG.height/_tileSize)+1;
			if(_screenRows > heightInTiles)
				_screenRows = heightInTiles;
			_screenCols = (int)Math.Ceiling(FlxG.width/_tileSize)+1;
			if(_screenCols > widthInTiles)
				_screenCols = widthInTiles;
        }
		
		//@desc		Draws the tilemap
		override public void render()
		{
			//NOTE: While this will only draw the tiles that are actually on screen, it will ALWAYS draw one screen's worth of tiles
			base.render();
			getScreenXY(out _p);
			int tx = (int)Math.Floor(-_p.X/_tileSize);
			int ty = (int)Math.Floor(-_p.Y/_tileSize);
			if(tx < 0) tx = 0;
			if(tx > widthInTiles-_screenCols) tx = widthInTiles-_screenCols;
			if(ty < 0) ty = 0;
			if(ty > heightInTiles-_screenRows) ty = heightInTiles-_screenRows;
			int ri = ty*widthInTiles+tx;
			_p.X += tx*_tileSize;
			_p.Y += ty*_tileSize;
			int opx = (int)_p.X;
			int c;
			int cri;
			for(int r = 0; r < _screenRows; r++)
			{
				cri = ri;
				for(c = 0; c < _screenCols; c++)
				{
                    if (_rects[cri] != null)
                    {
                        //FlxG.buffer.copyPixels(_pixels,_rects[cri],_p,null,null,true);
                        FlxG.buffer.CopyPixels(_pixels, _rects[cri], new IntPoint(_p));
                    }
					cri++;
					_p.X += _tileSize;
				}
				ri += widthInTiles;
				_p.X = opx;
				_p.Y += _tileSize;
			}
		}
		
		//@desc		Collides a FlxSprite against the tilemap
		//@param	Spr		The FlxSprite you want to collide
		public void collide(FlxSprite Spr)
		{
			int ix = (int)Math.Floor((Spr.x - x)/_tileSize);
			int iy = (int)Math.Floor((Spr.y - y)/_tileSize);
			int iw = (int)Math.Ceiling(Spr.width/_tileSize)+1;
			int ih = (int)Math.Ceiling(Spr.height/_tileSize)+1;
			int c;
			for(int r = 0; r < ih; r++)
			{
				if((r < 0) || (r >= heightInTiles)) continue;
				for(c = 0; c < iw; c++)
				{
					if((c < 0) || (c >= widthInTiles)) continue;
                    int index = (iy + r) * widthInTiles + ix + c;
                    if (index < _data.length)
                    {
                        if (_data[(iy + r) * widthInTiles + ix + c] >= _ci)
                        {
                            _block.x = x + (ix + c) * _tileSize;
                            _block.y = y + (iy + r) * _tileSize;
                            _block.collide(Spr);
                        }
                    }
				}
			}
		}
	}
}