using UnityEngine;

public class Sc4_BackgroundMoving : MonoBehaviour
{
    public float speed;
    public Transform bgTransform;

    private bool uTurn = false;
    void Start()
    {
        bgTransform = gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log(bgTransform.position.x);

        if (bgTransform.position.x >= 50f && uTurn == false)
        {
            uTurn = true;
        }
        else if (bgTransform.position.x <= 2 && uTurn == true)
        {
            uTurn = false;
        }

        //if (!uTurn) bgTransform.position = new Vector3(bgTransform.position.x * Time.deltaTime * speed, bgTransform.position.y, bgTransform.position.z);
        //if (uTurn) bgTransform.position = new Vector3(bgTransform.position.x * Time.deltaTime * speed, bgTransform.position.y, bgTransform.position.z);

        if (!uTurn) transform.Translate(Vector3.right * speed * Time.deltaTime);
        if (uTurn) transform.Translate(Vector3.left * speed * Time.deltaTime);
    }
}
