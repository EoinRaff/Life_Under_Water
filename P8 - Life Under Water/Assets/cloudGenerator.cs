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


    void Update()
    {
        if (counter % spawnInterval == 0)
        {
            Vector3 position = new Vector3(Random.Range(-75f, 75f), Random.Range(10, 20), spawnDistance);
            clouds.Add(Instantiate(cloudPrefab, position, Quaternion.Euler(new Vector3(0, Random.value, 0))));
        }
        foreach (GameObject cloud in clouds)
        {
            if (cloud.transform.position.z <= 0)
            {
                clouds.Remove(cloud);
                Destroy(cloud);
            }
        }
        counter++;
    }
}
