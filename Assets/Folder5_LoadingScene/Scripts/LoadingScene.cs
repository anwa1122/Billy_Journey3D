using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScene : MonoBehaviour
{
    public float waitTime = 2f;
    private IEnumerator Start()
    {
        // รอ 0.5 วิ (เอาไว้ให้เห็นหน้ากำลังโหลด)
        yield return new WaitForSeconds(waitTime);

        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneLoader.TargetScene);

        // รอจนโหลดเสร็จ
        while (!operation.isDone)
        {
            yield return null;
        }
    }
}