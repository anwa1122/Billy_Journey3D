using UnityEngine;
using System.Collections;
public class Sc3_GameManager : MonoBehaviour
{
    public static Sc3_GameManager Instance;

    public float showSummarizeTime = 3f;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void EndGame()
    {
        StartCoroutine(EndGameRoutine());
    }

    private IEnumerator EndGameRoutine()
    {
        Sc3_GameResultData gameResultData = Sc3_GameResultData.Instance;
        Sc3_SortingManager sortingManager = Sc3_SortingManager.Instance;

        if (Sc1_PlayerNameInput.Instance == null)
        {
            gameResultData.playerName = "Player";
        }
        else
        {
            Sc1_PlayerNameInput playerNameInput = Sc1_PlayerNameInput.Instance;
            gameResultData.playerName = playerNameInput.playerName;
        }


        gameResultData.playerScore = sortingManager.score;
        gameResultData.playerTrashCount = sortingManager.allTrashCount;

        yield return new WaitForSeconds(showSummarizeTime);
        SceneTransition.Instance.ChangeScene(GameScenes.Leaderboard);
    }
}
