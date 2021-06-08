using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using Linearstar.Windows.RawInput;

namespace RawInput.Sharp.TouchPadExample
{
	public class TouchPadManipulation : DependencyObject
	{
		public Point Current
		{
			get { return (Point)GetValue(CurrentProperty); }
			set { SetValue(CurrentProperty, value); }
		}
		public static readonly DependencyProperty CurrentProperty =
			DependencyProperty.Register("Current", typeof(Point), typeof(TouchPadManipulation), new PropertyMetadata(default(Point)));

		public Vector Delta
		{
			get { return (Vector)GetValue(DeltaProperty); }
			set { SetValue(DeltaProperty, value); }
		}
		public static readonly DependencyProperty DeltaProperty =
			DependencyProperty.Register("Delta", typeof(Vector), typeof(TouchPadManipulation), new PropertyMetadata(default(Vector)));

		private readonly DispatcherTimer _timer;

		public TouchPadManipulation(TimeSpan interval)
		{
			_timer = new DispatcherTimer { Interval = interval };
			_timer.Tick += OnTick;
		}

		private readonly object _lock = new();

		private Point _origin;

		public void ParseInput(RawInputDigitizerData data)
		{
			var contact = data.Contacts[0];
			if (contact.Identifier == 0) // Use the first contact only.
			{
				lock (_lock)
				{
					Current = new Point(contact.X, contact.Y);

					if (!_timer.IsEnabled)
					{
						_origin = Current;
					}
					else
					{
						_timer.Stop();
					}
					_timer.Start();
				}
			}
		}

		private void OnTick(object sender, EventArgs e)
		{
			lock (_lock)
			{
				_timer.Stop();

				Delta = new Vector(Current.X - _origin.X, Current.Y - _origin.Y);
				_origin = default;
			}
		}
	}
}
