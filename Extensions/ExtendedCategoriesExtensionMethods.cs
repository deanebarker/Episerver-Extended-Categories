﻿using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlendInteractive.ExtendedCategories.Extensions
{
    public static class ExtendedCategoriesExtensionMethods
    {
        public static IEnumerable<PageData> GetAssignedContent(this PageData page)
        {
            return CategoriesManager.GetContent(page.ContentLink);
        }
    }
}
