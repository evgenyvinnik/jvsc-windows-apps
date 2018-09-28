using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BubbleWrapUltimate
{

	public partial class MainPage
	{
		#region Variables
		private readonly Random _random;

		IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
		ObservableCollection<PlayerPosition> _playerPositions;

		ObservableCollection<Awards> _playerAwards;

		#region loadable variables
		private string _playerName;
		private Player _player;
		private Guid _phoneGuid;

		private bool _playSounds;
		#endregion

		#region constants
		const string UserName = "WindowsPhone7User";
		const string Password = "Upiter!7Saturn";
		const string Datasource = "hjh7y1qekw.database.windows.net";
		const string DatabaseName = "BubbleWrapTopScore";
		const string TableName = "BubbleWrapUltimateTopScore";
		#endregion

		//game timer
		private int _seconds;
		private System.Windows.Threading.DispatcherTimer _dt;
		#endregion


		// Constructor
		public MainPage()
		{
			_playerPositions = new ObservableCollection<PlayerPosition>();

			_playerAwards = new ObservableCollection<Awards>();

			InitializeComponent();

			//make new random variable
			_random = new Random();

			InitializePhoneGuid();
			InitializePlaySounds();

			_dt = new System.Windows.Threading.DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 1000) };
			_dt.Tick += DtTick;
		}

		// Load data for the ViewModel Items
		private void MainPageLoaded(object sender, RoutedEventArgs e)
		{
			InitializePlayer();
			Renovate();

			topPlayersList.ItemsSource = _playerPositions;
			awardsList.ItemsSource = _playerAwards;
		}

		private void PivotLoadedPivotItem(object sender, PivotItemEventArgs e)
		{
			if (CheckValidPlayer() && mainPivot.SelectedIndex == 1)
			{
				InitPlayerAwards();
			}
			else if (mainPivot.SelectedIndex == 2)
			{
				OrderByLevel();
			}
		}

		#region initialization functions
		private void InitializePhoneGuid()
		{
			if (settings.Contains("guid"))
			{
				_phoneGuid = (Guid)settings["guid"];
			}
			else
			{
				_phoneGuid = Guid.NewGuid();
				settings.Add("guid", _phoneGuid);
				settings.Save();
			}
		}

		private void InitializePlaySounds()
		{
			if (settings.Contains("sound"))
			{
				_playSounds = (bool)settings["sound"];
			}
			else
			{
				_playSounds = true;
				settings.Add("sound", _phoneGuid);
				settings.Save();
			}

			Sounds.IsChecked = _playSounds;
		}

		private void InitializePlayer()
		{
			if (settings.Contains("player"))
			{
				_playerName = (string)settings["player"];

				if (settings.Contains(_playerName + "save"))
				{
					_player = new Player((Player)settings[_playerName + "save"]);
				}
				else
				{
					_player = new Player(_playerName);
					settings.Add(_playerName + "save", _player);
					settings.Save();
				}

				PlayerName.Text = _player.Name;
				PlayerNameTextBox.Text = _player.Name;

				Score.Text = ScoreText.Text = "Bubbles popped so far: " + _player.BubblePopped;
				Level.Text = LevelText.Text = "Level: " + _player.Level;

				InitPlayerAwards();

				mainPivot.SelectedIndex = 0;
			}
			else
			{
				//init new player
				mainPivot.SelectedIndex = 1;
			}
		}


		void InitPlayerAwards()
		{
			_playerAwards.Clear();

			_playerAwards.Add(_player.Level >= 10
			                  	? new Awards("You've reached 10th level",
			                  	             new BitmapImage(new Uri("pack:\\Image\\Awards\\0_silver_plain.png")))
			                  	: new Awards("Reach 10th level to get!",
			                  	             new BitmapImage(new Uri("pack:\\Image\\Awards\\0_silver_plain_gray.png"))));

			_playerAwards.Add(_player.BubblePopped >= 500
			                  	? new Awards("You've popped 500 bubbles",
			                  	             new BitmapImage(new Uri("pack:\\Image\\Awards\\1_silver_stripe.png")))
			                  	: new Awards("Pop 500 bubbles to get!",
			                  	             new BitmapImage(new Uri("pack:\\Image\\Awards\\1_silver_stripe_gray.png"))));

			_playerAwards.Add(_player.Level >= 50
			                  	? new Awards("You've reached 50th level",
			                  	             new BitmapImage(new Uri("pack:\\Image\\Awards\\2_silver_star.png")))
			                  	: new Awards("Reach 50th level to get!",
			                  	             new BitmapImage(new Uri("pack:\\Image\\Awards\\2_silver_star_gray.png"))));

			_playerAwards.Add(_player.BubblePopped >= 1000
			                  	? new Awards("You've popped 1000 bubbles",
			                  	             new BitmapImage(new Uri("pack:\\Image\\Awards\\3_silver_stripe_star.png")))
			                  	: new Awards("Pop 1000 bubbles to get!",
			                  	             new BitmapImage(new Uri("pack:\\Image\\Awards\\3_silver_stripe_star_gray.png"))));

			_playerAwards.Add(_player.Level >= 75
			                  	? new Awards("You've reached 75th level",
			                  	             new BitmapImage(new Uri("pack:\\Image\\Awards\\4_gold_plain.png")))
			                  	: new Awards("Reach 75th level to get!",
			                  	             new BitmapImage(new Uri("pack:\\Image\\Awards\\4_gold_plain_gray.png"))));


			_playerAwards.Add(_player.BubblePopped >= 5000
			                  	? new Awards("You've popped 5000 bubbles",
			                  	             new BitmapImage(new Uri("pack:\\Image\\Awards\\5_gold_star.png")))
			                  	: new Awards("Pop 5000 bubbles to get!",
			                  	             new BitmapImage(new Uri("pack:\\Image\\Awards\\5_gold_star_gray.png"))));

			_playerAwards.Add(_player.Level >= 100
			                  	? new Awards("You've reached 100th level",
			                  	             new BitmapImage(new Uri("pack:\\Image\\Awards\\6_gold_stripe.png")))
			                  	: new Awards("Reach 100th level to get!",
			                  	             new BitmapImage(new Uri("pack:\\Image\\Awards\\6_gold_stripe_gray.png"))));


			_playerAwards.Add(_player.BubblePopped >= 10000
			                  	? new Awards("You've popped 10000 bubbles",
			                  	             new BitmapImage(new Uri("pack:\\Image\\Awards\\7_gold_badge.png")))
			                  	: new Awards("Pop 10000 bubbles to get!",
			                  	             new BitmapImage(new Uri("pack:\\Image\\Awards\\7_gold_badge_gray.png"))));

		}

		#endregion

		#region bubble popping code


		private void ImageManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
		{
			PopTheBubble(sender);
		}

		private void ImageMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			PopTheBubble(sender);
		}

		private void PopTheBubble(object sender)
		{
			var image = sender as Image;
			if (image == null)
				return;
			var bitmapImage = image.Source as BitmapImage;
			if (bitmapImage == null)
				return;

			if (
				String.Compare(bitmapImage.UriSource.ToString(), "pack:\\Image\\full1.png", StringComparison.OrdinalIgnoreCase) !=
				0 &&
				String.Compare(bitmapImage.UriSource.ToString(), "pack:\\Image\\full2.png", StringComparison.OrdinalIgnoreCase) !=
				0 &&
				String.Compare(bitmapImage.UriSource.ToString(), "pack:\\Image\\full3.png", StringComparison.OrdinalIgnoreCase) !=
				0 &&
				String.Compare(bitmapImage.UriSource.ToString(), "pack:\\Image\\full4.png", StringComparison.OrdinalIgnoreCase) !=
				0 &&
				String.Compare(bitmapImage.UriSource.ToString(), "pack:\\Image\\full5.png", StringComparison.OrdinalIgnoreCase) !=
				0 &&
				String.Compare(bitmapImage.UriSource.ToString(), "pack:\\Image\\full6.png", StringComparison.OrdinalIgnoreCase) !=
				0
				)
				return;

			switch (_random.Next(5))
			{
				case 0:
					image.Source = new BitmapImage(new Uri("pack:\\Image\\empty1.png"));
					break;
				case 1:
					image.Source = new BitmapImage(new Uri("pack:\\Image\\empty2.png"));
					break;
				case 2:
					image.Source = new BitmapImage(new Uri("pack:\\Image\\empty3.png"));
					break;
				case 3:
					image.Source = new BitmapImage(new Uri("pack:\\Image\\empty4.png"));
					break;
				case 4:
					image.Source = new BitmapImage(new Uri("pack:\\Image\\empty5.png"));
					break;
				case 5:
					image.Source = new BitmapImage(new Uri("pack:\\Image\\empty6.png"));
					break;
			}

			//update score
			if (_player != null)
			{
				_player.BubblePopped++;
				settings[_playerName + "save"] = _player;
				settings.Save();
				Score.Text = ScoreText.Text = "Bubbles popped so far: " + _player.BubblePopped;

				switch (_player.BubblePopped)
				{
					case 500:
						MessageBox.Show("500 bubbles popped", "New Award!", MessageBoxButton.OK);
						break;
					case 1000:
						MessageBox.Show("1000 bubbles popped", "New Award!", MessageBoxButton.OK);
						break;
					case 5000:
						MessageBox.Show("5000 bubbles popped", "New Award!", MessageBoxButton.OK);
						break;
					case 10000:
						MessageBox.Show("10000 bubbles popped", "New Award!", MessageBoxButton.OK);
						break;
				}
			}
			//play sound
			if (_playSounds)
			{
				Stream stream = null;
				switch (_random.Next(8))
				{
					case 0:
						stream = TitleContainer.OpenStream(".\\Sound\\bw-pop1.wav");
						break;
					case 1:
						stream = TitleContainer.OpenStream(".\\Sound\\bw-pop2.wav");
						break;
					case 2:
						stream = TitleContainer.OpenStream(".\\Sound\\bw-pop3.wav");
						break;
					case 3:
						stream = TitleContainer.OpenStream(".\\Sound\\bw-pop4.wav");
						break;
					case 4:
						stream = TitleContainer.OpenStream(".\\Sound\\bw-pop5.wav");
						break;
					case 5:
						stream = TitleContainer.OpenStream(".\\Sound\\bw-pop6.wav");
						break;
					case 6:
						stream = TitleContainer.OpenStream(".\\Sound\\bw-pop7.wav");
						break;
					case 7:
						stream = TitleContainer.OpenStream(".\\Sound\\bw-pop8.wav");
						break;
					case 8:
						stream = TitleContainer.OpenStream(".\\Sound\\bw-pop9.wav");
						break;
				}

				if (stream == null)
					return;

				var effect = SoundEffect.FromStream(stream);
				FrameworkDispatcher.Update();
				effect.Play();
			}

			if (!AllPopped()) return;

			if (_dt.IsEnabled)
			{
				_dt.Stop();

				MessageBox.Show("Congratulations you win!" + Environment.NewLine + "Advance to the next level!", "Victory!", MessageBoxButton.OK);

				_player.Level++;
				settings[_playerName + "save"] = _player;
				settings.Save();
				Level.Text = LevelText.Text = "Level: " + _player.Level;

				switch (_player.Level)
				{
					case 10:
						MessageBox.Show("10th level reached!", "New Award!", MessageBoxButton.OK);
						break;
					case 50:
						MessageBox.Show("50th level reached!", "New Award!", MessageBoxButton.OK);
						break;
					case 75:
						MessageBox.Show("75th level reached!", "New Award!", MessageBoxButton.OK);
						break;
					case 100:
						MessageBox.Show("100th level reached!", "New Award!", MessageBoxButton.OK);
						break;
				}

				timerOutput.Text = "Timer";
			}

			Renovate();
		}

		public bool AllPopped()
		{
			var count = 0;
			foreach (var image in bubbleField.Children.Select(child => child as Image))
			{
				if (image == null)
					return false;
				var bitmapImage = image.Source as BitmapImage;
				if (bitmapImage == null)
					return false;

				if (
					String.Compare(bitmapImage.UriSource.ToString(), "pack:\\Image\\full1.png", StringComparison.OrdinalIgnoreCase) != 0 &&
					String.Compare(bitmapImage.UriSource.ToString(), "pack:\\Image\\full2.png", StringComparison.OrdinalIgnoreCase) != 0 &&
					String.Compare(bitmapImage.UriSource.ToString(), "pack:\\Image\\full3.png", StringComparison.OrdinalIgnoreCase) != 0 &&
					String.Compare(bitmapImage.UriSource.ToString(), "pack:\\Image\\full4.png", StringComparison.OrdinalIgnoreCase) != 0 &&
					String.Compare(bitmapImage.UriSource.ToString(), "pack:\\Image\\full5.png", StringComparison.OrdinalIgnoreCase) != 0 &&
					String.Compare(bitmapImage.UriSource.ToString(), "pack:\\Image\\full6.png", StringComparison.OrdinalIgnoreCase) != 0
				)
					count++;
			}

			return count == 38;
		}

		public bool Renovate()
		{
			foreach (var image in bubbleField.Children.Select(child => child as Image))
			{
				if (image == null)
					return false;
				switch (_random.Next(5))
				{
					case 0:
						image.Source = new BitmapImage(new Uri("pack:\\Image\\full1.png"));
						break;
					case 1:
						image.Source = new BitmapImage(new Uri("pack:\\Image\\full2.png"));
						break;
					case 2:
						image.Source = new BitmapImage(new Uri("pack:\\Image\\full3.png"));
						break;
					case 3:
						image.Source = new BitmapImage(new Uri("pack:\\Image\\full4.png"));
						break;
					case 4:
						image.Source = new BitmapImage(new Uri("pack:\\Image\\full5.png"));
						break;
					case 5:
						image.Source = new BitmapImage(new Uri("pack:\\Image\\full6.png"));
						break;
				}
				
			}
			return true;
		}

		#endregion

		#region player setup
		private void SetNameButtonClick(object sender, RoutedEventArgs e)
		{
			if (CheckValidPlayer())
			{
				MessageBox.Show("Player's name set!");
			}
		}

		private bool CheckValidPlayer()
		{
			var playerNameString = PlayerNameTextBox.Text;

			//trim player name string
			playerNameString = playerNameString.Trim();

			//player name should not be empty string
			if (String.IsNullOrEmpty(playerNameString))
			{
				MessageBox.Show("Player's name could not be empty.");
				return false;
			}
			//player name could not be default name "Player"
			if (String.Compare(playerNameString, "Player", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				MessageBox.Show("You can't set default name \"Player\".");
				return false;
			}

			//set players name everywhere
			{
				_playerName = playerNameString;
				if (settings.Contains("player"))
				{
					settings["player"] = _playerName;
				}
				else
				{
					settings.Add("player", _playerName);
				}
				settings.Save();
				PlayerName.Text = _playerName;
				PlayerNameTextBox.Text = _playerName;
			}

			//everything is OK, initialize player
			if (settings.Contains(_playerName + "save"))
			{
				_player = new Player((Player)settings[_playerName + "save"]);
			}
			else
			{
				_player = new Player(_playerName);
				settings.Add(_playerName + "save", _player);
				settings.Save();
			}
			Score.Text = ScoreText.Text = "Bubbles popped so far: " + _player.BubblePopped;
			Level.Text = LevelText.Text = "Level: " + _player.Level;

			InitPlayerAwards();

			return true;
		}
		#endregion

		#region More button click and grid showing function
		private void MoreButtonClick(object sender, RoutedEventArgs e)
		{
			if (CheckValidPlayer())
			{
				if (!Renovate())
					return;

				if (_playSounds)
				{
					var stream = TitleContainer.OpenStream(".\\Sound\\reload.wav");
					var effect = SoundEffect.FromStream(stream);
					FrameworkDispatcher.Update();
					effect.Play();
				}

				_seconds = 100 - _player.Level;
				if (_seconds < 6)
					_seconds = 6;
				_dt.Start();
			}
		}

		void DtTick(object sender, EventArgs e)
		{
			_seconds--;
			timerOutput.Dispatcher.BeginInvoke(delegate { timerOutput.Text = "Time left: "+ _seconds; });

			if (_seconds != 0)
				return;

			if (!AllPopped())
			{
				MessageBox.Show("Time is up!" + Environment.NewLine + "Sorry, you failed!", "Fail!", MessageBoxButton.OK);
			}
			timerOutput.Dispatcher.BeginInvoke(delegate { timerOutput.Text = "Timer"; });
			_dt.Stop();
		}

		#endregion

		#region sound control
		private void SoundsChecked(object sender, RoutedEventArgs e)
		{
			_playSounds = true;
			settings["sound"] = _playSounds;
			settings.Save();
		}

		private void SoundsUnchecked(object sender, RoutedEventArgs e)
		{
			_playSounds = false;
			settings["sound"] = _playSounds;
			settings.Save();
		}
		#endregion

		#region azure intergration


		#region Bubble
		private void OrderBubblesClick(object sender, RoutedEventArgs e)
		{
			OrderByBubbles();
		}

		void OrderByBubbles()
		{
			if (_player != null)
				SubmitScoreBubbles();
		}

		void SubmitScoreBubbles()
		{
			var id = UserName + ',' + Password + ',' + Datasource + ',' + DatabaseName + ',' + TableName + ','
				 + _phoneGuid + ',' + _player.Name + ',' + _player.BubblePopped + ',' + _player.Level;
			var client = new WebClient();
			client.DownloadStringCompleted += SubmitScoreBubblesCompleted;
			client.DownloadStringAsync(new Uri("http://databasecloud.cloudapp.net/home/SaveScoreUltimate/" + id));
		}

		void SubmitScoreBubblesCompleted(object sender, DownloadStringCompletedEventArgs args)
		{
			try
			{
				GetBubblePosition();
			}
			catch (Exception)
			{
				MessageBox.Show("Can't submit the scores to server.");
			}
		}

		void GetBubblePosition()
		{
			var id = UserName + ',' + Password + ',' + Datasource + ',' + DatabaseName + ',' + TableName +
				',' + _player.Name + ',' + _phoneGuid;
			var client = new WebClient();
			client.DownloadStringCompleted += GetBubblePositionCompleted;
			client.DownloadStringAsync(new Uri("http://databasecloud.cloudapp.net/home/YourBubbleUltimate/" + id));
		}

		void GetBubblePositionCompleted(object sender, DownloadStringCompletedEventArgs args)
		{
			try
			{
				if (String.IsNullOrEmpty(args.Result))
					return;

				var result = args.Result.Split(',');

				playerPosition.DataContext = new PlayerPosition(_player.Name, _player.BubblePopped, _player.Level, Int32.Parse(result[0]));

				MakeBubbleList();
			}
			catch (Exception)
			{
				MessageBox.Show("Can't connect to server.");
			}
		}

		void MakeBubbleList()
		{
			var id = UserName + ',' + Password + ',' + Datasource + ',' + DatabaseName + ',' + TableName;
			var client = new WebClient();
			client.DownloadStringCompleted += MakeBubbleListCompleted;
			client.DownloadStringAsync(new Uri("http://databasecloud.cloudapp.net/home/TopBubbleUltimate/" + id));
		}

		void MakeBubbleListCompleted(object sender, DownloadStringCompletedEventArgs args)
		{
			try
			{
				if (String.IsNullOrEmpty(args.Result))
					return;

				var result = args.Result.Split(',');
				var position = result.Length / 3;
				_playerPositions.Clear();
				for (var i = 0; i < position; i++)
				{
					_playerPositions.Add(new PlayerPosition(result[i * 3], Int32.Parse(result[i * 3 + 1]), Int32.Parse(result[i * 3 + 2]), i + 1));
				}

				topPlayersList.ItemsSource = _playerPositions;
			}
			catch (Exception)
			{
				MessageBox.Show("Can't connect to server.");
			}
		} 
		#endregion


		#region Level
		private void OrderLevelClick(object sender, RoutedEventArgs e)
		{
			OrderByLevel();
		}

		void OrderByLevel()
		{
			if (_player != null)
				SubmitScoreLevel();
		}

		void SubmitScoreLevel()
		{
			var id = UserName + ',' + Password + ',' + Datasource + ',' + DatabaseName + ',' + TableName + ','
				 + _phoneGuid + ',' + _player.Name + ',' + _player.BubblePopped + ',' + _player.Level;
			var client = new WebClient();
			client.DownloadStringCompleted += SubmitScoreLevelCompleted;
			client.DownloadStringAsync(new Uri("http://databasecloud.cloudapp.net/home/SaveScoreUltimate/" + id));
		}

		void SubmitScoreLevelCompleted(object sender, DownloadStringCompletedEventArgs args)
		{
			try
			{
				GetLevelPosition();
			}
			catch (Exception)
			{
				MessageBox.Show("Can't submit the scores to server.");
			}
		}

		void GetLevelPosition()
		{
			var id = UserName + ',' + Password + ',' + Datasource + ',' + DatabaseName + ',' + TableName + 
				',' + _player.Name + ',' + _phoneGuid;
			var client = new WebClient();
			client.DownloadStringCompleted += GetLevelPositionCompleted;
			client.DownloadStringAsync(new Uri("http://databasecloud.cloudapp.net/home/YourLevelUltimate/" + id));
		}

		void GetLevelPositionCompleted(object sender, DownloadStringCompletedEventArgs args)
		{
			try
			{
				if (String.IsNullOrEmpty(args.Result))
					return;

				var result = args.Result.Split(',');

				playerPosition.DataContext = new PlayerPosition(_player.Name, _player.BubblePopped, _player.Level, Int32.Parse(result[0]));

				MakeLevelList();
			}
			catch (Exception)
			{
				MessageBox.Show("Can't connect to server.");
			}
		}

		void MakeLevelList()
		{
			var id = UserName + ',' + Password + ',' + Datasource + ',' + DatabaseName + ',' + TableName;
			var client = new WebClient();
			client.DownloadStringCompleted += MakeLevelListCompleted;
			client.DownloadStringAsync(new Uri("http://databasecloud.cloudapp.net/home/TopLevelUltimate/" + id));
		}

		void MakeLevelListCompleted(object sender, DownloadStringCompletedEventArgs args)
		{
			try
			{
				if (String.IsNullOrEmpty(args.Result))
					return;

				var result = args.Result.Split(',');
				var position = result.Length / 3;
				_playerPositions.Clear();
				for (var i = 0; i < position; i++)
				{
					_playerPositions.Add(new PlayerPosition(result[i * 3], Int32.Parse(result[i * 3 + 1]), Int32.Parse(result[i * 3 + 2]), i + 1));
				}

				topPlayersList.ItemsSource = _playerPositions;
			}
			catch (Exception)
			{
				MessageBox.Show("Can't connect to server.");
			}
		} 
		#endregion
		#endregion

	}

	[DataContract]
	public class Player
	{
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public int BubblePopped { get; set; }
		[DataMember]
		public int Level { get; set; }

		public Player(Player player)
		{
			Name = player.Name;
			BubblePopped = player.BubblePopped;
			Level = player.Level;
		}

		public Player(string name, int bubblePopped = 0, int level = 0)
		{
			Name = name;
			BubblePopped = bubblePopped;
			Level = level;
		}
	}

	public class PlayerPosition : Player
	{
		public int Position { get; set; }

		public Visibility AwardVisible1 { get; set; }
		public Visibility AwardVisible2 { get; set; }
		public Visibility AwardVisible3 { get; set; }
		public Visibility AwardVisible4 { get; set; }
		public Visibility AwardVisible5 { get; set; }
		public Visibility AwardVisible6 { get; set; }
		public Visibility AwardVisible7 { get; set; }
		public Visibility AwardVisible8 { get; set; }

		public PlayerPosition(string name, int bubblePopped = 0, int level = 0, int position = 0):
			base(name, bubblePopped, level)
		{
			Position = position;

			AwardVisible1 = level >= 10 ? Visibility.Visible : Visibility.Collapsed;

			AwardVisible2 = bubblePopped >= 500 ? Visibility.Visible : Visibility.Collapsed;

			AwardVisible3 = level >= 50 ? Visibility.Visible : Visibility.Collapsed;

			AwardVisible4 = bubblePopped >= 1000 ? Visibility.Visible : Visibility.Collapsed;

			AwardVisible5 = level >= 75 ? Visibility.Visible : Visibility.Collapsed;

			AwardVisible6 = bubblePopped >= 5000 ? Visibility.Visible : Visibility.Collapsed;

			AwardVisible7 = level >= 100 ? Visibility.Visible : Visibility.Collapsed;

			AwardVisible8 = bubblePopped >= 10000 ? Visibility.Visible : Visibility.Collapsed;
		}
	}


	public class Awards
	{
		public string AwardName { get; set; }

		public BitmapImage AwardImage { get; set; }

		public Awards(string awardName, BitmapImage awardImage)
		{
			AwardName = awardName;
			AwardImage = awardImage;
		}
	}
}