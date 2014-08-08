using System;
using System.Windows;

namespace org.flixel
{
	//@desc		This is the base class for most of the display objects (FlxSprite, FlxText, etc).  It includes some very simple basic attributes about game objects.
	public class FlxCore
	{
		//@desc	Kind of a global on/off switch for any objects descended from FlxCore
		public bool exists;
		//@desc	If an object is not alive, the game loop will not automatically call update() on it
		public bool active;
		//@desc	If an object is not visible, the game loop will not automatically call render() on it
		public bool visible;
		//@desc	If an object is dead, the functions that automate collisions will skip it (see overlapArrays in FlxSprite and collideArrays in FlxBlock)
		public bool dead;
        //@desc If an object is 'fixed' in space, it will not budge when it collides with a not-fixed object
		public bool isFixed;
		
		//Basic attributes variables
		public double x;
		public double y;
		public int width;
		public int height;
        public Point last;
		
		//@desc	A point that can store numbers from 0 to 1 (for X and Y independently) that governs how much this object is affected by the camera subsystem.  0 means it never moves, like a HUD element or far background graphic.  1 means it scrolls along a tthe same speed as the foreground layer.
		public Point scrollFactor;
		private bool _flicker;
		private double _flickerTimer;
		
		//@desc		Constructor
		public FlxCore()
		{
			exists = true;
			active = true;
			visible = true;
			dead = false;
            isFixed = false;
            width = 0;
            height = 0;
			x = 0;
			y = 0;
			
            last = new Point(x,y);
			
			scrollFactor = new Point(1,1);
			_flicker = false;
			_flickerTimer = -1;
		}
		
		//@desc		Just updates the flickering.  FlxSprite and other subclasses override this to do more complicated behavior.
		virtual public void update()
		{
			last.X = x;
			last.Y = y;
            
            if(flickering())
			{
				if(_flickerTimer > 0) _flickerTimer -= FlxG.elapsed;
				if(_flickerTimer < 0) flicker(-1);
				else
				{
					_flicker = !_flicker;
					visible = !_flicker;
				}
			}
		}
		
		//@desc		FlxSprite and other subclasses override this to render their materials to the screen
		virtual public void render() {}
		
		//@desc		Checks to see if some FlxCore object overlaps this FlxCore object
		//@param	Core	The object being tested
		//@return	Whether or not the two objects overlap
		virtual public bool overlaps(FlxCore Core)
		{
			double tx = x;
			double ty = y;
			if((scrollFactor.X != 1) || (scrollFactor.Y != 1))
			{
				tx -= Math.Floor(FlxG.scroll.X*scrollFactor.X);
				ty -= Math.Floor(FlxG.scroll.Y*scrollFactor.Y);
			}
			double cx = Core.x;
			double cy = Core.y;
			if((Core.scrollFactor.X != 1) || (Core.scrollFactor.Y != 1))
			{
				cx -= Math.Floor(FlxG.scroll.X*Core.scrollFactor.X);
				cy -= Math.Floor(FlxG.scroll.Y*Core.scrollFactor.Y);
			}
			if((cx <= tx-Core.width) || (cx >= tx+width) || (cy <= ty-Core.height) || (cy >= ty+height))
				return false;
			return true;
		}
		
		virtual public bool overlapsPoint(double X, double Y)
        {
            return overlapsPoint(X, Y, false);
        }
        
        //@desc		Checks to see if a point in 2D space overlaps this FlxCore object
		//@param	X			The X coordinate of the point
		//@param	Y			The Y coordinate of the point
		//@param	PerPixel	Whether or not to use per pixel collision checking (only available in FlxSprite subclass, included here because of Flash's F'd up lack of polymorphism)
		//@return	Whether or not the point overlaps this object
		virtual public bool overlapsPoint(double X, double Y, bool PerPixel)
		{
			double tx = x;
			double ty = y;
			if((scrollFactor.X != 1) || (scrollFactor.Y != 1))
			{
				tx -= Math.Floor(FlxG.scroll.X*scrollFactor.X);
				ty -= Math.Floor(FlxG.scroll.Y*scrollFactor.Y);
			}
			if((X <= tx) || (X >= tx+width) || (Y <= ty) || (Y >= ty+height))
				return false;
			return true;
		}
		
		virtual public void collide(FlxCore Core)
		{
			collideX(Core);
			collideY(Core);
		}
 
