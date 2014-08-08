using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace org.flixel
{
    public class IntRect
    {
        protected int left = 0;
        protected int right = 0;
        protected int top = 0;
        protected int bottom = 0;

        public int Left
        {
            get
            {
                return left;
            }
        }

        public int Right
        {
            get
            {
                return right;
            }
        }

        public int Top
        {
            get
            {
                return top;
            }
        }

        public int Bottom
        {
            get
            {
                return bottom;
            }
        }

        public IntRect()
        {

        }
        
        public IntRect(int left, int right, int top, int bottom)
        {
            this.top = top;
            this.bottom = bottom;
            this.left = left;
            this.right = right;
        }

        public IntRect(double left, double right, double top, double bottom)
        {
            this.top = (int)top;
            this.bottom = (int)bottom;
            this.left = (int)left;
            this.right = (int)right;
        }
    }
}
