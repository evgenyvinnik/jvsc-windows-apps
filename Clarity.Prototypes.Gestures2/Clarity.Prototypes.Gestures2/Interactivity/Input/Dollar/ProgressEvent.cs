using System;

namespace Clarity.Phone.Interactivity.Input.Dollar
{
	public class ProgressEventArgs : EventArgs
	{
		public ProgressEventArgs(double percent)
		{
			Percent = percent;
		}

		public double Percent { get; private set; }
	}
	
	public delegate void ProgressEventHandler(object source, ProgressEventArgs e);
}
