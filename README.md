# Extended Categories

This is an extenstion to the Episerver CMS that allows the use of managed pages as categories.

## To Use

### #1. Create the Category Page
Create a page type to represent a category (called `CategoryPage` in the examples below). This is no different than any other Episerver page. Model and template it however you like.

Once the type is created, create a structure of categories in the page tree.

### #2. Add the Category Assignment Property
Add the following property to _the page types you want to categorize_ -- so, to `ArticlePage` in the default Alloy install, for example.

```C#
[AllowedTypes(new Type[] { typeof(CategoryPage) })]
[IndexAsCategories]
public virtual IEnumerable<ContentReference> Categories { get; set; }
```

By default, this will render as a content area-style draggable interface.  You can browse for `CategoryPage` content to add, or just drag them in from the tree.

### #3. Display Assigned Content for Categories

```C#
<ul>
foreach(var page in CategoryManager.GetContent(thisCategoryPage.ContentLink))
{
	<li>@Html.PageLink(page)</li>
}
</ul>
```

`GetContent` returns an `IEnumerable<PageData>` of all the pages assigned to the category references by the passed-in `ContentReference`.