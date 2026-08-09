using UnityEngine;
using TMPro;
using System.Collections;

public class Sc2_GameTimer : MonoBehaviour
{
    [Header("Time")]
    public float tutorialTime = 3f;
    public float countdownTime = 3f;
    public float countdownInterval = 0.7f;
    public float goTime = 1f;
    public float gameTime = 60f;
    public float endTime = 3f;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text countdownText;
    public GameObject missionText;
    public TMP_Text timeOverText;

    private float currentTime;

    void Start()
    {
        timerText.gameObject.SetActive(false);

        if (missionText != null)
            missionText.gameObject.SetActive(false);

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        if (timeOverText != null)
            timeOverText.gameObject.SetActive(false);

        StartCoroutine(GameFlowRoutine());
    }

    IEnumerator GameFlowRoutine()
    {
        //---------------- Tutorial -----------------------------------------------------------
        //-----------------------------------------------------------------------------------
        Sc2_GameManager.Instance.ChangeState(Sc2_GameState.Tutorial);

        if (missionText != null)
        {

            missionText.gameObject.SetActive(true);
            missionText.GetComponent<Sc2_UIAnimation>().Play();
            yield return new WaitForSeconds(tutorialTime);
            missionText.gameObject.SetActive(false);
        }

        //---------------- Countdown ----------------------------------------------------------
        //-----------------------------------------------------------------------------------
        Sc2_GameManager.Instance.ChangeState(Sc2_GameState.Countdown);

        countdownText.gameObject.SetActive(true);

        string[] countdown =
        {
            "3",
            "2",
            "1",
            "GO!"
        };

        foreach (string text in countdown)
        {
            countdownText.text = text;

            // ถ้ามีเอฟเฟกต์ก็เรียกตรงนี้
            countdownText.GetComponent<Sc2_UIAnimation>().Play();

            yield return new WaitForSeconds(countdownInterval);
        }

        countdownText.gameObject.SetActive(false);

        //---------------- Playing ----------------------------------------------------------
        //-----------------------------------------------------------------------------------
        Sc2_GameManager.Instance.StartGame();

        timerText.gameObject.SetActive(true);

        currentTime = gameTime;

        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            timerText.text = Mathf.CeilToInt(currentTime).ToString();

            yield return null;
        }

        //---------------- End ---------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        Sc2_GameManager.Instance.ChangeState(Sc2_GameState.End);

        timerText.gameObject.SetActive(false);

        if (timeOverText != null)
        {
            timeOverText.gameObject.SetActive(true);
            timeOverText.GetComponent<Sc2_UIAnimation>().Play();
        }

        yield return new WaitForSeconds(endTime);

        Sc2_GameManager.Instance.EndGame();
    }
}