﻿using System;
using System.Windows;
using System.Xml.Linq;
using CollectionViewer;
using NUnit.Framework;
using Microsoft.ConfigurationManagement.ManagementProvider;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using model;

namespace CollectionViewerTests
{
    [TestFixture]
    public class CollectionQueryTests
    {
        [Test]
        [TestCase("SMS00001", ExpectedResult = null)]
        [TestCase("00100014", ExpectedResult = "SMS00001")]
        public string GetCollectionLimitingID(string collectionid)
        {
            string configfile = AppDomain.CurrentDomain.BaseDirectory + @"testauth.xml";

            XElement x;

            x = XmlHandler.Read(configfile);
            string authpw = XmlHandler.GetStringFromXElement(x, "Password", null);
            string authuser = XmlHandler.GetStringFromXElement(x, "User", null);
            string authdomain = XmlHandler.GetStringFromXElement(x, "Domain", null);

            WqlConnectionManager connection = new WqlConnectionManager();
            connection.Connect("syscenter03.home.local", authdomain + "\\" + authuser, authpw);
            SccmConnector connector = new SccmConnector();
            CollectionLibrary library = connector.GetDeviceCollectionLibrary(connection,"001");
            return library?.GetCollection(collectionid)?.LimitingCollectionID;
        }

        private void ReadConfigFile()
        {
            
        }

        [Test]
        [TestCase(ExpectedResult = true)]
        public bool BuildTest()
        {
            CollectionLibrary library = new CollectionLibrary();
            SccmCollection allsys = new SccmCollection();
            allsys.ID = "SMS00001";
            allsys.Name = "All Systems";
            library.AddCollection(allsys);
            SccmCollection laballsys = new SccmCollection("00100014", "LAB All Systems", "SMS00001");
            library.AddCollection(laballsys);
            MessageBoxResult result = MessageBox.Show("Does this look right?", "Eval", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) { return true; }
            else { return false; }
        }
    }
}