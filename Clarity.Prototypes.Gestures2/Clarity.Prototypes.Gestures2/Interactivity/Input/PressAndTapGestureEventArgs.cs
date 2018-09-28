
namespace Clarity.Phone.Interactivity.Input
{
	public class PressAndTapGestureEventArgs : GestureEventArgs
	{
		// Properties
		public override GestureType GestureType
		{
			get { return GestureType.PressAndTap; }
		}
	}
}
