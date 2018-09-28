using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Device.Location;

namespace Hextris
{
	public partial class MainPage
	{
		#region variables
		//geolocation for advertisements
		private readonly GeoCoordinateWatcher _gcw;

		#region additional controls
		private GamePanel _playPanel;
		private GamePanel _previewPanel;
		private TextBlock _gameOverLabel;
		private TextBlock _pausedLabel; 
		#endregion

		#region Geme counters
		private int _lines;
		private int _stones;
		private int _level; 
		#endregion

		private DispatcherTimer _moverThread;
		private int _initSpeed = 800;
		private int _levelSpeed = 50;

		#region Stones
		private Stone _currentStone;
		private Stone _nextStone; 
		#endregion

		private int _severity;

		#region flags
		private bool _paused;
		private bool _gameOver;
		private bool _demoMode; 
		#endregion
		#endregion

		// Constructor
		public MainPage()
		{
			InitializeComponent();

			InitAditionalLayout();
			Initialize();
		}

		private void InitAditionalLayout()
		{
			_playPanel = new GamePanel(15, 21, 6);
			LayoutRoot.Children.Add(_playPanel);
			Grid.SetColumn(_playPanel, 0);
			Grid.SetRow(_playPanel, 3);
			Grid.SetColumnSpan(_playPanel, 3);
			Grid.SetRowSpan(_playPanel, 4);
			_playPanel.Margin = new Thickness(20, 10, 0, 0);

			_previewPanel = new GamePanel(6, 6, 6.0);
			LayoutRoot.Children.Add(_previewPanel);
			Grid.SetColumn(_previewPanel, 4);
			Grid.SetRow(_previewPanel, 3);
			_previewPanel.Margin = new Thickness(0, 20, 0, 0);

			_gameOverLabel = new TextBlock
			{
				Text = "Game Over",
				Foreground = new SolidColorBrush(Colors.Red),
				FontWeight = FontWeights.Bold,
				FontSize = 40,
				HorizontalAlignment = HorizontalAlignment.Center,
				FontFamily = new FontFamily("/Resources/amiga4ever.ttf#Amiga Forever"),
				Visibility = Visibility.Collapsed
			};
			LayoutRoot.Children.Add(_gameOverLabel);
			Grid.SetColumn(_gameOverLabel, 0);
			Grid.SetColumnSpan(_gameOverLabel, 3);
			Grid.SetRow(_gameOverLabel, 4);

			_pausedLabel = new TextBlock
			{
				Text = "Paused ",
				Foreground = new SolidColorBrush(Colors.Cyan),
				FontWeight = FontWeights.Bold,
				FontSize = 40,
				HorizontalAlignment = HorizontalAlignment.Center,
				FontFamily = new FontFamily("/Resources/amiga4ever.ttf#Amiga Forever"),
				Visibility = Visibility.Collapsed
			};
			LayoutRoot.Children.Add(_pausedLabel);
			Grid.SetColumn(_pausedLabel, 0);
			Grid.SetColumnSpan(_pausedLabel, 3);
			Grid.SetRow(_pausedLabel, 4);

	
		}

		private void Initialize()
		{
			_gameOver = true;
			NewGame(false);
		}

		public void NewGame(bool demo)
		{
			_severity = 1;

			{
				_demoMode = demo;

				DemoImage.Source = _demoMode ? new BitmapImage(new Uri("pack:\\Resources\\Images\\hand.png")) : new BitmapImage(new Uri("pack:\\Resources\\Images\\demo.png"));
			}

			SetStones(0);
			SetLines(0);
			if (_nextStone != null)
			{
				_nextStone.Place(false);
			}
			_playPanel.Board.DrawPlayField();
			_nextStone = new Stone(_previewPanel.Board, _severity);
			CreateNextStone();
			_playPanel.Repaint();

			_gameOver = false;
			_gameOverLabel.Visibility = Visibility.Collapsed;

			{
				_paused = false;
				_pausedLabel.Visibility = Visibility.Collapsed;
				PauseImage.Source = new BitmapImage(new Uri("pack:\\Resources\\Images\\pause.png"));
			}

			if (_moverThread == null)
			{
				_moverThread = new DispatcherTimer();
				_moverThread.Tick += MoverThreadTick;
			}
			else
			{
				_moverThread.Stop();
			}

			SetLevel(demo ? 7 : 1);

			_moverThread.Start();
		}

		#region page load/unload
		private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
		{
			if(_moverThread != null)
				if(!_moverThread.IsEnabled)
					_moverThread.Start();
		}

		private void PhoneApplicationPageUnloaded(object sender, RoutedEventArgs e)
		{
			if (_moverThread != null)
				_moverThread.Stop();
		} 
		#endregion

		void MoverThreadTick(object sender, EventArgs e)
		{
			if (_paused)
				return;
			if(_gameOver)
				return;
			if(_currentStone == null)
				return;

			MoveDown();
		}

