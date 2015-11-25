using EPiServer;
using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlendInteractive.ExtendedCategories.Interfaces
{
    public interface ICategoryRepository
    {
        IEnumerable<PageData> GetContent(ContentReference contentRef, string scope = null);
        void AddAssignment(ContentReference contentRef, ContentReference categoryRef, string scope);
        void DeleteAssignmentsByContent(ContentReference contentRef, string scope = null);
        void DeleteAssignmentsByCategory(ContentReference contentRef);
        void DeleteAll();
    }
}