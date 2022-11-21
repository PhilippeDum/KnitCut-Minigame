using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "ScriptableObject/Cooking/Recipe")]
public class Recipe : ScriptableObject
{
    public string recipeName;
    public List<ConsumableRequired> consumablesRequired;
    public GameObject finalProduct;
    public bool canBeCooked;
}