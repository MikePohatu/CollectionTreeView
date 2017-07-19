﻿using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using Microsoft.Msagl.Drawing;
using viewmodel;
using System.Threading;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace Visualizer.Panes
{
    public abstract class BaseCollectionPane: BasePane
    {
        protected CollectionLibrary _library;
        protected List<SccmCollection> _highlightedcollections = new List<SccmCollection>();
        protected bool _filteredview = true;

        protected ResourceTabControl _pane;
        public ResourceTabControl Pane { get { return this._pane; } }

        protected new List<SccmCollection> _searchresults;
        public new List<SccmCollection> SearchResults
        {
            get { return this._searchresults; }
            set
            {
                this._searchresults = value;
                this.OnPropertyChanged(this, "SearchResults");
            }
        }

        protected new SccmCollection _selectedresult;
        public new SccmCollection SelectedResult
        {
            get { return this._selectedresult; }
            set
            {
                this._selectedresult = value;
                this.OnPropertyChanged(this, "SelectedResult");
            }
        }

        protected CollectionType CollectionsType { get; set; }

        public BaseCollectionPane(SccmConnector connector):base(connector)
        {
            this._pane = new ResourceTabControl();
            MsaglHelpers.ConfigureCollectionsGViewer(this._pane.gviewer);
            this._pane.DataContext = this;
            this._pane.buildbtn.Click += this.OnBuildButtonPressed;
            this._pane.gviewer.AsyncLayoutProgress += this.OnProgressUpdate;
            //this._pane.abortbtn.Click += OnAbortButtonClick;
            this._pane.findbtn.Click += this.OnFindButtonPressed;
            this._pane.findresourcetb.KeyUp += this.OnFindKeyUp;
            this._pane.searchbtn.Click += this.OnSearchButtonPressed;
            this._pane.searchtb.KeyUp += this.OnSearchKeyUp;
        }

        public Graph BuildGraphTree(string rootcollectionid, string mode)
        {
            Graph graph = null;
            if (string.IsNullOrWhiteSpace(rootcollectionid) == false)
            {
                SccmCollection col = this._library.GetCollection(rootcollectionid);
                if (col != null)
                {
                    if (mode == "Context")
                    {
                        if (this._filteredview == true) { graph = TreeBuilder.BuildCollectionTreeAllCollections(this._library); }
                        else { graph = this._graph; }
                        this._filteredview = false;
                        this._highlightedcollections = col.HighlightCollectionPathList();
                    }
                    else if (mode == "Mesh")
                    {
                        this._filteredview = true;
                        graph = TreeBuilder.BuildCollectionTreeMeshMode(this._connector, this._library, rootcollectionid);
                    }
                    else if (mode == "Limiting")
                    {
                        this._filteredview = true;
                        graph = TreeBuilder.BuildCollectionTreeLimitingMode(this._library, rootcollectionid);
                    }
                    col.IsHighlighted = true;
                }
            }
            else
            {
                this._filteredview = false;
                graph = TreeBuilder.BuildCollectionTreeAllCollections(this._library);
            }
            return graph;
        }

        protected void ClearHighlightedCollections()
        {
            foreach (SccmCollection col in this._highlightedcollections)
            {
                col.IsHighlighted = false;
            }
            this._highlightedcollections.Clear();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        protected override async void BuildGraph()
        {
            this.ControlsEnabled = false;
            this._processing = true;
            this.ClearHighlightedCollections();
            string mode = this._pane.modecombo.Text;
            Task.Run(() => this.NotifyProgress("Building"));

            string collectionid = this._selectedresult?.ID;

            await Task.Run(() => this._graph = this.BuildGraphTree(collectionid, mode));
            await Task.Run(() => this.UpdatePaneToTabControl());
            this._processing = false;
            this.ControlsEnabled = true;
        }

        protected void OnSearchButtonPressed(object sender, RoutedEventArgs e) { this.UpdateSearchResults(); }
        protected void OnSearchKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.UpdateSearchResults();
            }
        }

        protected void OnFindButtonPressed(object sender, RoutedEventArgs e) { this.Find(); }
        protected abstract void Find();
        protected void OnFindKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Find();
            }
        }

        protected void UpdatePaneToTabControl()
        {
            this._pane.gviewer.Graph = this._graph;
        }

        protected void Redraw()
        {
            this._pane.gviewer.Invalidate();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        private async void UpdateSearchResults()
        {
            this.ControlsEnabled = false;
            this._processing = true;

            Task.Run(() => this.NotifyProgress("Searching"));
            await Task.Run(() => this.SearchResults = this._connector.GetCollectionsFromSearch(this._searchtext,this.CollectionsType));

            this._processing = false;
            this.ControlsEnabled = true;
        }
    }
}