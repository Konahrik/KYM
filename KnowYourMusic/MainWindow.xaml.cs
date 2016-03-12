using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using KnowYourMusic.Details;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MahApps.Metro.Controls;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using KnowYourMusic.Features;

namespace KnowYourMusic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        [System.Runtime.InteropServices.DllImport("winmm.dll")]
        private static extern
            Boolean PlaySound(string lpszName, int hModule, int dwFlags);

        public WMPLib.WindowsMediaPlayer WMP = new WMPLib.WindowsMediaPlayer();
        public List<AudioResponse> AllAudioResults { get; private set; }
        public List<VideoResponse> AllVideoResults { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Slider.Value = 50;

            Browser.Visibility = Visibility.Visible;
            Browser.Navigate(String.Format("https://oauth.vk.com/authorize?client_id={0}&scope={1}&redirect_uri={2}&display=page&response_type=token",
                            ConfigurationManager.AppSettings["VKAppId"],
                            ConfigurationManager.AppSettings["VKScope"],
                            ConfigurationManager.AppSettings["VKRedirectUri"]));
        }

        private void OnSelectionChanged(Object sender, SelectionChangedEventArgs args)
        {
            if (Tabs.SelectedItem == VideoTab)
            {
                SaveAll.IsEnabled = false;
                SaveSelected.IsEnabled = false;
            }
            else
            {
                SaveAll.IsEnabled = true;
                SaveSelected.IsEnabled = true;
            }
        }

        private void WebBrouserNavigated(object sender, NavigationEventArgs e)
        {
            var clearUriFragment = e.Uri.Fragment.Replace("#", "").Trim();
            var parameters = HttpUtility.ParseQueryString(clearUriFragment);
            VkAccount.AccessToken = parameters.Get("access_token");
            VkAccount.UserId = parameters.Get("user_id");
            if (VkAccount.AccessToken != null && VkAccount.UserId != null)
            {
                Browser.Visibility = Visibility.Hidden;
            }
            else
            {
                MessageBox.Show("Для корректной работы приложения рекомендуется обновить IE.");
            }
        }

        private void LoadUserAudio(object sender, RoutedEventArgs e)
        {
            string userId;
            if (UserNameOrId.Text == "")
                userId = VkAccount.UserId;
            else
                userId = UserNameOrId.Text;
            var users = General.GetUsersInfo(userId);
            Title = String.Format("Audio files of {0} {1}", users.response[0].first_name, users.response[0].last_name);
            AllAudioResults = Audio.LoadAudio(userId);
            Compositions.ItemsSource = AllAudioResults;
        }

        private void LoadSearchResults(object sender, RoutedEventArgs e)
        {
            if (SearchRequest.Text != "")
            {
                AllAudioResults = Audio.SearchAudio(SearchRequest.Text);
                Compositions.ItemsSource = AllAudioResults;
                Title = String.Format("Music search results for '{0}'", SearchRequest.Text);
                return;
            }
        }

        private void PlayAudio(object sender, MouseButtonEventArgs e)
        {
            try
            {
                AudioResponse selected = (AudioResponse)Compositions.SelectedItem;
                if (selected != null)
                {
                    WMP.URL = selected.AudioUrl;
                    WMP.controls.play();
                    PlayingStatus.Text = "Playing:";
                    PlayingAudio.Text = String.Format(selected.Artist + " – " + selected.AudioTitle);

                }
            }
            catch (Exception) { }
        }
        private void PauseResumePlaying(object sender, RoutedEventArgs e)
        {
            if (PlayingAudio.Text != "")
            {
                if ((PlayingAudio.Text != "") && (PlayingStatus.Text == "Playing:"))
                {
                    WMP.controls.pause();
                    PlayingStatus.Text = "Paused:";
                }
                else
                {
                    WMP.controls.play();
                    PlayingStatus.Text = "Playing:";
                }
            }
        }

        private async void SaveSelectedAudio(object sender, RoutedEventArgs e)
        {
            var selectedBuf = Compositions.SelectedItems;
            var selected = selectedBuf.Cast<AudioResponse>().ToList();
            if (selected.Count > 0)
            {
                var dialog = new CommonOpenFileDialog { IsFolderPicker = true };
                var result = dialog.ShowDialog();

                if (result == CommonFileDialogResult.Ok)
                {
                    var path = dialog.FileName;
                    foreach (AudioResponse composition in selected)
                    {
                        await Audio.DownloadAudio(composition, path, ProgressBar);
                    }
                }
            }
        }

        private async void SaveAllAudio(object sender, RoutedEventArgs e)
        {
            var selectedBuf = Compositions.ItemsSource;
            var all = selectedBuf.Cast<AudioResponse>().ToList();
            if (all.Any())
            {
                var dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = true;
                CommonFileDialogResult result = dialog.ShowDialog();

                if (result == CommonFileDialogResult.Ok)
                {
                    var path = dialog.FileName;
                    foreach (AudioResponse composition in all)
                    {
                        await Audio.DownloadAudio(composition, path, ProgressBar);
                    }
                }
            }
        }

        private void Help(object sender, RoutedEventArgs e)
        {
            var Help = new Help();
            Help.Show();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider.Minimum = 0;
            Slider.Maximum = 100;
            WMP.settings.volume = Convert.ToInt32(Slider.Value);
        }

        private void FilterTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var str = FilterRequest.Text.ToLower();
            if (AllAudioResults != null && AllAudioResults.Any())
            {
                if (String.IsNullOrEmpty(str))
                {
                    Compositions.ItemsSource = AllAudioResults;
                    return;
                }
                Compositions.ItemsSource =
                    AllAudioResults.Where(
                        x =>
                            x.Artist.ToLower().IndexOf(str, StringComparison.Ordinal) >= 0 ||
                            x.AudioTitle.ToLower().IndexOf(str, StringComparison.Ordinal) > 0);
            }
        }

        private void LoadUserVideo(object sender, RoutedEventArgs e)
        {
            string userId;
            if (UserNameOrIdForVideo.Text == "")
                userId = VkAccount.UserId;
            else
                userId = UserNameOrIdForVideo.Text;

            var users = General.GetUsersInfo(userId);
            Title = String.Format("Video files of {0} {1}", users.response[0].first_name, users.response[0].last_name);
            AllVideoResults = Video.LoadVideo(userId);
            Videos.ItemsSource = AllVideoResults;
        }

        private void LoadSearchResultsForVideo(object sender, RoutedEventArgs e)
        {
            if (SearchRequestForVideo.Text != "")
            {
                AllVideoResults = Video.SearchVideo(SearchRequestForVideo.Text);
                Videos.ItemsSource = AllVideoResults;
                Title = String.Format("Video search results for '{0}'", SearchRequestForVideo.Text);
                return;
            }
        }

        private void PlayVideo(object sender, MouseButtonEventArgs e)
        {
            try
            {
                VideoResponse selected = (VideoResponse)Videos.SelectedItem;
                if (selected != null)
                {
                    var VideoPlayer = new VideoPlayer();
                    VideoPlayer.Show();
                    VideoPlayer.PlayerBrowser.Navigate(String.Format(selected.VideoPlayer));
                    VideoPlayer.Title = String.Format(selected.VideoTitle);
                }
            }
            catch (Exception) { }
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

