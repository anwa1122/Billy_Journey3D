using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Base class กลางของ UI Animation ทุกแบบ (Slide / Drop / Rise)
/// ทุกสคริปต์ที่สืบทอดจากคลาสนี้จะมีวิธีเรียกใช้เหมือนกันหมด: Play() / Stop()
/// ทำให้โค้ดอื่น (เช่น Sc2_GameTimer) เรียกผ่าน GetComponent<Sc2_UIAnimationBase>()
/// ได้โดยไม่ต้องสนใจว่า object นั้นติดสคริปต์แบบไหนอยู่
/// </summary>
[RequireComponent(typeof(RectTransform))]
public abstract class Sc2_UIAnimationBase : MonoBehaviour
{
    [Header("Timing")]
    [Tooltip("ระยะเวลาตอนเข้าฉาก (เคลื่อนที่ / ขยาย)")]
    public float moveDuration = 0.6f;
    [Tooltip("ระยะเวลาที่ค้างอยู่บนจอ ก่อนเล่น Exit")]
    public float stayTime = 1f;
    [Tooltip("ระยะเวลาตอนออกจากจอ (Exit)")]
    public float exitDuration = 0.5f;
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Entrance (ตอนเข้าฉาก)")]
    [Tooltip("เฟดจาง -> ชัด ตอนเข้าฉากไหม")]
    public bool useEntranceFade = true;
    [Tooltip("ขยาย scale จากเล็ก -> ปกติ ตอนเข้าฉากไหม")]
    public bool useEntranceScale = false;
    public Vector3 startScale = Vector3.one * 0.8f;
    public Vector3 endScale = Vector3.one;

    [Header("Exit (ตอนจบ)")]
    [Tooltip("เล่นเฟดจางหายตอนจบไหม")]
    public bool useExitFade = true;
    [Tooltip("เล่นเคลื่อนที่ออกจากจอตอนจบไหม")]
    public bool useExitMove = true;
    [Tooltip("เล่นย่อ scale กลับตอนจบไหม")]
    public bool useExitScale = false;
    [Tooltip("ปิด GameObject อัตโนมัติหลังเล่น Exit จบไหม")]
    public bool disableOnComplete = false;

    [Header("Wiggle (ตอนค้างอยู่บนจอ)")]
    public bool useWiggle = true;
    public float wiggleAngle = 8f;
    public float wiggleSpeed = 14f;
    public float wiggleDamping = 4f;
    [Tooltip("ถ้าเปิด จะสั่นด้วยมุมคงที่ไปเรื่อยๆ ไม่มีการหน่วงให้หยุดเอง (จะหยุดก็ต่อเมื่อถูกสั่ง Stop เท่านั้น เช่นตอนจบ stayTime)")]
    public bool foreverWiggle = false;
    [Tooltip("ถ้าเปิด จะเริ่มสั่นตั้งแต่ตอนเข้าฉากเลย (ระหว่างเลื่อน/ขยายเข้ามา) แทนที่จะรอเข้าฉากเสร็จก่อนแล้วค่อยเริ่มสั่น")]
    public bool wiggleFromStart = false;

    [Header("Events")]
    [Tooltip("เรียกเมื่ออนิเมชันทั้งหมดเล่นจบ เช่น จะเอาไว้เล่นอนิเมชันตัวถัดไปต่อก็ลาก object + เลือกฟังก์ชัน Play() ของมันมาใส่ตรงนี้ได้เลย")]
    public UnityEvent onAnimationComplete;

    protected RectTransform rect;
    protected CanvasGroup canvasGroup;
    protected Vector2 targetPosition;
    protected Vector3 targetScale;
    protected Quaternion targetRotation;

    Coroutine currentRoutine;
    Coroutine wiggleRoutine;

    protected virtual void Awake()
    {
        rect = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        targetPosition = rect.anchoredPosition;
        targetScale = rect.localScale;
        targetRotation = rect.localRotation;
    }

    /// <summary>เรียกเพื่อเล่นอนิเมชัน (ทุกสคริปต์ลูกใช้ชื่อนี้เหมือนกัน)</summary>
    public virtual void Play()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(PlayRoutine());
    }

    /// <summary>หยุดอนิเมชันกลางคัน</summary>
    public virtual void Stop()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        StopWiggle();
    }

    protected abstract IEnumerator PlayRoutine();

    protected void StartWiggle()
    {
        if (!useWiggle) return;

        StopWiggle();
        wiggleRoutine = StartCoroutine(Wiggle());
    }

    protected void StopWiggle()
    {
        if (wiggleRoutine != null)
        {
            StopCoroutine(wiggleRoutine);
            wiggleRoutine = null;
        }

        if (rect != null)
            rect.localRotation = targetRotation;
    }

    IEnumerator Wiggle()
    {
        if (foreverWiggle)
        {
            // สั่นด้วยมุมคงที่ไปเรื่อยๆ ไม่มีวันหน่วงจนหยุดเอง
            // (จะหยุดได้ก็ต่อเมื่อถูกเรียก StopWiggle() จากภายนอก)
            while (true)
            {
                float angle = Mathf.Sin(Time.time * wiggleSpeed) * wiggleAngle;
                rect.localRotation = targetRotation * Quaternion.Euler(0, 0, angle);
                yield return null;
            }
        }

        float currentAngle = wiggleAngle;

        while (currentAngle > 0.1f)
        {
            float angle = Mathf.Sin(Time.time * wiggleSpeed) * currentAngle;

            rect.localRotation = targetRotation * Quaternion.Euler(0, 0, angle);

            currentAngle -= wiggleDamping * Time.deltaTime;

            yield return null;
        }

        rect.localRotation = targetRotation;
    }

    /// <summary>เรียกตอนจบ routine ทุกครั้ง เพื่อยิง event / ปิด object ตามที่ตั้งค่าไว้</summary>
    protected void Complete()
    {
        if (disableOnComplete)
            gameObject.SetActive(false);

        onAnimationComplete?.Invoke();
    }

    [ContextMenu("Play Animation")]
    void TestPlay() => Play();

    [ContextMenu("Reset Position")]
    void ResetState()
    {
        Stop();

        rect.anchoredPosition = targetPosition;
        rect.localScale = targetScale;
        rect.localRotation = targetRotation;

        if (canvasGroup != null)
            canvasGroup.alpha = 1;
    }
}