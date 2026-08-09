using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sc4_LeaderboardManager : MonoBehaviour
{
    public static Sc4_LeaderboardManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }


    [Header("UI")]
    public Transform contentParent;
    public GameObject leaderboardItemPrefab;

    private const string SAVE_KEY = "Sc3_Leaderboard";

    void Start()
    {
        if (Sc3_GameResultData.Instance != null)
        {
            SaveScoreToDevice(
                Sc3_GameResultData.Instance.playerName,
                Sc3_GameResultData.Instance.playerScore,
                Sc3_GameResultData.Instance.playerTrashCount
            );
        }

        GenerateLeaderboardUI();
    }

    void GenerateLeaderboardUI()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        LeaderboardData data = LoadLeaderboardData();

        int rank = 1;

        foreach (LeaderboardEntry entry in data.list)
        {
            GameObject obj = Instantiate(leaderboardItemPrefab, contentParent);

            Sc4_LeaderboardItem item = obj.GetComponent<Sc4_LeaderboardItem>();

            item.Setup(
                rank,
                entry.playerName,
                entry.score,
                entry.trashCount
            );

            rank++;
        }

        //CLEAR
        if (Sc3_GameResultData.Instance != null) Destroy(Sc3_GameResultData.Instance.gameObject);
        if (Sc1_PlayerNameInput.Instance != null) Destroy(Sc1_PlayerNameInput.Instance.gameObject);

    }

    public static void SaveScoreToDevice(string playerName, float score, int trashCount)
    {
        string json = PlayerPrefs.GetString(SAVE_KEY, "");

        LeaderboardData data = string.IsNullOrEmpty(json)
            ? new LeaderboardData()
            : JsonUtility.FromJson<LeaderboardData>(json);

        data.list.Add(new LeaderboardEntry(playerName, score, trashCount));

        data.list = data.list
            .OrderByDescending(x => x.score)
            .Take(10)
            .ToList();

        PlayerPrefs.SetString(
            SAVE_KEY,
            JsonUtility.ToJson(data)
        );

        PlayerPrefs.Save();
    }

    LeaderboardData LoadLeaderboardData()
    {
        string json = PlayerPrefs.GetString(SAVE_KEY, "");

        if (string.IsNullOrEmpty(json))
            return new LeaderboardData();

        return JsonUtility.FromJson<LeaderboardData>(json);
    }

    [ContextMenu("Clear Leaderboard")]
    public void ClearLeaderboard()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();

        GenerateLeaderboardUI();

        Debug.Log("Leaderboard Cleared");
    }
}

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public float score;
    public int trashCount;

    public LeaderboardEntry(string playerName, float score, int trashCount)
    {
        this.playerName = playerName;
        this.score = score;
        this.trashCount = trashCount;
    }
}

[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> list = new List<LeaderboardEntry>();
}