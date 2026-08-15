using UnityEngine;
using System.Collections.Generic;

public class Sc2_TrashSpawner : MonoBehaviour
{
    [Header("ข้อมูลตัวขยะ")]
    public List<TrashData> trashDatabase;
    [Header("การตั้งค่าอื่นๆ")]
    public GameObject trashPrefab;
    public Transform spawnPoints;
    public Transform trashParent;

    [Header("ตำแหน่งการเกิดบน SpawnPoint")]
    public float spawnYOffset = 0.5f;

    // PRIVATE CODE SPACE  ---------------------------------------------------------------------------------------------

    private int totalRareRate = 0;
    private int CombineAllRareRate = 0;
    private int randomRareRate = 0;

    void Start()
    {
        BoxCollider box = trashPrefab.GetComponent<BoxCollider>();
        SpawnRandomTrashes();
    }

    public void SpawnRandomTrashes()
    {
        CombineAllRareRate = 0;
        foreach (TrashData data in trashDatabase)
        {
            CombineAllRareRate += data.rarity;
        }

        foreach (Transform point in spawnPoints)
        {
            //Debug.Log("มารอยบที่ : " + point.gameObject.name + " ----------------------------------------------------- ");
            randomRareRate = Random.Range(0, CombineAllRareRate);
            totalRareRate = 0;

            foreach (TrashData data in trashDatabase)
            {
                //Debug.Log("ตอนนี้เช็คขยะ : " + data.name + " || ค้าสุ่ม = " + randomRareRate + " || ค้่่าทั้งหมด " + CombineAllRareRate);
                if (totalRareRate <= randomRareRate && randomRareRate < (data.rarity + totalRareRate))
                {
                    //spawnYOffset = box.size.y / 2f;  /// เผื่อขยะมันขนาดไม่เท่ากัน
                    int randomNullRate = Random.Range(0, 2);
                    if (randomNullRate == 1)
                    {
                        continue;
                    }

                    Vector3 spawnPos = point.position + Vector3.up * spawnYOffset;

                    GameObject newTrash = Instantiate(
                        trashPrefab,
                        spawnPos,
                        Quaternion.identity,
                        trashParent
                    );

                    newTrash.GetComponent<Sc2_TrashObject>().Setup(data);
                    //Debug.Log("ตอนนี้ได้ขยะ : " + data.name + " ที่ " + totalRareRate + " : " + randomRareRate + " : " + (data.rarity + totalRareRate));
                    break;
                }
                else
                {
                    totalRareRate += data.rarity;
                }
            }
        }


    }
}
