﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Msagl.Drawing;
using System.Windows.Input;
using viewmodel;
using Visualizer.Panes;
using Visualizer.Auth;

namespace Visualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Graph[] _graphs = new Graph[2];
        private UserPane _userpane;
        private DevicePane _devicepane;
        private ApplicationPane _apppane;
        private DeploymentsPane _deploymentpane;
        private PackagePane _packagespane;
        private List<SccmCollection> _highlightedcollections;
        private SccmConnector _connector;
        private string _site;

        public MainWindow()
        {
            InitializeComponent();
            this.Login();          
        }

        private void Login()
        {
            this._connector = new SccmConnector();
            this._highlightedcollections = new List<SccmCollection>();
            LoginViewModel loginviewmodel = new LoginViewModel();
            LoginWindow loginwindow = new LoginWindow();

            loginwindow.DataContext = loginviewmodel;

            loginwindow.cancelbtn.Click += (sender, e) => {
                loginwindow.Close();
                this.Close();
            };

            loginwindow.okbtn.Click += (sender, e) => {
                if (loginviewmodel.TryConnect(this._connector, loginwindow.pwdbx.Password) == true) { this.Startup(loginwindow, loginviewmodel); }
            };

            loginwindow.pwdbx.KeyUp += (sender, e) => {
                if (e.Key == Key.Enter)
                { if (loginviewmodel.TryConnect(this._connector, loginwindow.pwdbx.Password) == true) { this.Startup(loginwindow, loginviewmodel); } }    
            };

            loginwindow.usertb.KeyUp += (sender, e) => {
                if (e.Key == Key.Enter)
                { if (loginviewmodel.TryConnect(this._connector, loginwindow.pwdbx.Password) == true) { this.Startup(loginwindow, loginviewmodel); } }
            };

            loginwindow.domaintb.KeyUp += (sender, e) => {
                if (e.Key == Key.Enter)
                { if (loginviewmodel.TryConnect(this._connector, loginwindow.pwdbx.Password) == true) { this.Startup(loginwindow, loginviewmodel); } }
            };

            loginwindow.ShowDialog();
        }

        private void Startup(LoginWindow loginwindow, LoginViewModel loginviewmodel)
        {
            loginwindow.Close();
            this._site = loginviewmodel.Site;

            this._devicepane = new DevicePane(this._connector);
            this.devtab.DataContext = this._devicepane;

            this._userpane = new UserPane(this._connector);
            this.usertab.DataContext = this._userpane;

            this._apppane = new ApplicationPane(this._connector);
            this.apptab.DataContext = this._apppane;

            this._packagespane = new PackagePane(this._connector);
            this.packagetab.DataContext = this._packagespane;

            this._deploymentpane = new DeploymentsPane(this._connector);
            this.deploymenttab.DataContext = this._deploymentpane;
        }
    }
}
