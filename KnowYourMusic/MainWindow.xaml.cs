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
        public List<AudioResponse> AllResults { get; private set; }

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

        private string VkRequest(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            var responseText = reader.ReadToEnd();
            return responseText;
        }

        private void LoadAudio(string userId)
        {
            var str = string.Format("https://api.vk.com/method/users.get?uids={0}", userId);
            var responseText = VkRequest(str);

            try
            {
                var users = JsonConvert.DeserializeObject<VkUsers>(responseText);
                var uid = users.response[0].uid;

                str = string.Format("https://api.vk.com/method/audio.get?uid={0}&access_token={1}", uid, VkAccount.AccessToken);
                responseText = VkRequest(str);
                var result = JsonConvert.DeserializeObject<VkAudio>(responseText);
                AllResults = result.response;
                Compositions.ItemsSource = result.response;
                Title = String.Format("Audio files of {0} {1}", users.response[0].first_name, users.response[0].last_name);

            }
            catch (Exception) { }
        }

        private void LoadUserAudio(object sender, RoutedEventArgs e)
        {
            if (UserNameOrId.Text == "")
            {
                LoadAudio(VkAccount.UserId);
                return;
            }
            if (UserNameOrId.Text != "")
            {
                LoadAudio(UserNameOrId.Text);
                return;
            }
        }
        

        private void SearchAudio(string q)
        {
            try
            {
                var str = string.Format("https://api.vk.com/method/audio.search?q={0}&access_token={1}&count=1000&auto_complete=1", q, VkAccount.AccessToken);

                var responseText = Regex.Replace(VkRequest(str), "[\t|\r\n]", "");
                if (responseText.IndexOf("response\":[") != -1)
                {
                    int start = responseText.IndexOf('[') + 1;
                    int end = responseText.IndexOf(',', start);
                    responseText = responseText.Substring(0, start) + responseText.Substring(end + 1);
                }
                var result = JsonConvert.DeserializeObject<VkAudio>(responseText);
                AllResults = result.response;
                Compositions.ItemsSource = result.response;
                Title = String.Format("Music search results for '{0}'", q);
            }
            catch (Exception) { }
        }
        private void LoadSearchResults(object sender, RoutedEventArgs e)
        {
            if (SearchRequest.Text != "")
            {
                SearchAudio(SearchRequest.Text);
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
                    WMP.URL = selected.url;
                    WMP.controls.play();
                    PlayingStatus.Text = "Playing:";
                    PlayingAudio.Text = String.Format(selected.artist + " – " + selected.title);
                    
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

        private async Task DownloadAudio(AudioResponse composition, String path)
        {
            try
            {
                if (path[path.Length - 1] != '\\')
                {
                    path = path + "\\";
                }
                var fileName = composition.artist + " – " + composition.title;
                if (fileName.Length > 40)
                {
                    fileName = fileName.Substring(0, 40);
                }
                fileName = fileName.Replace(":", "").Replace("\\", "").Replace("/", "").Replace("*", "").Replace("?", "").Replace("\"", "");
                using (var client = new WebClient())
                {

                    client.DownloadProgressChanged += (o, args) =>
                    {
                        ProgressBar.Value = args.ProgressPercentage;
                    };
                    client.DownloadFileCompleted += (o, args) =>
                    {
                        ProgressBar.Value = 0;
                    };
                    await client.DownloadFileTaskAsync(new Uri(composition.url), path + fileName + ".mp3");
                }
            }
            catch (Exception)
            {
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
                        await DownloadAudio(composition, path);
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
                        await DownloadAudio(composition, path);
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
            if (AllResults != null && AllResults.Any())
            {
                if (String.IsNullOrEmpty(str))
                {
                    Compositions.ItemsSource = AllResults;
                    return;
                }
                Compositions.ItemsSource =
                    AllResults.Where(
                        x =>
                            x.artist.ToLower().IndexOf(str, StringComparison.Ordinal) >= 0 ||
                            x.title.ToLower().IndexOf(str, StringComparison.Ordinal) > 0);
            }
        }

        private void Albums_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void Albums_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
