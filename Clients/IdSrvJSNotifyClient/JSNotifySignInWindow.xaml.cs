using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace Thinktecture.IdentityModel.Http.Wpf
{
    public partial class JSNotifySignInWindow : Window
    {
        public Uri EndpointUrl { get; set; }

        private SynchronizationContext _syncContext;
        public JsonNotifyRequestSecurityTokenResponse Response { get; set; }

        public JSNotifySignInWindow()
        {
            InitializeComponent();
            _syncContext = SynchronizationContext.Current;
            //Loaded += OnLoaded;
            
            var interop = new JavaScriptNotifyInterop();
            interop.ScriptNotify += this.OnScriptNotify;
            this.webBrowser.ObjectForScripting = interop;
        }

        public void SignIn()
        {
            webBrowser.Navigate(EndpointUrl);
            this.Show();
        }

        //private void OnLoaded(object sender, RoutedEventArgs e)
        //{
        //    webBrowser.Navigate(EndpointUrl);
        //    this.Show();
        //}

        private void OnScriptNotify(object sender, ScriptNotifyEventArgs e)
        {
            this.Response = JsonNotifyRequestSecurityTokenResponse.FromJson(e.Data);
            this.DialogResult = true;
            this.Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        [ComVisible(true)]
        public class JavaScriptNotifyInterop
        {
            public event EventHandler<ScriptNotifyEventArgs> ScriptNotify;

            public void Notify(string data)
            {
                OnScriptNotify(data);
            }

            protected virtual void OnScriptNotify(string data)
            {
                if (ScriptNotify != null)
                {
                    ScriptNotify.Invoke(this, new ScriptNotifyEventArgs { Data = data });
                }
            }
        }

        public class ScriptNotifyEventArgs : EventArgs
        {
            public string Data { get; set; }
        }
    }
}
