using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Linearstar.Windows.RawInput;

namespace RawInput.Sharp.TouchPadExample
{
	public partial class MainWindow : Window
	{
		public TouchPadManipulation Manipulation { get; } = new TouchPadManipulation(TimeSpan.FromMilliseconds(50));

		public MainWindow()
		{
			InitializeComponent();
		}

		private HwndSource _targetSource;

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			_targetSource = (HwndSource)PresentationSource.FromVisual(this);
			_targetSource.AddHook(WndProc);

			// Register the HidUsageAndPage to watch TouchPad.
			RawInputDevice.RegisterDevice(HidUsageAndPage.TouchPad, RawInputDeviceFlags.None, _targetSource.Handle);
		}

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			const int WM_INPUT = 0x00FF;

			switch (msg)
			{
				case WM_INPUT:
					try
					{
						if (RawInputData.FromHandle(lParam) is RawInputDigitizerData data)
						{
							Manipulation.ParseInput(data);
						}
					}
					catch (ArgumentException)
					{
						// RawInputDigitizerContact constructor may throw ArgumentException.
					}
					break;
			}
			return IntPtr.Zero;
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			RawInputDevice.UnregisterDevice(HidUsageAndPage.TouchPad);
		}
	}
}
