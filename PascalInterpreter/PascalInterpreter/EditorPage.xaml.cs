using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using PascalInterpreter.Interpreter;

namespace PascalInterpreter
{
	public partial class EditorPage
	{
		private string _programName;

		public EditorPage()
		{
			InitializeComponent();
		}

		#region Load/Unload page and navigation
		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (NavigationContext.QueryString.TryGetValue("val1", out _programName))
			{
				ProgramPivot.Title = _programName.ToUpper();

				var currentProgram = new IsolatedStorageFileStream(_programName, FileMode.Open, FileAccess.Read,
								   IsolatedStorageFile.GetUserStoreForApplication());
				var sr = new StreamReader(currentProgram);
				ProgramText.Text = sr.ReadToEnd();
				currentProgram.Close();
			}
		}

		protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
		{
			Save();
			base.OnNavigatingFrom(e);
		}
		#endregion

		#region Menu clicks

		private void SaveButtonClick(object sender, EventArgs e)
		{
			Save();
		}

		private void RunButtonClick(object sender, EventArgs e)
		{
			Save();
			RunProgram();
		}

		private void CloseButtonClick(object sender, EventArgs e)
		{
			Save();
			NavigationService.Navigate(new Uri("/MainPage.xaml",
										UriKind.Relative));
		}
		#endregion

		#region Program
		void Save()
		{
			var currentProgram = new IsolatedStorageFileStream(_programName, FileMode.Open, FileAccess.Write,
											   IsolatedStorageFile.GetUserStoreForApplication());
			var data = Encoding.GetEncoding("utf-8").GetBytes(ProgramText.Text);
			currentProgram.Write(data, 0, data.Length);
			currentProgram.Close();
		}

		void RunProgram()
		{
			Save();

			ProgramPivot.SelectedItem = ConsoleItem;

			var stdout = "";
			string stderr;

			try
			{
				var scanner = new Scanner(ProgramText.Text);
				stderr = scanner.stderr;
				stdout = scanner.stdout;
			}
			catch (Exception e)
			{
				stderr = "EXCEPTION!\n" + e + Environment.NewLine;
			}

			if (stderr.Length != 0)
			{
				ConsoleText.Text = stdout + Environment.NewLine + stderr;
			}
			else
			{
				ConsoleText.Text = stdout;
			}

		}
		#endregion


	}
}