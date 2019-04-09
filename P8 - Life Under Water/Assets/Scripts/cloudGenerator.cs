using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloudGenerator : MonoBehaviour
{
    public GameObject cloudPrefab;
    public float spawnDistance;
    public int spawnInterval;
    private int counter = 0;

    private List<GameObject> clouds = new List<GameObject>();
    public int minSpawnInterval;
    public int spawnSpeedIncrement;

    void Update()
    {
        if (counter % spawnInterval == 0)
        {
            Vector3 position = new Vector3(Random.Range(-75f, 75f), Random.Range(20, 50), spawnDistance);
            GameObject cloud = Instantiate(cloudPrefab, position, Random.rotation);
            cloud.GetComponent<moveTowardsCamera>().speed = Random.Range(5, 10);
            clouds.Add(cloud);
        }
        if (counter % spawnSpeedIncrement == 0)
        {
            spawnInterval--;
            if (spawnInterval <= minSpawnInterval)
            {
                spawnInterval = minSpawnInterval;
            }
        }

        foreach (GameObject cloud in clouds)
        {
            if (cloud == null)
            {
                continue;
            }
            if (cloud.transform.position.z <= -20)
            {
                ObjectCleaner.AddObjectToList(cloud);
                //toDestroy.Add(cloud);
            }
        }
        counter++;
    }
}
