using System;
using System.Windows;
using System.Windows.Interactivity;
using System.Linq;

namespace Clarity.Phone.Interactivity.Input
{
	public class GestureTrigger : TriggerBase<FrameworkElement>
	{
		public static readonly DependencyProperty GestureProperty = DependencyProperty.Register("Gesture", typeof(string), typeof(GestureTrigger), null);
		private GestureBehavior _gestureBehavior;

		public GestureTrigger()
		{
			
		}

		public GestureTrigger(string gesture)
		{
			Gesture = gesture;
		}


		protected override void OnAttached()
		{
			base.OnAttached();


			_gestureBehavior = Interaction.GetBehaviors(AssociatedObject).OfType<GestureBehavior>().First();
			_gestureBehavior.GestureRecognized += AssociatedObjectGestureRecognized;
			//this.AssociatedObject.GestureRecognized += new EventHandler<GestureEventArgs>(AssociatedObject_GestureRecognized);
		}

		void AssociatedObjectGestureRecognized(object sender, GestureEventArgs e)
		{
			if (String.IsNullOrEmpty(Gesture))
				return;

			if (e.GestureType.ToString() == Gesture)
			{
				InvokeActions(e);
			}
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();

			//this.AssociatedObject.GestureRecognized -= new EventHandler<GestureEventArgs>(AssociatedObject_GestureRecognized);
			_gestureBehavior.GestureRecognized -= AssociatedObjectGestureRecognized;
		}


		// Properties
		public string Gesture
		{
			get
			{
				return (string)GetValue(GestureProperty);
			}
			set
			{
				SetValue(GestureProperty, value);
			}
		}

	}
}
