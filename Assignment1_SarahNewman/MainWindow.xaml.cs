using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
        // additional guidance for slider -- https://wpf-tutorial.com/hu/101/audio-video/how-to-creating-a-complete-audio-video-player/
        public bool audioPlaying = false;
        public bool dragSlider = false;
        TagLib.File file;
        OpenFileDialog dialog;

        public MainWindow()
        {
            InitializeComponent();

            // counter for slider
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            HideEditBoxes();
        }

        // increases timer of the song
        private void timer_Tick(Object sender, EventArgs e)
        {
            if ((myMediaPlayer != null) && (myMediaPlayer.NaturalDuration.HasTimeSpan) && !dragSlider)
            {
                SongSlider.Minimum = 0;
                SongSlider.Maximum = myMediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                SongSlider.Value = myMediaPlayer.Position.TotalSeconds;

                Slider_ValueChanged(SongSlider.Value);
            }
        }


        //---------- Executable Events ----------//
        private void Open_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            HideEditBoxes();

            // create new taglib and media player source based on file opened
            // displays album art if available upon opening file
            dialog = new OpenFileDialog();
            dialog.Filter = "Media files (*.mp3)|*.mp3|All files (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                ResetTags();
                HideEditBoxes();
                myMediaPlayer.Close();

                file = TagLib.File.Create(dialog.FileName);
                myMediaPlayer.Source = new Uri(dialog.FileName);
                myMediaPlayer.Play();

                DisplayArt();

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

        private void Save_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // save funciton for saving edited tags
        private void Save_Executed(Object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                using (file)
                {
                    // check if the file has valid tag information
                    if (file.Tag != null)
                    {
                        // closes file to allow for saving of new tags
                        myMediaPlayer.Source = null;
                        myMediaPlayer.Close();

                        // temporary string array for saving purposes
                        string[] artist = { EditArtist.Text };

                        file.Tag.Title = EditTitle.Text;
                        file.Tag.AlbumArtists = artist;
                        file.Tag.Album = EditAlbum.Text;

                        file.Save();
                        file.Dispose();
                        MessageBox.Show("Tags were updated successfully!");

                        // updates current screen with changes
                        HideEditBoxes();
                        myMediaPlayer.Source = new Uri(dialog.FileName); // re-opens originally selected file for smooth funcitonality
                        myMediaPlayer.Play();
                        ShowTagsBtn_Click(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("No valid tag information found for the file.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        //---------- functions for song slider ----------//
        private void SongSlider_DragStarted(Object sender, DragStartedEventArgs e)
        {
            dragSlider = true;
        }

        // once drag is complete, set new position in song
        private void SongSlider_DragCompleted(Object sender, DragCompletedEventArgs e)
        {
            dragSlider = false;
            myMediaPlayer.Position = TimeSpan.FromSeconds(SongSlider.Value);

            Slider_ValueChanged(SongSlider.Value);
        }

        // prints new value on song timer in the below format
        private void Slider_ValueChanged(double songTime)
        {
            SongTimer.Text = TimeSpan.FromSeconds(songTime).ToString(@"hh\:mm\:ss");
        }


        //---------- OnClick Events ----------//
        // function to display album art if applicable
        private void DisplayArt()
        {
            // album art tag -- https://stackoverflow.com/a/17905163
            if (0 < file.Tag.Pictures.Length)
            {
                // load image data into memorystream
                TagLib.IPicture pic = file.Tag.Pictures[0];
                MemoryStream ms = new MemoryStream(pic.Data.Data);
                ms.Seek(0, SeekOrigin.Begin);

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.EndInit();

                AlbumArtTag.Source = bitmap;
                AlbumArtTag.Visibility = Visibility.Visible;
            }
            else
            {
                // display default art
                DefaultArtTag.Visibility = Visibility.Visible;
                AlbumArtTag.Visibility = Visibility.Collapsed;
            }
        }

        // displays file tags
        private void ShowTagsBtn_Click(Object sender, RoutedEventArgs e)
        {
            HideEditBoxes();

            if (file != null)
            {
                // get current file tags
                if (null != file.Tag.Title) TitleTag.Text = file.Tag.Title;
                if (0 < file.Tag.AlbumArtists.Length) ArtistTag.Text = file.Tag.AlbumArtists.FirstOrDefault();
                if (null != file.Tag.Album) AlbumTag.Text = file.Tag.Album;
            }
        }

        // sets up textboxes for editing
        private void EditTagsBtn_Click(Object sender, RoutedEventArgs e)
        {
            if (file != null)
            {
                // make edit textboxes visible
                ShowEditBoxes();

                if (null == file.Tag.Title) EditTitle.Text = "Title";
                else EditTitle.Text = file.Tag.Title;

                if (0 == file.Tag.AlbumArtists.Length) EditArtist.Text = "Artist";
                else EditArtist.Text = file.Tag.AlbumArtists.FirstOrDefault();

                if (null == file.Tag.Album) EditAlbum.Text = "Album";
                else EditAlbum.Text = file.Tag.Album;
            }
        }

        // reset tags
        private void ResetTags()
        {
            TitleTag.Text = "";
            ArtistTag.Text = "";
            AlbumTag.Text = "";
        }

        // displays edit functionality and texboxes
        private void ShowEditBoxes()
        {
            TagEditor.Visibility = Visibility.Visible;
            SaveBtn.Visibility = Visibility.Visible;

            EditTitle.Visibility = Visibility.Visible;
            EditArtist.Visibility = Visibility.Visible;
            EditAlbum.Visibility = Visibility.Visible;
        }

        // hides edit functionality and texboxes
        private void HideEditBoxes()
        {
            TagEditor.Visibility = Visibility.Collapsed;
            SaveBtn.Visibility = Visibility.Collapsed;

            EditTitle.Visibility = Visibility.Collapsed;
            EditArtist.Visibility = Visibility.Collapsed;
            EditAlbum.Visibility = Visibility.Collapsed;
        }

        // exit program
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
