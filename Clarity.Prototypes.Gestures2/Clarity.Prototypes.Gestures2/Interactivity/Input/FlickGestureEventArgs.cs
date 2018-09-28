using System;
using System.Windows;

namespace Clarity.Phone.Interactivity.Input
{
	public enum FlickDirection
	{
		Unknown,
		Up,
		Down,
		Left,
		Right
	}

	public class FlickGestureEventArgs : GestureEventArgs
	{
		private const double MaxAngleError = 30;

		public override GestureType GestureType
		{
			get { return GestureType.Flick; }
		}

		public FlickDirection FlickDirection { get; set; }
		public Point Velocity { get; set; }

		public override string ToString()
		{
			return String.Format("{0}: Velocity: ({1}), Direction: {2}", base.ToString(), Velocity, FlickDirection);
		}

		public static FlickDirection GetDirection(Point p1, Point p2)
		{
			var deltaX = p1.X - p2.X;
			var deltaY = p1.Y - p2.Y;
			var length = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

			var sin = deltaX / length;
			var cos = deltaY / length;

			var angle = Math.Asin(Math.Abs(sin)) * 180 / Math.PI;

			if ((sin >= 0) && (cos < 0))
				angle = 180 - angle;
			else if ((sin < 0) && (cos < 0))
				angle = angle + 180;
			else if ((sin < 0) && (cos >= 0))
				angle = 360 - angle;

			if ((angle > 360 - MaxAngleError) || (angle < 0 + MaxAngleError))
				return FlickDirection.Up;
			if ((angle > 90 - MaxAngleError) && (angle < 90 + MaxAngleError))
				return FlickDirection.Left;
			if ((angle > 180 - MaxAngleError) && (angle < 180 + MaxAngleError))
				return FlickDirection.Down;
			if ((angle > 270 - MaxAngleError) && (angle < 270 + MaxAngleError))
				return FlickDirection.Right;
			return FlickDirection.Unknown;
		}
	}
}
