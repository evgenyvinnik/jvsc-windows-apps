using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace BubbleWrap
{
	public partial class MainPage
	{
		#region Variables
		private readonly Random _random;

		IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
		List<PlayerPosition> _playerPositions;

		#region main grids (dialogs) enum
		private enum Grids
		{
			LayoutRootGrid,
			SettingsGrid,
			TopPlayersGrid
		}
		#endregion

		#region loadable variables
		private int _score;
		private string _playerName;
		private int _attempts;
		private Guid _phoneGuid;

		private Grids _currentGrid = Grids.LayoutRootGrid;
		#endregion

		#region constants
		const string UserName = "WindowsPhone7User";
		const string Password = "Upiter!7Saturn";
		const string Datasource = "hjh7y1qekw.database.windows.net";
		const string DatabaseName = "BubbleWrapTopScore";
		const string TableName = "BubbleWrapTopScore";
		#endregion

		#endregion

		// Constructor
		public MainPage()
		{
			_playerPositions = new List<PlayerPosition>();

			InitializeComponent();

			//make new random variable
			_random = new Random();


			InitializePhoneGuid();
			InitializePlayerName();
		}


		#region initialization functions
		private void InitializePhoneGuid()
		{
			if (settings.Contains("guid"))
			{
				_phoneGuid = (Guid) settings["guid"];
			}
			else
			{
				_phoneGuid = Guid.NewGuid();
				settings.Add("guid", _phoneGuid);
			}
		}

		private void InitializePlayerName()
		{
			if (settings.Contains("player"))
			{
				_playerName = (string) settings["player"];

				PlayerName.Text = _playerName;
				PlayerNameTextBox.Text = _playerName;

				InitializeScore();
				InitializeAttempts();
				ShowGrid(Grids.LayoutRootGrid);
			}
			else
			{
				//init new player
				ShowGrid(Grids.SettingsGrid);
			}

		}

		private void InitializeScore()
		{
			if (settings.Contains(_playerName + "score"))
			{
				_score = (int)settings[_playerName + "score"];
			}
			else
			{
				_score = 0;
				settings.Add(_playerName + "score", 0);
			}
			Score.Text = ScoreText.Text = "Bubbles popped so far: " + _score;

		}

		private void InitializeAttempts()
		{
			if (settings.Contains(_playerName + "attempts"))
			{
				_attempts = (int)settings[_playerName + "attempts"];
			}
			else
			{
				_attempts = 0;
				settings.Add(_playerName +"attempts", 0);
			}

			settings[_playerName + "attempts"] = _attempts++;
			Attempts.Text = "You had " + _attempts + " attempts";
		}
		#endregion

		#region bubble popping code
		private void ImageMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var image = sender as Image;
			if (image == null)
				return;
			var bitmapImage = image.Source as BitmapImage;
			var originalImage = original.Source as BitmapImage;
			if (bitmapImage == null)
				return;
			if (originalImage == null)
				return;
			if (bitmapImage.UriSource != originalImage.UriSource)
				return;

			switch (_random.Next(5))
			{
				case 0:
					image.Source = pushed1.Source;
					break;
				case 1:
					image.Source = pushed2.Source;
					break;
				case 2:
					image.Source = pushed3.Source;
					break;
				case 3:
					image.Source = pushed4.Source;
					break;
				case 4:
					image.Source = pushed5.Source;
					break;
				case 5:
					image.Source = pushed6.Source;
					break;
			}


			//update score
			settings[_playerName + "score"] = _score++;
			Score.Text = ScoreText.Text = "Bubbles popped so far: " + _score;
			//play sound

			Stream stream = null;
			switch (_random.Next(5))
			{
				case 0:
					stream = TitleContainer.OpenStream("sound 64.wav");
					break;
				case 1:
					stream = TitleContainer.OpenStream("sound 67.wav");
					break;
				case 2:
					stream = TitleContainer.OpenStream("sound 70.wav");
					break;
				case 3:
					stream = TitleContainer.OpenStream("sound 73.wav");
					break;
				case 4:
					stream = TitleContainer.OpenStream("sound 76.wav");
					break;
				case 5:
					stream = TitleContainer.OpenStream("sound 79.wav");
					break;
			}

			if (stream == null)
				return;

			var effect = SoundEffect.FromStream(stream);
			FrameworkDispatcher.Update();
			effect.Play();
		}

		public bool Renovate()
		{
			if (LayoutRoot.Visibility == Visibility.Collapsed)
			{
				if (CheckValidPlayer())
					ShowGrid(Grids.LayoutRootGrid);
				else
				{
					return false;
				}
			}

			foreach (var image in LayoutRoot.Children.Select(child => child as Image))
			{
				if (image == null)
					return false;
				var bitmapImage = image.Source as BitmapImage;
				var originalImage = original.Source as BitmapImage;
				if (bitmapImage == null)
					return false;
				if (originalImage == null)
					return false;
				image.Source = original.Source;
			}

			settings[_playerName + "attempts"] = _attempts++;
			Attempts.Text = "You had " + _attempts + " attempts";

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
				PlayerName.Text = _playerName;
				PlayerNameTextBox.Text = _playerName;
			}

			//everything is OK, initialize players attempts and scores and return true
			InitializeScore();
			InitializeAttempts();

			return true;
		}
		#endregion

		#region service buttons click
		private void SettingsButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			ShowGrid(Grids.SettingsGrid);
		}

		private void TopButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (!CheckValidPlayer())
				return;

			_playerPositions.Clear();
			topPlayersList.ItemsSource = null;
			topPlayersList.Items.Clear();

			ShowGrid(Grids.TopPlayersGrid);

			SubmitScore();
		}
		#endregion

		#region More button click and grid showing function
		private void MoreButtonClick(object sender, RoutedEventArgs e)
		{
			switch (_currentGrid)
			{
				case Grids.LayoutRootGrid:
					//do nothing, just renovate
					break;
				case Grids.SettingsGrid:
					if (CheckValidPlayer())
						ShowGrid(Grids.LayoutRootGrid);
					else
						return;
					break;
				case Grids.TopPlayersGrid:
					ShowGrid(Grids.LayoutRootGrid);
					break;
			}

			//put new bubbles
			if (!Renovate())
				return;

			//play sound
			var stream = TitleContainer.OpenStream("sound 1.wav");
			var effect = SoundEffect.FromStream(stream);
			FrameworkDispatcher.Update();
			effect.Play();
		}

		private void ShowGrid(Grids grid)
		{
			//pressed the same button
			if (_currentGrid == grid)
				return;

			//make all invisible
			LayoutRoot.Visibility = Visibility.Collapsed;
			Settings.Visibility = Visibility.Collapsed;
			TopPlayers.Visibility = Visibility.Collapsed;

			_currentGrid = grid;

			//make one visible
			switch (grid)
			{
				case Grids.LayoutRootGrid:
					LayoutRoot.Visibility = Visibility.Visible;
					MoreButton.Content = "Want more!";
					break;
				case Grids.SettingsGrid:
					Settings.Visibility = Visibility.Visible;
					MoreButton.Content = "Back to game!";
					break;
				case Grids.TopPlayersGrid:
					TopPlayers.Visibility = Visibility.Visible;
					MoreButton.Content = "Back to game!";
					break;
			}
		}
		#endregion

		#region azure intergration
		void SubmitScore()
		{
			var id = UserName + ',' + Password + ',' + Datasource + ',' + DatabaseName + ',' + TableName + ','
				 + _phoneGuid + ',' + PlayerName.Text + ',' + _score + ',' + _attempts;
			var client = new WebClient();
			client.DownloadStringCompleted += SubmitScoreCompleted;
			client.DownloadStringAsync(new Uri("http://databasecloud.cloudapp.net/home/SaveScore/" + id));
		}

		void SubmitScoreCompleted(object sender, DownloadStringCompletedEventArgs args)
		{
			try
			{
				//MessageBox.Show(args.Result);

				MakeTopList();
			}
			catch (Exception)
			{
				//MessageBox.Show("Can't submit the scores to server.");
			}
		}

		void MakeTopList()
		{
			var id = UserName + ',' + Password + ',' + Datasource + ',' + DatabaseName + ',' + TableName;
			var client = new WebClient();
			client.DownloadStringCompleted += MakeTopListCompleted;
			client.DownloadStringAsync(new Uri("http://databasecloud.cloudapp.net/home/TopScore/" + id));
		}

		void MakeTopListCompleted(object sender, DownloadStringCompletedEventArgs args)
		{
			try
			{
				if (String.IsNullOrEmpty(args.Result))
					return;

				var result = args.Result.Split(',');
				var position = result.Length/3;
				_playerPositions.Clear();
				for (var i = 0; i < position; i++)
				{
					_playerPositions.Add(new PlayerPosition(i+1, result[i * 3], result[i * 3 + 1], result[i * 3 + 2]));
				}

				topPlayersList.ItemsSource = _playerPositions;
			}
			catch (Exception)
			{
				MessageBox.Show("Can't connect to server.");
			}
		}



		#endregion
	}

	public class PlayerPosition
	{
		public int Position { get; set; }
		public string Name { get; set; }
		public string Score { get; set; }
		public string Attempts { get; set; }

		public PlayerPosition(int position, string name, string score, string attempts)
		{
			Position = position;
			Name = name;
			Score = score;
			Attempts = attempts;
		}
	}
}