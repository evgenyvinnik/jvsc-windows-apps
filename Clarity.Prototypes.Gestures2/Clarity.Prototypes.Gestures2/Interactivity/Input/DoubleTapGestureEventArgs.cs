using System;

namespace Clarity.Phone.Interactivity.Input
{
	public class DoubleTapGestureEventArgs : GestureEventArgs
	{
		public override GestureType GestureType
		{
			get { return GestureType.DoubleTap; }
		}
	}
}
