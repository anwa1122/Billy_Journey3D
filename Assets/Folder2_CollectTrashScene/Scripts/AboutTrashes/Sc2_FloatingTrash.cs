using UnityEngine;

public class Sc2_FloatingTrash : MonoBehaviour
{
    [Header("การลอยขึ้น-ลง (Bobbing)")]
    [Tooltip("ความเร็วในการลอยขึ้นลง (0 = ไม่ลอย)")]
    [Range(0f, 5f)]
    public float floatSpeed = 1f;

    [Tooltip("ระยะทางที่ลอยขึ้นลงจากจุดเริ่มต้น (หน่วย Unity)")]
    [Range(0f, 3f)]
    public float floatAmplitude = 0.5f;

    [Header("การหมุน")]
    [Tooltip("ความเร็วในการหมุน (0 = ไม่หมุน)")]
    [Range(0f, 100f)]
    public float rotateSpeed = 30f;

    [Tooltip("สุ่มทิศทางหมุนเมื่อเริ่ม (true = สุ่ม, false = ใช้ค่า isRotateRight)")]
    public bool randomizeDirection = true;

    [Tooltip("ถ้าไม่สุ่ม จะหมุนขวาไหม? (true = ขวา, false = ซ้าย)")]
    public bool isRotateRight = true;

    [Header("การสุ่มตอนเริ่ม")]
    [Tooltip("สุ่มความเร็วหมุนตอนเริ่มไหม?")]
    public bool randomizeRotateSpeed = true;
    public float minRotateSpeed = 10f;
    public float maxRotateSpeed = 50f;

    [Tooltip("สุ่มระยะลอยตอนเริ่มไหม?")]
    public bool randomizeFloatAmplitude = true;
    public float minFloatAmplitude = 0.2f;
    public float maxFloatAmplitude = 0.8f;

    [Tooltip("สุ่มความเร็วลอยตอนเริ่มไหม?")]
    public bool randomizeFloatSpeed = true;
    public float minFloatSpeed = 0.5f;
    public float maxFloatSpeed = 1.5f;

    // ตัวแปรภายใน
    private Vector3 startPosition;
    private float randomOffset;          // ทำให้แต่ละอันลอยไม่พร้อมกัน
    private int rotateDirection = 1;     // 1 = ขวา, -1 = ซ้าย

    void Start()
    {
        startPosition = transform.position;
        randomOffset = Random.Range(0f, 100f); // offset เพื่อไม่ให้ลอยพร้อมกัน

        // สุ่มทิศทางหมุน
        if (randomizeDirection)
        {
            rotateDirection = Random.value > 0.5f ? 1 : -1;
        }
        else
        {
            rotateDirection = isRotateRight ? 1 : -1;
        }

        // สุ่มความเร็วหมุน
        if (randomizeRotateSpeed)
        {
            rotateSpeed = Random.Range(minRotateSpeed, maxRotateSpeed);
        }

        // สุ่มระยะลอย
        if (randomizeFloatAmplitude)
        {
            floatAmplitude = Random.Range(minFloatAmplitude, maxFloatAmplitude);
        }

        // สุ่มความเร็วลอย
        if (randomizeFloatSpeed)
        {
            floatSpeed = Random.Range(minFloatSpeed, maxFloatSpeed);
        }
    }

    void Update()
    {
        // === ลอยขึ้น-ลง ===
        if (floatSpeed > 0f && floatAmplitude > 0f)
        {
            float newY = startPosition.y + Mathf.Sin((Time.time + randomOffset) * floatSpeed) * floatAmplitude;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }

        // === หมุน ===
        if (rotateSpeed > 0f)
        {
            transform.Rotate(Vector3.up, rotateSpeed * rotateDirection * Time.deltaTime, Space.World);
            // ถ้าอยากหมุนแกนอื่น เปลี่ยน Vector3.up เป็น Vector3.right หรือ Vector3.forward ได้
        }
    }
}