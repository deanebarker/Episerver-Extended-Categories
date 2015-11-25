# Extended Categories

This is an extenstion to the Episerver CMS that allows the use of managed pages as categories. This is intended to replace/deprecate the existing Episerver category system.

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

Note that this is content page-centric. So, when viewing a content page (an `ArticlePage` to continue our example), you link it to categories (`CategoryPage` objects) by dragging them in from the tree.  It does not work the other way around -- you do not edit a category and assign content to it.

### #3. Display Assigned Content for Categories

```C#
<ul>
foreach(var page in CategoryManager.GetContent(Model.CurrentPage.ContentLink))
{
	<li>@Html.PageLink(page)</li>
}
</ul>
```

Alternately, there's an extension method on `PageData`:

```C#
<ul>
foreach(var page in Model.CurrentPage.GetAssignedContent())
{
	<li>@Html.PageLink(page)</li>
}
</ul>
```

`GetContent` returns an `IEnumerable<PageData>` of all the pages assigned to the category referenced by the passed-in `ContentReference`.

## Technical Details

The only necessary modification is to index the reverse relationship between content and categories. Remember, the content page has an `IEnumerable<ContentReference>` representing the categories, so it already knows about the relationship. We only need to find a way to discover the reverse, from the category side.  A category needs to find all the pages _on which it has been assigned_.

An OnContentPublished event indexes this relationship. This index is then queryed by the `GetContent` method to produce the assigned pages.

`CategoryRepostory` is injected for `ICategoryRepository` by default. `CategoryRepository` uses the built-inDynamic Data Store (DDS) to store the reverse relationship.  (Episever Find users will likely want to re-implement to use Find as the index.)

In the event changes require a re-index, a scheduled job called "Reindex Category Assignments" will delete all assignments and re-index the entire repository. Execution time obviously depends on the volume of content and number of assignments.