		//@desc		Collides a FlxCore against this object on the X axis ONLY.
		//@param	Core	The FlxCore you want to collide
		virtual public void collideX(FlxCore Core)
		{
			//Helper variables for our collision process
			double split;
			Rect thisBounds = new Rect();
			Rect coreBounds = new Rect();
 
			//Calculate the Core's X axis collision bounds
			if(Core.x > Core.last.X)
			{
				coreBounds.X = Core.last.X;
				coreBounds.Width = (Core.x - Core.last.X) + Core.width;
			}
			else
			{
				coreBounds.X = Core.x;
				coreBounds.Width = (Core.last.X - Core.x) + Core.width;
			}
			coreBounds.Y = Core.last.Y;
			coreBounds.Height = Core.height;
 
			//Calculate this object's own X axis collision bounds
			if(x > last.X)
			{
				thisBounds.X = last.X;
				thisBounds.Width = (x - last.X) + width;
			}
			else
			{
				thisBounds.X = x;
				thisBounds.Width = (last.X - x) + width;
			}
			thisBounds.Y = last.Y;
			thisBounds.Height = height;
 
			//Basic overlap check
			if( (coreBounds.X + coreBounds.Width <= thisBounds.X) ||
				(coreBounds.X >= thisBounds.X + thisBounds.Width) ||
				(coreBounds.Y + coreBounds.Height <= thisBounds.Y) ||
				(coreBounds.Y >= thisBounds.Y + thisBounds.Height) )
				return;
 
			//Check for a right side collision if Core is moving right faster than 'this',
			// or if Core is moving left slower than 'this' we want to check the right side too
			bool coreToRight = Core.x > Core.last.X;
			if((coreToRight && (Core.x - Core.last.X > x - last.X)) || (!coreToRight && (Core.last.X - Core.x < last.X - x)))
			{
				//Right side collision
				if(coreBounds.Right > thisBounds.Left)
				{
					if(isFixed && !Core.isFixed)
					{	
						if(Core.hitWall(this))
							Core.x = x - Core.width;
					}
					else if(!isFixed && Core.isFixed)
					{
						if(hitWall(Core))
							x = Core.x + Core.width;
					}
					else if(Core.hitWall(this) && hitWall(Core))
					{
						split = (coreBounds.Right - thisBounds.Left) / 2;
						Core.x -= split;
						x += split;
					}
				}
			}
			else if((coreToRight && (Core.x - Core.last.X < x - last.X)) || (!coreToRight && (Core.last.X - Core.x > last.X - x)))
			{
				//Left side collision
				if(coreBounds.Left < thisBounds.Right)
				{
					if(isFixed && !Core.isFixed)
					{
						if(Core.hitWall(this))
							Core.x = x + width;
					}
					else if(!isFixed && Core.isFixed)
					{
						if(hitWall(Core))
							x = Core.x - width;
					}
					else if(Core.hitWall(this) && hitWall(Core))
					{
						split = (thisBounds.Right - coreBounds.Left) / 2;
						Core.x += split;
						x -= split;
					}
				}
			}
		}
 
