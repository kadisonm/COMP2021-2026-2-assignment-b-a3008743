using System;
using System.Collections;
using System.Collections.Generic;

namespace RecipeManagement.Core;

/// <summary>
/// Implement this class using the five Part A collections as private fields:
/// Dictionary&lt;int, Recipe&gt;, List&lt;string&gt;, LinkedList&lt;int&gt;,
/// Stack&lt;int&gt; and Queue&lt;string&gt;.
/// </summary>
public sealed class RecipeManager : IRecipeManager
{
    private readonly Dictionary<int, Recipe> _recipes = [];
    private readonly List<string> _shoppingList = [];
    private readonly LinkedList<int> _cookingPlan = [];
    private readonly Stack<int> _removedRecipe = [];
    private readonly Queue<string> _instructionQueue = [];

    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        if (recipes == null)
        {
            throw new ArgumentNullException();
        }

        // Reject duplicate recipe IDs and handle missing recipe IDs safely
        foreach (Recipe recipe in recipes)
        {
            if (_recipes.ContainsKey(recipe.Id) ||
                recipe.Title == string.Empty ||
                recipe.Id < 0)
            {
                throw new ArgumentException();
            }

            // Add valid recipe to the dictionary
            _recipes[recipe.Id] = recipe;
        }
    }

    public int RecipeCount => _recipes.Count;
    public int ShoppingItemCount => _shoppingList.Count;
    public int CookingPlanCount => _cookingPlan.Count;
    public int PendingInstructionCount => _instructionQueue.Count;
    public int RemovedRecipeCount => _removedRecipe.Count;

    public bool AddRecipe(Recipe recipe)
    {
        // Do not accept null recipes
        if (recipe == null)
        {
            throw new ArgumentNullException();
        }

        // Return false is a recipe already exists, has a blank title or negative Id
        if (_recipes.ContainsKey(recipe.Id) ||
            recipe.Title == string.Empty ||
            recipe.Id < 0)
        {
            return false;
        }

        // Add the recipe and return true
        _recipes[recipe.Id] = recipe;

        return true;
    }

    public Recipe? FindRecipe(int recipeId)
    {
        // If no key is found return null
        if (!_recipes.ContainsKey(recipeId))
        {
            return null;
        }

        return _recipes[recipeId];
    }

    public bool RemoveRecipe(int recipeId)
    {
        // TODO: Return false when the recipe is already in a cooking plan
        if (FindRecipe(recipeId) == null)
        {
            return false;
        }

        _recipes.Remove(recipeId);

        return true;
    }

    public int AddIngredientsToShoppingList(int recipeId)
    {
        var recipe = FindRecipe(recipeId);

        if (recipe == null)
        {
            return 0;
        }

        var added = 0;

        foreach (var ingredient in recipe.Ingredients)
        {
            _shoppingList.Add(ingredient);
            added++;
        }

        return added;
    }

    public IReadOnlyList<string> GetShoppingList()
    {
        List<string> shoppingList = [];

        foreach (var Item in _shoppingList)
        {
            shoppingList.Add(Item);
        }

        return shoppingList.AsReadOnly();
    }

    public void ClearShoppingList()
    {
        _shoppingList.Clear();
    }

    public bool AddRecipeToCookingPlan(int recipeId)
    {
        // Ensure duplicate or non existent recipes cannot be added.
        if (FindRecipe(recipeId) == null ||
            _cookingPlan.Find(recipeId) != null)
        {
            return false;
        }

        _cookingPlan.AddLast(recipeId);

        return true;
    }

    public bool RemoveRecipeFromCookingPlan(int recipeId)
    {
        // Remove the recipe from the cooking plan
        if (!_cookingPlan.Remove(recipeId))
        {
            return false;
        }

        // Add the removed recipe to the history
        _removedRecipe.Push(recipeId);

        return true;
    }

    public bool RestoreLastRemovedRecipe()
    {
        // False if there are no removed recipes
        if (_removedRecipe.Count == 0)
        {
            return false;
        }

        var recipeID = _removedRecipe.Pop();

        // False if the recipe still exists and is already planned
        if (_cookingPlan.Find(recipeID) != null)
        {
            return false;
        }

        // Restore it
        _cookingPlan.AddLast(recipeID);

        return true;
    }

    public int? PeekLastRemovedRecipe()
    {
        // Return null if there is no last removed recipe
        if (!_removedRecipe.TryPeek(out int lastRemoved))
        {
            return null;
        }

        return lastRemoved;
    }

    public IReadOnlyList<int> GetCookingPlan()
    {
        List<int> cookingPlan = [];

        var result = _cookingPlan.First;

        while (result != null)
        {
            cookingPlan.Add(result.Value);
            result = result.Next;
        }

        return cookingPlan.AsReadOnly();
    }

    public bool StartCooking(int recipeId)
    {
        var recipe = FindRecipe(recipeId);

        if (recipe == null)
        {
            return false;
        }

        if (recipe.Instructions.Count == 0)
        {
            return false;
        }

        _instructionQueue.Clear();

        foreach (var instruction in recipe.Instructions)
        {
            _instructionQueue.Enqueue(instruction);
        }

        return true;
    }

    public string? PeekNextInstruction()
    {
        if (_instructionQueue.Count == 0)
        {
            return null;
        }

        return _instructionQueue.Peek();
    }

    public string? CompleteNextInstruction()
    {
        if (_instructionQueue.Count == 0)
        {
            return null;
        }

        return _instructionQueue.Dequeue();
    }

    public IReadOnlyList<Recipe> SearchByTitle(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByTitle.");

    public IReadOnlyList<Recipe> SearchByIngredient(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByIngredient.");

    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count) =>
        throw new NotImplementedException("Part B: implement GetHighestProteinRecipes.");

    public bool AddSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement AddSavedRecipe.");

    public bool RemoveSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement RemoveSavedRecipe.");

    public bool IsRecipeSaved(int recipeId) =>
        throw new NotImplementedException("Part B: implement IsRecipeSaved.");

    public IReadOnlyList<int> GetSavedRecipes() =>
        throw new NotImplementedException("Part B: implement GetSavedRecipes.");
}
