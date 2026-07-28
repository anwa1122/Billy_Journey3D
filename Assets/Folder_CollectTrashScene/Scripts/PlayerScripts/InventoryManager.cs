using UnityEngine;
using System.Collections.Generic;
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<TrashData> items = new List<TrashData>();


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(TrashData data)
    {
        items.Add(data);
    }
}
