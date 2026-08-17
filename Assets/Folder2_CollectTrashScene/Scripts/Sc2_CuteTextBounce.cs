using UnityEngine;
using TMPro;
using System.Collections;

public class Sc2_CuteTextBounce : MonoBehaviour
{
    public static Sc2_CuteTextBounce Instance { get; private set; }

    [Header("References")]
    public TextMeshProUGUI targetText;

    [Header("Bounce Settings")]
    public float bounceDuration = 0.85f;
    public float startScale = 0.18f;
    public float startOffsetY = -280f;

    [Header("Tilt (ตอนเด้ง)")]
    public float tiltAngle = 11f;
    public float tiltSpeed = 3.8f;

    [Header("Idle Sway (ส่ายเบา ๆ หลังเด้งเสร็จ)")]
    public bool enableIdleSway = true;
    public float idleTiltAngle = 5.5f;
    public float idleTiltSpeed = 1.6f;

    [Header("Pop Settings (ขยายตอนกด)")]
    [Tooltip("ขยายใหญ่ขึ้นกี่เท่า (1.15 = ใหญ่ขึ้น 15%)")]
    public float popScale = 1.18f;

    [Tooltip("ระยะเวลาการขยาย + หดกลับ")]
    public float popDuration = 0.32f;

    [Header("Feel / Smoothness")]
    [Range(0.4f, 1.2f)]
    public float scaleSmoothness = 0.75f;

    [Range(0.4f, 1.2f)]
    public float positionSmoothness = 0.8f;

    public bool useOvershoot = true;

    // ================= Internal =================
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private RectTransform rectTransform;

    public bool isPlaying { get; private set; } = false;   // ทำให้เรียกจากข้างนอกได้
    private bool isIdleSwaying = false;
    private Coroutine currentCoroutine;
    private Coroutine idleCoroutine;
    private Coroutine popCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (targetText == null)
            targetText = GetComponent<TextMeshProUGUI>();

        rectTransform = targetText.rectTransform;
        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
        originalRotation = rectTransform.localRotation;

