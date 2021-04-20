using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using CoreTweet;
using static CoreTweet.OAuth;

namespace FollowerFollow {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {
        ViewModel vm;
        public MainWindow() {
            InitializeComponent();
            vm = new ViewModel();
            this.DataContext = vm;
            try {
                if (!string.IsNullOrEmpty(Properties.Settings.Default.AccessToken)) {
                    vm.Tokens = Tokens.Create(Properties.Resources.ConsumerKey
                                            , Properties.Resources.ConsumerSecret
                                            , Properties.Settings.Default.AccessToken
                                            , Properties.Settings.Default.AccessTokenSecret);
                    var res = vm.Tokens.Account.VerifyCredentials();
                    vm.Tokens.UserId = (long)res.Id;
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("screen_name", "twitter");
                    var users = vm.Tokens.Users.Lookup(ps);
                    if (users.Count == 0) throw new ApplicationException("Userが見つかりませんでした。");
                } else {
                    throw new ApplicationException("AccessTokenが設定されていません。");
                }
            } catch (Exception ex) {
                OAuthWindow ow = new OAuthWindow();
                ow.DataContext = vm;
                ow.vm = vm;
                ow.ShowDialog();
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e) {
            var followers = vm.Tokens.Followers.EnumerateIds(EnumerateMode.Next, user_id => vm.Tokens.UserId);
            var users = vm.Tokens.Friendships.Lookup(followers);
            for (int i = 0; i < users.Count; i++) {
                var user = users[i];
                var c = user.Connections;
                if (c.Contains("followed_by") && !c.Contains("following")) {
                    vm.Tokens.Friendships.Create(user_id => user.Id);
                    textBox1.Text += user.Name+"さんをフォローしました。\r\n";
                }
            }
        }
    }
    public class ViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        string _PINCode = "";
        public string PINCode {
            get { return _PINCode; }
            set {
                _PINCode = value;
                OnPropertyChanged("PINCode");
            }
        }
        public OAuthSession Session { get; set; }
        public Tokens Tokens { get; set; }
    }
}
