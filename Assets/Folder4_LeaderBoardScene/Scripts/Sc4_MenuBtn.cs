using UnityEngine;


public class Sc4_MenuBtn : MonoBehaviour
{
    public void goToMenu()
    {
        SceneTransition.Instance.ChangeScene(GameScenes.Menu);
    }
}
