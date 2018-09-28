using System;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Linq;
using System.Windows.Threading;
using Clarity.Phone.Interactivity.Input.Dollar;

namespace Clarity.Phone.Interactivity.Input
{

	public class GestureInterpreter
	{
		#region Internal Classes
		internal class FingerTap
		{
			public Point TapOrigin { get; set; }
			public long TapTimestamp { get; set; }
			public int TouchId { get; set; }
			public bool IsTapComplete { get; set; }
		}

		internal class FutureAction
		{
			// Fields
			private DispatcherTimer _timer = new DispatcherTimer();
			private Action _action;

			//Methods
			public FutureAction(Action action, TimeSpan timeout)
			{
				_timer.Tick += TimerTick;
				_action = action;
				_timer.Interval = timeout;
				_timer.Start();
			}

			void TimerTick(object sender, EventArgs e)
			{
				_action();
				_timer.Stop();
				Cleanup();
			}

			public void Cancel()
			{
				if (_timer.IsEnabled)
				{
					_timer.Stop();
				}
				Cleanup();
			}

			private void Cleanup()
			{
				_timer.Tick -= TimerTick;
				_timer = null;
			}
		}
		#endregion

		#region Constants
		//Size of touch down visualization
		private const double DebugMarkerDiameter = 60;

		//Distance between touches when tapping with two fingers
		//Should be large enough to accomodate two fingers side by side, but
		//not so large that any two touches raise the twofingertap event
		private const double TwoFingerTapDistanceThreshold = 215;

		//Time difference between two touches for twofingertap, should
		//be nearly at the same time to register as a tap
		private static readonly TimeSpan TwoFingerTapStartTimeThreshold = TimeSpan.FromMilliseconds(50);  
  
		//Distance between subsequent taps for a double tap. Should be in
		//approximately the same area to register as a double tap
		private const double DoubleTapDistanceThreshold = 25;

		// .3 seconds - time to wait for two taps
		//quicker time makes tap more responsive, but double tap more difficult to perform
		private static readonly TimeSpan DoubleTapTimeThreshold = TimeSpan.FromMilliseconds(300); 
		
		//tolerance for finger moving on a tap and hold
		private const double TapAndHoldDistanceThreshold = 25;

		// .8 seconds - time to wait for a press and hold. 
		//needs to be > than DoubleTapTimeThreshold
		private static readonly TimeSpan TapAndHoldTimeThreshold = TimeSpan.FromMilliseconds(800);

		private static readonly TimeSpan PressAndTapTimeThreshold = TimeSpan.FromMilliseconds(500);
		
		//amount of dragging before a pan event is raised
		private const double TranslationThreshold = 20;  

		//threshold for raising shape recognized event based on 
		//$1 recognizer score. 0 - 1. Higher requires greater accuracy.
		private const double DefaultDollarRecognizerScoreThreshold = .8;

		private static readonly Dictionary<ShapeGestureEventArgs.Shapes, double> DollarRecognizerScoreThresholds = 
			new Dictionary<ShapeGestureEventArgs.Shapes, double>();

		private const double RotationThreshold = 1;
			
		private const ushort InitialScaleThreshold = 1;
		
		//minimum velocity needed to raise a flick gesture
		//should be high enough to allow panning
		private const ushort MinimumFlickSpeed = 250;
		#endregion 

		#region Events
		public event EventHandler<GestureEventArgs> GestureRecognized;
		public event EventHandler<GestureEventArgs> DoubleTap;
		public event EventHandler<GestureEventArgs> Flick;
		public event EventHandler<GestureEventArgs> PressAndTap;
		public event EventHandler<GestureEventArgs> Pan;
		public event EventHandler<GestureEventArgs> Rotate;
		public event EventHandler<GestureEventArgs> Scale;
		public event EventHandler<GestureEventArgs> Shape;
		public event EventHandler<GestureEventArgs> Tap;
		public event EventHandler<GestureEventArgs> TapAndHold;
		public event EventHandler<GestureEventArgs> TwoFingerTap;
		public event EventHandler<GestureEventArgs> TwoFingerTapAndHold;
		#endregion Events

