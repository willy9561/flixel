namespace org.flixel.data
{
	//@desc		This automates the color-rotation effect on the 'f' logo during game launch, not used in actual game code
	public class FlxLogoPixel : FlxSprite
	{
		private int _curColor;
        private double countdown = 1;
        private uint[] colors;
        private int[] colorsIndex;
        private uint finalColor;
		
		public FlxLogoPixel(int xPos, int yPos, int pixelSize, int index, uint finalColor) :
            base(null, xPos, yPos, false, false, pixelSize, pixelSize, 0xFFFFFFFF, false)
		{
			//Build up the color layers
            colors = new uint[] { 0xFFFF0000, 0xFF00FF00, 0xFF0000FF, 0xFFFFFF00, 0xFF00FFFF };
            colorsIndex = new int[colors.Length];
            for (int i = 0; i < colors.Length; ++i)
            {
                colorsIndex[i] = index;
                if (++index >= colors.Length) index = 0;
            }
            _curColor = colors.Length - 1;
            this.finalColor = finalColor;
		}
		
		public override void update()
		{
            base.update();

            uint nextColor = _curColor == 0 ? finalColor : colors[colorsIndex[_curColor - 1]];
            uint thisColor = colors[colorsIndex[_curColor]];
            _pixels.SetColor(
                FlxExtensions.FromARGB(
                    (byte)(thisColor.GetAlpha() * countdown + nextColor.GetAlpha() * (1 - countdown)),
                    (byte)(thisColor.GetRed() * countdown + nextColor.GetRed() * (1 - countdown)),
                    (byte)(thisColor.GetGreen() * countdown + nextColor.GetGreen() * (1 - countdown)),
                    (byte)(thisColor.GetBlue() * countdown + nextColor.GetBlue() * (1 - countdown))
                )
            );
 
            if (countdown >= 0.1)
            {
                countdown -= 0.1;
            }
            else if (_curColor != 0)
            {
                
                _curColor--;
                countdown = 1;                
            }            
		}
    }
}