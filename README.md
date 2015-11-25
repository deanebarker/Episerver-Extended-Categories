# Extended Categories

This is an extenstion to the Episerver CMS that allows the use of managed pages as categories.

## To Use

### Create the Category Page
Create a page type to represent a category (called `CategoryPage` in the examples below). This is no different than any other Episerver page. Model and template it however you like.

Once the type is created, create a structure of categories in the page tree.

### Add the Category Assignment Property
Add the following property to _the page types you want to categorize_ -- so, to `ArticlePage` in the default Alloy install, for example.

```C#
[AllowedTypes(new Type[] { typeof(CategoryPage) })]
[IndexAsCategories]
public virtual IEnumerable<ContentReference> Categories { get; set; }
```

By default, this will render as a content area-style draggable interface.  You can browse for `CategoryPage` content to add, or just drag them in from the tree.

### Display Assigned Content for Categories

3. In the view for `CategoryPage`, iterate the assigned content like so:

```C#
foreach(var page in CategoryManager.GetContent(thisCategoryPage.ContentLink))
{
	@Html.PageLink(page)
}
```

`GetContent` returns an `IEnumerable<PageData>` of all the pages assigned to the category references by the passed-in `ContentReference`.