using System.Windows;

namespace org.flixel
{
    public class IntPoint
    {
        protected int x = 0;
        protected int y = 0;

        public int X
        {
            get
            {
                return x;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
        }

        public IntPoint()
        {

        }

        public IntPoint(Point point)
        {
            this.x = (int)point.X;
            this.y = (int)point.Y;
        }

        public IntPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public IntPoint(double x, double y)
        {
            this.x = (int)x;
            this.y = (int)y;
        }
    }
}
