using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Love
{
	public partial class BeautifulCanvas
	{

		private const int Shapes = 150;
		private System.Windows.Threading.DispatcherTimer _dt;
		private Random _random;

		public BeautifulCanvas()
		{
			InitializeComponent();

		}

		private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
		{
			_random = new Random();
			for (var i = 0; i < Shapes; ++i)
			{
				DrawShape();
			}

			_dt = new System.Windows.Threading.DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 250) };
			_dt.Tick += DtTick;
			_dt.Start();
		}

		void DrawShape()
		{
			var radius = _random.Next(40) + 20;
			var color = new Color
			{
				B = (byte)_random.Next(125),
				G = (byte)_random.Next(125),
				R = (byte)(_random.Next(125) + 125),
				A = (byte)(254 - 4.4 * radius)
			};

			var shape = new Heart(radius) {Fill = new SolidColorBrush(color)};
			//make sure we will not draw in main rectangle area (no need to waste resources)

			Canvas.SetTop(shape, _random.Next(800));
			Canvas.SetLeft(shape, _random.Next(480));
			drawCanvas.Children.Add(shape);

		}

		void DtTick(object sender, EventArgs e)
		{
			for (var i = 0; i < 100; ++i)
			{
				var item = _random.Next(Shapes);
				var shape = drawCanvas.Children[item] as Heart;

				if (shape == null)
					continue;
				var brush = shape.Fill as SolidColorBrush;

				if (brush == null)
					continue;

				var color = brush.Color;
				if (color.A < 5)
				{
					drawCanvas.Children.RemoveAt(item);
					DrawShape();
				}
				else
				{
					color.A -= 5;
					shape.Fill = new SolidColorBrush(color);
				}
			}
		}


		private void PhoneApplicationPageUnloaded(object sender, RoutedEventArgs e)
		{
			_dt.Stop();
		}
	}
}