		private void CreateNextStone()
		{
			_currentStone = new Stone(_nextStone, _playPanel.Board);
			_currentStone.SetPosition((_playPanel.PanelWidth - 5) / 2, -1);

			if (!_currentStone.MayPlace(_currentStone.PosX, _currentStone.PosY))
			{
				GameOver();
				return;
			}

			_nextStone.Place(false);
			_nextStone = new Stone(_previewPanel.Board, _severity);

			_nextStone.SetPosition(0, 0);
			_nextStone.Place(true);

			_currentStone.Place(true);

			_previewPanel.Repaint();

			if (!_demoMode)
				return;

			Thread.Sleep(50);
			_currentStone.Place(false);
			var bp = _currentStone.GetBestPosition();
			_currentStone.Place(true);
			for (var i = 0; i < bp[1]; i++)
			{
				
				_currentStone.MoveStone(Stone.Movements.RotateLeft);
				_playPanel.Repaint();
				Thread.Sleep(50);
			}

			_currentStone.Place(false);
			_currentStone.SetPosition(bp[0], -1);
			_currentStone.Place(true);
			var dx = bp[0] - _currentStone.PosX;
			for (var i = 0; Math.Abs(dx) > i; i++)
			{
				_currentStone.MoveStone(dx > 0 ? Stone.Movements.MoveRight : Stone.Movements.MoveLeft);
				_playPanel.Repaint();
				Thread.Sleep(50);
			}
		}

		private void MoveDown()
		{
			if (_currentStone.MoveDown())
			{
				_playPanel.Repaint();
				return;
			}

			ReleaseCurrentStone();
			AddLines(_playPanel.RemoveFillLines());
			CreateNextStone();
			return;
		}

		public void GameOver()
		{
			if(_moverThread!= null)
				_moverThread.Stop();

			_gameOver = true;
			_gameOverLabel.Visibility = Visibility.Visible;
		}

		private void ReleaseCurrentStone()
		{
			_currentStone = null;
			IncStones();
		}

		private void SetLevel(int i)
		{
			_level = i;
			levelLabel.Text = _level.ToString();
			if(_moverThread!= null)
				_moverThread.Interval = TimeSpan.FromMilliseconds(_initSpeed - _levelSpeed * _level > _levelSpeed ? _initSpeed - _levelSpeed * _level : _levelSpeed);
		}

		/**
		* Increases a numbers of stones used in the game.
		*/
		private void IncStones()
		{
			SetStones(++_stones);
			if (_stones > 20 * _level && _level < 10)
			{
				SetLevel(_level + 1);
			}
		}

		/**
		 * Sets stones count to the given amount.
		 * @param stones count
		 */
		private void SetStones(int s)
		{
			_stones = s;
			stonesLabel.Text = _stones.ToString();
		}

		/**
		 * Increases the lines variable.
		 * @param number of lines to be added.
		*/
		private void AddLines(int l)
		{
			SetLines(_lines += l);
		}

		/**
		 * Sets lines count to the given amount.
		 * @param lines count
		 */
		private void SetLines(int linesCount)
		{
			_lines = linesCount;
			linesLabel.Text = linesCount.ToString();
		}

		#region ButtonClicks
		private void LeftButtonClick(object sender, RoutedEventArgs e)
		{
			if (_gameOver || _demoMode)
			{
				return;
			}
			if (_currentStone == null)
			{
				return;
			}

			_currentStone.MoveStone(Stone.Movements.MoveLeft);
			_playPanel.Repaint();
		}

		private void DownButtonClick(object sender, RoutedEventArgs e)
		{
			if (_gameOver || _demoMode)
			{
				return;
			}
			if (_currentStone == null)
			{
				return;
			}

			while (_currentStone.MoveDown() && !_gameOver)
			{
			}

			ReleaseCurrentStone();
			AddLines(_playPanel.RemoveFillLines());
			CreateNextStone();

			_playPanel.Repaint();
		}

		private void RotateButtonClick(object sender, RoutedEventArgs e)
		{
			if (_gameOver || _demoMode)
			{
				return;
			}
			if (_currentStone == null)
			{
				return;
			}

			_currentStone.MoveStone(Stone.Movements.RotateRight);
			_playPanel.Repaint();
		}

		private void RightButtonClick(object sender, RoutedEventArgs e)
		{
			if (_gameOver || _demoMode)
			{
				return;
			}
			if (_currentStone == null)
			{
				return;
			}

			_currentStone.MoveStone(Stone.Movements.MoveRight);
			_playPanel.Repaint();
		} 
		#endregion

		#region Application bar clicks


		private void PauseButtonClick(object sender, EventArgs e)
		{
			if (_gameOver)
				return;
			if (_paused)
			{
				_paused = false;
				_pausedLabel.Visibility = Visibility.Collapsed;
			}
			else
			{
				_paused = true;
				_pausedLabel.Visibility = Visibility.Visible;
			}

			PauseImage.Source = _paused ? new BitmapImage(new Uri("pack:\\Resources\\Images\\pause.png")) : new BitmapImage(new Uri("pack:\\Resources\\Images\\play.png"));
		}

		private void DemoButtonClick(object sender, EventArgs e)
		{
			NewGame(!_demoMode);
		}

		private void NewGameButtonClick(object sender, EventArgs e)
		{
			_gameOver = true;
			NewGame(false);
		}
		#endregion
	}
}