using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Windows;
using System.Windows.Media;
using Microsoft.Phone.Shell;
using System.Windows.Controls;

using Microsoft.Live;
using Microsoft.Live.Controls;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Controls;

namespace Sfxr
{
	public partial class MainPage
	{
		#region variables

		private readonly SfxrSynth _synth;  				// Synthesizer instance
		//private uint _sampleRate = 44100;  		// Sample rate to export .wav at
		//private uint _bitDepth = 16;			// Bit depth to export .wav at

		private bool _playOnChange = true;	   	// If the sound should be played after releasing a slider or changing type
		private bool _mutePlayOnChange = true;		// If the change playing should be muted because of non-user changes

		private List<SfxrParams> _history; // List of generated settings
		private int _historyPos;


		// SkyDrive session
		private LiveConnectClient client;
		#endregion

		// Constructor
		public MainPage()
		{
			_synth = new SfxrSynth();

			InitializeComponent();

			_history = new List<SfxrParams>();


		}



		// Load data for the ViewModel Items
		private void MainPageLoaded(object sender, RoutedEventArgs e)
		{
			_mutePlayOnChange = false;

			//set application bar icons
			{
				var themeColor = (Color)Application.Current.Resources["PhoneForegroundColor"];

				if (themeColor.ToString() == "#FFFFFFFF")
				{
					((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IconUri = new Uri("/Image/dark/mutate.png",
																							   UriKind.Relative);

					((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IconUri = new Uri("/Image/dark/randomize.png",
																							   UriKind.Relative);

					((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IconUri = new Uri("/Image/dark/save.png",
																							   UriKind.Relative);

					((ApplicationBarIconButton)ApplicationBar.Buttons[3]).IconUri = new Uri("/Image/dark/undo.png",
																							   UriKind.Relative);
				}
				else if (themeColor.ToString() == "#DE000000")
				{
					((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IconUri = new Uri("/Image/light/mutate.png",
																							   UriKind.Relative);

					((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IconUri = new Uri("/Image/light/randomize.png",
																							   UriKind.Relative);

					((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IconUri = new Uri("/Image/light/save.png",
																							   UriKind.Relative);

					((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IconUri = new Uri("/Image/light/undo.png",
																							   UriKind.Relative);
				}
			}
		}

		#region Button clicks
		private void PickupCoinButtonClick(object sender, RoutedEventArgs e)
		{
			AddToHistory();
			_synth.Param.GeneratePickupCoin();
			UpdateSliders();
			UpdateButtons();
			_synth.Play();
		}

		private void LaserShootButtonClick(object sender, RoutedEventArgs e)
		{
			AddToHistory();
			_synth.Param.GenerateLaserShoot();
			UpdateSliders();
			UpdateButtons();
			_synth.Play();
		}

		private void ExplosionButtonClick(object sender, RoutedEventArgs e)
		{
			AddToHistory();
			_synth.Param.GenerateExplosion();
			UpdateSliders();
			UpdateButtons();
			_synth.Play();

		}

		private void PowerupButtonClick(object sender, RoutedEventArgs e)
		{
			AddToHistory();
			_synth.Param.GeneratePowerup();
			UpdateSliders();
			UpdateButtons();
			_synth.Play();

		}

		private void HitHurtButtonClick(object sender, RoutedEventArgs e)
		{
			AddToHistory();
			_synth.Param.GenerateHitHurt();
			UpdateSliders();
			UpdateButtons();
			_synth.Play();

		}

		private void JumpButtonClick(object sender, RoutedEventArgs e)
		{
			AddToHistory();
			_synth.Param.GenerateJump();
			UpdateSliders();
			UpdateButtons();
			_synth.Play();

		}

		private void BlipSelectButtonClick(object sender, RoutedEventArgs e)
		{
			AddToHistory();
			_synth.Param.GenerateBlipSelect();
			UpdateSliders();
			UpdateButtons();
			_synth.Play();

		}
		#endregion

		#region menu item clicks
		private void MutateButtonClick(object sender, EventArgs e)
		{
			AddToHistory();
			_synth.Param.Mutate();
			UpdateSliders();
			UpdateButtons();
			_synth.Play();
		}

		private void RandomizeButtonClick(object sender, EventArgs e)
		{
			AddToHistory();
			_synth.Param.Randomize();
			UpdateSliders();
			UpdateButtons();
			_synth.Play();
		}

		private void UndoButtonClick(object sender, EventArgs e)
		{
			_historyPos--;
			if (_historyPos == 0)
				((ApplicationBarIconButton)ApplicationBar.Buttons[3]).IsEnabled = false;
			if (_historyPos < _history.Count - 1)
				((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).IsEnabled = true;

			_synth.Param = _history[_historyPos];

			UpdateSliders();
			UpdateButtons();

			_synth.Play();
		}

		private void RedoButtonClick(object sender, EventArgs e)
		{
			_historyPos++;
			if (_historyPos > 0)
				((ApplicationBarIconButton)ApplicationBar.Buttons[3]).IsEnabled = true;
			if (_historyPos == _history.Count - 1)
				((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).IsEnabled = false;

			_synth.Param = _history[_historyPos];

			UpdateSliders();
			UpdateButtons();

			_synth.Play();

		}


		private void HelpAboutMenuItemClick(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
		}

		#endregion

		#region supporting functions
		/**
		 * Adds a new sound effect to the history. 
		 * Called just before a new sound effect is generated.
		 */
		private void AddToHistory()
		{
			_historyPos++;
			_synth.Param = _synth.Param.Clone();
			if (_history.Count > _historyPos)
				_history = _history.GetRange(0, _historyPos);
			_history.Add(_synth.Param);

			((ApplicationBarIconButton)ApplicationBar.Buttons[3]).IsEnabled = true;
			((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).IsEnabled = false;

		}

		/**
		 * Updates the sliders to reflect the synthesizer
		 */
		private void UpdateSliders()
		{
			_mutePlayOnChange = true;

			AttackTimeSlider.Value = _synth.Param.AttackTime;
			SustainTimeSlider.Value = _synth.Param.SustainTime;
			SustainPunchSlider.Value = _synth.Param.SustainPunch;
			DecayTimeSlider.Value = _synth.Param.DecayTime;
			StartFrequencySlider.Value = _synth.Param.StartFrequency;
			MinFrequencySlider.Value = _synth.Param.MinFrequency;
			SlideSlider.Value = _synth.Param.Slide;
			DeltaSlideSlider.Value = _synth.Param.DeltaSlide;
			VibratoDepthSlider.Value = _synth.Param.VibratoDepth;
			VibratoSpeedSlider.Value = _synth.Param.VibratoSpeed;
			ChangeAmountSlider.Value = _synth.Param.ChangeAmount;
			ChangeSpeedSlider.Value = _synth.Param.ChangeSpeed;
			SquareDutySlider.Value = _synth.Param.SquareDuty;
			DutySweepSlider.Value = _synth.Param.DutySweep;
			RepeatSpeedSlider.Value = _synth.Param.RepeatSpeed;
			PhaserOffsetSlider.Value = 0;
			PhaserSweepSlider.Value = 0;
			LpFilterCutoffSlider.Value = 1.0;
			LpFilterCutoffSweepSlider.Value = 0;
			LpFilterResonanceSlider.Value = 0;
			HpFilterCutoffSlider.Value = 0;
			HpFilterCutoffSweepSlider.Value = 0;

			_mutePlayOnChange = false;
		}

		private void UpdateButtons()
		{
			_mutePlayOnChange = true;

			switch (_synth.Param.WaveType)
			{
				case 0:
					SquareWaveRadioButton.IsChecked = true;
					break;
				case 1:
					SawToothRadioButton.IsChecked = true;
					break;
				case 2:
					SineWaveRadioButton.IsChecked = true;
					break;
				case 3:
					NoiseRadioButton.IsChecked = true;
					break;
			}

			_mutePlayOnChange = false;
		}

		#endregion

		#region wave form methods
		private void SquareWaveRadioButtonClick(object sender, RoutedEventArgs e)
		{
			_synth.Param.WaveType = 0;
			AddToHistory();
			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();
		}

		private void SawToothRadioButtonClick(object sender, RoutedEventArgs e)
		{
			_synth.Param.WaveType = 1;
			AddToHistory();
			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();

		}

		private void SineWaveRadioButtonClick(object sender, RoutedEventArgs e)
		{
			_synth.Param.WaveType = 2;
			AddToHistory();
			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();

		}

		private void NoiseRadioButtonClick(object sender, RoutedEventArgs e)
		{
			_synth.Param.WaveType = 3;
			AddToHistory();
			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();

		}
		#endregion


		#region playbutton click
		private void PlayButtonClick(object sender, RoutedEventArgs e)
		{
			_synth.Play();
		}
		#endregion

		#region labels and sliders
		#region label clicks


		private void AttackTimeButtonClick(object sender, RoutedEventArgs e)
		{
			AttackTimeSlider.Value = 0;
		}

		private void SustainTimeButtonClick(object sender, RoutedEventArgs e)
		{
			SustainTimeSlider.Value = 0;
		}

		private void SustainPunchButtonClick(object sender, RoutedEventArgs e)
		{
			SustainPunchSlider.Value = 0;
		}

		private void DecayTimeButtonClick(object sender, RoutedEventArgs e)
		{
			DecayTimeSlider.Value = 0;
		}

		private void StartFrequencyButtonClick(object sender, RoutedEventArgs e)
		{
			StartFrequencySlider.Value = 0.5;
		}

		private void MinFrequencyButtonClick(object sender, RoutedEventArgs e)
		{
			MinFrequencySlider.Value = 0;
		}

		private void SlideButtonClick(object sender, RoutedEventArgs e)
		{
			SlideSlider.Value = 0;
		}

		private void DeltaSlideButtonClick(object sender, RoutedEventArgs e)
		{
			DeltaSlideSlider.Value = 0;
		}

		private void VibratoDepthButtonClick(object sender, RoutedEventArgs e)
		{
			VibratoDepthSlider.Value = 0;
		}

		private void VibratoSpeedButtonClick(object sender, RoutedEventArgs e)
		{
			VibratoSpeedSlider.Value = 0;
		}

		private void ChangeAmountButtonClick(object sender, RoutedEventArgs e)
		{
			ChangeAmountSlider.Value = 0;
		}

		private void ChangeSpeedButtonClick(object sender, RoutedEventArgs e)
		{
			ChangeSpeedSlider.Value = 0;
		}

		private void SquareDutyButtonClick(object sender, RoutedEventArgs e)
		{
			SquareDutySlider.Value = 0;
		}

		private void DutySweepButtonClick(object sender, RoutedEventArgs e)
		{
			DutySweepSlider.Value = 0;
		}

		private void RepeatSpeedButtonClick(object sender, RoutedEventArgs e)
		{
			RepeatSpeedSlider.Value = 0;
		}

		private void PhaserOffsetButtonClick(object sender, RoutedEventArgs e)
		{
			PhaserOffsetSlider.Value = 0;
		}

		private void PhaserSweepButtonClick(object sender, RoutedEventArgs e)
		{
			PhaserSweepSlider.Value = 0;
		}

		private void LpFilterCutoffButtonClick(object sender, RoutedEventArgs e)
		{
			LpFilterCutoffSlider.Value = 1.0;
		}

		private void LpFilterCutoffSweepButtonClick(object sender, RoutedEventArgs e)
		{
			LpFilterCutoffSweepSlider.Value = 0;
		}

		private void LpFilterResonanceClick(object sender, RoutedEventArgs e)
		{
			LpFilterResonanceSlider.Value = 0;
		}

		private void HpFilterCutoffClick(object sender, RoutedEventArgs e)
		{
			HpFilterCutoffSlider.Value = 0;
		}

		private void HpFilterCutoffSweepClick(object sender, RoutedEventArgs e)
		{
			HpFilterCutoffSweepSlider.Value = 0;
		}
		#endregion

		#region slider value changes
		private void AttackTimeSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var phoneSlider = sender as Slider;
			if (phoneSlider == null)
				return;

			_synth.Param.AttackTime = phoneSlider.Value;

			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();
		}

		private void SustainTimeSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var phoneSlider = sender as Slider;
			if (phoneSlider == null)
				return;

			_synth.Param.SustainTime = phoneSlider.Value;

			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();
		}

		private void SustainPunchSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var phoneSlider = sender as Slider;
			if (phoneSlider == null)
				return;

			_synth.Param.SustainPunch = phoneSlider.Value;

			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();
		}

		private void DecayTimeSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var phoneSlider = sender as Slider;
			if (phoneSlider == null)
				return;

			_synth.Param.DecayTime = phoneSlider.Value;

			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();
		}

		private void StartFrequencySliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var phoneSlider = sender as Slider;
			if (phoneSlider == null)
				return;

			_synth.Param.StartFrequency = phoneSlider.Value;

			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();
		}

		private void MinFrequencySliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var phoneSlider = sender as Slider;
			if (phoneSlider == null)
				return;

			_synth.Param.MinFrequency = phoneSlider.Value;

			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();
		}

		private void SlideSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var phoneSlider = sender as Slider;
			if (phoneSlider == null)
				return;

			_synth.Param.Slide = phoneSlider.Value;

			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();
		}

		private void DeltaSlideSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var phoneSlider = sender as Slider;
			if (phoneSlider == null)
				return;

			_synth.Param.DeltaSlide = phoneSlider.Value;

			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();
		}

		private void VibratoDepthSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var phoneSlider = sender as Slider;
			if (phoneSlider == null)
				return;

			_synth.Param.VibratoDepth = phoneSlider.Value;

			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();
		}

		private void VibratoSpeedSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var phoneSlider = sender as Slider;
			if (phoneSlider == null)
				return;

			_synth.Param.VibratoSpeed = phoneSlider.Value;

			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();
		}

		private void ChangeAmountSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var phoneSlider = sender as Slider;
			if (phoneSlider == null)
				return;

			_synth.Param.ChangeAmount = phoneSlider.Value;

			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();
		}

		private void ChangeSpeedSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var phoneSlider = sender as Slider;
			if (phoneSlider == null)
				return;

			_synth.Param.ChangeSpeed = phoneSlider.Value;

			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();
		}

		private void SquareDutySliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var phoneSlider = sender as Slider;
			if (phoneSlider == null)
				return;

			_synth.Param.SquareDuty = phoneSlider.Value;

			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();
		}

		private void DutySweepSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var phoneSlider = sender as Slider;
			if (phoneSlider == null)
				return;

			_synth.Param.DutySweep = phoneSlider.Value;

			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();
		}

		private void RepeatSpeedSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var phoneSlider = sender as Slider;
			if (phoneSlider == null)
				return;

			_synth.Param.RepeatSpeed = phoneSlider.Value;

			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();
		}

		private void PhaserOffsetSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var phoneSlider = sender as Slider;
			if (phoneSlider == null)
				return;

			_synth.Param.PhaserOffset = phoneSlider.Value;

			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();
		}

		private void PhaserSweepSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var phoneSlider = sender as Slider;
			if (phoneSlider == null)
				return;

			_synth.Param.PhaserSweep = phoneSlider.Value;

			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();
		}

