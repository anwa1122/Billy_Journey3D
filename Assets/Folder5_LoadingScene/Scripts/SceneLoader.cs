using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static string TargetScene;

    public static void Load(string SceneName)
    {
        TargetScene = SceneName;

        SceneManager.LoadScene("LoadingScene");
    }
}
