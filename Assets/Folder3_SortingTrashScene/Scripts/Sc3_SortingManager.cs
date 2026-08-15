using UnityEngine;
using System.Collections.Generic;
//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI; // 👈 เพิ่มบรรทัดนี้ครับ
using System.Collections;
using TMPro;
public class Sc3_SortingManager : MonoBehaviour
{
    public static Sc3_SortingManager Instance;
    public GameObject trashUiPrefab;
    public RectTransform spawnArea;
    public RectTransform onDragParent;

    public List<TrashData> trashDataBaseDummy;

    public Transform allSlots;

    public Sc3_SummarizeData summarizePaper;

    public float showSummarizeTime = 2f;

    public string NextSceneName;
    public TextMeshProUGUI buttonText;


    [HideInInspector]
    public float score;
    [HideInInspector]
    public int allTrashCount;
    private int trashCorrectCount;
    private int trashInCorrectCount;
    private Color defaultTextColor;
    private bool alreadyConfirm;
    void Awake()
    {
        if (Instance == null) Instance = this;

        if (InventoryManager.Instance == null)
        {
            allTrashCount = trashDataBaseDummy.Count;
        }
        else
        {
            allTrashCount = InventoryManager.Instance.items.Count;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }
    void Start()
    {
        alreadyConfirm = false;
        defaultTextColor = buttonText.color;
        SpawnTrashes();
    }

    void ShowScore()
    {
        summarizePaper.SetupAndShow("Player", score, allTrashCount, trashCorrectCount, trashInCorrectCount);
        EndGame();
    }

    void EndGame()
    {
        Sc3_GameManager.Instance.EndGame();
    }

    public void CheckTrashes()
    {
        if (!alreadyConfirm)
        {
            alreadyConfirm = true;

            int trashCount = 0;
            foreach (RectTransform rect in spawnArea)
            {
                trashCount += 1;
            }

            if (trashCount < 1)
            {
                StartCoroutine(CheckTrashesRoutine());
            }
            else
            {
                StartCoroutine(ThereAreStillTrashLeft());
            }
        }
    }

    private IEnumerator ThereAreStillTrashLeft()
    {
        buttonText.text = "There are trashes left";
        buttonText.color = Color.red;
        yield return new WaitForSeconds(2f);
        alreadyConfirm = false;
        buttonText.text = "Confirm";
        buttonText.color = defaultTextColor;
    }

    private IEnumerator CheckTrashesRoutine()
    {
        buttonText.text = "Confirmed!!";
        buttonText.color = Color.green;
        foreach (Transform slot in allSlots)
        {
            TrashType slotType = slot.GetComponent<Sc3_TrashSlot>().trashType;

            if (slot.childCount > 0)
            {
                foreach (Transform trash in slot)
                {
                    TrashType trashType = trash.GetComponent<Sc3_TrashUi>().data.trashType;

                    if (slotType == trashType)
                    {
                        trash.GetComponent<Image>().color = Color.green;
                        //Debug.Log(trash.name + " : " + slotType + " || Correct!!");
                        trashCorrectCount += 1;
                        score += trash.GetComponent<Sc3_TrashUi>().data.score;
                    }
                    else
                    {
                        trash.GetComponent<Image>().color = Color.red;
                        //Debug.Log(trash.name + " : " + slotType + " || InCorrect!!");
                        trashInCorrectCount += 1;
                    }

                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        Debug.Log("Success");
        yield return new WaitForSeconds(1f);
        ShowScore();
    }

    void SpawnTrashes()
    {
        RectTransform area = spawnArea.GetComponent<RectTransform>();
        if (InventoryManager.Instance != null && InventoryManager.Instance.items.Count > 0)
        {

            foreach (TrashData data in InventoryManager.Instance.items)
            {
                SpawnTrashAndAddData(data, area);
            }
        }
        else
        {

            foreach (TrashData data in trashDataBaseDummy)
            {
                SpawnTrashAndAddData(data, area);
            }
        }
    }

    void SpawnTrashAndAddData(TrashData data, RectTransform area)
    {
        float randomX = Random.Range(-area.rect.width / 2f, area.rect.width / 2f);
        float randomY = Random.Range(-area.rect.height / 2f, area.rect.height / 2f);

        GameObject obj = Instantiate(trashUiPrefab, spawnArea);
        obj.GetComponent<Sc3_TrashUi>().Setup(data);
        obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(randomX, randomY);
    }
}
