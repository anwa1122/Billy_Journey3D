using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum GameState
{
    Tutorial,
    Countdown,
    Go,
    Playing,
    End
}

public class GameManager : MonoBehaviour
{
    public GameState currentState;

    public float tutorialTime = 3f;
    public float countdownTime = 3f;
    public float goTime = 2f;
    public float gameTime = 15f;
    public float endTime = 5f;


    public TextMeshProUGUI timerText;
    private float timeText;
    private bool hideTimerText;

    private float timer;
    private float currentTime;

    void Start()
    {
        currentState = GameState.Tutorial;
        timer = tutorialTime; // 3
        timeText = tutorialTime; //3
    }
    void Update()
    {
        currentTime += Time.deltaTime;



        if (!hideTimerText)
        {
            Debug.Log(currentTime + " : " + timer);
            timeText = timer - currentTime;
            timerText.text = timeText.ToString("F0");
        }


        switch (currentState)
        {
            case GameState.Tutorial:

                if (currentTime >= timer) // 2 >= 3
                {
                    timer += countdownTime;
                    currentState = GameState.Countdown;
                }
                break;

            case GameState.Countdown:
                if (currentTime >= timer)
                {
                    timer += goTime;
                    currentState = GameState.Go;
                }
                break;

            case GameState.Go:
                hideTimerText = true;
                timerText.text = "Go";
                if (currentTime >= timer)
                {
                    timer += gameTime;
                    hideTimerText = false;
                    currentState = GameState.Playing;
                }
                break;

            case GameState.Playing:
                if (currentTime >= timer)
                {
                    timer += endTime;
                    currentState = GameState.End;
                }
                break;

            case GameState.End:
                hideTimerText = true;
                timerText.text = "TIMESOVER";
                if (currentTime >= timer)
                {
                    EndGame();
                }
                break;
        }
    }

    void EndGame()
    {
        SceneManager.LoadScene("3_SortingTrash_Scene");
    }
}
