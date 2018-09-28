
namespace Clarity.Phone.Interactivity.Input
{
	public class TapGestureEventArgs : GestureEventArgs
	{
		public override GestureType GestureType
		{
			get { return GestureType.Tap; }
		}
	}   
}
