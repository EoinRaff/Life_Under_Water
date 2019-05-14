using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrashFactory : MonoBehaviour
{
    private enum Lane { Left, Center, Right};
    public GameObject trashPrefab;
    public float spawnDistance;
    public int spawnInterval;
    private float counter = 0;

    private List<GameObject> trashPiles = new List<GameObject>();
    public int minSpawnInterval;
    public int spawnSpeedIncrement;
    private bool spawned;

    void Update()
    {
        if ((int)counter % spawnInterval == 0)
        {
            if (spawned)
                return;

            spawned = true;
            int i = Mathf.FloorToInt(Random.Range(0, 2.9f));
            Lane lane = (Lane)Enum.ToObject(typeof(Lane), i);
            float x;

            switch (lane)
            {
                case Lane.Left:
                    x = -5f;
                    break;
                case Lane.Center:
                    x = 0;
                    break;
                case Lane.Right:
                    x = 5f;
                    break;
                default:
                    x = 0;
                    break;
            }

            Vector3 position = new Vector3(x, 0, spawnDistance);
            trashPiles.Add(Instantiate(trashPrefab, position, Quaternion.identity));
        }
        else
        {
            spawned = false;
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
        counter += Time.deltaTime;
    }
}
