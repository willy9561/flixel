using System.Windows.Browser;

namespace org.flixel.data
{
	public class FlxPanel : FlxCore
	{
        private const string ImgDonate = "Flixel;component/data/donate.png";
        private const string ImgStumble = "Flixel;component/data/stumble.png";
        private const string ImgDigg = "Flixel;component/data/digg.png";
        private const string ImgReddit = "Flixel;component/data/reddit.png";
        private const string ImgDelicious = "Flixel;component/data/delicious.png";
        private const string ImgTwitter = "Flixel;component/data/twitter.png";
        private const string ImgClose = "Flixel;component/data/close.png";

		private FlxSprite _topBar;
		private FlxSprite _mainBar;
		private FlxSprite _bottomBar;
		private FlxButton _donate;
		private FlxButton _stumble;
		private FlxButton _digg;
		private FlxButton _reddit;
		private FlxButton _delicious;
		private FlxButton _twitter;
		private FlxButton _close;
		private FlxText _caption;
		
		private string _payPalID;
		private double _payPalAmount;
		private string _gameTitle;
		private string _gameURL;
		
		private bool _initialized;
		private bool _closed;
		
		private double _ty;
		private double _s;
		
		public FlxPanel()
		{
			y = -21;
			_ty = y;
			_closed = false;
			_initialized = false;
			_topBar = new FlxSprite(null,0,0,false,false,FlxG.width,1,0x7fffffff,false);
			_topBar.scrollFactor.X = 0;
			_topBar.scrollFactor.Y = 0;
			_mainBar = new FlxSprite(null,0,0,false,false,FlxG.width,19,0x7f000000,false);
			_mainBar.scrollFactor.X = 0;
			_mainBar.scrollFactor.Y = 0;
			_bottomBar = new FlxSprite(null,0,0,false,false,FlxG.width,1,0x7fffffff,false);
			_bottomBar.scrollFactor.X = 0;
			_bottomBar.scrollFactor.Y = 0;
			_donate = new FlxButton(3,0,new FlxSprite(ImgDonate),onDonate);
			_donate.scrollFactor.X = 0;
			_donate.scrollFactor.Y = 0;
			_stumble = new FlxButton(FlxG.width/2-6-13-6-13-6,0,new FlxSprite(ImgStumble),onStumble);
			_stumble.scrollFactor.X = 0;
			_stumble.scrollFactor.Y = 0;
			_digg = new FlxButton(FlxG.width/2-6-13-6,0,new FlxSprite(ImgDigg),onDigg);
			_digg.scrollFactor.X = 0;
			_digg.scrollFactor.Y = 0;
			_reddit = new FlxButton(FlxG.width/2-6,0,new FlxSprite(ImgReddit),onReddit);
			_reddit.scrollFactor.X = 0;
			_reddit.scrollFactor.Y = 0;
			_delicious = new FlxButton(FlxG.width/2+7+6,0,new FlxSprite(ImgDelicious),onDelicious);
			_delicious.scrollFactor.X = 0;
			_delicious.scrollFactor.Y = 0;
			_twitter = new FlxButton(FlxG.width/2+7+6+12+6,0,new FlxSprite(ImgTwitter),onTwitter);
			_twitter.scrollFactor.X = 0;
			_twitter.scrollFactor.Y = 0;
			_caption = new FlxText(FlxG.width/2,0,FlxG.width/2-19,20,"",0xffffff,null,8,"right",0);
			_caption.scrollFactor.X = 0;
			_caption.scrollFactor.Y = 0;
			_close = new FlxButton(FlxG.width-16,0,new FlxSprite(ImgClose),onClose);
			_close.scrollFactor.X = 0;
			_close.scrollFactor.Y = 0;
			_s = 50;
		}
		
