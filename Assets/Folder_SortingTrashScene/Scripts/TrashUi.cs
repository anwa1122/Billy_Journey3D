using UnityEngine;
using UnityEngine.UI;


public class TrashUi : MonoBehaviour
{
    public TrashData data;
    public Image icon;

    public void Setup(TrashData newData)
    {
        data = newData;

        if (data.sprite2D != null)
        {
            icon.sprite = data.sprite2D;
        }
        else
        {
            Debug.Log("No data 2dSprte of " + data.trashName);
        }
        gameObject.name = "Trash_" + data.trashName;
    }
}
