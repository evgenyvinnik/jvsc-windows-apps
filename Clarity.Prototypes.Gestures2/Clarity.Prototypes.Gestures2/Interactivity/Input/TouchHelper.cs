//---------------------------------------------------------------------
// <copyright file="TouchHelper.cs" company="Microsoft">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Clarity.Phone.Interactivity.Input
{
	/// <summary>
	/// Passes data for TouchReported event.
	/// </summary>
	public class TouchReportedEventArgs: EventArgs
	{
		internal TouchReportedEventArgs(IEnumerable<TouchPoint> touchPoints)
		{
			TouchPoints = touchPoints;
		}

		/// <summary>
		/// Returns reported touch points. 
		/// </summary>
		public IEnumerable<TouchPoint> TouchPoints { get; private set; }
	}


	/// <summary>
	/// Passes data for touch events.
	/// </summary>
	public class TouchEventArgs : EventArgs
	{
		internal TouchEventArgs(TouchPoint touchPoint)
		{
			TouchPoint = touchPoint;
		}

		/// <summary>
		/// Returns the associated touch point.
		/// </summary>
		public TouchPoint TouchPoint { get; private set; }
	}


	/// <summary>
	/// A group of touch event handlers.
	/// </summary>
	public class TouchHandlers
	{
		public EventHandler<TouchEventArgs> TouchDown { get; set; }
		public EventHandler<TouchEventArgs> CapturedTouchUp { get; set; }
		public EventHandler<TouchReportedEventArgs> CapturedTouchReported { get; set; }
		public EventHandler<TouchEventArgs> LostTouchCapture { get; set; }
	}

	/// <summary>
	/// A helper class to process, deliver and capture touch related events.
	/// Note: the class is not thread safe.
	/// </summary>
	public static class TouchHelper
	{
		// indicates if touch input is enabled or not
		private static bool _isEnabled;

		// current event handlers
		private static readonly Dictionary<UIElement, TouchHandlers> CurrentHandlers = 
			new Dictionary<UIElement, TouchHandlers>();

		// current captured touch devices (touchDevice.Id -> capturing UIElement)
		private static readonly Dictionary<int, UIElement> CurrentCaptures = new Dictionary<int, UIElement>();

		// current touch points (for captured touch devices only)
		private static readonly Dictionary<int, TouchPoint> CurrentTouchPoints = new Dictionary<int, TouchPoint>();
 
		// an empty array of TouchPoints
		private static readonly TouchPoint[] EmptyTouchPoints = new TouchPoint[0];

		/// <summary>
		/// Returns true if there is at least one touch over the root. Otherwise - false.
		/// </summary>
		public static bool AreAnyTouches
		{
			get
			{
				return CurrentTouchPoints.Count != 0;
			}
		}

		/// <summary>
		/// Captured the given touchDevice. To release capture, pass element=null.
		/// </summary>
		/// <param name="touchDevice"></param>
		/// <param name="element"></param>
		/// <returns></returns>
		public static bool Capture(this TouchDevice touchDevice, UIElement element)
		{
			if (touchDevice == null)
			{
				throw new ArgumentNullException("element");
			}

			// raise LostCapture if the capturing element is different than the existing one
			UIElement existingCapture;
			if (CurrentCaptures.TryGetValue(touchDevice.Id, out existingCapture) &&
				!ReferenceEquals(existingCapture, element))
			{
				// check if a handler exists
				TouchHandlers handlers;
				if (CurrentHandlers.TryGetValue(existingCapture, out handlers))
				{
					var handler = handlers.LostTouchCapture;
					if (handler != null)
					{
						// raise LostCapture with the last known touchPoint
						TouchPoint touchPoint;
						if (CurrentTouchPoints.TryGetValue(touchDevice.Id, out touchPoint))
						{
							handler(existingCapture, new TouchEventArgs(touchPoint));
						}
					}
				}
			}

			// update currentCaptures dictionary
			if (element != null)
			{
				// capture
				CurrentCaptures[touchDevice.Id] = element;
			}
			else
			{
				// release
				CurrentCaptures.Remove(touchDevice.Id);
			}

			return true;
		}

		/// <summary>
		/// Adds event handlers for the given UIElement. Note: the method overrides all touch handler for the given element.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="handlers"></param>
		public static void AddHandlers(UIElement element, TouchHandlers handlers)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}

			if (handlers == null)
			{
				throw new ArgumentNullException("handlers");
			}

			CurrentHandlers[element] = handlers;
		}

		/// <summary>
		/// Removes event handlers from the given element.
		/// </summary>
		/// <param name="element"></param>
		public static void RemoveHandlers(UIElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}

			CurrentHandlers.Remove(element);
		}

		/// <summary>
		/// Enables or disables touch input.
		/// </summary>
		/// <param name="enable"></param>
		public static void EnableInput(bool enable)
		{
			if (enable)
			{
				if (!_isEnabled)
				{
					EnableInput();
					_isEnabled = true;
				}
			}
			else
			{
				if (_isEnabled)
				{
					DisableInput();
					_isEnabled = false;
				}
			}
		}

		/// <summary>
		/// Enables touch input.
		/// </summary>
		private static void EnableInput()
		{
			Touch.FrameReported += TouchFrameReported;
		}

		/// <summary>
		/// Disables touch input and clear all dictionaries.
		/// </summary>
		private static void DisableInput()
		{
			Touch.FrameReported -= TouchFrameReported;
			CurrentCaptures.Clear();
			CurrentHandlers.Clear();
			CurrentTouchPoints.Clear();
		}


		/// <summary>
		/// Handles TouchFrameReported event and raise TouchDown/Up/Move events.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void TouchFrameReported(object sender, TouchFrameEventArgs e)
		{
			// get te root
			var root = Application.Current.RootVisual;
			if (root == null)
			{
				return;
			}

			foreach (var touchPoint in e.GetTouchPoints(null))
			{
				var id = touchPoint.TouchDevice.Id;

				// check if the touchDevice is captured or not.
				UIElement captured;
				CurrentCaptures.TryGetValue(id, out captured);

				switch (touchPoint.Action)
				{
					// TouchDown
					case TouchAction.Down:
						HitTestAndRaiseDownEvent(root, touchPoint);
						CurrentTouchPoints[id] = touchPoint;
						break;

					// TouchUp
					case TouchAction.Up:
						// handle only captured touches
						if (captured != null)
						{
							RaiseUpEvent(captured, touchPoint);

							// release capture
							Capture(touchPoint.TouchDevice, null);
							captured = null;
						}
						CurrentTouchPoints.Remove(id);
						break;

					// TouchMove
					case TouchAction.Move:
						// just remember the new touchPoint, the event will be raised in bulk later
						CurrentTouchPoints[id] = touchPoint;
						break;
				}
			}

			// raise CapturedReportEvents
			RaiseCapturedReportEvent();
		}

		/// <summary>
		/// Iterates through all event handlers, combines all touches captured by the corresponding UIElement
		/// and raise apturedReportEvent.
		/// </summary>
		private static void RaiseCapturedReportEvent()
		{
			// walk through all handlers
			foreach (var pairHandler in CurrentHandlers)
			{
				var handler = pairHandler.Value.CapturedTouchReported;
				if (handler == null)
				{
					continue;
				}

				// walk through all touch devices captured by the current UIElement
				List<TouchPoint> capturedTouchPoints = null;
				var handler1 = pairHandler;
				foreach (var pairCapture in
					CurrentCaptures.Where(pairCapture => ReferenceEquals(pairCapture.Value, handler1.Key)))
				{
					// add the captured touchPoint
					TouchPoint capturedTouchPoint;
					if (!CurrentTouchPoints.TryGetValue(pairCapture.Key, out capturedTouchPoint))
						continue;
					if (capturedTouchPoints == null)
					{
						capturedTouchPoints = new List<TouchPoint>();
					}
					capturedTouchPoints.Add(capturedTouchPoint);
				}

				// raise event
				handler(pairHandler.Key, new TouchReportedEventArgs(
					capturedTouchPoints ?? (IEnumerable<TouchPoint>)EmptyTouchPoints));
			}
		}

		/// <summary>
		/// Performs hit testing, find the first element in the parent chain that has TouchDown event handler and
		/// raises TouchTouch event.
		/// </summary>
		/// <param name="root"></param>
		/// <param name="touchPoint"></param>
		private static void HitTestAndRaiseDownEvent(UIElement root, TouchPoint touchPoint)
		{
			foreach (var element in InputHitTest(root, touchPoint.Position))
			{
				TouchHandlers handlers;
				if (!CurrentHandlers.TryGetValue(element, out handlers))
					continue;

				var handler = handlers.TouchDown;
				if (handler == null)
					continue;

				// call the first found handler and break
				handler(element, new TouchEventArgs(touchPoint));
				break;
			}
		}

		/// <summary>
		/// Performs hit testing and returns a collection of UIElement the given point is within.
		/// </summary>
		/// <param name="root"></param>
		/// <param name="position"></param>
		/// <returns></returns>
		private static  IEnumerable<UIElement> InputHitTest(UIElement root, Point position)
		{
			foreach (var element in VisualTreeHelper.FindElementsInHostCoordinates(position, root))
			{
				yield return element;

				for (var parent = VisualTreeHelper.GetParent(element) as UIElement;
					 parent != null;
					 parent = VisualTreeHelper.GetParent(parent) as UIElement)
				{
					yield return parent;
				}
			}
		}

		/// <summary>
		/// Raises TouchUp event.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="touchPoint"></param>
		private static void RaiseUpEvent(UIElement element, TouchPoint touchPoint)
		{
			TouchHandlers handlers;
			if (!CurrentHandlers.TryGetValue(element, out handlers))
				return;

			var handler = handlers.CapturedTouchUp;
			if (handler != null)
			{
				handler(element, new TouchEventArgs(touchPoint));
			}
		}
	}
}
