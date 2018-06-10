using log4net;
using log4net.Config;
using System;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Clickami
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(MainWindow));

        private Hotkeys ghk_abortClick, ghk_startClick;

        private Timer timerForLoop;
        private bool isTimerForLoopRunning = false;
        private Timer timerForInfinitely;
        private bool isTimerForInfinitelyRunning = false;
        private DispatcherTimer timerForUpdateUI;

        private int loops = 1;
        private Point mouseCoordinates = new Point();

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        // This is a replacement for Cursor.Position in WinForms
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        public static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButton, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        // This simulates a left mouse click
        public static void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == Hotkeys.Constants.WM_HOTKEY_MSG_ID)
            {
                if (wParam.ToInt32() == this.ghk_abortClick.GetHashCode())
                {
                    //MessageBox.Show("AbortClick pressed!");
                    LOG.Debug("ghk_abortClick pressed");
                    if (!this.ghk_startClick.Register())
                    {
                        LOG.Warn("ghk_startClick already registered!");
                    }
                    if (!this.ghk_abortClick.Unregister())
                    {
                        LOG.Warn("ghk_abortClick already unregistered!");
                    }
                    if (this.isTimerForLoopRunning)
                    {
                        this.isTimerForLoopRunning = false;
                        this.timerForLoop.Stop();
                        LOG.Info("Timer for loop has stopped.");
                    }
                    if (this.isTimerForInfinitelyRunning)
                    {
                        this.isTimerForInfinitelyRunning = false;
                        this.timerForInfinitely.Stop();
                    }
                }
                else if (wParam.ToInt32() == this.ghk_startClick.GetHashCode() && !this.isTimerForLoopRunning && !this.isTimerForInfinitelyRunning)
                {
                    //MessageBox.Show("StartClick pressed!");
                    LOG.Debug("ghk_startClick pressed");
                    this.mouseCoordinates.X = this.iudMouseX.Value.Value;
                    this.mouseCoordinates.Y = this.iudMouseY.Value.Value;
                    if (!this.ghk_startClick.Unregister())
                    {
                        LOG.Warn("ghk_startClick already unregistered!");
                    }
                    if (!this.ghk_abortClick.Register())
                    {
                        LOG.Warn("ghk_abortClick already registered!");
                    }
                    LOG.Debug("this.cbInfinitely.IsChecked = " + this.cbInfinitely.IsChecked);
                    if (this.cbInfinitely.IsChecked == false)
                    {
                        Settings.loops = this.iudLoops.Value.Value;
                        this.loops = this.iudLoops.Value.Value;
                        LOG.Debug("this.loops = " + this.loops);
                        this.timerForLoop.Interval = this.dudDelay.Value.Value * 1000.0;
                        this.isTimerForLoopRunning = true;
                        this.timerForLoop.Start();
                    }
                    else
                    {
                        this.timerForInfinitely.Interval = this.dudDelay.Value.Value * 1000.0;
                        this.isTimerForInfinitelyRunning = true;
                        this.timerForInfinitely.Start();
                    }
                }

            }
            return IntPtr.Zero;
        }

        public MainWindow()
        {
            BasicConfigurator.Configure();
            InitializeComponent();
        }

        private void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            this.mouseCoordinates.X = this.iudMouseX.Value.Value;
            this.mouseCoordinates.Y = this.iudMouseY.Value.Value;
            if (this.isTimerForLoopRunning)
            {
                if (!this.ghk_startClick.Register())
                {
                    LOG.Warn("ghk_startClick already registered!");
                }
                if (!this.ghk_abortClick.Unregister())
                {
                    LOG.Warn("ghk_abortClick already unregistered!");
                }
                this.timerForLoop.Stop();
                LOG.Info("Timer for loop has stopped.");
                this.isTimerForLoopRunning = false;
            }
            else if (this.isTimerForInfinitelyRunning)
            {
                if (!this.ghk_startClick.Register())
                {
                    LOG.Warn("ghk_startClick already registered!");
                }
                if (!this.ghk_abortClick.Unregister())
                {
                    LOG.Warn("ghk_abortClick already unregistered!");
                }
                this.isTimerForInfinitelyRunning = false;
            }
            else
            {
                if (!this.ghk_startClick.Unregister())
                {
                    LOG.Warn("ghk_startClick already unregistered!");
                }
                if (!this.ghk_abortClick.Register())
                {
                    LOG.Warn("ghk_abortClick already registered!");
                }
                if (this.cbInfinitely.IsChecked == false)
                {
                    Settings.loops = this.iudLoops.Value.Value;
                    this.loops = this.iudLoops.Value.Value;
                    this.timerForLoop.Interval = this.dudDelay.Value.Value * 1000.0;
                    this.timerForLoop.Start();
                    LOG.Info("Timer for loop has started.");
                }
                else
                {
                    this.isTimerForInfinitelyRunning = true;
                    this.timerForInfinitely.Interval = this.dudDelay.Value.Value * 1000.0;
                    this.timerForInfinitely.Start();
                }
            }
        }

        private void cbTopmost_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = this.cbTopmost.IsChecked == true;
        }

        private void TimerForUpdateUI(object sender, EventArgs e)
        {
            Point currMouseCoordinates = GetMousePosition();
            this.lblCursorPosition.Content = currMouseCoordinates.X + "," + currMouseCoordinates.Y;
            if (this.isTimerForLoopRunning)
            {
                this.lblDurationLeft.Content = string.Format("{0:F3} Sec", this.loops * this.dudDelay.Value);
                this.btnStartStop.Content = "Stop (ESC)";
            }
            else if (this.isTimerForInfinitelyRunning)
            {
                this.lblDurationLeft.Content = "currently infinitely";
                this.btnStartStop.Content = "Stop (ESC)";
            }
            else
            {
                this.lblDurationLeft.Content = string.Format("{0:F3} Sec", 0.0);
                this.btnStartStop.Content = "Start (F1)";
            }
        }

        private void TimerForLoop(object sender, ElapsedEventArgs e)
        {
            LeftMouseClick((int)this.mouseCoordinates.X, (int)this.mouseCoordinates.Y);
            this.isTimerForLoopRunning = true;
            if (this.loops-- <= 0)
            {
                if (!this.ghk_startClick.Register())
                {
                    LOG.Warn("ghk_startClick already registered!");
                }
                if (!this.ghk_abortClick.Unregister())
                {
                    LOG.Warn("ghk_abortClick already unregistered!");
                }
                this.loops = Settings.loops;
                this.isTimerForLoopRunning = false;
                this.timerForLoop.Stop();
                LOG.Info("Timer for loop has stopped.");
            }
        }

        private void TimerForInfinitely(object sender, ElapsedEventArgs e)
        {
            LeftMouseClick((int)this.mouseCoordinates.X, (int)this.mouseCoordinates.Y);
            if (!this.isTimerForInfinitelyRunning)
            {
                this.timerForInfinitely.Stop();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            this.ghk_abortClick = new Hotkeys(Hotkeys.Constants.NOMOD, KeyInterop.VirtualKeyFromKey(Key.Escape), handle);
            this.ghk_startClick = new Hotkeys(Hotkeys.Constants.NOMOD, KeyInterop.VirtualKeyFromKey(Key.F1), handle);

            //this.ghk_abortClick.Register();
            this.ghk_startClick.Register();

            this.iudMouseX.Maximum = (int)SystemParameters.VirtualScreenWidth;
            this.iudMouseY.Maximum = (int)SystemParameters.VirtualScreenHeight;

            this.timerForLoop = new Timer();
            this.timerForLoop.Elapsed += new ElapsedEventHandler(TimerForLoop);

            this.timerForInfinitely = new Timer();
            this.timerForInfinitely.Elapsed += new ElapsedEventHandler(TimerForInfinitely);

            this.timerForUpdateUI = new DispatcherTimer();
            this.timerForUpdateUI.Interval = TimeSpan.FromMilliseconds(50);
            this.timerForUpdateUI.Tick += new EventHandler(TimerForUpdateUI);
            this.timerForUpdateUI.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Write settings.xml
            WriteSettings();

            this.ghk_abortClick.Unregister();
            this.ghk_startClick.Unregister();
            if (this.isTimerForLoopRunning)
            {
                this.timerForLoop.Stop();
                LOG.Info("Timer for loop has stopped.");
            }
            if (this.isTimerForInfinitelyRunning)
            {
                this.timerForInfinitely.Stop();
            }
            this.timerForUpdateUI.Stop();
        }

        private void ReadSettings()
        {
            Settings.Read();
            iudMouseX.Value = Settings.xCoord;
            iudMouseY.Value = Settings.yCoord;
            dudDelay.Value = Settings.delay;
            iudLoops.Value = Settings.loops;
            cbInfinitely.IsChecked = Settings.isInfinitely;
            cbTopmost.IsChecked = Settings.isTopmost;
        }

        private void WriteSettings()
        {
            Settings.xCoord = iudMouseX.Value.Value;
            Settings.yCoord = iudMouseY.Value.Value;
            Settings.delay = dudDelay.Value.Value;
            Settings.loops = iudLoops.Value.Value;
            Settings.isInfinitely = cbInfinitely.IsChecked.Value;
            Settings.isTopmost = cbTopmost.IsChecked.Value;
            Settings.Write();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            // Read settings.xml if exists, otherwise use default values
            ReadSettings();
        }
    }
}
