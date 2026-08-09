using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Sc3_TrashDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    RectTransform rect;
    CanvasGroup canvasGroup;

    // เพิ่ม: reference ไปยัง Float Animation (อาจไม่มีก็ได้ ถ้า Prefab ไหนไม่ต้องการลอย)
    Sc3_TrashFloatAnimation floatAnim;

    public RectTransform spawnArea;
    public RectTransform onDragParent;


    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        floatAnim = GetComponent<Sc3_TrashFloatAnimation>(); // เพิ่ม

        // ดึงพื้นที่เกิดจาก Manager
        spawnArea = Sc3_SortingManager.Instance.spawnArea;
        onDragParent = Sc3_SortingManager.Instance.onDragParent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetAsLastSibling();

        canvasGroup.alpha = 0.5f;
        canvasGroup.blocksRaycasts = false;

        // เพิ่ม: ผู้เล่นเริ่มควบคุมตำแหน่งเอง ต้องหยุด Animation ทันที
        // และปลดล็อกไว้ก่อน เผื่อว่าก่อนหน้านี้เคยถูกล็อกถาวรตอนอยู่ใน Slot มาแล้ว
        // (ถ้าไม่ปลดล็อก พอดึงออกมาแล้วปล่อยใหม่ StartFloat() จะไม่ทำงาน)
        if (floatAnim != null)
        {
            floatAnim.UnlockFloat();
            floatAnim.StopFloat();
        }
    }


    public void OnDrag(PointerEventData eventData)
    {
        rect.position = eventData.position;
        gameObject.transform.SetParent(onDragParent);
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        //Debug.Log("Drop : " + eventData.position);
        //Debug.Log("Spawn Area : " + spawnArea.rect);

        bool landedOnSlot = CheckSlot();

        if (landedOnSlot)
        {
            //Debug.Log("Easy");

            // เจอ Slot -> หยุด Animation ถาวร
            if (floatAnim != null) floatAnim.LockFloat();
        }
        else
        {
            // ถ้าวางอยู่นอกกรอบ SpawnArea จริง ๆ ค่อยเด้งกลับไปชิดขอบ
            // ถ้าวางอยู่ในกรอบอยู่แล้ว ให้คงตำแหน่งที่ผู้เล่นปล่อยไว้ (ไม่ต้องขยับ)
            if (!IsInsideSpawnArea(eventData.position))
            {
                rect.position = GetClosestEdge(eventData.position);
            }

            if (floatAnim != null) floatAnim.StartFloat();
        }
    }

    bool IsInsideSpawnArea(Vector3 screenPosition)
    {
        Vector3[] corners = new Vector3[4];
        spawnArea.GetWorldCorners(corners);

        float left = corners[0].x;
        float right = corners[2].x;
        float bottom = corners[0].y;
        float top = corners[1].y;

        return screenPosition.x >= left && screenPosition.x <= right
            && screenPosition.y >= bottom && screenPosition.y <= top;
    }


    bool CheckSlot()
    {
        List<RaycastResult> items = SearchItemsWithRayCast();

        foreach (RaycastResult result in items)
        {
            if (result.gameObject.CompareTag("Slot"))
            {
                gameObject.transform.SetParent(result.gameObject.transform);
                return true;
            }
        }

        gameObject.transform.SetParent(spawnArea);
        return false;
    }

    List<RaycastResult> SearchItemsWithRayCast()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results;
    }


    Vector3 GetClosestEdge(Vector3 dropPosition)
    {
        Vector3[] corners = new Vector3[4];
        spawnArea.GetWorldCorners(corners);

        // corners:
        // 0 = bottom left
        // 1 = top left
        // 2 = top right
        // 3 = bottom right

        float left = corners[0].x;
        float right = corners[2].x;
        float bottom = corners[0].y;
        float top = corners[1].y;  //กำหนดตำแหน่งความกว้างและยาวน้อยสุดและมากสุดของ spawnArea


        Vector3 target = dropPosition; //กำหนดให้ตำแหน่งการปล่อยเก็บค่าไว้ตัวแปรนี้


        float x = Mathf.Clamp(dropPosition.x, left, right); // หาค่าแกน x และ  y ไว้เอาไว้เผื่อผู้เล่นลากเกินไปทั้งแกน x และ y 
        float y = Mathf.Clamp(dropPosition.y, bottom, top); // ถ้าลากเกินไปจาก spawnArea ก้จำกำหนดค่แากน x และ y มากสุดตามความกว้างยาวของ spawnArea


        float distLeft = Mathf.Abs(dropPosition.x - left); //หาระยะห่างของ target โดยจะเอาตำแหน่งที่ปล่อยมาลบกับตำแหน่งทุกด้านติดค่า abs ไว้เผื่อมันติดลบ
        float distRight = Mathf.Abs(dropPosition.x - right);
        float distBottom = Mathf.Abs(dropPosition.y - bottom);
        float distTop = Mathf.Abs(dropPosition.y - top);


        float min = Mathf.Min(distLeft, distRight, distBottom, distTop); //หาค่าที่น้อยที่สุด


        if (min == distLeft) //เอาไปเทียบ
            target = new Vector3(left, y, 0);

        else if (min == distRight)
            target = new Vector3(right, y, 0);

        else if (min == distBottom)
            target = new Vector3(x, bottom, 0);

        else
            target = new Vector3(x, top, 0);

        return target; //distLeft หมายความว่าวัตถุปล่อยใกล้ด้านนี้มากทีุ่สดแกน x = left , y = ตำแหน่งของ y ที่ปล่อย , 0 เงื่อนไขอื่นก้เช่นกัน
    }
}