using UnityEngine;

/// <summary>
/// ทำให้ Prefab ขยะ 2D ลอยขึ้นลง/ซ้ายขวา/หมุนเบา ๆ อยู่ตลอดเวลา
/// โดยไม่ทำให้ลอยทะลุออกนอกขอบของ SpawnArea (RectTransform)
///
/// วิธีใช้งานจาก Script อื่น (เช่น Sc3_TrashDragHandler):
///   floatAnim.StopFloat();          // หยุดชั่วคราว (เช่นตอน BeginDrag)
///   floatAnim.StartFloat();         // เริ่ม/กลับมาลอยต่อ (เช่นตอนเด้งกลับ SpawnArea)
///   floatAnim.LockFloat();          // หยุดถาวร (เช่นตอนวางลง Slot สำเร็จ)
///   floatAnim.EnableFloating(bool); // เรียกรวม true = StartFloat, false = StopFloat
///
/// สคริปต์นี้ "ไม่" ยุ่งกับ anchoredPosition ระหว่างที่ isFloating == false
/// ดังนั้นจะไม่ชนกับ Drag Script ตราบใดที่ Drag Script เรียก StopFloat()
/// ใน OnBeginDrag ก่อนเริ่มลาก
/// </summary>
[DisallowMultipleComponent]
public class Sc3_TrashFloatAnimation : MonoBehaviour
{
    [Header("Float Movement")]
    [Tooltip("ความเร็วโดยรวมของการลอย (ยิ่งมากยิ่งลอยเร็ว)")]
    public float floatSpeed = 1f;

    [Tooltip("ระยะลอยซ้าย-ขวา สูงสุด (หน่วยเดียวกับ anchoredPosition)")]
    public float floatDistanceX = 10f;

    [Tooltip("ระยะลอยขึ้น-ลง สูงสุด (หน่วยเดียวกับ anchoredPosition)")]
    public float floatDistanceY = 15f;

    [Header("Rotation")]
    [Tooltip("เปิด/ปิดการหมุนเบา ๆ ระหว่างลอย")]
    public bool enableRotation = true;

    [Tooltip("มุมหมุนสูงสุด (องศา) จากจุดปกติ")]
    public float rotationAmount = 5f;

    [Tooltip("ความเร็วในการหมุน")]
    public float rotationSpeed = 1f;

    [Header("Randomization (ให้แต่ละชิ้นลอยไม่พร้อมกัน)")]
    [Tooltip("ช่วงสุ่ม phase เริ่มต้น (0 - 6.28 คือหนึ่งรอบเต็มของ sine wave)")]
    public float randomOffsetRange = 6.2832f;

    [Tooltip("สุ่มความเร็วแกน Y ให้ต่างจากแกน X เล็กน้อย เพื่อไม่ให้ลอยเป็นเส้นตรงทแยง")]
    public Vector2 randomYFrequencyRange = new Vector2(1.2f, 1.8f);

    [Header("Behaviour")]
    [Tooltip("เริ่มลอยอัตโนมัติเมื่อ Object ถูก Spawn/Enable")]
    public bool autoStartOnEnable = true;

    [Tooltip("ถ้าเปิดไว้ เมื่อ StopFloat() จะรีเซ็ตการหมุนกลับเป็น 0 (เหมาะตอนเริ่มลาก)")]
    public bool resetRotationOnStop = true;

    [Header("SpawnArea Override (ไม่ใส่ก็ได้ จะไปดึงจาก Sc3_SortingManager อัตโนมัติ)")]
    [Tooltip("ถ้าไม่ได้ตั้งค่า จะใช้ Sc3_SortingManager.Instance.spawnArea แทน")]
    public RectTransform spawnAreaOverride;

    // --- runtime state ---
    private RectTransform rect;
    private RectTransform spawnAreaRect;

    private bool isFloating;
    private bool isLocked; // true = หยุดถาวร ห้าม StartFloat() อีก

    private Vector2 basePosition;   // ตำแหน่งศูนย์กลางที่จะแกว่งรอบ ๆ
    private float startTime;

    private float phaseX;
    private float phaseY;
    private float phaseRot;
    private float freqYMultiplier;

    public bool IsFloating => isFloating;
    public bool IsLocked => isLocked;

    void Awake()
    {
        rect = GetComponent<RectTransform>();

        // สุ่ม phase/ความถี่ ต่อชิ้น เพื่อให้ขยะแต่ละชิ้นลอยไม่พร้อมกัน
        phaseX = Random.Range(0f, randomOffsetRange);
        phaseY = Random.Range(0f, randomOffsetRange);
        phaseRot = Random.Range(0f, randomOffsetRange);
        freqYMultiplier = Random.Range(randomYFrequencyRange.x, randomYFrequencyRange.y);
    }

