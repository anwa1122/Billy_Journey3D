using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
public class Sc4_ClearBtn : MonoBehaviour
{
    public TextMeshProUGUI text;
    public void ClearLeaderBoard()
    {
        StartCoroutine(ClearLeaderBoardData());
    }

    IEnumerator ClearLeaderBoardData()
    {
        Sc4_LeaderboardManager.Instance.ClearLeaderboard();
        text.text = "Cleared!";
        yield return new WaitForSeconds(1f);
        text.text = "Clear Data";
    }
}
