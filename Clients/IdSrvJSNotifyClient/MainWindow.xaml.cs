using System;
using System.Windows;
using Thinktecture.IdentityModel.Http.Wpf;
using Thinktecture.Samples;

namespace IdSrvJSNotifyClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void _btnSignin_Click(object sender, RoutedEventArgs e)
        {
            var signin = new JSNotifySignInWindow
                {
                    EndpointUrl = new Uri(string.Format("https://{0}/idsrv/issue/jsnotify?realm={1}&tokenType={2}",
                                                Constants.IdSrv,
                                                Constants.Realm,
                                                "http://schemas.xmlsoap.org/ws/2009/11/swt-token-profile-1.0"))
                };

            signin.SignIn();
        }

        private void _btnCallService_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
