using System.Collections.Generic;
using org.flixel;

namespace org.flixel.data
{
	public class FlxPause : FlxLayer
	{
        private const string ImgKeyX = "Flixel;component/data/key_x.png";
        private const string ImgKeyC = "Flixel;component/data/key_c.png";
        private const string ImgKeyMouse = "Flixel;component/data/key_mouse.png";
        private const string ImgKeysArrows = "Flixel;component/data/keys_arrows.png";
        private const string ImgKeyMinus = "Flixel;component/data/key_minus.png";
        private const string ImgKeyPlus = "Flixel;component/data/key_plus.png";
        private const string ImgKey0 = "Flixel;component/data/key_0.png";
        private const string ImgKey1 = "Flixel;component/data/key_1.png";

		public FlxPause(int X, int Y, int Zoom, List<string> Help)
		{
			visible = false;

			int w = 160;
			int h = 100;
            int fontSize = 10;

            x = (X + FlxG.width / 2) - (w / 2);
            y = (Y + FlxG.height / 2) - (h / 2);

            add(new FlxSprite(null, (int)x, (int)y, false, false, w, h, 0xBF000000, false));			

            FlxText text = new FlxText(x, y, w, 20, "GAME PAUSED", 0xFFFFFFFF, null, fontSize, null, 0);
            add(text);			
			
            int spc = 15;
            add(new FlxSprite(ImgKeyX, (int)x + 4, (int)y + 36, false));
            add(new FlxSprite(ImgKeyX, (int)x + 4, (int)y + 36 + spc, false));
            add(new FlxSprite(ImgKeyMouse, (int)x + 4, (int)y + 36 + spc * 2, false));
            add(new FlxSprite(ImgKeysArrows, (int)x + 4, (int)y + 36 + spc * 3, false));
            add(new FlxSprite(ImgKeyMinus, (int)x + 84, (int)y + 36, false));
            add(new FlxSprite(ImgKeyPlus, (int)x + 84, (int)y + 36 + spc, false));
            add(new FlxSprite(ImgKey0, (int)x + 84, (int)y + 36 + spc * 2, false));
            add(new FlxSprite(ImgKey1, (int)x + 84, (int)y + 36 + spc * 3, false));
            
            string helpText = string.Empty;
            for(int i = 0; i < Help.Count; i++)
            {
                if(i == Help.Count - 1)
					helpText += "          ";
				if(Help[i] != null) 
                    helpText += Help[i];
				helpText += "\n";

            }
            FlxText text2 = new FlxText(x + 15, y + 35, w / 2, h - 20, helpText, 0xFFFFFFFF, null, fontSize, null, 0);
            add(text2);

            FlxText text3 = new FlxText(x + 95, y + 35, w / 2, h - 20, "Sound Down\nSound Up\nMute\nConsole", 0xFFFFFFFF, null, fontSize, null, 0);            
            add(text3);		
		}

        public override void render()
        {
            if (this.visible)
                base.render();
        }
		
	}
}