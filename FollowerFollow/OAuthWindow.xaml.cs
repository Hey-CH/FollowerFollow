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
using System.Windows.Shapes;
using CoreTweet;
using static CoreTweet.OAuth;

namespace FollowerFollow {
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class OAuthWindow : Window {
        public ViewModel vm;
        public OAuthWindow() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            vm.Session = OAuth.Authorize(Properties.Resources.ConsumerKey, Properties.Resources.ConsumerSecret);
            System.Diagnostics.Process.Start(vm.Session.AuthorizeUri.AbsoluteUri);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            vm.Tokens = OAuth.GetTokens(vm.Session, vm.PINCode);
            Properties.Settings.Default.AccessToken = vm.Tokens.AccessToken;
            Properties.Settings.Default.AccessTokenSecret = vm.Tokens.AccessTokenSecret;
            Properties.Settings.Default.Save();
            this.Close();
        }
    }
}
