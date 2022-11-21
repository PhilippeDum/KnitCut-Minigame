using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cooking : MonoBehaviour
{
    [SerializeField] private List<Recipe> recipes = new List<Recipe>();
    [SerializeField] private List<Consumable> consumablesPossessed;
    [SerializeField] private GameObject recipeUI;
    [SerializeField] private GameObject consumableUI;
    [SerializeField] private Transform contentRecipes;
    [SerializeField] private Transform contentConsumables;

    private void Update()
    {
        ShowRecipesPossessed();
        ShowConsumablesPossessed();

        CheckIfRecipesCanBeCooked();
    }

    private void ShowRecipesPossessed()
    {
        for (int i = 0; i < recipes.Count; i++)
        {
            if (!UIContainsObject(contentRecipes, recipes[i].name))
            {
                GameObject recipe = Instantiate(recipeUI, contentRecipes);
                recipe.name = recipes[i].name;
                recipe.transform.GetChild(0).GetComponent<Text>().text = recipes[i].name;
            }
        }
    }

    private void ShowConsumablesPossessed()
    {
        for (int i = 0; i < consumablesPossessed.Count; i++)
        {
            if (!UIContainsObject(contentConsumables, consumablesPossessed[i].name))
            {
                GameObject consumable = Instantiate(consumableUI, contentConsumables);
                consumable.name = consumablesPossessed[i].name;
                consumable.transform.GetChild(0).GetComponent<Text>().text = consumablesPossessed[i].name;
                consumable.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = consumablesPossessed[i].quantity.ToString();
            }
        }
    }

    private bool UIContainsObject(Transform content, string consumableName)
    {
        bool contains = false;

        for (int i = 0; i < content.childCount; i++)
        {
            if (content.GetChild(i).name.Equals(consumableName))
            {
                contains = true;
            }
        }

        return contains;
    }

    private void CheckIfRecipesCanBeCooked()
    {
        // For each recipe
        foreach (Recipe recipe in recipes)
        {
            bool recipeCanBeCooked = true;

            // For each consumable required for the recipe
            for (int i = 0; i < recipe.consumablesRequired.Count; i++)
            {
                ConsumableRequired consumableRequiredOfRecipe = recipe.consumablesRequired[i];
                // If player do not possessed the required consumable, can not cook recipe
                if (!consumablesPossessed.Contains(consumableRequiredOfRecipe.consumable))
                {
                    recipeCanBeCooked = false;
                }

                if (consumablesPossessed.Contains(consumableRequiredOfRecipe.consumable) && consumablesPossessed[i].quantity < consumableRequiredOfRecipe.quantity)
                {
                    recipeCanBeCooked = false;
                }
            }

            recipe.canBeCooked = recipeCanBeCooked;
        }
    }
}