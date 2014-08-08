using System;
using System.Collections.Generic;

namespace org.flixel
{
	//@desc		This class wraps the normal Flash array and adds a couple of extra functions...
	public class FlxArray<T> : List<T> 
	{
		//@desc		Constructor
		public FlxArray()
		{
           
		}

        public int length
        {
            get
            {
                return this.Count;
            }
        }

		//@desc		Picks an entry at random from an array
		//@param	Arr		The array you want to pick the object from
		//@return	Any object
		static public T getRandom(List<T> A)
		{
			Random random = new Random();            
            return A[random.Next(A.Count-1)];
		}
		
		//@desc		Find the first entry in the array that doesn't "exist"
		//@return	Anything based on FlxCore (FlxSprite, FlxText, FlxBlock, etc)
		public T getNonexist()
		{
            if (this.Count <= 0) return default(T);
			int i = 0;
			do
			{
				if(!(this[i] as FlxCore).exists)
					return this[i];
			} while (++i < this.Count);
            return default(T);
		}
		
		//@desc		Add an object to this array
		//@param	Obj		The object you want to add to the array
		//@return	Just returns the object you passed in in the first place
		public T add(T Obj)
		{
            for (int i = 0; i < this.Count; i++)
            {
				if(this[i] == null)
                {
					this[i] = Obj;
                    return Obj;
                }
            }
            this.Add(Obj);
			return Obj;
		}

        public void remove(T Obj)
        {
            this.remove(Obj, false);
        }
		
		//@desc		Remove any object from this array
		//@param	Core	The object you want to remove from this array
		public void remove(T Obj, bool Splice)
		{
			removeAt(IndexOf(Obj),Splice);
		}
		
		public void removeAt(int Index)
        {
            this.removeAt(Index, false);
        }
        
        //@desc		Remove any object from this array
		//@param	Index	The entry in the array that you want to remove
		public void removeAt(int Index, bool Splice)
		{
			if(Splice)
				this.removeAt(Index);
			else
                this[Index] = default(T);
		}
		
		//@desc		Kills the specified FlxCore-based object (FlxSprite, FlxText, etc) in this array
		//@param	Core	The object you want to kill
		public void kill(T Core)
		{
			killAt(IndexOf(Core));
		}
		
		//@desc		Kills the specified FlxCore-based object (FlxSprite, FlxText, etc) in this array
		//@param	Index	The entry in the array that you want to kill
		public void killAt(int Index)
		{
            if (this[Index] is FlxCore)
            {
                FlxCore core = this[Index] as FlxCore;
                core.kill();
            }
		}
		
		//@desc		Pops every entry out of the array
		public void clear()
		{
            this.Clear();
		}
	}
}