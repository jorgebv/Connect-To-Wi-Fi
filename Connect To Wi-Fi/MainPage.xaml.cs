using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.WiFi;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.Security.Credentials;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Connect_To_Wi_Fi
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<WiFiAvailableNetwork> ListOfNetworks { get; set; }

        private Visibility _InitializingUI;
        public Visibility InitializingUI { get { return _InitializingUI; }
                                     set { _InitializingUI = value;
                                           NotifyPropertyChanged("InitializingUI");
                                           NotifyPropertyChanged("ShowProgressRing"); } }

        private Visibility _ShowError;
        public Visibility ShowError { get { return _ShowError; }
                                      set { _ShowError = value; NotifyPropertyChanged("ShowError"); } }

        public bool ShowProgressRing { get { return InitializingUI == Visibility.Visible; } }

        public WiFiAdapter Adapter { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ViewModel()
        {
            ListOfNetworks = new ObservableCollection<WiFiAvailableNetwork>();
            InitializingUI = Visibility.Visible;
            ShowError = Visibility.Collapsed;
        }
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ViewModel Model { get; set; }

        public MainPage()
        {
            Model = new ViewModel();
            this.DataContext = Model;
            this.InitializeComponent();
            StartInitializingAsyncUI();
        }

        private async void StartInitializingAsyncUI()
        {
            var adapters = await WiFiAdapter.FindAllAdaptersAsync();
            var firstAdapter = adapters.FirstOrDefault();
            if (firstAdapter == null)
            {
                Model.ShowError = Visibility.Visible;
                Model.InitializingUI = Visibility.Collapsed;
                return;
            }

            Model.Adapter = firstAdapter;
            var mutableList = new List<WiFiAvailableNetwork>(firstAdapter.NetworkReport.AvailableNetworks);
            mutableList.Sort((x, y) => x.Ssid.CompareTo(y.Ssid));

            foreach (var network in mutableList)
            {
                if (!string.IsNullOrEmpty(network.Ssid))
                {
                    Model.ListOfNetworks.Add(network);
                }
            }
            
            Model.InitializingUI = Visibility.Collapsed;
        }

        private void UserPassClicked(object sender, RoutedEventArgs e)
        {
            PasswordCredential credential = new PasswordCredential();
            credential.UserName = UIUsernameTextBox.Text;
            credential.Password = UIPasswordTextBox.Text;
            Model.Adapter.ConnectAsync((WiFiAvailableNetwork)UIListOfNetworks.SelectedItem, WiFiReconnectionKind.Manual, credential);
        }

        private void WpsPinClicked(object sender, RoutedEventArgs e)
        {
            PasswordCredential credential = new PasswordCredential();
            credential.Password = UIWpsPinTextBox.Text;
            Model.Adapter.ConnectAsync((WiFiAvailableNetwork)UIListOfNetworks.SelectedItem, WiFiReconnectionKind.Manual, credential, string.Empty, WiFiConnectionMethod.WpsPin);
        }

        private void WpsPushClicked(object sender, RoutedEventArgs e)
        {
            Model.Adapter.ConnectAsync((WiFiAvailableNetwork)UIListOfNetworks.SelectedItem, WiFiReconnectionKind.Manual, null, string.Empty, WiFiConnectionMethod.WpsPushButton);
        }
    }

    public class InverseVisibilityConvert : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target must be a Visibility");

            var incomingVis = (Visibility)value;
            if (incomingVis == Visibility.Visible)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
