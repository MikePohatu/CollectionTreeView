﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace viewmodel
{
    public abstract class SccmResource
    {
        public string ID { get; set; }
        public string Name { get; set; }

        protected List<string> _collectionids = new List<string>();
        public List<string> CollectionIDs
        {
            get { return this._collectionids; }
            set { this._collectionids = value; }
        }
    }
}