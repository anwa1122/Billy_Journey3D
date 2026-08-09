using UnityEngine;

public enum Sc2_GameState
{
    Tutorial,
    Countdown,
    Go,
    Playing,
    End
}

public class Sc2_GameManager : MonoBehaviour
{
    public static Sc2_GameManager Instance;
    public PlayerScript playerScript;

    public Sc2_GameState currentState;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void ChangeState(Sc2_GameState newState)
    {
        currentState = newState;
    }


    void Start()
    {
        ChangeState(Sc2_GameState.Tutorial);
        playerScript.freezePlayer = true;
    }


    public void StartGame()
    {
        ChangeState(Sc2_GameState.Playing);
        playerScript.freezePlayer = false;
    }

    public void EndGame()
    {
        ChangeState(Sc2_GameState.End);

        SceneTransition.Instance.ChangeScene(GameScenes.Sorting);
    }
}