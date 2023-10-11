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
        string title;
        string[] artist;
        string album;

        // additional guidance for slider -- https://wpf-tutorial.com/hu/101/audio-video/how-to-creating-a-complete-audio-video-player/
        public bool audioPlaying = false;
        public bool dragSlider = false;
        TagLib.File file;
        OpenFileDialog dialog;

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

            dialog = new OpenFileDialog();
            dialog.Filter = "Media files (*.mp3)(*.mp4)|*.mp3|All files (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                ResetTags();

                // hide edit textboxes

                HideEditBoxes();

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

        private void Save_Executed(Object sender, ExecutedRoutedEventArgs e)
        {


            if (null != TitleTag.Text) file.Tag.Title = TitleTag.Text;
            if (null != ArtistTag.Text) file.Tag.Artists[0] = ArtistTag.Text;
            if (null != AlbumTag.Text) file.Tag.Album = AlbumTag.Text;

            try
            {
                using (file)
                {
                    // Check if the file has valid tag information
                    if (file.Tag != null)
                    {
                        // Update the title tag with the content from the TextBox
                        file.Tag.Title = EditTitle.Text;

                        myMediaPlayer.Source = null;
                        file.Save();
                        myMediaPlayer.Source = new Uri(dialog.FileName);
                        MessageBox.Show("Title tag updated successfully!");
                    }
                    else
                    {
                        MessageBox.Show("No valid tag information found for the file.");
                    }
                }  // The using block ensures that the file is properly closed after use
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void SongSlider_DragStarted(Object sender, DragStartedEventArgs e)
        {
            dragSlider = true;
        }

        private void SongSlider_DragCompleted(Object sender, DragCompletedEventArgs e)
        {
            dragSlider = false;
            myMediaPlayer.Position = TimeSpan.FromSeconds(SongSlider.Value);

            Slider_ValueChanged(SongSlider.Value);
        }

        private void Slider_ValueChanged(double songTime)
        {
            SongTimer.Text = TimeSpan.FromSeconds(songTime).ToString(@"hh\:mm\:ss");
        }


        //---------- OnClick Events ----------//
        private void ShowTagsBtn_Click(Object sender, RoutedEventArgs e)
        {
            HideEditBoxes();

            if (file != null)
            {
                // get tags
                var title = file.Tag.Title;
                var artist = file.Tag.AlbumArtists.FirstOrDefault();
                var album = file.Tag.Album;


                if (title != null) TitleTag.Text = title;
                if (artist != null) ArtistTag.Text = artist;
                if (album != null) AlbumTag.Text = album;
            }
        }

        private void EditTagsBtn_Click(Object sender, RoutedEventArgs e)
        {
            if (file != null)
            {
                // make edit textboxes visible
                ShowEditBoxes();

                if (null == file.Tag.Title)
                {
                    EditTitle.Text = "Title";
                }
                else
                {
                    EditTitle.Text = file.Tag.Title;
                }


                /*
                if (null == currFile.Tag.Performers) ArtistTag.Text = "Artist";
                else ArtistTag.Text = currFile.Tag.Performers[0];

                if (null == currFile.Tag.Album) AlbumTag.Text = "Album";
                else AlbumTag.Text = currFile.Tag.Album;
                */
            }
        }

        private void DisplayArt()
        {
            // album art tag -- https://stackoverflow.com/a/17905163
            if (0 < file.Tag.Pictures.Length)
            {
                // load image data into memorystream
                TagLib.IPicture pic = file.Tag.Pictures[0];
                MemoryStream ms = new MemoryStream(pic.Data.Data);
                ms.Seek(0, SeekOrigin.Begin);

                // imagesource for system.windows.controls.image
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.EndInit();

                AlbumArtTag.Source = bitmap;
                AlbumArtTag.Visibility = Visibility.Visible;
            }
            else
            {
                DefaultArtTag.Visibility = Visibility.Visible;
                AlbumArtTag.Visibility = Visibility.Collapsed;

            }
        }

        private void ResetTags()
        {
            // reset tags
            TitleTag.Text = "";
            ArtistTag.Text = "";
            AlbumTag.Text = "";
        }

        private void ShowEditBoxes()
        {
            TagEditor.Visibility = Visibility.Visible;
            SaveBtn.Visibility = Visibility.Visible;

            EditTitle.Visibility = Visibility.Visible;
        }

        private void HideEditBoxes()
        {
            TagEditor.Visibility = Visibility.Collapsed;
            SaveBtn.Visibility = Visibility.Collapsed;
            EditTitle.Visibility = Visibility.Collapsed;
        }

        private void SaveBtn_Click(Object sender, RoutedEventArgs e)
        {
            if (null != TitleTag.Text) file.Tag.Title = TitleTag.Text;
            /*
            if (null != ArtistTag.Text) currFile.Tag.Artists[0] = ArtistTag.Text;
            if (null != AlbumTag.Text) currFile.Tag.Album = AlbumTag.Text;
            */

            try
            {
                using (file)
                {
                    // Check if the file has valid tag information
                    if (file.Tag != null)
                    {
                        // Update the title tag with the content from the TextBox
                        file.Tag.Title = EditTitle.Text;

                        // Save the changes to the file
                        file.Save();
                        file.Dispose();
                        MessageBox.Show("Title tag updated successfully!");
                    }
                    else
                    {
                        MessageBox.Show("No valid tag information found for the file.");
                    }
                }  // The using block ensures that the file is properly closed after use
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
    }
}
