using EPiServer.Core;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlendInteractive.ExtendedCategories.Models
{
    [EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true, StoreName = "CategoryAssignment")]
    public class CategoryAssignment : IDynamicData
    {
        public string Scope { get; set; }

        public ContentReference ContentPage { get; set; }

        public ContentReference CategoryPage { get; set; }

        public Identity Id { get; set; }
    }
}