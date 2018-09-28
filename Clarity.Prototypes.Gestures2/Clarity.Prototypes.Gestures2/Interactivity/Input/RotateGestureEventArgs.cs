using System;

namespace Clarity.Phone.Interactivity.Input
{
	public class RotateGestureEventArgs : GestureEventArgs
	{
		public override GestureType GestureType
		{
			get { return GestureType.Rotate; }
		}

		public double Angle { get; set; }

		public override string ToString()
		{
			return String.Format("{0}: Angle: {1}", base.ToString(), Angle);
		}
	}
}
