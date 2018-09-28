
namespace Clarity.Phone.Interactivity.Input
{
	public class TwoFingerTapAndHoldGestureEventArgs : GestureEventArgs
	{
		public override GestureType GestureType
		{
			get { return GestureType.TwoFingerTapAndHold; }
		}
	}
}
