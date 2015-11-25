using EPiServer.BaseLibrary.Scheduling;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using System;
using EPiServer;
using EPiServer.Filters;
using EPiServer.PlugIn;

namespace BlendInteractive.ExtendedCategories.Jobs
{
    [ScheduledPlugIn(DisplayName = "Rebuild Category Assignments")]
    public class RebuildCategoryAssignmentIndex : JobBase
    {
        private int contentReviewed;
        private int assignmentsIndexed;

        public override string Execute()
        {
            var searchService = ServiceLocator.Current.GetInstance<IPageCriteriaQueryService>();

            foreach (var type in CategoriesManager.CategoryAssignmentProperties)
            {
                var typeName = type.Key.Name;
                var criteria = new PropertyCriteriaCollection
                    {
                        new PropertyCriteria
                        {
                            Name = "PageTypeName",
                            Type = PropertyDataType.PageType,
                            Condition = CompareCondition.Equal,
                            Value = typeName
                        }
                    };

                var pages = searchService.FindPagesWithCriteria(ContentReference.StartPage, criteria);

                foreach (var page in pages)
                {
                    contentReviewed++;
                    assignmentsIndexed = assignmentsIndexed + CategoriesManager.IndexContentPage(page);
                }
            }


            return String.Format("Content Reviewed: {0}, Assignments Indexed: {1}", contentReviewed, assignmentsIndexed);
        }
    }
}