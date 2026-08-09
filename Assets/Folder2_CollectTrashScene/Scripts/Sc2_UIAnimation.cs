using UnityEngine;
using System.Collections;

public class Sc2_UIAnimation : MonoBehaviour
{
    public enum AnimationType
    {
        SlideFromRight,
        DropFromTop,
        RiseFromBottom
    }

    [Header("Animation")]
    public AnimationType animationType;

    [Header("Move")]
    public float distance = 500f;
    public float moveDuration = 0.6f;
    public float stayTime = 1f;
    public float exitDuration = 0.5f;

    [Header("Animation Curve")]
    public AnimationCurve moveCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Fade")]
    public bool useFade = true;

    [Header("Scale")]
    public bool useScale = false;
    public Vector3 startScale = Vector3.one * 0.8f;
    public Vector3 endScale = Vector3.one;

    [Header("Impact")]
    public float impactScale = 5f;

    [Header("Wiggle")]
    public bool useWiggle = true;
    public float wiggleAngle = 8f;
    public float wiggleSpeed = 14f;
    public float wiggleDamping = 4f;

    RectTransform rect;
    CanvasGroup canvasGroup;

    Vector2 targetPosition;
    Vector3 targetScale;

    Coroutine currentRoutine;

    void Awake()
    {
        rect = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        targetPosition = rect.anchoredPosition;
        targetScale = rect.localScale;
    }

    public void Play()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(PlayRoutine());
    }

    public void Stop()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = null;
    }

    IEnumerator PlayRoutine()
    {
        switch (animationType)
        {
            case AnimationType.SlideFromRight:

                yield return StartCoroutine(SlideFromRight());

                break;

            case AnimationType.DropFromTop:

                yield return StartCoroutine(DropFromTop());

                break;

            case AnimationType.RiseFromBottom:

                yield return StartCoroutine(RiseFromBottom());

                break;
        }
    }
    IEnumerator SlideFromRight()
    {
        Vector2 startPos = targetPosition + Vector2.right * distance;

        rect.anchoredPosition = startPos;

        if (useFade)
            canvasGroup.alpha = 0;

        if (useScale)
            rect.localScale = startScale;

        float time = 0;

        while (time < moveDuration)
        {
            time += Time.deltaTime;

            float t = moveCurve.Evaluate(time / moveDuration);

            rect.anchoredPosition = Vector2.Lerp(
                startPos,
                targetPosition,
                t);

            if (useFade)
                canvasGroup.alpha = t;

            if (useScale)
                rect.localScale = Vector3.Lerp(
                    startScale,
                    endScale,
                    t);

            yield return null;
        }

        rect.anchoredPosition = targetPosition;

        if (useFade)
            canvasGroup.alpha = 1;

        if (useScale)
            rect.localScale = endScale;

        Coroutine wiggleRoutine = null;

        if (useWiggle)
            wiggleRoutine = StartCoroutine(Wiggle());

        yield return new WaitForSeconds(stayTime);

        if (wiggleRoutine != null)
            StopCoroutine(wiggleRoutine);

        rect.rotation = Quaternion.identity;

        time = 0;

        Vector2 exitPos = targetPosition + Vector2.left * distance;

        while (time < exitDuration)
        {
            time += Time.deltaTime;

            float t = moveCurve.Evaluate(time / exitDuration);

            rect.anchoredPosition = Vector2.Lerp(
                targetPosition,
                exitPos,
                t);

            if (useFade)
                canvasGroup.alpha = 1 - t;

            yield return null;
        }

        rect.anchoredPosition = exitPos;

        if (useFade)
            canvasGroup.alpha = 0;
    }

    IEnumerator DropFromTop()
    {
        rect.anchoredPosition = targetPosition;

        rect.localScale = Vector3.one * impactScale;

        if (useFade)
            canvasGroup.alpha = 0;

        float time = 0;

        while (time < moveDuration)
        {
            time += Time.deltaTime;

            float t = moveCurve.Evaluate(time / moveDuration);

            rect.localScale = Vector3.Lerp(
                Vector3.one * 5f,
                Vector3.one,
                t);

            if (useFade)
                canvasGroup.alpha = t;

            yield return null;
        }

        rect.localScale = Vector3.one;

        Coroutine wiggleRoutine = null;

        if (useWiggle)
            wiggleRoutine = StartCoroutine(Wiggle());

        yield return new WaitForSeconds(stayTime);

        if (wiggleRoutine != null)
            StopCoroutine(wiggleRoutine);

        rect.localRotation = Quaternion.identity;

        if (useFade)
        {
            time = 0;

            while (time < exitDuration)
            {
                time += Time.deltaTime;

                canvasGroup.alpha = 1 - (time / exitDuration);

                yield return null;
            }

            canvasGroup.alpha = 0;
        }
    }

    IEnumerator RiseFromBottom()
    {
        Vector2 startPos = targetPosition + Vector2.down * distance;

        rect.anchoredPosition = startPos;

        if (useFade)
            canvasGroup.alpha = 0;

        if (useScale)
            rect.localScale = startScale;

        float time = 0;

        while (time < moveDuration)
        {
            time += Time.deltaTime;

            float t = moveCurve.Evaluate(time / moveDuration);

            rect.anchoredPosition = Vector2.Lerp(
                startPos,
                targetPosition,
                t);

            if (useFade)
                canvasGroup.alpha = t;

            if (useScale)
                rect.localScale = Vector3.Lerp(
                    startScale,
                    endScale,
                    t);

            yield return null;
        }

        rect.anchoredPosition = targetPosition;

        Coroutine wiggleRoutine = null;

        if (useWiggle)
            wiggleRoutine = StartCoroutine(Wiggle());

        yield return new WaitForSeconds(stayTime);

        if (wiggleRoutine != null)
            StopCoroutine(wiggleRoutine);

        rect.rotation = Quaternion.identity;

        time = 0;

        Vector2 exitPos = targetPosition + Vector2.up * distance;

        while (time < exitDuration)
        {
            time += Time.deltaTime;

            float t = moveCurve.Evaluate(time / exitDuration);

            rect.anchoredPosition = Vector2.Lerp(
                targetPosition,
                exitPos,
                t);

            if (useFade)
                canvasGroup.alpha = 1 - t;

            yield return null;
        }

        rect.anchoredPosition = exitPos;

        if (useFade)
            canvasGroup.alpha = 0;
    }

    IEnumerator Wiggle()
    {
        float currentAngle = wiggleAngle;

        while (currentAngle > 0.1f)
        {
            float angle =
                Mathf.Sin(Time.time * wiggleSpeed) * currentAngle;

            rect.localRotation =
                Quaternion.Euler(0, 0, angle);

            currentAngle -= wiggleDamping * Time.deltaTime;

            yield return null;
        }

        rect.localRotation = Quaternion.identity;
    }

    public void Play(AnimationType type)
    {
        animationType = type;
        Play();
    }

    [ContextMenu("Play Animation")]
    void TestPlay()
    {
        Play();
    }

    [ContextMenu("Reset Position")]
    void ResetPosition()
    {
        Stop();

        rect.anchoredPosition = targetPosition;
        rect.localScale = targetScale;
        rect.localRotation = Quaternion.identity;

        if (canvasGroup != null)
            canvasGroup.alpha = 1;
    }
}