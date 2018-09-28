using System;
using System.Windows;

namespace Clarity.Phone.Interactivity.Input
{
	public class PanGestureEventArgs : GestureEventArgs
	{
		// Properties
		public override GestureType GestureType
		{
			get { return GestureType.Pan; }
		}

		public Point Translation { get; set; }

		public override string ToString()
		{
			return String.Format("{0}: Translation: ({1})", base.ToString(), Translation);
		}
	}
}
