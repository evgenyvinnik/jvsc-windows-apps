﻿using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Daltonism
{
	public partial class App
	{
		/// <summary>
		/// Provides easy access to the root frame of the Phone Application.
		/// </summary>
		/// <returns>The root frame of the Phone Application.</returns>
		public PhoneApplicationFrame RootFrame { get; private set; }
		// To store the instance for the application lifetime

		private ShellTileSchedule _shellTileSchedule;

		/// <summary>
		/// Constructor for the Application object.
		/// </summary>
		public App()
		{
			// Global handler for uncaught exceptions. 
			UnhandledException += Application_UnhandledException;

			// Show graphics profiling information while debugging.
			//if (System.Diagnostics.Debugger.IsAttached)
			//{
				// Display the current frame rate counters.
				//Current.Host.Settings.EnableFrameRateCounter = true;

				// Show the areas of the app that are being redrawn in each frame.
				//Application.Current.Host.Settings.EnableRedrawRegions = true;

				// Enable non-production analysis visualization mode, 
				// which shows areas of a page that are being GPU accelerated with a colored overlay.
				//Application.Current.Host.Settings.EnableCacheVisualization = true;
			//}

			// Standard Silverlight initialization
			InitializeComponent();

			// Phone-specific initialization
			InitializePhoneApplication();

			// Create the shell tile schedule instance
			CreateShellTileSchedule();

		}


		/// <summary>
		/// Create the application shell tile schedule instance
		/// </summary>
		private void CreateShellTileSchedule()
		{
			_shellTileSchedule = new ShellTileSchedule
			                    	{
			                    		Recurrence = UpdateRecurrence.Interval,
			                    		Interval = UpdateInterval.EveryHour,
			                    		StartTime = DateTime.Now
			                    	};

			var random = new Random();
			switch (random.Next(3))
			{
				case 0:
					_shellTileSchedule.RemoteImageUri = new Uri(@"http://jvsh.ca/Tile1.png");
					break;
				case 1:
					_shellTileSchedule.RemoteImageUri = new Uri(@"http://jvsh.ca/Tile2.png");
					break;
				case 2:
					_shellTileSchedule.RemoteImageUri = new Uri(@"http://jvsh.ca/Tile3.png");
					break;
				case 3:
					_shellTileSchedule.RemoteImageUri = new Uri(@"http://jvsh.ca/Tile4.png");
					break;
			}
			_shellTileSchedule.Start();
		}

 



		// Code to execute when the application is launching (eg, from Start)
		// This code will not execute when the application is reactivated
		private void ApplicationLaunching(object sender, LaunchingEventArgs e)
		{
		}

		// Code to execute when the application is activated (brought to foreground)
		// This code will not execute when the application is first launched
		private void ApplicationActivated(object sender, ActivatedEventArgs e)
		{
		}

		// Code to execute when the application is deactivated (sent to background)
		// This code will not execute when the application is closing
		private void ApplicationDeactivated(object sender, DeactivatedEventArgs e)
		{
		}

		// Code to execute when the application is closing (eg, user hit Back)
		// This code will not execute when the application is deactivated
		private void ApplicationClosing(object sender, ClosingEventArgs e)
		{
		}

		// Code to execute if a navigation fails
		private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			if (System.Diagnostics.Debugger.IsAttached)
			{
				// A navigation has failed; break into the debugger
				System.Diagnostics.Debugger.Break();
			}
		}

		// Code to execute on Unhandled Exceptions
		private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			if (System.Diagnostics.Debugger.IsAttached)
			{
				// An unhandled exception has occurred; break into the debugger
				System.Diagnostics.Debugger.Break();
			}
		}

		#region Phone application initialization

		// Avoid double-initialization
		private bool _phoneApplicationInitialized = false;

		// Do not add any additional code to this method
		private void InitializePhoneApplication()
		{
			if (_phoneApplicationInitialized)
				return;

			// Create the frame but don't set it as RootVisual yet; this allows the splash
			// screen to remain active until the application is ready to render.
			RootFrame = new PhoneApplicationFrame();
			RootFrame.Navigated += CompleteInitializePhoneApplication;

			// Handle navigation failures
			RootFrame.NavigationFailed += RootFrame_NavigationFailed;

			// Ensure we don't initialize again
			_phoneApplicationInitialized = true;
		}

		// Do not add any additional code to this method
		private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
		{
			// Set the root visual to allow the application to render
			if (RootVisual != RootFrame)
				RootVisual = RootFrame;

			// Remove this handler since it is no longer needed
			RootFrame.Navigated -= CompleteInitializePhoneApplication;
		}

		#endregion
	}
}