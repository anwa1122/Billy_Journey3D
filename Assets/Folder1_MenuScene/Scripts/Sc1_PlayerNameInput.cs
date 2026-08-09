using System.Collections;
using TMPro;
using UnityEngine;

public class Sc1_PlayerNameInput : MonoBehaviour
{
    public static Sc1_PlayerNameInput Instance;

    public TMP_InputField inputField;
    public TMP_Text errorText;

    public string playerName;

    private Coroutine errorCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        errorText.alpha = 0;
    }

    public void SaveName()
    {
        playerName = inputField.text;
    }

    public void ShowNameError()
    {
        if (errorCoroutine != null)
            StopCoroutine(errorCoroutine);

        errorCoroutine = StartCoroutine(ErrorAnimation());
    }

    IEnumerator ErrorAnimation()
    {
        RectTransform rect = errorText.rectTransform;

        Vector3 startScale = Vector3.one * 0.8f;
        Vector3 endScale = Vector3.one;

        rect.localScale = startScale;
        errorText.alpha = 0;

        // Fade In + Pop
        float t = 0;
        while (t < 0.2f)
        {
            t += Time.deltaTime;

            float p = t / 0.2f;

            errorText.alpha = p;

            // Ease Out Back
            float s = 1.70158f;
            float x = p - 1f;
            float eased = 1 + (s + 1) * x * x * x + s * x * x;

            rect.localScale = Vector3.LerpUnclamped(startScale, endScale, eased);

            yield return null;
        }

        errorText.alpha = 1;
        rect.localScale = endScale;

        yield return new WaitForSeconds(1f);

        // Fade Out
        t = 0;
        while (t < 0.25f)
        {
            t += Time.deltaTime;

            float p = t / 0.25f;

            errorText.alpha = 1 - p;

            yield return null;
        }

        errorText.alpha = 0;
    }
}