		//Private Fields
		private Recognizer _recognizer;
		private Dictionary<int, Popup> _popups = new Dictionary<int, Popup>();
		private readonly List<FingerTap> _fingerTaps = new List<FingerTap>();
		private UIElement _element;
		private bool _isTap;
		private bool _waitingFor2ndTap;
		private FutureAction _doubleTap;
		private FutureAction _tapAndHold;
		private FutureAction _pressAndTap;
		private List<PointR> _gesturePoints = new List<PointR>();
		private bool _recoginizingShape;
		private Point? _primaryTouchPoint;
		private Point? _secondaryTouchPoint;
		private Point? _lastPrimaryTouchPoint;
		private Point? _lastSecondaryTouchPoint;

		//tracked seperately from FingerTaps since that requires a touch screen
		//and this can work on mouse clicks
		//TODO: try to combine wrapping the taps from the manipulation started events and the touch down events
		private Point _lastTapOrigin;
		private long _lastTapTimestamp;
		private long _tapStartTimestamp;

		public bool DebugMode { get; set; }

		private int FingersDown
		{
			get { return _fingerTaps.Where(ft => ft.IsTapComplete == false).Count(); }
		}

		public GestureInterpreter(UIElement element)
		{
			Subscribe(element);

			LoadGestureRecognizer(@"Interactivity/Input/Dollar/Gestures/{0}");
		}

		private void LoadGestureRecognizer(string basePath)
		{
			_recognizer = new Recognizer();

			_recognizer.LoadGesture(String.Format(basePath, "circle01.xml"));
			_recognizer.LoadGesture(String.Format(basePath, "circle02.xml"));
			_recognizer.LoadGesture(String.Format(basePath, "circle03.xml"));
			_recognizer.LoadGesture(String.Format(basePath, "check01.xml"));
			_recognizer.LoadGesture(String.Format(basePath, "check02.xml"));
			_recognizer.LoadGesture(String.Format(basePath, "check03.xml"));
			_recognizer.LoadGesture(String.Format(basePath, "pigtail01.xml"));
			_recognizer.LoadGesture(String.Format(basePath, "pigtail02.xml"));
			_recognizer.LoadGesture(String.Format(basePath, "pigtail03.xml"));
			_recognizer.LoadGesture(String.Format(basePath, "question_mark01.xml"));
			_recognizer.LoadGesture(String.Format(basePath, "question_mark02.xml"));
			_recognizer.LoadGesture(String.Format(basePath, "question_mark03.xml"));

			DollarRecognizerScoreThresholds[ShapeGestureEventArgs.Shapes.Circle] = DefaultDollarRecognizerScoreThreshold;
			DollarRecognizerScoreThresholds[ShapeGestureEventArgs.Shapes.PigTail] = DefaultDollarRecognizerScoreThreshold;
			DollarRecognizerScoreThresholds[ShapeGestureEventArgs.Shapes.Question_Mark] = DefaultDollarRecognizerScoreThreshold;
			DollarRecognizerScoreThresholds[ShapeGestureEventArgs.Shapes.Check] = .9;
		}

		private void Subscribe(UIElement element)
		{
			_element = element;

			_element.ManipulationStarted += OnManipulationStarted;
			_element.ManipulationCompleted += OnManipulationCompleted;
			_element.ManipulationDelta += OnManipulationDelta;

			TouchHelper.AddHandlers(_element, new TouchHandlers
			{
				TouchDown = OnTouchDown,
				CapturedTouchUp = OnCapturedTouchUp,
				CapturedTouchReported = OnTouchReported
			});
			TouchHelper.EnableInput(true);
		}

		public void Unsubscribe()
		{
			_element.ManipulationStarted -= OnManipulationStarted;
			_element.ManipulationCompleted -= OnManipulationCompleted;
			_element.ManipulationDelta -= OnManipulationDelta;
			TouchHelper.RemoveHandlers(_element);

			if (_doubleTap != null)
			{
				_doubleTap.Cancel();
				_doubleTap = null;
			}

			if (_tapAndHold != null)
			{
				_tapAndHold.Cancel();
				_tapAndHold = null;
			}

			if (_pressAndTap != null)
			{
				_pressAndTap.Cancel();
				_pressAndTap = null;
			}
		}

		private static double PercentDifferent(double n1, double n2)
		{
			var diff = Math.Abs(n1 - n2);
			return diff / n1;
		}

		private static Point Delta(Point p1, Point p2)
		{
			return new Point(p2.X - p1.X, p2.Y - p1.Y);
		}

		private static double Distance(Point p1, Point p2)
		{
			return Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
		}

