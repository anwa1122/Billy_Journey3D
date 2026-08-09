using UnityEngine;

public class Sc1_GameManager : MonoBehaviour
{
    public void gameStart()
    {
        if (!string.IsNullOrWhiteSpace(Sc1_PlayerNameInput.Instance.playerName))
        {
            SceneTransition.Instance.ChangeScene(GameScenes.Collect);
        }
        else
        {
            Sc1_PlayerNameInput.Instance.ShowNameError();
        }
    }

    public void leaderboardStart()
    {
        SceneTransition.Instance.ChangeScene(GameScenes.Leaderboard);
    }


    public void QuitGame()
    {
#if UNITY_EDITOR
        // หยุดการเล่นใน Unity Editor (เฉพาะตอนทดสอบสร้างเกม)
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // ออกจากโปรแกรมเมื่อ Build เป็นไฟล์เกมเล่นจริง
            Application.Quit();
#endif
    }
}
