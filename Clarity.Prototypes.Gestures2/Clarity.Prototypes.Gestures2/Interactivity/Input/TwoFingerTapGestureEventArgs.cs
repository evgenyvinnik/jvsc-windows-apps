
namespace Clarity.Phone.Interactivity.Input
{
	public class TwoFingerTapGestureEventArgs : GestureEventArgs
	{
		public override GestureType GestureType
		{
			get { return GestureType.TwoFingerTap; }
		}
	}
}
