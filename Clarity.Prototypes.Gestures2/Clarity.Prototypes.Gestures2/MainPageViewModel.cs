using System.ComponentModel;
using System.Windows;
using Clarity.Phone.Interactivity.Input;
using GalaSoft.MvvmLight.Command;

namespace Clarity.Prototypes.Gestures2
{
	public class MainPageViewModel : INotifyPropertyChanged
	{
		private string _recognizedGesture;

		public MainPageViewModel()
		{
			GestureRecognizedCommand = new RelayCommand<GestureEventArgs>(e =>
			{
				RecognizedGesture = e.ToString();

				MessageBox.Show(e.ToString());
			});
			RecognizedGesture = "None";
		}

		public string RecognizedGesture
		{
			get
			{
				return _recognizedGesture;
			}
			set
			{
				_recognizedGesture = value;
				RaisePropertyChanged("RecognizedGesture");
			}
		}

		public RelayCommand<GestureEventArgs> GestureRecognizedCommand
		{
			get;
			private set;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged!=null)
			{
				PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
