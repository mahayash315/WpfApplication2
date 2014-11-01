using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication2
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        VideoCaptureDevice videoSource;
        ImageSourceConverter imageSourceConverter;

        public MainWindow()
        {
            InitializeComponent();
            Initialize();

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            this.Closed += MainWindow_Closed;
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            if (videoSource != null)
            {
                videoSource.WaitForStop();
            }
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (videoSource != null)
            {
                videoSource.SignalToStop();
            }
        }

        public void Initialize()
        {
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (0 < videoDevices.Count)
            {
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            }

            imageSourceConverter = new ImageSourceConverter();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (videoSource == null)
            {
                MessageBox.Show("利用可能なカメラが存在しません.");
                return;
            }

            VideoCapabilities[] capabilities = videoSource.VideoCapabilities;
            videoSource.VideoResolution = capabilities[0];
            videoSource.NewFrame += videoSource_NewFrame;

            videoSource.Start();
        }

        void videoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();

            using (Stream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);
                stream.Seek(0, SeekOrigin.Begin);
                try
                {
                    Dispatcher.Invoke(delegate()
                    {

                        ImageView.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    }
                    );
                }
                catch (InvalidOperationException e)
                {
                    Debug.WriteLine(e);
                }
            }
        }
    }
}
