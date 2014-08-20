using System.Collections.Generic;
using System.Windows.Input;
using System.Reflection;

namespace org.flixel.data
{
	public class FlxKeyboardData
    {
        public FlxKeyboardData(string name, int current, int last)
        {
            this.name = name;
            this.current = current;
            this.last = last;
        }
        
        public string name;
        public int current;
        public int last;
    }
    
    public class FlxKeyboard
	{
		public bool ESC;
		public bool F1;
		public bool F2;
		public bool F3;
		public bool F4;
		public bool F5;
		public bool F6;
		public bool F7;
		public bool F8;
		public bool F9;
		public bool F10;
		public bool F11;
		public bool F12;
		public bool ONE;
		public bool TWO;
		public bool THREE;
		public bool FOUR;
		public bool FIVE;
		public bool SIX;
		public bool SEVEN;
		public bool EIGHT;
		public bool NINE;
		public bool ZERO;
		public bool MINUS;
		public bool PLUS;
		public bool DELETE;
		public bool Q;
		public bool W;
		public bool E;
		public bool R;
		public bool T;
		public bool Y;
		public bool U;
		public bool I;
		public bool O;
		public bool P;
		public bool LBRACKET;
		public bool RBRACKET;
		public bool BACKSLASH;
		public bool CAPSLOCK;
		public bool A;
		public bool S;
		public bool D;
		public bool F;
		public bool G;
		public bool H;
		public bool J;
		public bool K;
		public bool L;
		public bool SEMICOLON;
		public bool QUOTE;
		public bool ENTER;
		public bool SHIFT;
		public bool Z;
		public bool X;
		public bool C;
		public bool V;
		public bool B;
		public bool N;
		public bool M;
		public bool COMMA;
		public bool PERIOD;
		public bool SLASH;
		public bool CONTROL;
		public bool ALT;
		public bool SPACE;
		public bool UP;
		public bool DOWN;
		public bool LEFT;
		public bool RIGHT;
		
		protected Dictionary<string, int> _lookup;
        protected Dictionary<int, FlxKeyboardData> _map;
        protected Dictionary<string, int> _flxToDotNetKeyMap;
		
		public FlxKeyboard()
		{
			//BASIC STORAGE & TRACKING			
			_lookup = new Dictionary<string, int>();
            _map = new Dictionary<int, FlxKeyboardData>();
            _flxToDotNetKeyMap = new Dictionary<string, int>();         

            _flxToDotNetKeyMap.Add("LBRACKET", 219);
            _flxToDotNetKeyMap.Add("RBRACKET", 221);
            _flxToDotNetKeyMap.Add("BACKSLASH", 220);
            _flxToDotNetKeyMap.Add("COMMA", 188);
            _flxToDotNetKeyMap.Add("PERIOD", 190);
            _flxToDotNetKeyMap.Add("SLASH", 191);
            _flxToDotNetKeyMap.Add("SEMICOLON", 186);
            _flxToDotNetKeyMap.Add("QUOTE", 222);            
            _flxToDotNetKeyMap.Add("ESC", (int)Key.Escape);
            _flxToDotNetKeyMap.Add("F1", (int)Key.F1);
            _flxToDotNetKeyMap.Add("F2", (int)Key.F2);
            _flxToDotNetKeyMap.Add("F3", (int)Key.F3);
            _flxToDotNetKeyMap.Add("F4", (int)Key.F4);
            _flxToDotNetKeyMap.Add("F5", (int)Key.F5);
            _flxToDotNetKeyMap.Add("F6", (int)Key.F6);
            _flxToDotNetKeyMap.Add("F7", (int)Key.F7);
            _flxToDotNetKeyMap.Add("F8", (int)Key.F8);
            _flxToDotNetKeyMap.Add("F9", (int)Key.F9);
            _flxToDotNetKeyMap.Add("F10", (int)Key.F10);
            _flxToDotNetKeyMap.Add("F11", (int)Key.F11);
            _flxToDotNetKeyMap.Add("F12", (int)Key.F12);
            _flxToDotNetKeyMap.Add("ONE", (int)Key.D1);
            _flxToDotNetKeyMap.Add("TWO", (int)Key.D2);
            _flxToDotNetKeyMap.Add("THREE", (int)Key.D3);
            _flxToDotNetKeyMap.Add("FOUR", (int)Key.D4);
            _flxToDotNetKeyMap.Add("FIVE", (int)Key.D5);
            _flxToDotNetKeyMap.Add("SIX", (int)Key.D6);
            _flxToDotNetKeyMap.Add("SEVEN", (int)Key.D7);
            _flxToDotNetKeyMap.Add("EIGHT", (int)Key.D8);
            _flxToDotNetKeyMap.Add("NINE", (int)Key.D9);
            _flxToDotNetKeyMap.Add("ZERO", (int)Key.D0);
            _flxToDotNetKeyMap.Add("MINUS", (int)Key.Subtract);
            _flxToDotNetKeyMap.Add("PLUS", (int)Key.Add);
            _flxToDotNetKeyMap.Add("DELETE", (int)Key.Delete);            
            _flxToDotNetKeyMap.Add("CAPSLOCK", (int)Key.CapsLock);            
            _flxToDotNetKeyMap.Add("SHIFT", (int)Key.Shift);            
            _flxToDotNetKeyMap.Add("CONTROL", (int)Key.Ctrl);
            _flxToDotNetKeyMap.Add("ALT", (int)Key.Alt);
            _flxToDotNetKeyMap.Add("SPACE", (int)Key.Space);
            _flxToDotNetKeyMap.Add("UP", (int)Key.Up);
            _flxToDotNetKeyMap.Add("DOWN", (int)Key.Down);
            _flxToDotNetKeyMap.Add("LEFT", (int)Key.Left);
            _flxToDotNetKeyMap.Add("RIGHT", (int)Key.Right);
            
            _flxToDotNetKeyMap.Add("Q", (int)Key.Q);
            _flxToDotNetKeyMap.Add("W", (int)Key.W);
            _flxToDotNetKeyMap.Add("E", (int)Key.E);
            _flxToDotNetKeyMap.Add("R", (int)Key.R);
            _flxToDotNetKeyMap.Add("T", (int)Key.T);
            _flxToDotNetKeyMap.Add("Y", (int)Key.Y);
            _flxToDotNetKeyMap.Add("U", (int)Key.U);
            _flxToDotNetKeyMap.Add("I", (int)Key.I);
            _flxToDotNetKeyMap.Add("O", (int)Key.O);
            _flxToDotNetKeyMap.Add("P", (int)Key.P);
            _flxToDotNetKeyMap.Add("A", (int)Key.A);
            _flxToDotNetKeyMap.Add("S", (int)Key.S);
            _flxToDotNetKeyMap.Add("D", (int)Key.D);
            _flxToDotNetKeyMap.Add("F", (int)Key.F);
            _flxToDotNetKeyMap.Add("G", (int)Key.G);
            _flxToDotNetKeyMap.Add("H", (int)Key.H);
            _flxToDotNetKeyMap.Add("J", (int)Key.J);
            _flxToDotNetKeyMap.Add("K", (int)Key.K);
            _flxToDotNetKeyMap.Add("L", (int)Key.L);
            _flxToDotNetKeyMap.Add("Z", (int)Key.Z);
            _flxToDotNetKeyMap.Add("X", (int)Key.X);
            _flxToDotNetKeyMap.Add("C", (int)Key.C);
            _flxToDotNetKeyMap.Add("V", (int)Key.V);
            _flxToDotNetKeyMap.Add("B", (int)Key.B);
            _flxToDotNetKeyMap.Add("N", (int)Key.N);
            _flxToDotNetKeyMap.Add("M", (int)Key.M);

            Dictionary<string,int>.Enumerator iter = _flxToDotNetKeyMap.GetEnumerator();
            while (iter.MoveNext())
                addKey(iter.Current.Key, iter.Current.Value);
		}
		
