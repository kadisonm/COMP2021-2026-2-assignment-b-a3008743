# Design
## 1. Saved recipe collection
The `savedRecipes` property is a Dictionary of recipeID and recipe objects (see definition below). I selected a Dictionary since the required methods (add, remove, check, get) had similar requirements to the already existing (add, remove, find, get) methods for the normal recipe dictionary already made.

`Dictionary<int, Recipe>`

For example, `FindRecipe` and `IsRecipeSaved` have similar requirements despite the latter returning a boolean value. Since I used the same data structure (Dictionary), implementation of this method was quick as it only required changing the returns types to true/false and checking the `savedRecipes` property instead.

Dictionaries are also very useful for ensuring duplicate keys are not added or doing membership checks through the `.ContainsKey` method. It conveniently has methods that suit the add and remove requirements which are named appropriately (`.Add(key, value), .Remove(key)`), making Dictionaries the logical choice.

## 2. Integration
The new Part B LINQ features use the `_recipes.Values` property to select from Part A's recipe catalogue without needing to write a lambda select function that retrieves the object from the dictionary's key-value pair. Part B's `AddSavedRecipe()` method also uses Part A's `FindRecipe` method to ensure that only existing recipes inside the `RecipeManager` can actually be saved.

## 3. Basic complexity
| Operation | Structure | Expected complexity | Reason |
| --- | --- | --- | --- |
| Lookup recipe by ID | Dictionary | Average O(1) | Hash-based key lookup. |
| Traverse cooking plan | LinkedList | O(n) | Each planned recipe may need to be visited. |
| Complete next instruction | Queue | O(1) | The item at the front is removed. |
| LINQ title/ingredient search | Recipe collection | O(n) | Each recipe may need to be inspected. |
| Check whether a recipe is saved | Dictionary | Average O(1) | Hash-based key lookup.<br><br> The time is constant as the lookup complexity does not change based on the length of the dictionary. It accesses the value directly in memory by the key's hash. |
