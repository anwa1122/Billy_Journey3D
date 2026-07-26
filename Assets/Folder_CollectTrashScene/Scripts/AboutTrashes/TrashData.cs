using UnityEngine;

[CreateAssetMenu(fileName = "New Trash", menuName = "New Trash/Trash Data")]
public class TrashData : ScriptableObject
{
    [Header("Basic")]
    public string trashName;
    [TextArea]
    public string description;

    [Header("Type")]
    public TrashType trashType;

    [Header("Value")]
    public int price = 10;

    [Range(1, 100)]
    public int rarity = 50;
    public TrashRarity rarityType;

    [Header("Visual")]
    public Sprite sprite2D;
    public GameObject model3D;

    [Header("Spawn")]
    public bool canSpawn = true;
}