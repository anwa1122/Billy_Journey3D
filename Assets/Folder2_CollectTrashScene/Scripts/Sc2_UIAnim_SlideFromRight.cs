using System.Collections;
using UnityEngine;

/// <summary>
/// เข้าฉากโดยเลื่อนจากขวา -> ตำแหน่งเดิม แล้วออกฉากโดยเลื่อนไปทางซ้าย
/// </summary>
public class Sc2_UIAnim_SlideFromRight : Sc2_UIAnimationBase
{
    [Header("Slide From Right")]
    public float distance = 500f;

    protected override IEnumerator PlayRoutine()
    {
        Vector2 startPos = targetPosition + Vector2.right * distance;
        rect.anchoredPosition = startPos;

        if (useEntranceFade) canvasGroup.alpha = 0;
        if (useEntranceScale) rect.localScale = startScale;

        if (wiggleFromStart) StartWiggle();

        float time = 0;

        while (time < moveDuration)
        {
            time += Time.deltaTime;
            float t = moveCurve.Evaluate(time / moveDuration);

            rect.anchoredPosition = Vector2.Lerp(startPos, targetPosition, t);

            if (useEntranceFade) canvasGroup.alpha = t;
            if (useEntranceScale) rect.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        rect.anchoredPosition = targetPosition;
        if (useEntranceFade) canvasGroup.alpha = 1;
        if (useEntranceScale) rect.localScale = endScale;

        if (!wiggleFromStart) StartWiggle();
        yield return new WaitForSeconds(stayTime);
        StopWiggle();

        Vector2 exitPos = targetPosition + Vector2.left * distance;
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
                rect.localScale = Vector3.Lerp(endScale, startScale, t);

            yield return null;
        }

        if (useExitMove) rect.anchoredPosition = exitPos;
        if (useExitFade) canvasGroup.alpha = 0;
        if (useExitScale) rect.localScale = startScale;

        Complete();
    }
}