		private static double Slope(Point p1, Point p2)
		{
			return (p1.Y - p2.Y) / (p1.X - p2.X);
		}

		private static double Angle(double m1, double m2)
		{
			return Math.Atan((m2 > m1 ? m2 - m1 : m1 - m2) / (1 + m1 * m2)) * (180.0 / Math.PI);
		}

		private static bool IsDoubleTap(Point previousPoint, long previousTimestamp, Point currentPoint, long currentTimestamp)
		{
			return ((TimeSpan.FromTicks(currentTimestamp - previousTimestamp) < DoubleTapTimeThreshold) &&
				(Distance(previousPoint, currentPoint) < DoubleTapDistanceThreshold));
		}

		private bool IsTwoFingerTap()
		{
			if (_fingerTaps.Count == 2)
			{
				Debug.WriteLine("TwoFingerTap Distance: {0}, Time {1}", Distance(_fingerTaps[0].TapOrigin, _fingerTaps[1].TapOrigin), 
					TimeSpan.FromTicks(_fingerTaps[1].TapTimestamp - _fingerTaps[0].TapTimestamp));

				return ((Distance(_fingerTaps[0].TapOrigin, _fingerTaps[1].TapOrigin) < TwoFingerTapDistanceThreshold) &&
					(TimeSpan.FromTicks(_fingerTaps[1].TapTimestamp - _fingerTaps[0].TapTimestamp) < TwoFingerTapStartTimeThreshold));
			}

			return false;
		}

		private void RaiseEvent(object sender, EventHandler<GestureEventArgs> e, GestureEventArgs args)
		{
			if (GestureRecognized != null)
				GestureRecognized(sender, args);

			if (e != null)
			{
				e(sender, args);
			}
			Debug.WriteLine(args);
		}

		private void OnTouchDown(object sender, TouchEventArgs e)
		{
			if (e.TouchPoint.TouchDevice.Capture(_element))
			{
				_fingerTaps.Add(new FingerTap
				                	{
					TapOrigin = e.TouchPoint.Position,
					TapTimestamp = DateTime.Now.Ticks,
					TouchId = e.TouchPoint.TouchDevice.Id
				});
				if (e.TouchPoint.TouchDevice.Id == 0)
				{
					_gesturePoints.Add(new PointR(e.TouchPoint.Position.X, e.TouchPoint.Position.Y));
					_recoginizingShape = true;
					_primaryTouchPoint = e.TouchPoint.Position;
				}
				if (e.TouchPoint.TouchDevice.Id == 1)
					_secondaryTouchPoint = e.TouchPoint.Position;

				Debug.WriteLine("TouchDown: Id {0} at ({1})", e.TouchPoint.TouchDevice.Id, e.TouchPoint.Position);
				Debug.WriteLine("Fingers down: {0}", _fingerTaps.Where(ft => ft.IsTapComplete == false).Count());

				if (DebugMode)
				{
					CreateTouchMarker(e.TouchPoint);
				}
			}
		}

		private void OnTouchReported(object sender, TouchReportedEventArgs e)
		{
			foreach (TouchPoint tp in e.TouchPoints)
			{
				Debug.WriteLine("TouchReported: Id {0} at ({1})", tp.TouchDevice.Id, tp.Position);

				if (tp.TouchDevice.Id == 0 && _recoginizingShape)
				{
					_gesturePoints.Add(new PointR(tp.Position.X, tp.Position.Y));
				}
				else
					_recoginizingShape = false;

				switch (tp.TouchDevice.Id)
				{
					case 0:
						_lastPrimaryTouchPoint = _primaryTouchPoint;
						_primaryTouchPoint = tp.Position;
						break;
					case 1:
						_lastSecondaryTouchPoint = _secondaryTouchPoint;
						_secondaryTouchPoint = tp.Position;
						break;
				}

				if (DebugMode)
				{
					if (_popups.ContainsKey(tp.TouchDevice.Id))
					{
						_popups[tp.TouchDevice.Id].HorizontalOffset = tp.Position.X - (DebugMarkerDiameter / 2);
						_popups[tp.TouchDevice.Id].VerticalOffset = tp.Position.Y - (DebugMarkerDiameter / 2);
					}
				}
			}
		}