		//@desc		Collides a FlxCore against this object on the Y axis ONLY.
		//@param	Core	The FlxCore you want to collide
		virtual public void collideY(FlxCore Core)
		{
			//Helper variables for our collision process
			double split;
			Rect thisBounds = new Rect();
			Rect coreBounds = new Rect();
 
			//Now we just repeat this basic process only for the Y axis
			if(Core.y > Core.last.Y)
			{
				coreBounds.Y = Core.last.Y;
				coreBounds.Height = (Core.y - Core.last.Y) + Core.height;
			}
			else
			{
				coreBounds.Y = Core.y;
				coreBounds.Height = (Core.last.Y - Core.y) + Core.height;
			}
			coreBounds.X = Core.x;
			coreBounds.Width = Core.width;
 
			//Calculate this object's own Y axis collision bounds
			if(y > last.Y)
			{
				thisBounds.Y = last.Y;
				thisBounds.Height = (y - last.Y) + height;
			}
			else
			{
				thisBounds.Y = y;
				thisBounds.Height = (last.Y - y) + height;
			}
			thisBounds.X = x;
			thisBounds.Width = width;
 
			//Basic overlap check
			if( (coreBounds.X + coreBounds.Width <= thisBounds.X) ||
				(coreBounds.X >= thisBounds.X + thisBounds.Width) ||
				(coreBounds.Y + coreBounds.Height <= thisBounds.Y) ||
				(coreBounds.Y >= thisBounds.Y + thisBounds.Height) )
				return;
 
			//Check for a bottom collision if Core is moving down faster than 'this',
			// or if Core is moving up slower than 'this' we want to check the bottom too
			bool coreDown = Core.y > Core.last.Y;
			if((coreDown && (Core.y - Core.last.Y > y - last.Y)) || (!coreDown && (Core.last.Y - Core.y < last.Y - y)))
			{
				//Bottom collision
				if(coreBounds.Bottom > thisBounds.Top)
				{
					if(isFixed && !Core.isFixed)
					{
						if(Core.hitFloor(this))
							Core.y = y - Core.height;
					}
					else if(!isFixed && Core.isFixed)
					{
						if(hitCeiling(Core))
							y = Core.y + Core.height;
					}
					else if(Core.hitFloor(this) && hitCeiling(Core))
					{
						split = (coreBounds.Bottom - thisBounds.Top) / 2;
						Core.y -= split;
						y += split;
					}
				}
			}
			else if((coreDown && (Core.y - Core.last.Y < y - last.Y)) || (!coreDown && (Core.last.Y - Core.y > last.Y - y)))
			{
				//Top collision
				if(coreBounds.Top < thisBounds.Bottom)
				{
					if(isFixed && !Core.isFixed)
					{
						if(Core.hitCeiling(this))
							Core.y = y + height;
					}
                    else if (!isFixed && Core.isFixed)
					{
						if(hitFloor(Core))
							y = Core.y - height;
					}
					else if(Core.hitCeiling(this) && hitFloor(Core))
					{
						split = (thisBounds.Bottom - coreBounds.Top) / 2;
						Core.y += split;
						y -= split;
					}
				}
			}
		}
		
		//@desc		Called when this object collides with a FlxBlock on one of its sides
		//@return	Whether you wish the FlxBlock to collide with it or not
        virtual public bool hitWall() { return hitWall(null); }
        virtual public bool hitWall(FlxCore Contact) { return true; }
		
		//@desc		Called when this object collides with the top of a FlxBlock
		//@return	Whether you wish the FlxBlock to collide with it or not
        virtual public bool hitFloor() { return hitFloor(null); }
        virtual public bool hitFloor(FlxCore Contact) { return true; }
		
		//@desc		Called when this object collides with the bottom of a FlxBlock
		//@return	Whether you wish the FlxBlock to collide with it or not
        virtual public bool hitCeiling() { return hitCeiling(null); }
		virtual public bool hitCeiling(FlxCore Contact) { return true; }
		
		//@desc		Call this function to "kill" a sprite so that it no longer 'exists'
		virtual public void kill()
		{
			exists = false;
			dead = true;
		}

        public void flicker() { flicker(1); }
        //@desc		Tells this object to flicker for the number of seconds requested (0 = infinite, negative number tells it to stop)
		public void flicker(double Duration) { _flickerTimer = Duration; if(_flickerTimer < 0) { _flicker = false; visible = true; } }
		
		//@desc		Called when this object collides with the bottom of a FlxBlock
		//@return	Whether the object is flickering or not
		public bool flickering() { return _flickerTimer >= 0; }
		
		//@desc		Call this to check and see if this object is currently on screen
		//@return	Whether the object is on screen or not
		public bool onScreen()
		{
			Point p = new Point();
			getScreenXY(out p);
			if((p.X + width < 0) || (p.X > FlxG.width) || (p.Y + height < 0) || (p.Y > FlxG.height))
				return false;
			return true;
		}
		
		//@desc		Call this function to figure out the post-scrolling "screen" position of the object
		//@param	p	Takes a Flash Point object and assigns the post-scrolled X and Y values of this object to it
		virtual protected void getScreenXY(out Point P)
		{
            P = new Point();
            P.X = Math.Floor(x)+Math.Floor(FlxG.scroll.X*scrollFactor.X);
			P.Y = Math.Floor(y)+Math.Floor(FlxG.scroll.Y*scrollFactor.Y);
		}

        virtual public void reset(double X, double Y)
		{
			exists = true;
			active = true;
			visible = true;
			dead = false;
			last.X = x = X;
			last.Y = y = Y;
		}
	}
}