		private void LpFilterCutoffSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var phoneSlider = sender as Slider;
			if (phoneSlider == null)
				return;

			_synth.Param.LpFilterCutoff = phoneSlider.Value;

			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();
		}

		private void LpFilterCutoffSweepSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var phoneSlider = sender as Slider;
			if (phoneSlider == null)
				return;

			_synth.Param.LpFilterCutoffSweep = phoneSlider.Value;

			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();
		}

		private void LpFilterResonanceSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var phoneSlider = sender as Slider;
			if (phoneSlider == null)
				return;

			_synth.Param.LpFilterResonance = phoneSlider.Value;

			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();
		}

		private void HpFilterCutoffSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var phoneSlider = sender as Slider;
			if (phoneSlider == null)
				return;

			_synth.Param.HpFilterCutoff = phoneSlider.Value;

			if (_playOnChange && !_mutePlayOnChange)
				_synth.Play();
		}

		private void HpFilterCutoffSweepSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var phoneSlider = sender as Slider;
			if (phoneSlider == null)
				return;

			_synth.Param.HpFilterCutoffSweep = phoneSlider.Value;

			if (_playOnChange && !_mutePlayOnChange) _synth.Play();
		}
		#endregion

		#endregion

		#region SkyDrive
		private void skydrive_SessionChanged(object sender, LiveConnectSessionChangedEventArgs e)
		{

			if (e != null && e.Status == LiveConnectSessionStatus.Connected)
			{
				this.client = new LiveConnectClient(e.Session);
				this.GetAccountInformations();
			}
			else
			{
				this.client = null;

				MessageBoxResult result =
					MessageBox.Show(e.Error != null ? e.Error.ToString() : string.Empty,
					"SkyDrive Not Signed-in", MessageBoxButton.OK);
			}

		}

		private async void GetAccountInformations()
		{
			try
			{
				LiveOperationResult operationResult = await this.client.GetAsync("me");
				var jsonResult = operationResult.Result as dynamic;
				string firstName = jsonResult.first_name ?? string.Empty;
				string lastName = jsonResult.last_name ?? string.Empty;

				MessageBoxResult result =
					MessageBox.Show("Welcome " + firstName + " " + lastName,
					"SkyDrive Sign-in Success", MessageBoxButton.OK);
			}
			catch (Exception e)
			{
				MessageBoxResult result =
					MessageBox.Show(e.ToString(),
					"SkyDrive Sign-in Error", MessageBoxButton.OK);
			}
		}



		private  void SaveButtonClick(object sender, EventArgs e)
		{
			TextBox textBox = new TextBox()
			{
				Margin = new Thickness(0, 14, 0, -2)
			};

			CustomMessageBox box = new CustomMessageBox()
			{
				Caption = "Save to SkyDrive",
				Message = "Enter file name",
				LeftButtonContent = "ok",
				RightButtonContent = "cancel",
				Content = textBox    
			};

			box.Dismissed += async (s, boxEventArgs) =>
				{
					switch (boxEventArgs.Result)
					{
						case CustomMessageBoxResult.LeftButton:
							if (!string.IsNullOrWhiteSpace(textBox.Text))
							{
								using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
								{
									try
									{
										using (var fileStream = store.OpenFile("sound.wav", FileMode.Open, FileAccess.Read, FileShare.Read))
										{

											string filename = textBox.Text + ".wav";
											LiveOperationResult res = await client.UploadAsync("me/skydrive",
																										 filename,
																										fileStream,
																										OverwriteOption.Overwrite);
											MessageBoxResult result =
												MessageBox.Show("File " + filename + " uploaded",
												"SkyDrive Sign-in", MessageBoxButton.OK);


										}
									}
									catch (Exception ex)
									{

									}
								}
							}
							break;
						default:
							break;
					}
				};

			box.Show();


			/**/
		}
		#endregion
	}
}