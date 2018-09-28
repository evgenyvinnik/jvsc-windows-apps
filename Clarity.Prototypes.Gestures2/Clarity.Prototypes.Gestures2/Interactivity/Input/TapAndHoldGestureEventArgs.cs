
namespace Clarity.Phone.Interactivity.Input
{
	public class TapAndHoldGestureEventArgs : GestureEventArgs
	{
		// Properties
		public override GestureType GestureType
		{
			get { return GestureType.TapAndHold; }
		}
	}   
}
