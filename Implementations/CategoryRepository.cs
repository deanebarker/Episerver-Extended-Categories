using BlendInteractive.ExtendedCategories.Interfaces;
using BlendInteractive.ExtendedCategories.Models;
using EPiServer;
using EPiServer.Core;
using EPiServer.Data.Dynamic;
using EPiServer.ServiceLocation;
using System.Collections.Generic;
using System.Linq;

namespace BlendInteractive.ExtendedCategories.Implementations
{
    [ServiceConfiguration(typeof(ICategoryRepository))]
    public class CategoryRepository : ICategoryRepository
    {
        public static DynamicDataStore Store
        {
            get
            {
                return DynamicDataStoreFactory.Instance.GetStore(typeof(CategoryAssignment));
            }
        }

        public IEnumerable<PageData> GetContent(ContentReference contentRef, string scope = null)
        {
            var refs = scope == null ?
                Store.Items<CategoryAssignment>().Where(ca => ca.CategoryPage == contentRef.ToReferenceWithoutVersion()).ToList() :
                Store.Items<CategoryAssignment>().Where(ca => ca.CategoryPage == contentRef.ToReferenceWithoutVersion() && ca.Scope == scope).ToList();

            var repo = ServiceLocator.Current.GetInstance<IContentRepository>();
            
            return refs.Select(ca => repo.Get<PageData>(ca.ContentPage));
        }

        public void DeleteAssignmentsByContent(ContentReference contentRef, string scope = null)
        {
            var assignments = Store.Items<CategoryAssignment>().Where(ca => ca.ContentPage == contentRef && ca.Scope == scope);

            if (assignments.Count() > 0)
            {
                foreach (var assignment in assignments)
                {
                    Store.Delete(assignment);
                }
            }
        }

        public void DeleteAssignmentsByCategory(ContentReference categoryRef)
        {
            if (Store.Items<CategoryAssignment>().Where(ca => ca.ContentPage == categoryRef.ToReferenceWithoutVersion()).Count() > 0)
            {
                foreach (var item in Store.Items<CategoryAssignment>().Where(ca => ca.CategoryPage == categoryRef.ToReferenceWithoutVersion()))
                {
                    Store.Delete(item);
                }
            }
        }

        public void AddAssignment(ContentReference contentRef, ContentReference categoryRef, string scope)
        {
            contentRef = contentRef.ToReferenceWithoutVersion();
            categoryRef = categoryRef.ToReferenceWithoutVersion();

            var matchingAssignments = Store.Items<CategoryAssignment>().Where(ca => ca.ContentPage == contentRef && ca.CategoryPage == categoryRef && ca.Scope == scope);
            if(matchingAssignments.Count() > 0)
            {
                return;
            }
            
            var categoryAssignment = new CategoryAssignment()
            {
                ContentPage = contentRef,
                CategoryPage = categoryRef,
                Scope = scope
            };
            Store.Save(categoryAssignment);
        }

        public void DeleteAll()
        {
            Store.DeleteAll();
        }
    }
}