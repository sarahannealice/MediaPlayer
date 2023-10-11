using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Assignment1_SarahNewman
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // additional guidance -- https://wpf-tutorial.com/hu/101/audio-video/how-to-creating-a-complete-audio-video-player/
        public bool audioPlaying = false;
        public bool dragSlider = false;
        TagLib.File currFile;

        public MainWindow()
        {
            InitializeComponent();

            OpenFileImg.BringIntoView();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if ((myMediaPlayer != null) && (myMediaPlayer.NaturalDuration.HasTimeSpan) && !dragSlider)
            {
                SongSlider.Minimum = 0;
                SongSlider.Maximum = myMediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                SongSlider.Value = myMediaPlayer.Position.TotalSeconds;

                Slider_ValueChanged(SongSlider.Value);
            }
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Media files (*.mp3)|*.mp3|All files (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                currFile = TagLib.File.Create(dialog.FileName);
                myMediaPlayer.Source = new Uri(dialog.FileName);
                myMediaPlayer.Play();
            }
        }

        private void Play_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Play_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            myMediaPlayer.Play();
            audioPlaying = true;
        }

        private void Pause_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Pause_Executed(Object sender, ExecutedRoutedEventArgs e) 
        { 
            myMediaPlayer.Pause(); 
        }

        private void Stop_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Stop_Executed(Object sender, ExecutedRoutedEventArgs e) 
        { 
            myMediaPlayer.Stop(); 
            audioPlaying = false;
        }

        private void SongSlider_DragStarted(object sender, DragStartedEventArgs e)
        {
            dragSlider = true;
        }

        private void SongSlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            dragSlider = false;
            myMediaPlayer.Position = TimeSpan.FromSeconds(SongSlider.Value);

            Slider_ValueChanged(SongSlider.Value);
        }

        private void Slider_ValueChanged(double songTime)
        {
            SongTimer.Text = TimeSpan.FromSeconds(songTime).ToString(@"hh\:mm\:ss");
        }

        private void ShowTagsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (currFile != null)
            {
                var year = currFile.Tag.Year;
                var title = currFile.Tag.Title;
                var artist = currFile.Tag.AlbumArtists.FirstOrDefault();

            }
        }
    }
}
