using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance;

    [Header("Target References")]
    public Transform target;
    public Transform povAnchor;

    [Header("State Control")]
    public bool isPOVMode = false;
    public bool canRotate = true;
    public bool isSubtleMouseMode = false;

    [Header("Follow Settings")]
    public float mouseSensitivity = 200f;
    public Vector2 pitchMinMax = new Vector2(-10, 75);

    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float minDistance = 0.5f;
    public float maxDistance = 10f;
    public float smoothSpeed = 15f;

    [Header("POV Transition")]
    public float transitionSpeed = 5f;

    [Header("POV Subtle Mouse (New)")]
    public float povMouseInfluence = 2.0f;
    public float povMouseSmooth = 5.0f;

    [Header("Collision Settings")]
    public LayerMask collisionLayers;
    public float cameraRadius = 0.25f;
    public float collisionOffset = 0.2f;

    float yaw;
    float pitch;
    float currentDistance;
    float targetDistance;
    bool lastState;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        SetCameraActive(true);

        currentDistance = 5f;
        targetDistance = currentDistance;

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
        lastState = isPOVMode;


    }

    public void SetCameraActive(bool isActive)
    {
        this.enabled = isActive;
        if (isActive)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void SetPOV(bool active)
    {
        isPOVMode = active;
    }


    void LateUpdate()
    {

        if (target == null) return;

        if (isPOVMode != lastState)
        {
            OnStateChanged(isPOVMode);
            lastState = isPOVMode;
        }

        if (!isPOVMode)
        {
            HandleThirdPerson();
        }
        else
        {
            HandlePOVTransition();

            if (isSubtleMouseMode)
            {
                HandlePOVSubtleMouse();
            }
        }
    }

    private void OnStateChanged(bool enteringPOV)
    {
        if (!enteringPOV)
        {
            Vector3 angles = transform.localEulerAngles;
            yaw = angles.y;
            pitch = angles.x;
            targetDistance = 3f;
        }
    }

    void HandleThirdPerson()
    {
        if (canRotate)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            targetDistance -= scroll * zoomSpeed;
            targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 focusPosition = target.position + Vector3.up * 1.5f;
        Vector3 desiredPosition = focusPosition - (rotation * Vector3.forward * targetDistance);

        RaycastHit hit;
        float finalDistance = targetDistance;

        if (Physics.SphereCast(focusPosition, cameraRadius, (desiredPosition - focusPosition).normalized, out hit, targetDistance, collisionLayers))
        {
            finalDistance = Mathf.Clamp(hit.distance - collisionOffset, minDistance, targetDistance);
            if (currentDistance > finalDistance)
            {
                currentDistance = finalDistance;
            }
        }

        currentDistance = Mathf.Lerp(currentDistance, finalDistance, Time.deltaTime * smoothSpeed);

        transform.rotation = rotation;
        transform.position = focusPosition - (rotation * Vector3.forward * currentDistance);
    }

    void HandlePOVTransition()
    {
        if (povAnchor == null) return;
        transform.position = Vector3.Lerp(transform.position, povAnchor.position, Time.deltaTime * transitionSpeed);

        if (!isSubtleMouseMode)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, povAnchor.rotation, Time.deltaTime * transitionSpeed);
        }
    }

    void HandlePOVSubtleMouse()
    {
        if (povAnchor == null) return;

        float mouseX = (Input.mousePosition.x / Screen.width) - 0.5f;
        float mouseY = (Input.mousePosition.y / Screen.height) - 0.5f;

        Quaternion mouseOffset = Quaternion.Euler(-mouseY * povMouseInfluence, mouseX * povMouseInfluence, 0);
        Quaternion targetRotation = povAnchor.rotation * mouseOffset;

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * povMouseSmooth);
    }
}