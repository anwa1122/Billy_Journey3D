using UnityEngine;

public class TrashObject : MonoBehaviour
{
    public TrashData data;

    public void Setup(TrashData newData)
    {
        data = newData;

        if (data.model3D != null)
        {
            GameObject visual = Instantiate(data.model3D, transform.position, transform.rotation);
            visual.transform.SetParent(this.transform);

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