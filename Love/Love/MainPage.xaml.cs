using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Point = System.Windows.Point;

namespace Love
{
	public partial class MainPage
	{
		#region variables
		//geolocation for advertisements
		private readonly GeoCoordinateWatcher _gcw;

		private Point _startPoint;
		private double _originalLeft;
		private double _originalTop;
		private double _topOffset;
		private double _leftOffset;
		private bool _isDown;
		private bool _isDragging;
		private Letter _originalElement;

		private readonly Random _rand;
		//private Letter[] _letters;
		//private Placing[] _placings;
		private string _loveString;
		#region limits

		private const int HeartSize = 48;
		private const int TopLimit = 0;
		private const int LowLimit = 400;
		private const int LeftLimit = 0;
		private const int RightLimit = 440;

		private List<Letter> _letters;
		private List<Placing> _placings;
		List<LanguageString> _loveStrings;
		#endregion

		#endregion

		// Constructor
		public MainPage()
		{
			InitializeComponent();

			_rand = new Random();
		}

		private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
		{
			MakeLevel();
		}

		private void MakeLevel()
		{
			brokenLove.Visibility = Visibility.Collapsed;
			happyLove.Visibility = Visibility.Collapsed;
			drawCanvas.Children.Clear();

			SetLanguage();

			var offset = (10 - _loveString.Length) * HeartSize / 2;

			if (_placings == null)
			{
				_placings = new List<Placing>();
			}

			if (_placings.Count != 0)
			{
				_placings.Clear();
			}

			for (var i = 0; i < _loveString.Length; i++)
			{
				_placings.Add(new Placing {Placed = false});
				Canvas.SetTop(_placings[i], LowLimit);
				Canvas.SetLeft(_placings[i], i * HeartSize + offset);
				drawCanvas.Children.Add(_placings[i]);
			}

			if (_letters == null)
			{
				_letters = new List<Letter>();
			}

			if (_letters.Count != 0)
			{
				_letters.Clear();
			}

			for (var i = 0; i < _loveString.Length; i++)
			{
				_letters.Add(new Letter(_loveString[i]) {Placed = false});
				Canvas.SetTop(_letters[i], _rand.Next(LowLimit - HeartSize));
				Canvas.SetLeft(_letters[i], _rand.Next(RightLimit) + LeftLimit);
				drawCanvas.Children.Add(_letters[i]);
			}
		}


		private void PopulateList()
		{
			_loveStrings.Add(new LanguageString("Say it in English", "Love"));
			_loveStrings.Add(new LanguageString("Say it in Russian", "Любовь"));

			_loveStrings.Add(new LanguageString("Say it in Japanese", "愛"));
			_loveStrings.Add(new LanguageString("Say it in Afrikaans", "Liefde"));
			_loveStrings.Add(new LanguageString("Say it in Albanian", "Dashuri"));
			_loveStrings.Add(new LanguageString("Say it in Arabic", "الحب"));
			//_loveStrings.Add(new LanguageString("Say it in Armenian", "Սեր"));
			_loveStrings.Add(new LanguageString("Say it in Azerbaijani", "Sevgi"));
			_loveStrings.Add(new LanguageString("Say it in Belarusian", "Каханне"));
			_loveStrings.Add(new LanguageString("Say it in Bulgarian", "Любов"));
			_loveStrings.Add(new LanguageString("Say it in Catalan", "Amor"));
			_loveStrings.Add(new LanguageString("Say it in Chinese (Simplified)", "爱"));
			_loveStrings.Add(new LanguageString("Say it in Chinese (Traditional)", "愛"));
			_loveStrings.Add(new LanguageString("Say it in Croatian", "Ljubav"));
			_loveStrings.Add(new LanguageString("Say it in Czech", "Láska"));
			_loveStrings.Add(new LanguageString("Say it in Danish", "Kærlighed"));
			_loveStrings.Add(new LanguageString("Say it in Dutch", "Liefde"));
			_loveStrings.Add(new LanguageString("Say it in Estonian", "Armastus"));
			_loveStrings.Add(new LanguageString("Say it in Filipino", "Mahal"));
			_loveStrings.Add(new LanguageString("Say it in Finnish", "Rakkaus"));
			_loveStrings.Add(new LanguageString("Say it in French", "Amour"));
			_loveStrings.Add(new LanguageString("Say it in Galician", "Amor"));
			//_loveStrings.Add(new LanguageString("Say it in Georgian", "სიყვარული"));
			_loveStrings.Add(new LanguageString("Say it in German", "Liebe"));
			_loveStrings.Add(new LanguageString("Say it in Greek", "Αγάπη"));
			_loveStrings.Add(new LanguageString("Say it in Haitian Creole", "Renmen"));
			_loveStrings.Add(new LanguageString("Say it in Hebrew", "אהבה"));
			//_loveStrings.Add(new LanguageString("Say it in Hindi", "प्यार"));
			_loveStrings.Add(new LanguageString("Say it in Hungarian", "Szerelem"));
			_loveStrings.Add(new LanguageString("Say it in Icelandic", "Ást"));
			_loveStrings.Add(new LanguageString("Say it in Indonesian", "Cinta"));
			_loveStrings.Add(new LanguageString("Say it in Irish", "Grá"));
			_loveStrings.Add(new LanguageString("Say it in Italian", "Amore"));
			_loveStrings.Add(new LanguageString("Say it in Korean", "사랑"));
			_loveStrings.Add(new LanguageString("Say it in Latvian", "Mīlestība"));
			_loveStrings.Add(new LanguageString("Say it in Lithuanian", "Meilė"));
			_loveStrings.Add(new LanguageString("Say it in Macedonian", "Љубовта"));
			_loveStrings.Add(new LanguageString("Say it in Malay", "Cinta"));
			_loveStrings.Add(new LanguageString("Say it in Maltese", "Imħabba"));
			_loveStrings.Add(new LanguageString("Say it in Norwegian", "Kjærleik"));
			_loveStrings.Add(new LanguageString("Say it in Persian", "عشق"));
			_loveStrings.Add(new LanguageString("Say it in Polish", "Miłość"));
			_loveStrings.Add(new LanguageString("Say it in Portuguese", "Amor"));
			_loveStrings.Add(new LanguageString("Say it in Romanian", "Dragoste"));
			_loveStrings.Add(new LanguageString("Say it in Serbian", "Љубав"));
			_loveStrings.Add(new LanguageString("Say it in Slovak", "Láska"));
			_loveStrings.Add(new LanguageString("Say it in Slovenian", "Ljubezen"));
			_loveStrings.Add(new LanguageString("Say it in Spanish", "Amor"));
			_loveStrings.Add(new LanguageString("Say it in Swahili", "Upendo"));
			_loveStrings.Add(new LanguageString("Say it in Swedish", "Kärlek"));
			_loveStrings.Add(new LanguageString("Say it in Thai", "ความรัก"));
			_loveStrings.Add(new LanguageString("Say it in Turkish", "Aşk"));
			_loveStrings.Add(new LanguageString("Say it in Ukrainian", "Любов"));
			_loveStrings.Add(new LanguageString("Say it in Urdu", "محبت"));
			_loveStrings.Add(new LanguageString("Say it in Vietnamese", "Tình yêu"));
			_loveStrings.Add(new LanguageString("Say it in Yiddish", "ליבע"));
		}

