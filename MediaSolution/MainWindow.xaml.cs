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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using System.Windows.Controls.Primitives;

namespace MediaSolution
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        DispatcherTimer timer;
        bool dragStarted = false, volDragStarted = false;
        double vol = 0;

        public MainWindow()
        {
            InitializeComponent();
            Uri iconUri = new Uri("C:/Users/User/Documents/Visual Studio 2013/Projects/MediaSolution/MediaSolution/windows_media_player.png", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += new EventHandler(timer_Tick);
            sliVol.Value = 100;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            sliSeek.Value = mePlayer.Position.TotalSeconds;
        }

        private void btnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (mePlayer.Source != null)
            {
                if (btn.Content.Equals("Play"))
                {
                    mePlayer.Play();
                    btn.Content = "Pause";
                    timer.Start();
                }
                else if (btn.Content.Equals("Pause"))
                {
                    mePlayer.Pause();
                    btn.Content = "Play";
                }
            }
            else
            {
                MessageBox.Show("Please drag and drop video to play");
            } 
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (mePlayer.Source != null)
            {
                mePlayer.Stop();
                btnPlayPause.Content = "Play";
            }
            else
            {
                MessageBox.Show("There is no video to stop");
            } 
        }

        private void sliVol_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!volDragStarted)
            {
                mePlayer.Volume = (double)sliVol.Value / 100;

                if (sliVol.Value > 0)
                    tBtnMute.IsChecked = false;
                else
                    tBtnMute.IsChecked = true;
            }
        }

        private void sliSeek_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mePlayer.Source != null)
            {
                if (!dragStarted)
                {
                    mePlayer.Position = TimeSpan.FromSeconds(sliSeek.Value);
                }

                lblSeek.Content = mePlayer.Position.ToString(@"hh\:mm\:ss");
            }
        }

        private void MySlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.dragStarted = false;
            mePlayer.Position = TimeSpan.FromSeconds(sliSeek.Value);
            timer.Start();
        }

        private void MySlider_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.dragStarted = true;
            timer.Stop();
        }

        private void VolSlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.volDragStarted = false;
            mePlayer.Volume = (double)sliVol.Value / 100;
        }

        private void VolSlider_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.volDragStarted = true;
        }
        
        private void MP_Drop(object sender, DragEventArgs e)
        {
            string filename = (string)((DataObject)e.Data).GetFileDropList()[0];
            mePlayer.Source = new Uri(filename);
            mePlayer.LoadedBehavior = MediaState.Manual;
            mePlayer.UnloadedBehavior = MediaState.Manual;
            mePlayer.Volume = (double)sliVol.Value / 100;
            sliSeek.Value = 0;
            mePlayer.Play();
            mePlayer.Pause();

            if (mePlayer.Source.IsFile)
            {
                filename = System.IO.Path.GetFileName(mePlayer.Source.LocalPath);
                maFlyout.Content = filename;
            }
        }

        private void mePlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            TimeSpan ts = mePlayer.NaturalDuration.TimeSpan;
            sliSeek.Maximum = ts.TotalSeconds;
            lblTotalSeek.Content = mePlayer.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss");
        }

        private void mePlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            btnPlayPause.Content = "Play";
            sliSeek.Value = 0;
            mePlayer.Position = TimeSpan.FromSeconds(sliSeek.Value);
            mePlayer.Play();
            mePlayer.Pause();
            timer.Stop();
        }

        private void sliVol_MouseEnter(object sender, MouseEventArgs e)
        {
            sliVol.ToolTip = sliVol.Value.ToString();
        }

        private void tBtnMute_Click(object sender, RoutedEventArgs e)
        {
            if (tBtnMute.IsChecked == true)
            {
                vol = sliVol.Value;
                sliVol.Value = 0;
            }
            else
                sliVol.Value = vol;
        }

        private void tBtnExpand_Click(object sender, RoutedEventArgs e)
        {
            if (tBtnExpand.IsChecked == true)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }
    }
}
