using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Daltonism
{
	public partial class ResultPage
	{

		private const int Circles = 700;
		private System.Windows.Threading.DispatcherTimer _dt;
		private Random _random;

		private byte _originalRed;
		private byte _originalGreen;
		private byte _originalBlue;

		private byte _sliderRed;
		private byte _sliderGreen;
		private byte _sliderBlue;

		private int _hintsUsed;

		public ResultPage()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			string value1;
			string value2;
			string value3;
			string value4;
			string value5;
			string value6;
			string value7;

			NavigationContext.QueryString.TryGetValue("val1", out value1);
			NavigationContext.QueryString.TryGetValue("val2", out value2);
			NavigationContext.QueryString.TryGetValue("val3", out value3);
			NavigationContext.QueryString.TryGetValue("val4", out value4);
			NavigationContext.QueryString.TryGetValue("val5", out value5);
			NavigationContext.QueryString.TryGetValue("val6", out value6);
			NavigationContext.QueryString.TryGetValue("val7", out value7);

			_originalRed = byte.Parse(value1);
			_originalGreen = byte.Parse(value2);
			_originalBlue = byte.Parse(value3);
			_sliderRed = byte.Parse(value4);
			_sliderGreen = byte.Parse(value5);
			_sliderBlue = byte.Parse(value6);

			_hintsUsed = int.Parse(value7);

			OriginalRectangle.Fill = new SolidColorBrush(Color.FromArgb(255, _originalRed, _originalGreen, _originalBlue));
			PlayerRectangle.Fill = new SolidColorBrush(Color.FromArgb(255, _sliderRed, _sliderGreen, _sliderBlue));

			OriginalRed.Text = _originalRed.ToString();
			OriginalGreen.Text = _originalGreen.ToString();
			OriginalBlue.Text = _originalBlue.ToString();

			PlayerRed.Text = _sliderRed.ToString();
			PlayerGreen.Text = _sliderGreen.ToString();
			PlayerBlue.Text = _sliderBlue.ToString();

			var difference = ColorManipulator.CIELab2000(
											ColorManipulator.RGBtoLab(_originalRed, _originalGreen, _originalBlue),
											ColorManipulator.RGBtoLab(_sliderRed, _sliderGreen, _sliderBlue)
									);

			ResultText.Text = difference.ToString("F2", CultureInfo.InvariantCulture);

			gradeText.Text = ColorManipulator.Grade(difference);

			hintsTextBlock.Text = _hintsUsed.ToString();
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

			Canvas.SetTop(ellipse, _random.Next(720) + 80);
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

		private void NewGameButtonClick(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
		}


	}
}