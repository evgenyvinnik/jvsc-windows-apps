using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Device.Location;
using Telerik.Windows.Controls;

namespace Daltonism
{
	public partial class MainPage
	{

		#region variables
		//geolocation for advertisements
		private readonly GeoCoordinateWatcher _gcw;

		//dispatcher timer for beautiful backgound
		private System.Windows.Threading.DispatcherTimer _dt;

		//random variable for various random things
		private Random _random;

		private const int Circles = 700;

		private byte _originalRed;
		private byte _originalGreen;
		private byte _originalBlue;

		private int _hintsUsed;
		#endregion

		// Constructor
		public MainPage()
		{
			InitializeComponent();

			_random = new Random();

			for (var i = 0; i < Circles; ++i)
			{
				var ellipse = new Ellipse();
				DrawCircle(ellipse);
				drawCanvas.Children.Add(ellipse);
			}
		}

		#region Page load/unload
		private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
		{
			_dt = new System.Windows.Threading.DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 250) };
			_dt.Tick += DtTick;
			_dt.Start();

			_originalRed = (byte) _random.Next(255);
			_originalGreen = (byte) _random.Next(255);
			_originalBlue = (byte) _random.Next(255);

			taskRectangle.Fill = new SolidColorBrush(Color.FromArgb(255, _originalRed, _originalGreen, _originalBlue));

			_hintsUsed = 0;
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			_originalRed = (byte)_random.Next(255);
			_originalGreen = (byte)_random.Next(255);
			_originalBlue = (byte)_random.Next(255);

			taskRectangle.Fill = new SolidColorBrush(Color.FromArgb(255, _originalRed, _originalGreen, _originalBlue));

			_hintsUsed = 0;

			RecallButton.IsEnabled = true;
			GradeButton.IsEnabled = true;
			RedButton.IsEnabled = true;
			GreenButton.IsEnabled = true;
			BlueButton.IsEnabled = true;

			ShowField(false);

			//make start button invisible;
			StartButton.Visibility = Visibility.Visible;

