using UnityEngine;
using TMPro;
using System.Collections;

public class Sc3_SummarizeData : MonoBehaviour
{
    [Header("UI Text References")]
    public TextMeshProUGUI namePlayerText;
    public TextMeshProUGUI scorePlayerText;
    public TextMeshProUGUI allTrashText;
    public TextMeshProUGUI correctTrashText;
    public TextMeshProUGUI incorrectTrashText;

    [Header("Animation")]

    [Tooltip("ตำแหน่งเริ่มต้น")]
    public float startPosY = -800f;

    [Tooltip("ตำแหน่งปลายทาง")]
    public float targetPosY = 0f;

    [Tooltip("ยิ่งมาก ยิ่งช้าลงตอนท้าย")]
    [Range(0.1f, 5f)]
    public float smoothTime = 1.2f;

    [Tooltip("ความเร็ว (1 = ช้ามาก, 20 = เร็วมาก)")]
    [Range(1f, 20f)]
    public float speed = 8f;

    private RectTransform rectTransform;
    private Coroutine slideRoutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        rectTransform.anchoredPosition = new Vector2(
            rectTransform.anchoredPosition.x,
            startPosY
        );
    }

    public void SetupAndShow(string playerName, float score, int total, int correct, int incorrect)
    {
        namePlayerText.text = "Name : " + playerName;
        scorePlayerText.text = "Score : " + score.ToString("0");
        allTrashText.text = "All Trash : " + total;
        correctTrashText.text = "Correct Trash : " + correct;
        incorrectTrashText.text = "Incorrect Trash : " + incorrect;

        gameObject.SetActive(true);

        if (slideRoutine != null)
            StopCoroutine(slideRoutine);

        slideRoutine = StartCoroutine(SlideUpRoutine());
    }

    private IEnumerator SlideUpRoutine()
    {
        float velocity = 0f;

        rectTransform.anchoredPosition = new Vector2(
            rectTransform.anchoredPosition.x,
            startPosY
        );

        // แปลง speed เป็น Pixel/วินาที
        float maxSpeed = speed * 1000f;

        while (Mathf.Abs(rectTransform.anchoredPosition.y - targetPosY) > 0.05f)
        {
            float newY = Mathf.SmoothDamp(
                rectTransform.anchoredPosition.y,
                targetPosY,
                ref velocity,
                smoothTime,
                maxSpeed,
                Time.deltaTime
            );

            rectTransform.anchoredPosition = new Vector2(
                rectTransform.anchoredPosition.x,
                newY
            );

            yield return null;
        }

        rectTransform.anchoredPosition = new Vector2(
            rectTransform.anchoredPosition.x,
            targetPosY
        );
    }
}