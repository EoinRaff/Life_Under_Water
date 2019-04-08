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


    void Update()
    {
        if (counter % spawnInterval == 0)
        {
            Vector3 position = new Vector3(Random.Range(-75f, 75f), 0, spawnDistance);
            trashPiles.Add(Instantiate(trashPrefab, position, Quaternion.identity));
        }
        foreach (GameObject trashPile in trashPiles)
        {
            if (trashPile.transform.position.z <= 0)
            {
                trashPiles.Remove(trashPile);
                Destroy(trashPile);
            }
        }
        counter++;
    }
}
