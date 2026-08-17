using Unity.VisualScripting;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public CharacterController controller;
    [Header("การตั้งค่าผู้เล่น")]
    public float speed = 5f;          // ความเร็วปกติ
    public float sprintSpeed = 8.75f;       // ความเร็วสูงสุดตอนกด Shift (5 * 1.75)
    public float acceleration = 8f;
    float currentSpeed;


    public float gravity = -9.81f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public bool freezePlayer = false;
    [Header("")]
    public float speedAnim = 2f;
    public float currentSpeedAnim;
    Vector3 velocity;

    // PRIVATE SPACE  ---------------------------------
    private float playerSpeed;
    private bool foundTrash = false;
    private GameObject trash;
    private Animator anim;
    private bool runAnimation;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        playerSpeed = speed;
        currentSpeedAnim = anim.speed;
    }

    void Update()
    {
        if (freezePlayer)
        {
            speed = 0f;
            anim.SetFloat("Speed", 0f);
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, sprintSpeed, acceleration * Time.deltaTime);
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, playerSpeed, acceleration * Time.deltaTime);
            }
            speed = currentSpeed;
            anim.speed = currentSpeed / playerSpeed * 1.15f;
        }

        if (!controller.enabled) return;


        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (anim != null && !freezePlayer)
        {
            anim.SetFloat("Speed", direction.magnitude);
        }



        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            if (!freezePlayer)
            {
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        //--------------------------------------------------------------------------------------------------------------------------------------

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (foundTrash && trash != null)
            {
                foundTrash = false;
                Sc2_CuteTextBounce.Instance.PlayBounceOut();
                trash.GetComponent<Sc2_TrashObject>().Collect();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trash"))
        {
            foundTrash = true;
            Sc2_CuteTextBounce.Instance.PlayBounce();
            trash = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Trash"))
        {
            foundTrash = false;
            Sc2_CuteTextBounce.Instance.PlayBounceOut();
            trash = null;
        }
    }
}