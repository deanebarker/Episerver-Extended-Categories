# Extended Categories

This is an extenstion to the Episerver CMS that allows the use of managed pages as categories.

## To Use

1. Create a page type to represent a category (called "CategoryPage" in the examples below).

2. Add the following property to _the page types you want to categorize_.

```C#
[AllowedTypes(new Type[] { typeof(CategoryPage) })]
[IndexAsCategories]
public virtual IEnumerable<ContentReference> Categories { get; set; }

3. In the view for CategoryPage, iterate the assigned content like so:

```C#
foreach(var page in CategoryManager.GetContent(thisCategoryPage.ContentLink))
{
	@Html.PageLink(page)
}
```