using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class Sc3_ExpandHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale Settings")]
    [Tooltip("ขนาดที่ต้องการขยายตอน Hover (1.0 = ปกติ)")]
    [Range(1.0f, 1.3f)]
    public float targetScale = 1.08f;

    [Tooltip("ความนุ่มนวลในการขยายและหดกลับ (ยิ่งมากยิ่งเร็ว)")]
    public float smoothSpeed = 8f;

    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Vector3 targetScaleVector;
    private bool isHovering = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        targetScaleVector = originalScale * targetScale;
    }

    void Update()
    {
        // ขยายหรือหดกลับอย่างนุ่มนวล
        Vector3 desiredScale = isHovering ? targetScaleVector : originalScale;
        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            desiredScale,
            Time.deltaTime * smoothSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        // อัปเดตค่า target เผื่อมีการปรับใน Inspector ตอนรัน
        targetScaleVector = originalScale * targetScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    void OnDisable()
    {
        isHovering = false;
        if (rectTransform != null)
            rectTransform.localScale = originalScale;
    }
}