        // เริ่มต้นให้เล็ก + อยู่ข้างล่าง
        rectTransform.localScale = Vector3.one * startScale;
        rectTransform.anchoredPosition = new Vector2(originalPosition.x, originalPosition.y + startOffsetY);
        rectTransform.localRotation = Quaternion.identity;
    }

    // ================= Public Methods =================

    public void PlayBounce()
    {
        StopAllAnimations();
        currentCoroutine = StartCoroutine(BounceRoutine(false));
    }

    public void PlayBounceOut()
    {
        StopAllAnimations();                 // ← บังคับหยุดทุกอย่างก่อน
        currentCoroutine = StartCoroutine(BounceRoutine(true));
    }

    public void PlayPop()
    {
        if (isPlaying) return;               // ถ้ากำลังเด้งอยู่ไม่ต้อง Pop

        StopPop();
        popCoroutine = StartCoroutine(PopRoutine());
    }

    public void StopBounce()
    {
        StopAllAnimations();
        ResetToOriginal();
    }

    // ================= เพิ่มเมธอดนี้ =================
    private void StopAllAnimations()
    {
        // หยุด Bounce
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        // หยุด Idle Sway
        isIdleSwaying = false;
        if (idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
            idleCoroutine = null;
        }

        // หยุด Pop
        if (popCoroutine != null)
        {
            StopCoroutine(popCoroutine);
            popCoroutine = null;
        }

        isPlaying = false;
    }

    // ================= Internal =================

    private void ResetToOriginal()
    {
        rectTransform.anchoredPosition = originalPosition;
        rectTransform.localScale = originalScale;
        rectTransform.localRotation = originalRotation;
    }

    private void StopIdleSway()
    {
        isIdleSwaying = false;
        if (idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
            idleCoroutine = null;
        }
    }

    private void StopPop()
    {
        if (popCoroutine != null)
        {
            StopCoroutine(popCoroutine);
            popCoroutine = null;
        }
    }

    private IEnumerator BounceRoutine(bool isReverse)
    {
        isPlaying = true;

        Vector2 startPos, endPos;
        float startS, endS;

        if (!isReverse)
        {
            startPos = new Vector2(originalPosition.x, originalPosition.y + startOffsetY);
            endPos = originalPosition;
            startS = startScale;
            endS = originalScale.x;
        }
        else
        {
            startPos = originalPosition;
            endPos = new Vector2(originalPosition.x, originalPosition.y + startOffsetY);
            startS = originalScale.x;
            endS = startScale;
        }

        rectTransform.anchoredPosition = startPos;
        rectTransform.localScale = Vector3.one * startS;

        float elapsed = 0f;

        while (elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / bounceDuration);

            float posT = EaseOutBack(t, positionSmoothness);
            rectTransform.anchoredPosition = Vector2.LerpUnclamped(startPos, endPos, posT);

            float scaleT = useOvershoot ? EaseOutBack(t, scaleSmoothness) : EaseOutCubic(t);
            float currentScale = Mathf.LerpUnclamped(startS, endS, scaleT);
            rectTransform.localScale = Vector3.one * currentScale;

            float tiltStrength = Mathf.Lerp(1f, 0.35f, t);
            float tilt = Mathf.Sin(elapsed * tiltSpeed) * tiltAngle * tiltStrength;
            rectTransform.localRotation = Quaternion.Euler(0f, 0f, tilt);

            yield return null;
        }

        rectTransform.anchoredPosition = endPos;
        rectTransform.localScale = Vector3.one * endS;

        isPlaying = false;

        if (!isReverse && enableIdleSway)
        {
            float lastTilt = rectTransform.localEulerAngles.z;
            if (lastTilt > 180f) lastTilt -= 360f;

            idleCoroutine = StartCoroutine(IdleSwayRoutine(lastTilt));
        }
        else
        {
            float startTilt = rectTransform.localEulerAngles.z;
            if (startTilt > 180f) startTilt -= 360f;

            float settleTime = 0.25f;
            float settleElapsed = 0f;

            while (settleElapsed < settleTime)
            {
                settleElapsed += Time.deltaTime;
                float t = Mathf.Clamp01(settleElapsed / settleTime);
                float smoothT = t * t * (3f - 2f * t);
                float currentTilt = Mathf.Lerp(startTilt, 0f, smoothT);
                rectTransform.localRotation = Quaternion.Euler(0f, 0f, currentTilt);
                yield return null;
            }

            rectTransform.localRotation = originalRotation;
        }
    }

    private IEnumerator IdleSwayRoutine(float startTilt)
    {
        isIdleSwaying = true;

        float blendTime = 0.35f;
        float blendElapsed = 0f;
        float time = 0f;

        while (blendElapsed < blendTime && isIdleSwaying)
        {
            blendElapsed += Time.deltaTime;
            time += Time.deltaTime;

            float blend = Mathf.Clamp01(blendElapsed / blendTime);
            float smoothBlend = blend * blend * (3f - 2f * blend);

            float idleTilt = Mathf.Sin(time * idleTiltSpeed) * idleTiltAngle;
            float currentTilt = Mathf.Lerp(startTilt, idleTilt, smoothBlend);

            rectTransform.localRotation = Quaternion.Euler(0f, 0f, currentTilt);
            yield return null;
        }

        while (isIdleSwaying)
        {
            time += Time.deltaTime;
            float tilt = Mathf.Sin(time * idleTiltSpeed) * idleTiltAngle;
            rectTransform.localRotation = Quaternion.Euler(0f, 0f, tilt);
            yield return null;
        }
    }

    private IEnumerator PopRoutine()
    {
        float halfDuration = popDuration * 0.5f;
        float elapsed = 0f;

        Vector3 startScale = rectTransform.localScale;
        Vector3 peakScale = originalScale * popScale;

        // ขยายขึ้นแบบเด้ง
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            float easeT = EaseOutBack(t, 0.7f); // เด้งนิดหน่อย

            rectTransform.localScale = Vector3.LerpUnclamped(startScale, peakScale, easeT);
            yield return null;
        }

        // หดกลับ
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            float easeT = EaseOutCubic(t);

            rectTransform.localScale = Vector3.LerpUnclamped(peakScale, originalScale, easeT);
            yield return null;
        }

        rectTransform.localScale = originalScale;
        popCoroutine = null;
    }

    // ========== Easing Functions ==========
    private float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }

    private float EaseOutBack(float t, float smoothness)
    {
        float c1 = 1.70158f * (2f - smoothness);
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
}