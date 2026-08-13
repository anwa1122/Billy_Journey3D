using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
public class Sc4_ClearBtn : MonoBehaviour
{
    public TextMeshProUGUI text;
    private bool alreadyClick = false;

    void Start()
    {
        alreadyClick = false;
    }
    public void ClearLeaderBoard()
    {
        if (!alreadyClick)
        {
            alreadyClick = true;
            StartCoroutine(ClearLeaderBoardData());
        }
    }


    IEnumerator ClearLeaderBoardData()
    {
        Sc4_LeaderboardManager.Instance.ClearLeaderboard();
        text.text = "Cleared!";
        yield return new WaitForSeconds(1f);
        text.text = "Clear Data";
        alreadyClick = false;
    }
}
