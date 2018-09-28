using System;
using System.Windows;

namespace Clarity.Phone.Interactivity.Input
{
	public class ScaleGestureEventArgs : GestureEventArgs
	{
		// Properties
		public Point DeltaScale { get; set; }
		public Point CumulativeScale { get; set; }

		public override GestureType GestureType
		{
			get { return GestureType.Scale; }
		}

		public override string ToString()
		{
			return String.Format("{0}: Scale CS: ({1}), DS ({2})", base.ToString(), DeltaScale, CumulativeScale);
		}
	}

	//public class StretchGestureEventArgs : ScaleGestureEventArgs
	//{
	//    public override GestureType GestureType
	//    {
	//        get { return GestureType.Stretch; }
	//    }
	//}

	//public class PinchGestureEventArgs : ScaleGestureEventArgs
	//{
	//    // Properties
	//    public override GestureType GestureType
	//    {
	//        get { return GestureType.Pinch; }
	//    }
	//}
}
