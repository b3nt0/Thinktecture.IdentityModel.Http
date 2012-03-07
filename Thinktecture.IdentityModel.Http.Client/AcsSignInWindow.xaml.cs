﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Thinktecture.IdentityModel.Http.Wpf
{
    public partial class AcsSignInWindow : Window
    {
        public string AcsNamespace { get; set; }
        public string Realm { get; set; }
        

        private List<IdentityProviderInformation> _providerList;
        private SynchronizationContext _syncContext;
        public JsonNotifyRequestSecurityTokenResponse Response { get; set; }
        
        public AcsSignInWindow()
        {
            InitializeComponent();
            _syncContext = SynchronizationContext.Current;
            Loaded += OnLoaded;
            
            var interop = new JavaScriptNotifyInterop();
            interop.ScriptNotify += this.OnScriptNotify;
            this.webBrowser.ObjectForScripting = interop;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.AcsNamespace)) throw new ArgumentException("Missing AcsNamespace");
            if (string.IsNullOrEmpty(this.Realm)) throw new ArgumentException("Missing Realm");
            
            this.Show();
            Mouse.OverrideCursor = Cursors.Wait;
            
            ThreadPool.QueueUserWorkItem(_ =>
                {
                    var disco = new IdentityProviderDiscoveryClient(this.AcsNamespace, this.Realm);
                    disco.GetCompleted += this.OnDiscoveryCompleted;
                    disco.GetAsync(IdentityProviderDiscoveryClient.Protocols.JavaScriptNotify);
                });
        }

        private void OnDiscoveryCompleted(object sender, IdentityProviderDiscoveryClient.GetCompletedEventArgs e)
        {
            var s = sender as IdentityProviderDiscoveryClient;
            s.GetCompleted -= OnDiscoveryCompleted;
            _providerList = e.IdentityProvider;
            
            _syncContext.Post(o =>
            {
                this.DataContext = _providerList;
                Mouse.OverrideCursor = Cursors.Arrow;
            }, null);
        }
        
        private void OnScriptNotify(object sender, ScriptNotifyEventArgs e)
        {
            this.Response = JsonNotifyRequestSecurityTokenResponse.FromJson(e.Data);
            this.DialogResult = true;
            this.Close();
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var provider = ((TextBlock)sender).DataContext as IdentityProviderInformation;
            if (provider != null)
            {
                this.webBrowser.Navigate(provider.LoginUrl);
                this.tabControl.SelectedIndex = 1;
            }
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
