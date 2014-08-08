using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace org.flixel.data
{
	//@desc		Contains all the logic for the developer console
	public class FlxConsole
	{
		protected const int MAX_CONSOLE_LINES = 256;
		protected TextBox _text;
		protected TextBox _fpsDisplay;
		protected double[] _fps;
		protected int _curFPS;
		protected List<string> _lines;
		protected double _Y;
		protected double _YT;
		protected bool _fpsUpdate;
		protected int _bx;
		protected int _by;
		protected int _byt;
		
		public FlxConsole(int X,int Y, int Zoom)
		{
			/*visible = false;
			x = X*Zoom;
			_by = Y*Zoom;
			_byt = _by - FlxG.height*Zoom;
			_YT = _Y = y = _byt;
			var tmp:Bitmap = new Bitmap(new BitmapData(FlxG.width*Zoom,FlxG.height*Zoom,true,0x7F000000));
			addChild(tmp);
			
			_fps = new double[8];
			_curFPS = 0;
			_fpsUpdate = true;

			_text = new TextBox();
			_text.width = tmp.width;
			_text.height = tmp.height;
			_text.multiline = true;
			_text.wordWrap = true;
			_text.embedFonts = true;
			_text.antiAliasType = AntiAliasType.NORMAL;
			_text.gridFitType = GridFitType.PIXEL;
			_text.defaultTextFormat = new TextFormat("system",8,0xffffff);
			addChild(_text);

			_fpsDisplay = new TextField();
			_fpsDisplay.width = tmp.width;
			_fpsDisplay.height = 20;
			_fpsDisplay.multiline = true;
			_fpsDisplay.wordWrap = true;
			_fpsDisplay.embedFonts = true;
			_fpsDisplay.antiAliasType = AntiAliasType.NORMAL;
			_fpsDisplay.gridFitType = GridFitType.PIXEL;
			_fpsDisplay.defaultTextFormat = new TextFormat("system",16,0xffffff,true,null,null,null,null,"right");
			addChild(_fpsDisplay);
			
			_lines = new List<string>();*/
		}
		
		//@desc		Log data to the developer console
		//@param	Data		The data (in string format) that you wanted to write to the console
		public void log(string Data)
		{
			/*if(Data == null)
				Data = "NULL";
			//trace(Data);
			_lines.Add(Data);
			if(_lines.Count > MAX_CONSOLE_LINES)
			{
				_lines.RemoveAt(_lines.Count - 1);
				string newText = "";
				for(int i = 0; i < _lines.Count; i++)
					newText += _lines[i]+"\n";
				_text.Text = newText;
			}
			else
				_text.Text = Data + "\n" + _text.Text;*/
		}
		
		//@desc		Shows/hides the console
		public void toggle()
		{
			/*if(_YT == _by)
				_YT = _byt;
			else
			{
				_YT = _by;
				visible = true;
			}*/
		}
		
		//@desc		Updates and/or animates the dev console
		public void update()
		{
			/*if(visible)
			{
				_fps[_curFPS] = 1/FlxG.elapsed;
				if(++_curFPS >= _fps.length) _curFPS = 0;
				_fpsUpdate = !_fpsUpdate;
				if(_fpsUpdate)
				{
					var fps:uint = 0;
					for(var i:uint = 0; i < _fps.length; i++)
						fps += _fps[i];
					_fpsDisplay.text = Math.floor(fps/_fps.length)+" fps";
				}
			}
			if(_Y < _YT)
				_Y += FlxG.height*10*FlxG.elapsed;
			else if(_Y > _YT)
				_Y -= FlxG.height*10*FlxG.elapsed;
			if(_Y > _by)
				_Y = _by;
			else if(_Y < _byt)
			{
				_Y = _byt;
				visible = false;
			}
			y = Math.floor(_Y);*/
		}
	}
}