namespace org.flixel
{
	//@desc		This is an organizational class that can update and render a bunch of FlxCore objects
	public class FlxLayer : FlxCore
	{
		private FlxArray<FlxCore> _children;

		//@desc		Constructor		
		public FlxLayer()
		{
			_children = new FlxArray<FlxCore>();
		}
		
		//@desc		Adds a new FlxCore subclass (FlxSprite, FlxBlock, etc) to the list of children
		//@param	Core	The object you want to add
        virtual public FlxCore add(FlxCore Core) 
		{
			return _children.add(Core);
		}
		
		//@desc		Automatically goes through and calls update on everything you added, override this function to handle custom input and perform collisions
		override public void update()
		{
			base.update();
			for(int i = 0; i < _children.Count; i++)
				if((_children[i] != null) && _children[i].exists && _children[i].active) _children[i].update();
		}
		
		//@desc		Automatically goes through and calls render on everything you added, override this loop to do crazy graphical stuffs I guess?
		override public void render()
		{
			base.render();
			    for(int i = 0; i < _children.Count; i++)
				    if((_children[i] != null) && _children[i].exists && _children[i].visible) _children[i].render();
		}
		
		//@desc		Override this function to handle any deleting or "shutdown" type operations you might need (such as removing traditional Flash children like Sprite objects)
		public void destroy() { _children.Clear(); }
		
		//@desc		Returns the array of children
		public FlxArray<FlxCore> children() { return _children; }
	}
}
