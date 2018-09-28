using System.Windows;
//using System.Drawing;

namespace Clarity.Phone.Interactivity.Input.Dollar
{
	public struct PointR
	{
		public static readonly PointR Empty;

		public double X, Y;
		public int T;

		public PointR(double x, double y) : this(x, y, 0)
		{
		}

		public PointR(double x, double y, int t)
		{
			X = x;
			Y = y;
			T = t;
		}

		// copy constructor
		public PointR(PointR p)
		{
			X = p.X;
			Y = p.Y;
			T = p.T;
		}

		//public static explicit operator PointF(PointR p)
		public static explicit operator Point(PointR p)
		{
			//return new PointF((float)p.X, (float)p.Y);
			return new Point((float)p.X, (float)p.Y);
		}

		public static bool operator==(PointR p1, PointR p2)
		{
			return (p1.X == p2.X && p1.Y == p2.Y);
		}

		public static bool operator!=(PointR p1, PointR p2)
		{
			return (p1.X != p2.X || p1.Y != p2.Y);
		}

		public override bool Equals(object obj)
		{
			if (obj is PointR)
			{
				var p = (PointR) obj;
				return (X == p.X && Y == p.Y);
			}
			return false;
		}

		public override int GetHashCode()
		{
			//return ((PointF)this).GetHashCode();
			return ((Point)this).GetHashCode();
		}
	}
}