		private void SetLanguage()
		{
			if (_loveStrings == null)
			{
				_loveStrings = new List<LanguageString>();
			}

			if (_loveStrings.Count == 0)
			{
				PopulateList();
			}

			var position = _rand.Next(_loveStrings.Count);

			ApplicationTitle.Text = _loveStrings[position].ApplicationTitleText;
			_loveString = _loveStrings[position].LoveStringText;

			_loveStrings.RemoveAt(position);
		}

		private void PhoneApplicationPageMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.OriginalSource == drawCanvas)
			{
			}
			else
			{

				Grid grid = null;

				var text = e.OriginalSource as TextBlock;
				if (text != null)
				{
					grid = text.Parent as Grid;
				}
				else
				{
					var polygon = e.OriginalSource as Polygon;
					if(polygon != null)
					{
						var heart = polygon.Parent as Heart;
						if (heart != null)
						{
							grid = heart.Parent as Grid;
							
						}
					}
				}

				if (grid != null)
				{
					_originalElement = grid.Parent as Letter;
					if (_originalElement != null)
					{
						if (!_originalElement.Placed)
						{
							_isDown = true;
							_startPoint = e.GetPosition(drawCanvas);

							drawCanvas.CaptureMouse();
							e.Handled = true;
						}
						else
						{
							_originalElement = null;
						}
					}
				}
			}
		}

		private void PhoneApplicationPageMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (!_isDown)
				return;

			DragFinished(false);
			e.Handled = true;
		}

		private void DragFinished(bool cancelled)
		{
			drawCanvas.ReleaseMouseCapture();
			if (_isDragging)
			{

				if (cancelled == false)
				{
					if (_originalElement != null)
					{
						if (_originalElement.Placed)
							return;
						var newTop = _originalTop + _topOffset;

						if (newTop < LowLimit && newTop > TopLimit)
							Canvas.SetTop(_originalElement, newTop);
						else if (newTop <= TopLimit)
							Canvas.SetTop(_originalElement, TopLimit);
						else if (newTop >= LowLimit)
							Canvas.SetTop(_originalElement, LowLimit);

						var newLeft = _originalLeft + _leftOffset;

						if (newLeft < RightLimit && newLeft > LeftLimit)
							Canvas.SetLeft(_originalElement, newLeft);
						else if (newLeft <= LeftLimit)
							Canvas.SetLeft(_originalElement, LeftLimit);
						else if (newLeft >= RightLimit)
							Canvas.SetLeft(_originalElement, RightLimit);

						//CheckPlacing();
					}
				}

			}
			_isDragging = false;
			_isDown = false;
		}

		private void PhoneApplicationPageMouseMove(object sender, MouseEventArgs e)
		{
			if (!_isDown)
				return;

			if ((_isDragging == false) && ((Math.Abs(e.GetPosition(drawCanvas).X - _startPoint.X) > 5) ||
										   (Math.Abs(e.GetPosition(drawCanvas).Y - _startPoint.Y) > 5)))
			{
				DragStarted();
			}

			if (!_isDragging)
				return;
			if (_originalElement == null)
				return;

			if (_originalElement.Placed)
				return;
			var currentPosition = e.GetPosition(drawCanvas);

			_leftOffset = currentPosition.X - _startPoint.X;
			_topOffset = currentPosition.Y - _startPoint.Y;

			var newTop = _originalTop + _topOffset;

			if (newTop < LowLimit && newTop > TopLimit)
				Canvas.SetTop(_originalElement, newTop);
			else if (newTop <= TopLimit)
				Canvas.SetTop(_originalElement, TopLimit);
			else if (newTop >= LowLimit)
				Canvas.SetTop(_originalElement, LowLimit);

			var newLeft = _originalLeft + _leftOffset;

			if (newLeft < RightLimit && newLeft > LeftLimit)
				Canvas.SetLeft(_originalElement, newLeft);
			else if (newLeft <= LeftLimit)
				Canvas.SetLeft(_originalElement, LeftLimit);
			else if (newLeft >= RightLimit)
				Canvas.SetLeft(_originalElement, RightLimit);

			CheckPlacing();

		}


		private void DragStarted()
		{
			_isDragging = true;
			if (_originalElement == null)
				return;

			_originalLeft = Canvas.GetLeft(_originalElement);
			_originalTop = Canvas.GetTop(_originalElement);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void CheckPlacing()
		{
			if (_originalElement == null)
				return;
			if (_originalElement.Placed)
				return;

			var left = Canvas.GetLeft(_originalElement);
			var top = Canvas.GetTop(_originalElement);
			var right = left + HeartSize;
			var bottom = top - HeartSize;

			for (var i = 0; i < _placings.Count; i++)
			{
				if (_placings[i].Placed)
					continue;

				var placementLeft = Canvas.GetLeft(_placings[i]);
				var placementTop = Canvas.GetTop(_placings[i]);
				var placementRight = placementLeft + HeartSize;
				var placementBottom = placementTop - HeartSize;

				var square = Math.Max(Math.Min(right, placementRight) - Math.Max(left, placementLeft), 0) *
							 Math.Max(Math.Min(top, placementTop) - Math.Max(bottom, placementBottom), 0);
				if (square <= 1700)
					continue;

				_originalElement.Fill = new SolidColorBrush(Colors.Magenta);

				Canvas.SetTop(_originalElement, placementTop);
				Canvas.SetLeft(_originalElement, placementLeft);
				_placings[i].Placed = true;
				_originalElement.Placed = true;
				_originalElement.Placing = i;

				Debug.WriteLine("Placed " + i);

				_isDragging = false;
				_isDown = false;

				CheckEnd();
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void CheckEnd()
		{
			var allPlaced = true;
			var love = "";

			for (var i = 0; i < _loveString.Length; i++)
			{
				if (_letters[i].Placed)
				{
					love += _letters[i].LetterTextBlock.Text;
				}
				else
				{
					allPlaced = false;
					break;
				}
			}

			if (!allPlaced)
				return;



			if (love == _loveString)
			{
				//Debug.WriteLine("Congratulation!");
				happyLove.Visibility = Visibility.Visible;
				var stream = TitleContainer.OpenStream("happy.wav");
				
				if (stream == null)
					return;

				var effect = SoundEffect.FromStream(stream);
				FrameworkDispatcher.Update();
				effect.Play();
			}
			else
			{
				//Debug.WriteLine("Fail!");
				brokenLove.Visibility = Visibility.Visible;
				var stream = TitleContainer.OpenStream("sad.wav");

				if (stream == null)
					return;

				var effect = SoundEffect.FromStream(stream);
				FrameworkDispatcher.Update();
				effect.Play();
			}
		


			//
		}

		private void HappyLoveClick(object sender, RoutedEventArgs e)
		{
			MakeLevel();
		}

		private void BrokenLoveClick(object sender, RoutedEventArgs e)
		{
			MakeLevel();
		}

		private void AboutButtonClick(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
		}

	}

	public class LanguageString
	{
		public string ApplicationTitleText { get; set; }
		public string LoveStringText { get; set; }

		public LanguageString(string applicationTitle, string loveString)
		{
			ApplicationTitleText = applicationTitle;
			LoveStringText = loveString;
		}

	}
}