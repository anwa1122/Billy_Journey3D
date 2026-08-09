using UnityEngine;

public class Sc3_GameResultData : MonoBehaviour
{
    public static Sc3_GameResultData Instance;

    public string playerName;
    public float playerScore;
    public int playerTrashCount;
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
    }
}