    void Start()
    {
        CacheSpawnArea();

        if (autoStartOnEnable)
        {
            StartFloat();
        }
    }

    void CacheSpawnArea()
    {
        if (spawnAreaOverride != null)
        {
            spawnAreaRect = spawnAreaOverride;
            return;
        }

        if (Sc3_SortingManager.Instance != null && Sc3_SortingManager.Instance.spawnArea != null)
        {
            spawnAreaRect = Sc3_SortingManager.Instance.spawnArea;
            return;
        }

        // fallback สุดท้าย: ใช้ parent ปัจจุบัน (ตอน spawn ปกติ parent คือ spawnArea อยู่แล้ว)
        spawnAreaRect = transform.parent as RectTransform;
    }

    /// <summary>
    /// เรียกรวมสำหรับเปิด/ปิดการลอย
    /// </summary>
    public void EnableFloating(bool enable)
    {
        if (enable) StartFloat();
        else StopFloat();
    }

    /// <summary>
    /// เริ่ม/กลับมาลอยต่อ โดยจะจับตำแหน่งปัจจุบันเป็นจุดศูนย์กลางการแกว่งใหม่
    /// เรียกตอน Spawn เสร็จ หรือหลังเด้งกลับ SpawnArea สำเร็จ
    /// </summary>
    public void StartFloat()
    {
        if (isLocked) return;

        // เผื่อกรณี spawnArea ถูกตั้งค่าใน Inspector ทีหลัง หรือยังไม่เคย cache
        if (spawnAreaRect == null) CacheSpawnArea();

        basePosition = rect.anchoredPosition;
        startTime = Time.time;
        isFloating = true;
    }

    /// <summary>
    /// หยุดลอยชั่วคราว (ยังเรียก StartFloat() กลับมาใหม่ได้)
    /// เรียกตอน OnBeginDrag
    /// </summary>
    public void StopFloat()
    {
        isFloating = false;

        if (resetRotationOnStop)
        {
            Vector3 e = rect.localEulerAngles;
            e.z = 0f;
            rect.localEulerAngles = e;
        }
    }

    /// <summary>
    /// หยุดลอยถาวร ห้ามเริ่มใหม่อีก (จนกว่าจะเรียก Unlock())
    /// เรียกตอนวางลง Slot สำเร็จ
    /// </summary>
    public void LockFloat()
    {
        StopFloat();
        isLocked = true;
    }

    /// <summary>
    /// ปลดล็อกให้กลับมาลอยได้อีกครั้ง (เผื่อกรณีต้องดึงขยะออกจาก Slot กลับมา)
    /// </summary>
    public void UnlockFloat()
    {
        isLocked = false;
    }

    void LateUpdate()
    {
        if (!isFloating || isLocked || rect == null) return;

        float t = Time.time - startTime;

        float offsetX = Mathf.Sin(t * floatSpeed + phaseX) * floatDistanceX;
        float offsetY = Mathf.Sin(t * floatSpeed * freqYMultiplier + phaseY) * floatDistanceY;

        Vector2 targetPos = basePosition + new Vector2(offsetX, offsetY);
        ClampToSpawnArea(ref targetPos);

        rect.anchoredPosition = targetPos;

        if (enableRotation)
        {
            float rotZ = Mathf.Sin(t * rotationSpeed + phaseRot) * rotationAmount;
            Vector3 euler = rect.localEulerAngles;
            euler.z = rotZ;
            rect.localEulerAngles = euler;
        }
    }

    /// <summary>
    /// จำกัดตำแหน่งไม่ให้ลอยทะลุขอบ SpawnArea
    /// ใช้ convention เดียวกับ Sc3_SortingManager.SpawnTrashAndAddData
    /// คือ anchoredPosition (0,0) = จุดกึ่งกลาง SpawnArea, ขอบอยู่ที่ ±width/2, ±height/2
    /// </summary>
    void ClampToSpawnArea(ref Vector2 pos)
    {
        if (spawnAreaRect == null) return;

        float areaHalfW = spawnAreaRect.rect.width / 2f;
        float areaHalfH = spawnAreaRect.rect.height / 2f;

        float objHalfW = rect.rect.width / 2f;
        float objHalfH = rect.rect.height / 2f;

        float maxX = Mathf.Max(0f, areaHalfW - objHalfW);
        float maxY = Mathf.Max(0f, areaHalfH - objHalfH);

        pos.x = Mathf.Clamp(pos.x, -maxX, maxX);
        pos.y = Mathf.Clamp(pos.y, -maxY, maxY);
    }
}