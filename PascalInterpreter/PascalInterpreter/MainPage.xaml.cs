using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;

namespace PascalInterpreter
{
	public partial class MainPage
	{
		readonly ObservableCollection<Programs> _programs;

		// Constructor
		public MainPage()
		{
			_programs = new ObservableCollection<Programs>();


			InitializeComponent();
		}

		#region Load/Unload page and navigation

		private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
		{
			InitPrograms();
			programList.ItemsSource = _programs;
		}
		#endregion

		#region Menu clicks
		private void NewButtonClick(object sender, EventArgs e)
		{
			var input = new InputPrompt
			{
				Title = "Program Name",
				Message = "Enter new file name (*.pas)",
				IsCancelVisible = true
			};

			input.Completed += InputCompleted;
			input.Show();
		}
 
		void InputCompleted(object sender, PopUpEventArgs<string, PopUpResult> e)
		{
			switch (e.PopUpResult)
			{
				case PopUpResult.Ok:
					var extension = Path.GetExtension(e.Result);
					if (extension != null)
					{
						if (extension.Equals(".pas", StringComparison.CurrentCultureIgnoreCase))
						{
							try
							{
								var newProgram = new IsolatedStorageFileStream(e.Result, FileMode.CreateNew, FileAccess.ReadWrite,
											   IsolatedStorageFile.GetUserStoreForApplication());
								newProgram.Close();
								InitPrograms();
							}
							catch (IsolatedStorageException)
							{
								MessageBox.Show("Failed to create new program " + e.Result);
							}

						}
						else
						{
							MessageBox.Show("Failed to create new program as extension is not *.pas");
						}
					}
					break;
				case PopUpResult.Cancelled:
					MessageBox.Show("CANCELLED! " + e.Result);
					break;
				default:
					MessageBox.Show("Error  " + e.Result);
					break;
			}
		}

		private void EditButtonClick(object sender, EventArgs e)
		{
			var selectedItemName = programList.SelectedItem;
			if(selectedItemName != null)
				NavigationService.Navigate(new Uri(string.Format("/EditorPage.xaml?val1={0}", ((Programs) selectedItemName).ProgramName),
											UriKind.Relative));
		}

		private void DeleteButtonClick(object sender, EventArgs e)
		{
			var selectedItemName = programList.SelectedItem;
			if (selectedItemName != null)
			{
				if (MessageBox.Show("Are your sure want to delete " + ((Programs)selectedItemName).ProgramName + "?", "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
				{
					IsolatedStorageFile.GetUserStoreForApplication().DeleteFile(((Programs)selectedItemName).ProgramName);
					InitPrograms();
				}
			}

		}


		private void HelpAboutMenuItemClick(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/HelpAboutPage.xaml",
										UriKind.Relative));
		}
		#endregion

		#region List manipulation
		void InitPrograms()
		{
			_programs.Clear();

			var files = IsolatedStorageFile.GetUserStoreForApplication().GetFileNames("*").ToList();

			foreach (var file in files)
			{
				var fileOpened = IsolatedStorageFile.GetUserStoreForApplication().OpenFile(file, FileMode.Open, FileAccess.Read);
				_programs.Add(new Programs(file, fileOpened.Length));
				fileOpened.Close();
			}
		}

		private void ListBoxItemDoubleTap(object sender, GestureEventArgs e)
		{
			var selectedItemName = programList.SelectedItem;
			if (selectedItemName != null)
				NavigationService.Navigate(new Uri(string.Format("/EditorPage.xaml?val1={0}", ((Programs)selectedItemName).ProgramName),
											UriKind.Relative));
		}
		#endregion

	}

	public class Programs
	{
		public string ProgramName { get; set; }
		public string ProgramSize { get; set; }

		public Programs(string programName, long programSize)
		{
			ProgramName = programName;
			ProgramSize = programSize + " bytes";
		}
	}

}