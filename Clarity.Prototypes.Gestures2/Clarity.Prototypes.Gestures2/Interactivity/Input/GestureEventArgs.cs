using System;
using System.Windows;

namespace Clarity.Phone.Interactivity.Input
{
	public enum GestureType
	{
		Tap,
		DoubleTap,
		TapAndHold,
		PressAndTap,
		TwoFingerTap,
		TwoFingerDoubleTap,
		TwoFingerTapAndHold,
		Shape,
		Pan,
		Flick,
		Scale,
		Rotate
	}

	public abstract class GestureEventArgs : EventArgs
	{
		internal GestureEventArgs() { }

		//useful for doing switch statements though multiple gestures
		public abstract GestureType GestureType { get; }

		public Point Origin { get; set; }

		public override string ToString()
		{
			return GestureType.ToString();
		}
	}
}

 

