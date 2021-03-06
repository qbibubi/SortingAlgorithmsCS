using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Security.Cryptography;

namespace SortingAlgorithms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    

    // All of this bullshit code is used to not fuck up maximizing the window.
    public partial class MainWindow : Window
    {
        private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }
            return (IntPtr)0;
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero)
            {
                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }
            Marshal.StructureToPtr(mmi, lParam, true);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>x coordinate of point.</summary>
            public int x;
            /// <summary>y coordinate of point.</summary>
            public int y;
            /// <summary>Construct a point of coordinates (x,y).</summary>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public static readonly RECT Empty = new RECT();
            public int Width { get { return Math.Abs(right - left); } }
            public int Height { get { return bottom - top; } }
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
            public RECT(RECT rcSrc)
            {
                left = rcSrc.left;
                top = rcSrc.top;
                right = rcSrc.right;
                bottom = rcSrc.bottom;
            }
            public bool IsEmpty { get { return left >= right || top >= bottom; } }
            public override string ToString()
            {
                if (this == Empty) { return "RECT {Empty}"; }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }
            public override bool Equals(object obj)
            {
                if (!(obj is Rect)) { return false; }
                return (this == (RECT)obj);
            }
            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode() => left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2) { return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom); }
            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2) { return !(rect1 == rect2); }
        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        public MainWindow()
        {
            InitializeComponent();

            SourceInitialized += (s, e) =>
            {
                IntPtr handle = (new WindowInteropHelper(this)).Handle;
                HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowProc));
            };

            // custom title bar buttons
            Minimize.Click += (s, e) => WindowState = WindowState.Minimized;
            Maximize.Click += (s, e) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            Exit.Click += (s, e) => Close();
        }

        public List<int> data = new List<int>();
        private List<int> LoadFile(List<int> list)
        {
            DataDisplay.Items.Clear();
            list.Clear();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select input file";
            openFileDialog.Filter = "txt files|*.txt";

            if (openFileDialog.ShowDialog() == true)
            {
                StreamReader reader = new StreamReader(openFileDialog.OpenFile());

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    int value = 0;

                    if (!string.IsNullOrWhiteSpace(line) && int.TryParse(line, out value))
                        data.Add(value);
                }
                MessageBox.Show("File loaded successfully!", "Success!");
            }

            foreach (int num in list)
            {
                DataDisplay.Items.Add(num.ToString());
            }

            return list;
        }

        private void LoadFile_Click(object sender, RoutedEventArgs e)
        {
            LoadFile(data);
        }

        /* Helper function for setting contents of labels (current alg, elapsed time, data size) */
        private void StringChanged(string alg, long delta_time, int data_size, List<int> list)
        {
            DataDisplay.Items.Clear();

            CurrentAlgorithmLabel.Content = "Current algorithm: " + alg;
            ElapsedTimeLabel.Content = "Elapsed time: " + delta_time + "ms";
            DataSetSizeLabel.Content = "Data size: " + data_size.ToString();
            
            foreach(int num in list)
            {
                DataDisplay.Items.Add(num.ToString());
            }
        }

        private int GetIterations(TextBox textbox)
        {
            string s_it = textbox.Text;
            int it = Int32.Parse(s_it);

            return it;
        }
        private void BubbleSort_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            int it = GetIterations(CustomInput);
            stopwatch.Start();

            for(int i = 0; i < it; i++)
                SortAlgs.BubbleSort(data);

            stopwatch.Stop();

            StringChanged("Bubble sort", stopwatch.ElapsedMilliseconds, data.Count, data);
        }

        private void InsertionSort_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            int it = GetIterations(CustomInput);

            stopwatch.Start();
            for (int i = 0; i < it; i++)
                SortAlgs.InsertionSort(data);

            stopwatch.Stop();

            StringChanged("Insertion sort", stopwatch.ElapsedMilliseconds, data.Count, data);
        }

        private void Quicksort_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            int it = GetIterations(CustomInput);

            stopwatch.Start();
            for (int i = 0; i < it; i++)
                SortAlgs.Quicksort(data, 0, data.Count-1);

            stopwatch.Stop();

            StringChanged("Quicksort", stopwatch.ElapsedMilliseconds, data.Count, data);
        }

        private void Heapsort_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            int it = GetIterations(CustomInput);

            stopwatch.Start();
            for (int i = 0; i < it; i++)
                SortAlgs.Heapsort(data, data.Count);

            stopwatch.Stop();
            StringChanged("Heapsort", stopwatch.ElapsedMilliseconds, data.Count, data);
        }

        private void Mergesort_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            int it = GetIterations(CustomInput);

            stopwatch.Start();
            for (int i = 0; i < it; i++)
                SortAlgs.Mergesort(data, 0, data.Count - 1);

            stopwatch.Stop();
            StringChanged("Mergesort", stopwatch.ElapsedMilliseconds, data.Count, data);
        }

        /* Disables possibility for input of non-numeric values into textbox */
        private void NumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            int iValue = -1;

            if (Int32.TryParse(textBox.Text, out iValue) == false)
            {
                TextChange textChange = e.Changes.ElementAt<TextChange>(0);
                int iAddedLength = textChange.AddedLength;
                int iOffset = textChange.Offset;

                textBox.Text = textBox.Text.Remove(iOffset, iAddedLength);
            }
        }

        private List<int> ShuffleList(List<int> list)
        {
            DataDisplay.Items.Clear();

            var rand = new Random();
            var randData = list.OrderBy(x => rand.Next()).ToList();

            foreach (int num in list)
            {
                DataDisplay.Items.Add(num.ToString());
            }

            return randData;
        }

        private void DataRandomizer_Click(object sender, RoutedEventArgs e)
        {
            data = ShuffleList(data);
        }
    }
}
