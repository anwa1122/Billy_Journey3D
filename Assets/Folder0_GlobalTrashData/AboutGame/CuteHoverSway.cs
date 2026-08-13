using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class CuteHoverTilt : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("Tilt Settings (Hover)")]
    [Tooltip("มุมเอียงสูงสุด (องศา)")]
    public float amplitude = 12f;

    [Tooltip("ความถี่การโยก (รอบต่อวินาที)")]
    public float frequency = 2.2f;

    [Tooltip("ความเร็วการเคลื่อนไหวโดยรวม")]
    public float speed = 1.1f;

    [Tooltip("ความนุ่มนวลตอนกลับตำแหน่งตรง")]
    public float returnSmoothness = 7f;

    [Header("Press Squash Settings")]
    [Tooltip("ขนาดตอนถูกกด (ยิ่งน้อยยิ่งหุบตัวมาก)")]
    [Range(0.7f, 1f)]
    public float pressScale = 0.88f;

    [Tooltip("ความนุ่มนวลตอนหุบและเด้งกลับ")]
    public float squashSmoothness = 12f;

    private RectTransform rectTransform;
    private Quaternion originalRotation;
    private Vector3 originalScale;
    private Vector3 targetScale;

    private bool isHovering = false;
    private bool isPressed = false;
    private float timeOffset;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalRotation = rectTransform.localRotation;
        originalScale = rectTransform.localScale;
        targetScale = originalScale;
        timeOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        // ===== Tilt (เอียงซ้าย-ขวา) =====
        if (isHovering && !isPressed)
        {
            float angle = Mathf.Sin((Time.time + timeOffset) * frequency * speed) * amplitude;
            rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            rectTransform.localRotation = Quaternion.Slerp(
                rectTransform.localRotation,
                originalRotation,
                Time.deltaTime * returnSmoothness
            );
        }

        // ===== Scale (หุบตัวตอนกด) =====
        targetScale = isPressed ? originalScale * pressScale : originalScale;

        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            targetScale,
            Time.deltaTime * squashSmoothness
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        isPressed = false; // กันค้างตอนลากเมาส์ออกขณะกดค้าง
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    void OnDisable()
    {
        isHovering = false;
        isPressed = false;

        if (rectTransform != null)
        {
            rectTransform.localRotation = originalRotation;
            rectTransform.localScale = originalScale;
        }
    }
}