using System.Collections.Generic;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Example tests from the assignment specification. Add your own tests as you work.
/// </summary>
public sealed class RecipeManagerTests
{
    // Construction and catalogue
    [Fact]
    public void Constructor_BuildsRecipeDictionary()
    {
        var manager = CreateManager();

        Assert.Equal(4, manager.RecipeCount);
        Assert.Equal("Recipe A", manager.FindRecipe(10)?.Title);
    }

    [Fact]
    public void Constructor_InvalidRecipesAreDiscarded()
    {
        // Duplicate IDs
        Assert.Throws<ArgumentException>(() =>
        {
            new RecipeManager([
                new Recipe
                {
                    Id = 10,
                    Title = "Recipe A",
                },
                new Recipe
                {
                    Id = 10,
                    Title = "Recipe B"
                }
            ]);
        });

        // Negative IDs
        Assert.Throws<ArgumentException>(() =>
        {
            new RecipeManager([
                new Recipe
                {
                    Id = -5,
                    Title = "Recipe C"
                }
            ]);
        });

        // Blank titles
        Assert.Throws<ArgumentException>(() =>
        {
            new RecipeManager([
                new Recipe
                {
                    Id = 20,
                    Title = ""
                }
            ]);
        });
    }

    [Fact]
    public void FindRecipe_FindsExistingRecipes()
    {
        var manager = CreateManager();

        Assert.Null(manager.FindRecipe(-5)); // Should not exist
        Assert.Equal("Recipe A", manager.FindRecipe(10)?.Title); // Should find
    }

    [Fact]
    public void RemoveRecipe_RemovesExistingRecipes()
    {
        var manager = CreateManager();

        Assert.False(manager.RemoveRecipe(-5)); // Should not exist
        Assert.True(manager.RemoveRecipe(10)); // Should exist
        Assert.Null(manager.FindRecipe(10)); // Should be removed
    }

    // Shopping list
    [Fact]
    public void AddIngredientsToShoppingList_AddsIngredientsToList()
    {
        var manager = CreateManager();
        manager.AddIngredientsToShoppingList(30);
        manager.AddIngredientsToShoppingList(40);

        Assert.Equal(
            ["1 potato", "2 bacon", "3 cheese", "1 tomato", "1 carrot"], 
            manager.GetShoppingList());
    }

    [Fact]
    public void AddIngredientsToShoppingList_ReturnsCorrectAmountAdded()
    {
        var manager = CreateManager();

        var recipeA = manager.FindRecipe(30)?.Ingredients.Count;
        var recipeB = manager.FindRecipe(40)?.Ingredients.Count;

        // Add recipeA test
        Assert.Equal(
            recipeA,
            manager.AddIngredientsToShoppingList(30)); // Returns amount added
        Assert.Equal(recipeA, manager.ShoppingItemCount); // Count increased

        // Adding recipeB test
        Assert.Equal(
            recipeB,
            manager.AddIngredientsToShoppingList(40)); // Returns amount added
        Assert.Equal(recipeA + recipeB, manager.ShoppingItemCount); // Count increased independently
    }

    [Fact]
    public void AddIngredientsToShoppingList_RejectsInvalidRecipes()
    {
        var manager = CreateManager();

        Assert.Equal(0, manager.AddIngredientsToShoppingList(-5)); // Should not exist
    }

    [Fact]
    public void GetShoppingList_AreInCorrectOrder()
    {
        var manager = CreateManager();

        // Add ingredients to list
        manager.AddIngredientsToShoppingList(30);

        var correct = new List<string> {"1 potato", "2 bacon", "3 cheese"};

        // Get shopping list
        var result = manager.GetShoppingList();
        Assert.True(result.SequenceEqual(correct)); // Copies in order
    }

    [Fact]
    public void ClearShoppingList_IsEmpty()
    {
        var manager = CreateManager();

        // Add recipes to shopping list
        manager.AddIngredientsToShoppingList(30);
        manager.AddIngredientsToShoppingList(40);

        // Get total ingredient count
        var totalIngredients =
            manager.FindRecipe(30)?.Ingredients.Count +
            manager.FindRecipe(40)?.Ingredients.Count;

        // Clear the shopping list
        manager.ClearShoppingList();

        Assert.NotEqual(totalIngredients, manager.ShoppingItemCount); // Should not match total ingredients
        Assert.Equal(0, manager.ShoppingItemCount); // Should clear to 0
        Assert.Equal([], manager.GetShoppingList()); // Should return an empty list
    }

    // Cooking Plan
    [Fact]
    public void AddRecipeToCookingPlan_AddsValidRecipes()
    {
        var manager = CreateManager();

        Assert.True(manager.AddRecipeToCookingPlan(10));
        Assert.True(manager.AddRecipeToCookingPlan(20));
        Assert.Equal(2, manager.CookingPlanCount);
        Assert.Equal(new[] { 10, 20 }, manager.GetCookingPlan());
    }

    [Fact]
    public void AddRecipeToCookingPlan_RejectsDuplicates()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);

        Assert.False(manager.AddRecipeToCookingPlan(10));
        Assert.Equal(1, manager.CookingPlanCount);
    }

    // Removed history
    [Fact]
    public void PeekLastRemovedRecipe_IsCorrect()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);
        manager.RemoveRecipeFromCookingPlan(10);

        Assert.Equal(10, manager.PeekLastRemovedRecipe());
    }

    [Fact]
    public void RemoveRecipeFromCookingPlan_RemovesExistingRecipes()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);

        Assert.True(manager.RemoveRecipeFromCookingPlan(10));
        Assert.Equal(1, manager.CookingPlanCount);
        Assert.Equal(new[] { 20 }, manager.GetCookingPlan());
    }

    [Fact]
    public void RestoreLastRemovedRecipe_RecipesAreRestoredLastInFirstOut()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);
        manager.RemoveRecipeFromCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(20);
        Assert.Equal(20, manager.PeekLastRemovedRecipe());
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 20 }, manager.GetCookingPlan());
    }

    // Cooking instructions
    [Fact]
    public void StartCooking_InstructionsAreCompletedInFileOrder()
    {
        var manager = CreateManager();

        Assert.True(manager.StartCooking(10));

        // First in
        Assert.Equal("First step", manager.PeekNextInstruction());

        // First out
        Assert.Equal("First step", manager.CompleteNextInstruction());

        // Continues correctly
        Assert.Equal("Second step", manager.PeekNextInstruction());
    }

    [Fact]
    public void PeekNextInstruction_EmptyInstructionsAreIgnored()
    {
        var manager = CreateManager();

        Assert.Null(manager.PeekNextInstruction());
    }

    [Fact]
    public void CompleteNextInstruction_EmptyInstructionsAreIgnored()
    {
        var manager = CreateManager();

        Assert.Null(manager.CompleteNextInstruction());
    }

    // Utility
    private static RecipeManager CreateManager()
    {
        return new RecipeManager(
        [
            new Recipe
            {
                Id = 10,
                Title = "Recipe A",
                Ingredients = ["1 apple"],
                Instructions = ["First step", "Second step"]
            },
            new Recipe
            {
                Id = 20,
                Title = "Recipe B"
            },
            new Recipe {
                Id = 30,
                Title = "Recipe C",
                Ingredients = ["1 potato", "2 bacon", "3 cheese"],
            },
            new Recipe {
                Id = 40,
                Title = "Recipe D",
                Ingredients = ["1 tomato", "1 carrot"],
            }
        ]);
    }
}
