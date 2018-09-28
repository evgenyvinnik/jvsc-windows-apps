using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Hextris
{
	public partial class BeautifulCanvas
	{

		private const int Shapes = 150;
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
		}

		void DrawShape()
		{
			var radius = _random.Next(20) + 10;
			var color = new Color
			{
				B = (byte)_random.Next(100),
				G = (byte)_random.Next(100),
				R = (byte)(_random.Next(100)),
				A = (byte)(254 - 4.4 * radius)
			};

			var shape = new HexTile(radius) { Fill = new SolidColorBrush(color) };
			//new Heart(radius) {Fill = new SolidColorBrush(color)};
			//make sure we will not draw in main rectangle area (no need to waste resources)

			Canvas.SetTop(shape, _random.Next(800));
			Canvas.SetLeft(shape, _random.Next(520) - 60);
			drawCanvas.Children.Add(shape);

		}
	}
}
