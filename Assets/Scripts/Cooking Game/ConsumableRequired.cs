using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableRequired", menuName = "ScriptableObject/Cooking/ConsumableRequired")]
public class ConsumableRequired : ScriptableObject
{
    public Consumable consumable;
    public int quantity;
}