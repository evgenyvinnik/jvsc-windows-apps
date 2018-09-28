using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AlchemyGame
{
	public partial class MainPage
	{
		// Constructor
		public MainPage()
		{
			InitializeComponent();
		}

		private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
		{
			var brush = new SolidColorBrush(Colors.Blue);
			var rect1 = new Rectangle {Height = 100, Width = 100, Fill = brush};

			Canvas.SetTop(rect1, 8);
			Canvas.SetLeft(rect1, 8);
			AlchemyCanvas.Children.Add(rect1);
		}

		private Point _startPoint;
		private double _originalLeft;
		private double _originalTop;
		private double _topOffset;
		private double _leftOffset;
		private bool _isDown;
		private bool _isDragging;
		private UIElement _originalElement;


		private void PhoneApplicationPageMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (e.OriginalSource == AlchemyCanvas)
			{
			}
			else
			{
				_isDown = true;
				_startPoint = e.GetPosition(AlchemyCanvas);
				_originalElement = e.OriginalSource as UIElement;
				AlchemyCanvas.CaptureMouse();
				e.Handled = true;
			}
		}

		private void PhoneApplicationPageMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (_isDown)
			{
				DragFinished(false);
				e.Handled = true;
			}
		}

		private void DragFinished(bool cancelled)
		{
			AlchemyCanvas.ReleaseMouseCapture();
			if (_isDragging)
			{

				if (cancelled == false)
				{
					Canvas.SetTop(_originalElement, _originalTop + _topOffset);
					Canvas.SetLeft(_originalElement, _originalLeft + _leftOffset);
				}

			}
			_isDragging = false;
			_isDown = false;
		}

		private void PhoneApplicationPageMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (_isDown)
			{
				if ((_isDragging == false) && ((Math.Abs(e.GetPosition(AlchemyCanvas).X - _startPoint.X) > 5) ||
					(Math.Abs(e.GetPosition(AlchemyCanvas).Y - _startPoint.Y) > 5)))
				{
					DragStarted();
				}
				if (_isDragging)
				{
					var currentPosition = e.GetPosition(AlchemyCanvas);

					_leftOffset = currentPosition.X - _startPoint.X;
					_topOffset = currentPosition.Y - _startPoint.Y;

					Canvas.SetTop(_originalElement, _originalTop + _topOffset);
					Canvas.SetLeft(_originalElement, _originalLeft + _leftOffset);
	
				}
			}
		}


		private void DragStarted()
		{
			_isDragging = true;
			_originalLeft = Canvas.GetLeft(_originalElement);
			_originalTop = Canvas.GetTop(_originalElement);


		}


	}
}