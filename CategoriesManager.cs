using BlendInteractive.ExtendedCategories.Attributes;
using BlendInteractive.ExtendedCategories.Interfaces;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BlendInteractive.ExtendedCategories
{
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    [InitializableModule]
    public class CategoriesManager : IInitializableModule
    {
        public static Dictionary<Type, List<string>> CategoryAssignmentProperties;

        public void Initialize(InitializationEngine context)
        {
            DataFactory.Instance.PublishedContent += OnContentPublished;
            DataFactory.Instance.DeletedContent += OnContentDeleted;
            PopulateCategoryAssignmentProperties();
        }

        public void OnContentPublished(object sender, ContentEventArgs e)
        {
            if (HasCategoryAssignments(e.Content))
            {
                IndexContentPage(e.Content);
            }
        }

        public static bool HasCategoryAssignments(IContent content)
        {
            return CategoryAssignmentProperties.ContainsKey(content.GetType().BaseType);
        }

        public static int IndexContentPage(IContent content)
        {
            // Does this type have any CategoryAssignment properties?
            if (!HasCategoryAssignments(content))
            {
                return 0;
            }

            var baseType = content.GetType().BaseType;


            var contentRef = content.ContentLink.ToReferenceWithoutVersion();

            var categoryRepo = ServiceLocator.Current.GetInstance<ICategoryRepository>();
            categoryRepo.DeleteAssignmentsByContent(contentRef);

            var assignmentsAdded = 0;
            foreach (var propertyName in CategoryAssignmentProperties[baseType])
            {
                var propertyData = content.Property[propertyName].Value;
                if(propertyData == null)
                {
                    continue;
                }
                foreach (var value in (IEnumerable<ContentReference>)propertyData)
                {
                    assignmentsAdded++;
                    categoryRepo.AddAssignment(contentRef, value, propertyName);
                }
            }

            return assignmentsAdded;
        }

        public void OnContentDeleted(object sender, ContentEventArgs e)
        {
            var contentRef = e.ContentLink.ToReferenceWithoutVersion();
            var categoryRepo = ServiceLocator.Current.GetInstance<ICategoryRepository>();

            // We'll delete in both directions, so this method is valid for both categories and assigned content
            categoryRepo.DeleteAssignmentsByCategory(contentRef);
            categoryRepo.DeleteAssignmentsByContent(contentRef);
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }

        private static void PopulateCategoryAssignmentProperties()
        {
            // This method populates a "map" of what types and properties contain category assignments.
            // This map is consulted to determine when we need to be concerned about indexing assignments.

            CategoryAssignmentProperties = new Dictionary<Type, List<string>>();

            // Iterate all types which have a backing class...
            var contentTypeRepo = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
            foreach (var contentType in contentTypeRepo.List().Where(t => t.ModelType != null))
            {
                // Iterate all properties...
                foreach (var property in contentType.ModelType.GetProperties())
                {
                    // Does this property have the CategoryAssignment attribute?
                    if (!property.GetCustomAttributes<IndexAsCategoriesAttribute>().Any())
                    {
                        continue;
                    }

                    // Is this property the correct type?
                    if(property.PropertyType != typeof(IEnumerable<ContentReference>))
                    {
                        continue;
                    }

                    // We're good, add it to the map...
                    if(!CategoryAssignmentProperties.ContainsKey(contentType.ModelType))
                    {
                        CategoryAssignmentProperties.Add(contentType.ModelType, new List<string>());
                    }
                    CategoryAssignmentProperties[contentType.ModelType].Add(property.Name);
                }
            }
        }

        public static IEnumerable<PageData> GetContent(ContentReference contentRef, string scope = null)
        {
            return ServiceLocator.Current.GetInstance<ICategoryRepository>().GetContent(contentRef, scope);
        }
        
        public static void DeleteAll()
        {
            ServiceLocator.Current.GetInstance<ICategoryRepository>().DeleteAll();
        }

    }
}