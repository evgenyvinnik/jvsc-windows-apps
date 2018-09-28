using System.Windows;
using Microsoft.Phone.Tasks;

namespace Sfxr
{
	public partial class AboutPage
	{
		public AboutPage()
		{
			InitializeComponent();
		}

		private void MailButtonClick(object sender, RoutedEventArgs e)
		{
			var emailComposeTask = new EmailComposeTask
			                                    	{
			                                    		To = "evgenyvinnik@ya.ru",
			                                    		Body = "Hi Evgeny! I like your 8BitSndGen app!",
			                                    		Subject = "8BitSndGen"
			                                    	};
			emailComposeTask.Show(); 
		}

		private void WebSiteButtonClick(object sender, RoutedEventArgs e)
		{
			//var task = new WebBrowserTask {URL = "http://www.jvsh.ca/"};
			Windows.System.Launcher.LaunchUriAsync(new System.Uri("http://www.jvsh.ca/"));
			//task.Show();  
		}

		private void PetterButtonClick(object sender, RoutedEventArgs e)
		{
			//var task = new WebBrowserTask { URL = "http://www.drpetter.se/" };
			//task.Show();  
			Windows.System.Launcher.LaunchUriAsync(new System.Uri("http://www.drpetter.se/"));
		}

		private void TomVianButtonClick(object sender, RoutedEventArgs e)
		{
			//var task = new WebBrowserTask { URL = "http://www.tomvian.com/" };
			//task.Show();
			Windows.System.Launcher.LaunchUriAsync(new System.Uri("http://www.tomvian.com/"));
		}
	}
}