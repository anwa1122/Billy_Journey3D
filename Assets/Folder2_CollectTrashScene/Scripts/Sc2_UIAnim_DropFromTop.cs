using System.Collections;
using UnityEngine;

/// <summary>
/// เข้าฉากด้วยเอฟเฟกต์ scale กระแทก (ใหญ่ -> ปกติ) แล้วออกฉากด้วยเฟด/ย่อ/เลื่อนลง (เลือกได้)
/// </summary>
public class Sc2_UIAnim_DropFromTop : Sc2_UIAnimationBase
{
    [Header("Drop From Top (Impact)")]
    public float impactScale = 5f;

    [Tooltip("ระยะที่จะเคลื่อนลง ตอน Exit (มีผลเมื่อเปิด Use Exit Move เท่านั้น)")]
    public float exitDistance = 500f;

    protected override IEnumerator PlayRoutine()
    {
        rect.anchoredPosition = targetPosition;
        rect.localScale = Vector3.one * impactScale;

        if (useEntranceFade) canvasGroup.alpha = 0;

        if (wiggleFromStart) StartWiggle();

        float time = 0;

        while (time < moveDuration)
        {
            time += Time.deltaTime;
            float t = moveCurve.Evaluate(time / moveDuration);

            rect.localScale = Vector3.Lerp(Vector3.one * impactScale, targetScale, t);

            if (useEntranceFade) canvasGroup.alpha = t;

            yield return null;
        }

        rect.localScale = targetScale;
        if (useEntranceFade) canvasGroup.alpha = 1;

        if (!wiggleFromStart) StartWiggle();
        yield return new WaitForSeconds(stayTime);
        StopWiggle();

        Vector2 exitPos = targetPosition + Vector2.down * exitDistance;
        float startAlpha = canvasGroup.alpha;

        time = 0;

        while (time < exitDuration)
        {
            time += Time.deltaTime;
            float t = moveCurve.Evaluate(time / exitDuration);

            if (useExitMove)
                rect.anchoredPosition = Vector2.Lerp(targetPosition, exitPos, t);

            if (useExitFade)
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t);

            if (useExitScale)
                rect.localScale = Vector3.Lerp(targetScale, Vector3.one * impactScale, t);

            yield return null;
        }

        if (useExitMove) rect.anchoredPosition = exitPos;
        if (useExitFade) canvasGroup.alpha = 0;
        if (useExitScale) rect.localScale = Vector3.one * impactScale;

        Complete();
    }
}