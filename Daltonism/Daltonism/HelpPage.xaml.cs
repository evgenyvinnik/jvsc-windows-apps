using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace Daltonism
{
	public partial class Page1 : PhoneApplicationPage
	{

		private const int Circles = 700;
		private System.Windows.Threading.DispatcherTimer _dt;
		private Random _random;

		public Page1()
		{
			InitializeComponent();
		}


		private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
		{
			_random = new Random();
			for (var i = 0; i < Circles; ++i)
			{
				var ellipse = new Ellipse();
				DrawCircle(ellipse);
				drawCanvas.Children.Add(ellipse);
			}

			_dt = new System.Windows.Threading.DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 250) };
			_dt.Tick += DtTick;
			_dt.Start();
		}

		void DrawCircle(Shape ellipse)
		{
			var radius = _random.Next(50) + 5;
			var color = new Color
			{
				B = (byte)_random.Next(255),
				G = (byte)_random.Next(255),
				R = (byte)_random.Next(255),
				A = (byte)(254 - 4.4 * radius)
			};

			ellipse.Fill = new SolidColorBrush(color);
			ellipse.Height = radius;
			ellipse.Width = radius;

			//make sure we will not draw in main rectangle area (no need to waste resources)

			Canvas.SetTop(ellipse, _random.Next(800));
			Canvas.SetLeft(ellipse, _random.Next(480));

		}

		void DtTick(object sender, EventArgs e)
		{
			for (var i = 0; i < 100; ++i)
			{
				var item = _random.Next(Circles);
				var ellipse = drawCanvas.Children[item] as Ellipse;

				if (ellipse == null)
					continue;
				var brush = ellipse.Fill as SolidColorBrush;

				if (brush == null)
					continue;

				var color = brush.Color;
				if (color.A < 5)
				{
					drawCanvas.Children.RemoveAt(item);
					var newEllipse = new Ellipse();
					DrawCircle(newEllipse);
					drawCanvas.Children.Add(newEllipse);
				}
				else
				{
					color.A -= 5;
					ellipse.Fill = new SolidColorBrush(color);
				}
			}
		}


		private void PhoneApplicationPageUnloaded(object sender, RoutedEventArgs e)
		{
			_dt.Stop();
		}

		private void BackToGameButtonClick(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
		}
	}
}