			//set beginning slider position
			{
				redSlider.Value = _random.Next(255);
				greenSlider.Value = _random.Next(255);
				blueSlider.Value = _random.Next(255);
			}

		}

		private void PhoneApplicationPageUnloaded(object sender, RoutedEventArgs e)
		{
			_dt.Stop();
		} 
		#endregion

		#region Circles drawing code
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
			var top = _random.Next(720);
			var left = _random.Next(480);

			while (top > 20 && top < 380 && left > 50 && left < 430)
			{
				top = _random.Next(720);
				left = _random.Next(480);
			}
			Canvas.SetTop(ellipse, top);
			Canvas.SetLeft(ellipse, left);

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
		#endregion

		#region Main buttons click
		private void HelpButtonClick(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Uri(string.Format("/HelpPage.xaml"), UriKind.Relative));
		}

		private void RecallButtonClick(object sender, RoutedEventArgs e)
		{
			ShowField(false);

			//make start button invisible;
			CloseButton.Visibility = Visibility.Visible;

			RecallButton.IsEnabled = false;
			_hintsUsed++;
		}

		private void GradeButtonClick(object sender, RoutedEventArgs e)
		{
			//make buttons visible
			MakeButtonsVisible( Visibility.Collapsed);

			//make sliders enabled;
			MakeSlidersEnabled(false);

			CheckButton.IsEnabled = false;

			GradeText.Text = ColorManipulator.Grade(
									ColorManipulator.CIELab2000(
											ColorManipulator.RGBtoLab(_originalRed, _originalGreen, _originalBlue),
											ColorManipulator.RGBtoLab(Convert.ToByte(redSlider.Value), Convert.ToByte(greenSlider.Value), Convert.ToByte(blueSlider.Value))
									)
							);

			GradeCloseButton.Visibility = Visibility.Visible;

			GradeButton.IsEnabled = false;
			_hintsUsed++;
		}

		private void GradeCloseButtonClick(object sender, RoutedEventArgs e)
		{
			GradeCloseButton.Visibility = Visibility.Collapsed;
			//make buttons visible
			MakeButtonsVisible(Visibility.Visible);

			//make sliders enabled;
			MakeSlidersEnabled(true);

			CheckButton.IsEnabled = true;
		}

		private void RedButtonClick(object sender, RoutedEventArgs e)
		{
			redSlider.Value = _originalRed;

			RedButton.IsEnabled = false;
			_hintsUsed++;
		}

		private void GreenButtonClick(object sender, RoutedEventArgs e)
		{
			greenSlider.Value = _originalGreen;

			GreenButton.IsEnabled = false;
			_hintsUsed++;
		}

		private void BlueButtonClick(object sender, RoutedEventArgs e)
		{
			blueSlider.Value = _originalBlue;

			BlueButton.IsEnabled = false;
			_hintsUsed++;
		}

		private void CloseButtonClick(object sender, RoutedEventArgs e)
		{
			ShowField(true);

			//make start button invisible;
			CloseButton.Visibility = Visibility.Collapsed;
		}

		private void StartButtonClick(object sender, RoutedEventArgs e)
		{
			ShowField(true);

			//make start button invisible;
			StartButton.Visibility = Visibility.Collapsed;

			//set beginning slider position
			{
				redSlider.Value = _random.Next(255);
				greenSlider.Value = _random.Next(255);
				blueSlider.Value = _random.Next(255);
			}

		}
		#endregion

		#region Controls Management;
		void MakeButtonsVisible(Visibility visibility)
		{
			HelpButton.Visibility = visibility;
			RecallButton.Visibility = visibility;
			GradeButton.Visibility = visibility;
			RedButton.Visibility = visibility;
			GreenButton.Visibility = visibility;
			BlueButton.Visibility = visibility;
		}

		void MakeSlidersEnabled(bool enable)
		{
			//make sliders enabled;
			redSlider.IsEnabled = enable;
			greenSlider.IsEnabled = enable;
			blueSlider.IsEnabled = enable;
		}

		void ShowField(bool show)
		{

			//make task rectangle invisible.
			taskRectangle.Visibility = show ? Visibility.Collapsed : Visibility.Visible;



			//make our rectangle visible
			drawRectangle.Visibility = show ? Visibility.Visible : Visibility.Collapsed;

			var rectangleFadeAnimation = Resources["rectangleFadeAnimation"] as RadFadeAnimation;
			if (rectangleFadeAnimation != null)
			{
				if (show)
				{
					rectangleFadeAnimation.StartOpacity = 0.0;
					rectangleFadeAnimation.EndOpacity = 1.0;
				}
				else
				{
					rectangleFadeAnimation.StartOpacity = 1.0;
					rectangleFadeAnimation.EndOpacity = 0.0;
				}

				RadAnimationManager.Play(drawRectangle, rectangleFadeAnimation);
			}

			//RadialGauge.Visibility = show ? Visibility.Visible : Visibility.Collapsed;


			//make buttons visible
			MakeButtonsVisible(show ? Visibility.Visible : Visibility.Collapsed);

			//make sliders enabled;
			MakeSlidersEnabled(show);

			CheckButton.IsEnabled = show;
		}

		#endregion

		private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var slider = sender as Slider;
			if (slider != null)
				slider.Value = Math.Round(e.NewValue);

			drawRectangle.Fill = new SolidColorBrush(Color.FromArgb(255, Convert.ToByte(redSlider.Value), Convert.ToByte(greenSlider.Value), Convert.ToByte(blueSlider.Value)));

			var diff = ColorManipulator.CIELab2000(
				ColorManipulator.RGBtoLab(_originalRed, _originalGreen, _originalBlue),
				ColorManipulator.RGBtoLab(Convert.ToByte(redSlider.Value), Convert.ToByte(greenSlider.Value),
				                          Convert.ToByte(blueSlider.Value))
				);
			//arrowIndicator.Value = diff;
		}

		private void CheckButtonClick(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Uri(string.Format("/ResultPage.xaml?val1={0}&val2={1}&val3={2}&val4={3}&val5={4}&val6={5}&val7={6}",
				_originalRed, _originalGreen, _originalBlue,
				Convert.ToByte(redSlider.Value), Convert.ToByte(greenSlider.Value), Convert.ToByte(blueSlider.Value), _hintsUsed),
				UriKind.Relative));
		}


	}
}
