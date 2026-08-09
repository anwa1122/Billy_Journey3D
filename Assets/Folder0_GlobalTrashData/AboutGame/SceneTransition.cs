using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [Header("UI")]
    public Canvas transitionCanvas;
    public List<Image> transitionImages = new();

    [Header("Transition")]
    public float transitionSpeed = 2f;

    private readonly List<RectTransform> imgRects = new();

    private Coroutine currentTransition;

    private Vector2 centerPos;
    private Vector2 rightPos;
    private Vector2 leftPos;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        imgRects.Clear();

        foreach (Image img in transitionImages)
        {
            if (img != null)
                imgRects.Add(img.GetComponent<RectTransform>());
        }

        CalculatePositions();
    }

    void Start()
    {
        PlayTransition(FadeInRoutine());
    }

    void CalculatePositions()
    {
        RectTransform canvasRect = transitionCanvas.GetComponent<RectTransform>();

        float width = canvasRect.rect.width;

        centerPos = Vector2.zero;
        rightPos = new Vector2(width * 1.5f, 0f);
        leftPos = new Vector2(-width * 1.5f, 0f);
    }

    public void ChangeScene(string targetScene)
    {
        PlayTransition(FadeOutAndLoadRoutine(targetScene));
    }

    void PlayTransition(IEnumerator routine)
    {
        if (currentTransition != null)
            StopCoroutine(currentTransition);

        currentTransition = StartCoroutine(routine);
    }

    //==========================================
    // Fade In (กลาง -> ซ้าย)
    //==========================================
    IEnumerator FadeInRoutine()
    {
        transitionCanvas.enabled = true;

        foreach (RectTransform rect in imgRects)
            rect.anchoredPosition = centerPos;

        SetAllAlpha(1f);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;

            float ease = 1f - Mathf.Pow(1f - Mathf.Clamp01(t), 3f);

            Vector2 pos = Vector2.Lerp(centerPos, leftPos, ease);

            foreach (RectTransform rect in imgRects)
                rect.anchoredPosition = pos;

            SetAllAlpha(Mathf.Lerp(1f, 0f, ease));

            yield return null;
        }

        transitionCanvas.enabled = false;
        currentTransition = null;
    }

    IEnumerator FadeOutAndLoadRoutine(string targetScene)
    {
        transitionCanvas.enabled = true;

        foreach (RectTransform rect in imgRects)
            rect.anchoredPosition = rightPos;

        SetAllAlpha(0f);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;

            float ease = 1f - Mathf.Pow(1f - Mathf.Clamp01(t), 3f);

            Vector2 pos = Vector2.Lerp(rightPos, centerPos, ease);

            foreach (RectTransform rect in imgRects)
                rect.anchoredPosition = pos;

            SetAllAlpha(Mathf.Lerp(0f, 1f, ease));

            yield return null;
        }

        currentTransition = null;

        SceneManager.LoadScene(targetScene);
    }

    void SetAllAlpha(float alpha)
    {
        foreach (Image img in transitionImages)
        {
            if (img == null) continue;

            Color c = img.color;
            c.a = alpha;
            img.color = c;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}