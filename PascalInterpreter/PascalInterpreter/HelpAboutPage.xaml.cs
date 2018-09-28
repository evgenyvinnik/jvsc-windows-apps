using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace PascalInterpreter
{
	public partial class HelpAboutPage : PhoneApplicationPage
	{
		public HelpAboutPage()
		{
			InitializeComponent();
		}

		private void MailButtonClick(object sender, RoutedEventArgs e)
		{
			var emailComposeTask = new EmailComposeTask
			{
				To = "evgenyvinnik@ya.ru",
				Body = "Hi Evgeny!",
				Subject = "Pascal Interpreter for WP7"
			};
			emailComposeTask.Show();
		}

		private void WebSiteButtonClick(object sender, RoutedEventArgs e)
		{
			var task = new WebBrowserTask { URL = "http://www.jvsh.ca/" };
			task.Show();
		}

		private void BiffButtonClick(object sender, RoutedEventArgs e)
		{
			var task = new WebBrowserTask { URL = "http://biff.sourceforge.net/pl/" };
			task.Show();

		}
	}
}