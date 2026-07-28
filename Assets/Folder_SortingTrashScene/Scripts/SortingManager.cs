using UnityEngine;
using System.Collections.Generic;
public class SortingManager : MonoBehaviour
{
    public GameObject trashUiPrefab;
    public Transform trashParent;

    public List<TrashData> trashDataBaseDummy;
    void Start()
    {
        RectTransform area = trashParent.GetComponent<RectTransform>();
        if (InventoryManager.Instance != null && InventoryManager.Instance.items.Count > 0)
        {
            //Debug.Log("Found it");

            foreach (TrashData data in InventoryManager.Instance.items)
            {
                float randomX = Random.Range(-area.rect.width / 2f, area.rect.width / 2f);
                float randomY = Random.Range(-area.rect.height / 2f, area.rect.height / 2f);

                GameObject obj = Instantiate(trashUiPrefab, trashParent);
                obj.GetComponent<TrashUi>().Setup(data);
                obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(randomX, randomY);
            }
        }
        else
        {
            //Debug.Log("Where is it");

            foreach (TrashData data in trashDataBaseDummy)
            {
                float randomX = Random.Range(-area.rect.width / 2f, area.rect.width / 2f);
                float randomY = Random.Range(-area.rect.height / 2f, area.rect.height / 2f);

                GameObject obj = Instantiate(trashUiPrefab, trashParent);
                obj.GetComponent<TrashUi>().Setup(data);
                obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(randomX, randomY);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