		private void OnCapturedTouchUp(object sender, TouchEventArgs e)
		{
			e.TouchPoint.TouchDevice.Capture(null);

			_fingerTaps.Where(ft => ft.TouchId == e.TouchPoint.TouchDevice.Id).First().IsTapComplete = true;

			Debug.WriteLine("CapturedTouchUp: Id {0}", e.TouchPoint.TouchDevice.Id);
			Debug.WriteLine("Fingers down: {0}", _fingerTaps.Where(ft => ft.IsTapComplete == false).Count());

			if (e.TouchPoint.TouchDevice.Id == 0)
			{
				if (_gesturePoints.Count > 5 && _recoginizingShape)
				{
					var result = _recognizer.Recognize(_gesturePoints);

					var shape = (ShapeGestureEventArgs.Shapes)Enum.Parse(typeof(ShapeGestureEventArgs.Shapes), 
						result.Name.Substring(0, result.Name.Length - 2), true);

					if (result.Score > DollarRecognizerScoreThresholds[shape])
					{
						RaiseEvent(sender, Shape, new ShapeGestureEventArgs
						                          	{
							Origin = new Point(_gesturePoints[0].X, _gesturePoints[0].Y),
							Shape = shape,
							Confidence = result.Score
						});
					}
				}
				_gesturePoints.Clear();
				_recoginizingShape = false;
			}

			switch (e.TouchPoint.TouchDevice.Id)
			{
				case 0:
					_primaryTouchPoint = null;
					break;
				case 1:
					_secondaryTouchPoint = null;
					break;
			}

			if (DebugMode)
			{
				if (_popups.ContainsKey(e.TouchPoint.TouchDevice.Id))
				{
					_popups[e.TouchPoint.TouchDevice.Id].IsOpen = false;
					_popups[e.TouchPoint.TouchDevice.Id] = null;
					_popups.Remove(e.TouchPoint.TouchDevice.Id);
				}
			}
		}

		private void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
		{
			Point origin = e.ManipulationOrigin;
			DateTime now = DateTime.Now;

			if (_isTap)
			{
				AbortTapHold();

				if (_waitingFor2ndTap && IsDoubleTap(_lastTapOrigin, _lastTapTimestamp, origin, now.Ticks))
				{
					_doubleTap.Cancel();
					_doubleTap = null;
					_waitingFor2ndTap = false;
					RaiseEvent(sender, DoubleTap, new DoubleTapGestureEventArgs
					                              	{
						Origin = origin
					});
				}
				else
				{
					_waitingFor2ndTap = true;
					_lastTapTimestamp = now.Ticks;
					_lastTapOrigin = origin;
					_doubleTap = new FutureAction(() =>
						{
							if (IsTwoFingerTap())
								RaiseEvent(sender, TwoFingerTap, new TwoFingerTapGestureEventArgs
								                                 	{
									Origin = e.ManipulationOrigin
								});
							else
								RaiseEvent(sender, Tap, new TapGestureEventArgs
								                        	{
									Origin = e.ManipulationOrigin
								});
						},
						DoubleTapTimeThreshold);
				}
			}
			else
			{
				_isTap = false;
				_waitingFor2ndTap = false;
			}

			if (e.IsInertial && ((Math.Abs(e.FinalVelocities.LinearVelocity.X) > MinimumFlickSpeed) || (Math.Abs(e.FinalVelocities.LinearVelocity.Y) > MinimumFlickSpeed)))
			{
				RaiseEvent(sender, Flick, new FlickGestureEventArgs
				                          	{ 
					Origin = e.ManipulationOrigin,
					Velocity = e.FinalVelocities.LinearVelocity,
					FlickDirection = FlickGestureEventArgs.GetDirection(e.ManipulationOrigin, e.FinalVelocities.LinearVelocity)
				});
			}

			_primaryTouchPoint = null;
			_secondaryTouchPoint = null;
			_lastPrimaryTouchPoint = null;
			_lastSecondaryTouchPoint = null;
		}

		private static bool IsScaling(ManipulationDeltaEventArgs m, int fingersDown)
		{
			bool exceedsInitialScaleThreshold = (PercentDifferent(m.CumulativeManipulation.Scale.X, 1.0) > 0.05 || PercentDifferent(m.CumulativeManipulation.Scale.X, 1.0) > 0.05);
			return (fingersDown >= 2 && exceedsInitialScaleThreshold && ((PercentDifferent(m.DeltaManipulation.Scale.X, 1.0) > 0.01) || (PercentDifferent(m.DeltaManipulation.Scale.Y, 1.0) > 0.01)));
		}

