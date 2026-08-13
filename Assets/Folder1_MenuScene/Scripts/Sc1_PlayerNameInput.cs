using System.Collections;
using TMPro;
using UnityEngine;

public class Sc1_PlayerNameInput : MonoBehaviour
{
    public static Sc1_PlayerNameInput Instance;

    public TMP_InputField inputField;
    public TMP_Text errorText;

    [Header("Name Settings")]
    [Tooltip("จำนวนตัวอักษรสูงสุดที่อนุญาต")]
    public int maxCharacter = 10;

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

        // ตั้งค่า Character Limit ให้ InputField ด้วย (แนะนำ)
        if (inputField != null)
            inputField.characterLimit = maxCharacter;
    }

    /// <summary>
    /// เช็คว่าชื่อที่กรอกถูกต้องหรือไม่
    /// </summary>
    public bool IsNameValid()
    {
        string name = inputField.text.Trim();

        // ว่าง
        if (string.IsNullOrEmpty(name))
            return false;

        // เกินจำนวนตัวอักษร
        if (name.Length > maxCharacter)
            return false;

        return true;
    }

    /// <summary>
    /// ใช้เรียกตอนกดปุ่ม Confirm / Next
    /// </summary>
    public void ValidateAndSaveName()
    {
        string name = inputField.text.Trim();

        if (string.IsNullOrEmpty(name))
        {
            // กรณีว่าง
            errorText.text = "กรุณากรอกชื่อ";
            ShowNameError();
            return;
        }

        if (name.Length > maxCharacter)
        {
            // กรณีเกินตัวอักษร
            errorText.text = $"ชื่อต้องไม่เกิน {maxCharacter} ตัวอักษร";
            ShowNameError();
            return;
        }

        // ผ่าน → เซฟชื่อ
        playerName = name;
        Debug.Log("บันทึกชื่อ: " + playerName);
    }

    // ยังคงใช้ได้เหมือนเดิม ถ้าอยากเซฟแบบไม่เช็ค
    public void SaveName()
    {
        playerName = inputField.text.Trim();
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