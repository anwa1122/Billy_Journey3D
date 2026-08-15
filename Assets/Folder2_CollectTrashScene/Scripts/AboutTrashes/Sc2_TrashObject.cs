using System.Collections.Generic;
using UnityEngine;

public class Sc2_TrashObject : MonoBehaviour
{
    public TrashData data;

    public void Collect()
    {
        if (data != null)
        {
            InventoryManager.Instance.AddItem(data);
        }
        Destroy(gameObject);
    }

    public void Setup(TrashData newData)
    {
        data = newData;

        if (data.model3D != null)
        {
            GameObject visual = Instantiate(data.model3D, transform.position, transform.rotation);
            visual.transform.SetParent(this.transform);
            Vector3 euler = visual.transform.eulerAngles;

            // ตัวอย่าง: สุ่มเฉพาะแกน X (แกนอื่นคงเดิม)
            euler.y = Random.Range(0f, 360f);
            // euler.y = Random.Range(0f, 360f); // ถ้าอยากสุ่ม Y ด้วย
            // euler.z คงเดิมตาม Prefab

            visual.transform.rotation = Quaternion.Euler(euler);

            if (GetComponent<MeshRenderer>() != null)
            {
                GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                Debug.Log("No MeshRenderer of " + data.trashName);
            }
        }
        else
        {
            Debug.Log("No data 3Dmodel of " + data.trashName);
        }

        gameObject.name = "Trash_" + data.trashName;
    }
}