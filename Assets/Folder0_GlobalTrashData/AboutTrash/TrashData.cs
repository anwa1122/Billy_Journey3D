using UnityEngine;

[CreateAssetMenu(fileName = "New Trash", menuName = "New Trash/Trash Data")]
public class TrashData : ScriptableObject
{
    [Header("Basic")]
    public string trashName;

    // 💡 ฟังก์ชันนี้จะทำงานอัตโนมัติใน Unity Editor
    private void OnValidate()
    {
        // ถ้าช่อง trashName ว่างอยู่ มันจะเอาชื่อไฟล์ Asset ด้านบนมาใส่ให้เองทันที!
        if (string.IsNullOrEmpty(trashName))
        {
            trashName = name;
        }
    }
    [TextArea]
    public string description;

    [Header("Type")]
    public TrashType trashType;

    [Header("Value")]
    public int score = 10;

    [Range(1, 100)]
    public int rarity = 50;

    [Header("Visual")]
    public Sprite sprite2D;
    public GameObject model3D;

    [Header("Spawn")]
    public bool canSpawn = true;
}