using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashFactory : MonoBehaviour
{
    public GameObject trashPrefab;
    public float spawnDistance;
    public int spawnInterval;
    private int counter = 0;

    private List<GameObject> trashPiles = new List<GameObject>();
    public int minSpawnInterval;
    public int spawnSpeedIncrement;

    void Update()
    {
        if (counter % spawnInterval == 0)
        {
            Vector3 position = new Vector3(Random.Range(-25f, 25f), 0, spawnDistance);
            trashPiles.Add(Instantiate(trashPrefab, position, Quaternion.identity));
        }
        if (counter % spawnSpeedIncrement == 0)
        {
            spawnInterval--;
            if (spawnInterval <= minSpawnInterval)
            {
                spawnInterval = minSpawnInterval;
            }
        }

        foreach (GameObject trashPile in trashPiles)
        {
            if (trashPile == null)
            {
                continue;
            }
            if (trashPile.transform.position.z <= 0)
            {
                ObjectCleaner.AddObjectToList(trashPile);
            }
        }
    }

    private void LateUpdate()
    {
        counter++;
    }
}
