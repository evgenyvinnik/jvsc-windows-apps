using System;
using System.Windows;
using System.Windows.Interactivity;

namespace Clarity.Phone.Interactivity.Input
{
	public class GestureBehavior : Behavior<FrameworkElement>
	{
		private GestureInterpreter _gestureInterpreter;

		public bool EnableDebugMode { get; set; }

		public event EventHandler<GestureEventArgs> GestureRecognized;

		public GestureBehavior() { }

		protected override void OnAttached()
		{
			base.OnAttached();

			_gestureInterpreter = new GestureInterpreter(AssociatedObject);
			_gestureInterpreter.GestureRecognized += GestureInterpreterGestureRecognized;
			_gestureInterpreter.DebugMode = EnableDebugMode;
		}

		void GestureInterpreterGestureRecognized(object sender, GestureEventArgs e)
		{
			if (GestureRecognized != null)
				GestureRecognized(this, e);
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();

			_gestureInterpreter.GestureRecognized -= GestureInterpreterGestureRecognized;
			_gestureInterpreter.Unsubscribe();
			_gestureInterpreter = null;
		}
	}
}
