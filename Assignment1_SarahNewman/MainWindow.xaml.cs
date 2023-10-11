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

            TagEditor.Visibility = Visibility.Collapsed;
            SaveBtn.Visibility = Visibility.Collapsed;
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
            TagEditor.Visibility = Visibility.Collapsed;
            SaveBtn.Visibility = Visibility.Collapsed;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Media files (*.mp3)|*.mp3|All files (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                // reset tags
                TitleTag.Text = "";
                ArtistTag.Text = "";
                YearTag.Text = "";
                AlbumTag.Text = "";

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
            TagEditor.Visibility = Visibility.Collapsed;
            SaveBtn.Visibility = Visibility.Collapsed;
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
            TagEditor.Visibility = Visibility.Collapsed;
            SaveBtn.Visibility = Visibility.Collapsed;

            // set tags to readonly
            TitleTag.IsReadOnly = true;
            ArtistTag.IsReadOnly = true;
            AlbumTag.IsReadOnly = true;
            YearTag.IsReadOnly = true;

            if (currFile != null)
            {
                // get tags
                var title = currFile.Tag.Title;
                var artist = currFile.Tag.AlbumArtists.FirstOrDefault();
                var album = currFile.Tag.Album;
                var year = currFile.Tag.Year;

                if (title != null) TitleTag.Text = title;
                if (artist != null) ArtistTag.Text = artist;
                if (album != null) AlbumTag.Text = album;
                if (year != 0) YearTag.Text = year.ToString();

                // figure out album art
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();

                if (bitmap != null)
                {
                    ArtTag.Source = bitmap;
                } else
                {
                    //ArtTag.Source = 
                }
                
            }
        }

        private void EditTagsBtn_Click(Object sender, RoutedEventArgs e)
        {
            if (currFile != null)
            {
                myMediaPlayer.Stop();
                TagEditor.Visibility = Visibility.Visible;
                SaveBtn.Visibility = Visibility.Visible;

                // set tags to edit
                TitleTag.IsReadOnly = false;
                ArtistTag.IsReadOnly = false;
                AlbumTag.IsReadOnly = false;
                YearTag.IsReadOnly = false;

                if (null == currFile.Tag.Title) TitleTag.Text = "Title: ";
                else TitleTag.Text = currFile.Tag.Title;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (null != TitleTag.Text) currFile.Tag.Title = TitleTag.Text;
            if (null != ArtistTag.Text) currFile.Tag.Artists[0] = ArtistTag.Text;
            if (null != AlbumTag.Text) currFile.Tag.Album = AlbumTag.Text;

            if (null != YearTag.Text)
            {
                int temp = Int32.Parse(YearTag.Text);
                currFile.Tag.Year = (uint)temp;
            }

            currFile.Save();
        }
    }
}
