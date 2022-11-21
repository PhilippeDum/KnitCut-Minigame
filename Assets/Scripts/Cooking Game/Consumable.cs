using UnityEngine;

[CreateAssetMenu(fileName = "Consumable", menuName = "ScriptableObject/Cooking/Consumable")]
public class Consumable : ScriptableObject
{
    public string consumableName;
    public int quantity;
    public GameObject consumableObject;
}