		public void init(string PayPalID, double PayPalAmount, string GameTitle, string GameURL, string Caption)
		{
			_payPalID = PayPalID;
			if(_payPalID.Length <= 0) _donate.visible = false;
			_payPalAmount = PayPalAmount;
			_gameTitle = GameTitle;
			_gameURL = GameURL;
			_caption.setText(Caption);
			_initialized = true;
		}
		
		override public void update()
		{
			if(!_initialized) return;
			if(_ty != y)
			{
				if(y < _ty)
				{
					y += FlxG.elapsed*_s;
					if(y > _ty) y = _ty;
				}
				else
				{
					y -= FlxG.elapsed*_s;
					if(y < _ty) y = _ty;
				}
			}
			if((y <= -21) || (y > FlxG.height)) visible = false;
			_topBar.y = y;
			_mainBar.y = y+1;
			_bottomBar.y = y+20;
			_donate.y = y+4;
			_stumble.y = y+4;
			_digg.y = y+4;
			_reddit.y = y+4;
			_delicious.y = y+5;
			_twitter.y = y+4;
			_caption.y = y+4;
			_close.y = y+4;
			if(_donate.active) _donate.update();
			if(_stumble.active) _stumble.update();
			if(_digg.active) _digg.update();
			if(_reddit.active) _reddit.update();
			if(_delicious.active) _delicious.update();
			if(_twitter.active) _twitter.update();
			if(_caption.active) _caption.update();
			if(_close.active) _close.update();
		}
		
		override public void render()
		{
			if(!_initialized) return;
			if(_topBar.visible) _topBar.render();
			if(_mainBar.visible) _mainBar.render();
			if(_bottomBar.visible) _bottomBar.render();
			if(_donate.visible) _donate.render();
			if(_stumble.visible) _stumble.render();
			if(_digg.visible) _digg.render();
			if(_reddit.visible) _reddit.render();
			if(_delicious.visible) _delicious.render();
			if(_twitter.visible) _twitter.render();
			if(_caption.visible) _caption.render();
			if(_close.visible) _close.render();
		}
		
		public void show()
        {
            show(true);
        }
        
        public void show(bool Top)
		{
			if(_closed) return;
			if(Top)
			{
				y = -21;
				_ty = -1;
			}
			else
			{
				y = FlxG.height;
				_ty = FlxG.height-20;
			}
			//Mouse.show();
			visible = true;
		}
		
		public void hide()
		{
			if(y < 0) _ty = -21;
			else _ty = FlxG.height;
			//Mouse.hide();
			visible = false;
		}
		
		public void onDonate()
		{
            FlxG.openURL("https://www.paypal.com/cgi-bin/webscr?cmd=_xclick&business=" + HttpUtility.HtmlEncode(_payPalID) + "&item_name=" + HttpUtility.HtmlEncode(_gameTitle + " Contribution (" + _gameURL) + ")&currency_code=USD&amount=" + _payPalAmount);
		}
		
		public void onStumble()
		{
            FlxG.openURL("http://www.stumbleupon.com/submit?url=" + HttpUtility.HtmlEncode(_gameURL));
		}
		
		public void onDigg()
		{
            FlxG.openURL("http://digg.com/submit?url=" + HttpUtility.HtmlEncode(_gameURL) + "&title=" + HttpUtility.HtmlEncode(_gameTitle));
		}
		
		public void onReddit()
		{
            FlxG.openURL("http://www.reddit.com/submit?url=" + HttpUtility.HtmlEncode(_gameURL));
		}
		
		public void onDelicious()
		{
            FlxG.openURL("http://delicious.com/save?v=5&amp;noui&amp;jump=close&amp;url=" + HttpUtility.HtmlEncode(_gameURL) + "&amp;title=" + HttpUtility.HtmlEncode(_gameTitle));
		}
		
		public void onTwitter()
		{
            FlxG.openURL("http://twitter.com/home?status=Playing" + HttpUtility.HtmlEncode(" " + _gameTitle + " - " + _gameURL));
		}
		
		public void onClose()
		{
			_closed = true;
			hide();
		}
	}
}
