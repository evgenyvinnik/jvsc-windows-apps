using System;

namespace Clarity.Phone.Interactivity.Input
{

	public class ShapeGestureEventArgs : GestureEventArgs
	{
		public enum Shapes
		{
			Circle,
			Check,
			PigTail,
			Question_Mark
		}

		// Properties
		public override GestureType GestureType
		{
			get { return GestureType.Shape; }
		}

		public Shapes Shape { get; set; }
		public double Confidence { get; set; }

		public override string ToString()
		{
			return String.Format("{0}: {1}, Confidence: {2}", base.ToString(), Shape, Confidence);
		}
	}
}