		private static bool IsRotating(Point? lastPrimary, Point? lastSecondary, Point? primary, Point? secondary, int fingersDown, out double angle)
		{
			angle = double.NaN;
			if (!(lastPrimary.HasValue && lastSecondary.HasValue && primary.HasValue && secondary.HasValue && fingersDown == 2)) return false;

			double lastSlope = Slope(lastPrimary.Value, lastSecondary.Value);
			double currentSlope = Slope(primary.Value, secondary.Value);
			angle = Angle(lastSlope, currentSlope);

			if (lastSlope > currentSlope)
				angle *= -1;

			Debug.WriteLine("Angle: {0}", angle);

			return true;
		}

		private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
		{
			var scaling = IsScaling(e, FingersDown);
			var translating = FingersDown == 1 && (Math.Abs(e.DeltaManipulation.Translation.X) > 0 || Math.Abs(e.DeltaManipulation.Translation.Y) > 0);

			double angle;
			var rotating = IsRotating(_lastPrimaryTouchPoint, _lastSecondaryTouchPoint, _primaryTouchPoint, _secondaryTouchPoint, FingersDown, out angle);
			//bool rotating = false;

			if (scaling || translating || rotating)
			{
				_isTap = false;
				AbortTapHold();
			}

			if (rotating && Math.Abs(angle) > RotationThreshold)
			{
				RaiseEvent(sender, Rotate, new RotateGestureEventArgs
				                           	{
					Origin = e.ManipulationOrigin,
					Angle = angle
				});
			}
			else if (scaling)
			{
				RaiseEvent(sender, Scale, new ScaleGestureEventArgs
				                          	{
					Origin = e.ManipulationOrigin,
					DeltaScale = e.DeltaManipulation.Scale,
					CumulativeScale = e.CumulativeManipulation.Scale
				});
			}
			else if (translating)
			{
				RaiseEvent(sender, Pan, new PanGestureEventArgs
				                        	{
					Origin = e.ManipulationOrigin,
					Translation = e.DeltaManipulation.Translation
				});
			}
		}

		private void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
		{
			_fingerTaps.Clear();

			Debug.WriteLine("OnManipulationStarted: ({0})", e.ManipulationOrigin);

			_isTap = true;
			_tapStartTimestamp = DateTime.Now.Ticks;

			_tapAndHold = new FutureAction(() => 
				{
					if (IsTwoFingerTap())
					{
						RaiseEvent(sender, TwoFingerTapAndHold, new TwoFingerTapAndHoldGestureEventArgs
						                                        	{
							Origin = e.ManipulationOrigin
						});
					}
					else
					{
						RaiseEvent(sender, TapAndHold, new TapAndHoldGestureEventArgs
						                               	{
							Origin = e.ManipulationOrigin
						});
					}

					_isTap = false;
					e.Complete();

				}, TapAndHoldTimeThreshold);

			_pressAndTap = new FutureAction(() =>
			{
				if (_fingerTaps.Count == 2 && _fingerTaps[0].IsTapComplete == false && _fingerTaps[1].IsTapComplete)
				{
					AbortTapHold();

					RaiseEvent(sender, PressAndTap, new PressAndTapGestureEventArgs
					                                	{
						Origin = e.ManipulationOrigin
					});

					_isTap = false;
					e.Complete();
				}
			}, PressAndTapTimeThreshold);
		}

		private void AbortTapHold()
		{
			if (_tapAndHold != null)
			{
				_tapAndHold.Cancel();
				_tapAndHold = null;
			}
		}

		private void CreateTouchMarker(TouchPoint tp)
		{
			var popup = new Popup();
			var e = new Ellipse
			        	{
			        		Fill = new SolidColorBrush(Colors.Blue),
			        		Opacity = .5,
			        		Height = DebugMarkerDiameter,
			        		Width = DebugMarkerDiameter
			        	};
			popup.Child = e;
			popup.HorizontalOffset = tp.Position.X - (DebugMarkerDiameter / 2);
			popup.VerticalOffset = tp.Position.Y - (DebugMarkerDiameter / 2);
			popup.IsOpen = true;

			_popups[tp.TouchDevice.Id] = popup;
		}

	}
}