		public void update()
		{
            foreach (int keyCode in _map.Keys)
            {
                FlxKeyboardData o = _map[keyCode];
                if (o != null)
                {
                    if ((o.last == -1) && (o.current == -1)) o.current = 0;
                    else if ((o.last == 2) && (o.current == 2)) o.current = 1;
                    o.last = o.current;
                }
            }
		}
		
		public void reset()
		{
            foreach (int keyCode in _map.Keys)
            {
                FlxKeyboardData o = _map[keyCode];
                if (o != null)
                {
                    o.current = 0;
                    o.last = 0;  
                }
            }        
		}
		
		//@desc		Check to see if this key is pressed
		//@param	Key		One of the key constants listed above (e.g. LEFT or A)
		//@return	Whether the key is pressed
        public bool pressed(string Key) 
        {
            FieldInfo fi = typeof(FlxKeyboard).GetField(Key);
            if (fi != null)
            {
                return (bool)fi.GetValue(this);
            }
            
            return false; 
        }
		
		//@desc		Check to see if this key was JUST pressed
		//@param	Key		One of the key constants listed above (e.g. LEFT or A)
		//@return	Whether the key was just pressed
        public bool justPressed(string Key) { return _map[_flxToDotNetKeyMap[Key]].current == 2; }
		
		//@desc		Check to see if this key is NOT pressed
		//@param	Key		One of the key constants listed above (e.g. LEFT or A)
		//@return	Whether the key is not pressed
        public bool justReleased(string Key) { return _map[_flxToDotNetKeyMap[Key]].current == -1; }
		
		public void handleKeyDown(KeyboardEvent keyboardEvent)
		{
			FlxKeyboardData o = _map[keyboardEvent.keyCode];
			if(o == null) return;
			if(o.current > 0) o.current = 1;
			else o.current = 2;
			setFlag(o.name, true);
		}
		
		public void handleKeyUp(KeyboardEvent keyboardEvent)
		{
			FlxKeyboardData o = _map[keyboardEvent.keyCode];
			if(o == null) return;
			if(o.current > 0) o.current = -1;
			else o.current = 0;
            setFlag(o.name, false);
		}
		
		protected void addKey(string KeyName, int KeyCode)
		{
            if (!_lookup.ContainsKey(KeyName))
                _lookup.Add(KeyName, KeyCode);
            else
                _lookup[KeyName] = KeyCode;

            if (!_map.ContainsKey(KeyCode))
                _map.Add(KeyCode, new FlxKeyboardData(KeyName, 0, 0));
            else
			    _map[KeyCode] = new FlxKeyboardData(KeyName, 0, 0);
		}

        protected void setFlag(string flag, bool value)
        {
            FieldInfo fi = typeof(FlxKeyboard).GetField(flag);
            if (fi != null)
            {
                fi.SetValue(this, value);
            }
        }
	}
}
