using UnityEngine;


public class Sc4_MenuBtn : MonoBehaviour
{
    private bool alreadyClick = false;

    void Start()
    {
        alreadyClick = false;
    }
    public void goToMenu()
    {
        if (!alreadyClick)
        {
            alreadyClick = true;
            SceneTransition.Instance.ChangeScene(GameScenes.Menu);
        }
    }
}
