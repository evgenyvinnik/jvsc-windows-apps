using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;


namespace TestPattern
{
	public partial class MainPage
	{

		private DispatcherTimer _rendererTimer;
		private TransformGroup transformGroup;
		private TranslateTransform translation;

		// Constructor
		public MainPage()
		{
			InitializeComponent();

			transformGroup = new TransformGroup();
			translation = new TranslateTransform();

			transformGroup.Children.Add(translation);

			_rendererTimer = new DispatcherTimer
			{
				Interval = new TimeSpan(0, 0, 0, 0, 30)
			};

			/* Set the callback of each timer tick */
			_rendererTimer.Tick += RendererTimerTick;

			_rendererTimer.Start();
		}

		private void RendererTimerTick(object sender, EventArgs e)
		{
			translation.X += 1;
			if (translation.X > ActualWidth)
				translation.X = 0;

		}

		private void PhoneApplicationPageManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
		{
			// Move the rectangle.
			//translation.X += e.DeltaManipulation.Translation.X;
			//translation.Y += e.DeltaManipulation.Translation.Y;
		}

		private void PhoneApplicationPageLoaded(object sender, System.Windows.RoutedEventArgs e)
		{
			drawCanvas.Children.Add(new SmpteWide());
			drawCanvas.Children.Add(new SmpteWide());
			drawCanvas.Children[0].RenderTransform = transformGroup;
			Canvas.SetLeft(drawCanvas.Children[1], -ActualWidth);
			drawCanvas.Children[1].RenderTransform = transformGroup;
		}

		private void PhoneApplicationPageUnloaded(object sender, System.Windows.RoutedEventArgs e)
		{
			_rendererTimer.Stop();
		}


	}
}