using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace org.flixel
{	
	//@desc		A basic text display class, can do some fun stuff though like flicker and rotate
	public class FlxText : FlxCore
	{
		public double angle;
		private TextBlock _tf;
		private double _ox;
		private double _oy;
		private double _oa;
		
		public FlxText(double X, double Y, int Width, int Height)
        {
            constructor(X, Y, Width, Height, string.Empty, 0xFFFFFFFF, null, 15, null, 0);
        }
        
        //@desc		Constructor
		//@param	X		The X position of the text
		//@param	Y		The Y position of the text
		//@param	Width	The width of the text object
		//@param	Height	The height of the text object (eventually these may be unnecessary by leveraging text metrics, but I couldn't get it together for this release)
		//@param	Text	The actual text you would like to display initially
		//@param	Color	The color of the text object
		//@param	Font	The name of the font you'd like to use (pass null to use the built-in pixel font)
		//@param	Size	The size of the font (recommend using multiples of 8 for cleanest rendering)
		//@param	Justification	Valid strings include "left", "center", and "right"
		//@param	Angle	How much the text should be rotated
		public FlxText(double X, double Y, int Width, int Height, string Text, uint Color, string Font, int Size, string Justification, double Angle)
		{
			constructor(X, Y, Width, Height, Text, Color, Font, Size, Justification, Angle);
		}

        protected void constructor(double X, double Y, int Width, int Height, string Text, uint Color, string Font, int Size, string Justification, double Angle)
        {
            _ox = x = X;
			_oy = y = Y;
			_oa = angle = Angle;
			width = Width;
			height = Height;
			
			//if(Font == null)
				//Font = "Arial";
			if(Text == null)
				Text = "";
        	_tf = new TextBlock();
			//_tf.Width = width;
			//_tf.Height = height;
            _tf.Text = Text;
            //_tf.FontFamily = new FontFamily(Font);
            _tf.FontSize = Size;

            //if (Justification == "center")
            //{
            //    _tf.TextAlignment = TextAlignment.Center;
            //}
            _tf.Foreground = new SolidColorBrush(Color.ToColor());
        }
		
		//@desc		Changes the text being displayed
		//@param	Text	The new string you want to display
		public void setText(string Text)
		{
			_tf.Text = Text;            
		}
		
		//@desc		Changes the color being used by the text
		//@param	Color	The new color you want to use
		public void setColor(uint Color)
		{
            _tf.Foreground = new SolidColorBrush(Color.ToColor());
		}
		
		//@desc		Called by the game loop automatically, blits the text object to the screen
		override public void render()
		{		    	
            TransformGroup tg = new TransformGroup();
            
            TranslateTransform tt = new TranslateTransform();
            tt.X = x;
            tt.Y = y;
            tg.Children.Add(tt);

            RotateTransform rt = new RotateTransform();
            rt.Angle = Math.PI * 2 * (angle / 360);
            rt.CenterX = width >> 1;
            rt.CenterY = height >> 1;
            tg.Children.Add(rt);

            WriteableBitmap wb = new WriteableBitmap((int)_tf.ActualWidth, (int)_tf.ActualHeight);
            wb.Render(_tf, new TranslateTransform());
            wb.Invalidate();

            FlxG.buffer.CopyPixels(wb, wb.GetRect(), new IntPoint(x, y));
		}
    }
}