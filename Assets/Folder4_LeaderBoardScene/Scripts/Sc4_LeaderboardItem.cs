using TMPro;
using UnityEngine;
public class Sc4_LeaderboardItem : MonoBehaviour
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI trashCountText;

    public void Setup(int rank, string playerName, float score, int trashCount)
    {
        rankText.text = "#" + rank;
        nameText.text = playerName;
        scoreText.text = score.ToString();
        trashCountText.text = trashCount.ToString();
    }
}
