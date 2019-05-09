using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnTrash : MonoBehaviour
{
    // TODO: Make trash not spawn on top of each other
    // TODO: Instantiate with animations

    public GameObject Trash;
    public Texture[] trashTextures;
    public int maxTrash;

    GameObject currentTrash;
    float width, height, x, y, wait;
    int trashCount;

    [Range(0, 250)]
    public int edge;

    [Range(0.5f, 10f)]
    public float lowerSpawnSpan, upperSpawnSpan;

    // Start is called before the first frame update
    void OnEnable()
    {
        trashCount = 0;

        // Get dimensions of the canvas
        width = gameObject.GetComponent<RectTransform>().rect.width;
        height = gameObject.GetComponent<RectTransform>().rect.height;

        StartCoroutine(ProduceTrash());
    }

    // Where we spawn the trash
    IEnumerator ProduceTrash()
    {
        //float[,] trashLocations = new float[maxTrash, 2];

        while (trashCount <= maxTrash)
        {
            // Coordinates for where to spawn the next trash
            x = Random.Range(edge, width - edge);
            y = Random.Range(edge, height - edge);
            
            // For controlling trash coordinates by putting it in an array (currently unused) 
            //trashLocations[trashCount, 0] = x;
            //trashLocations[trashCount, 1] = y;

            // ---- Instantiate the trash
            int p = Random.Range(0, trashTextures.Length); //To access the randomly chosen index of the array

            currentTrash = Trash;

            currentTrash.GetComponent<RawImage>().texture = trashTextures[p]; // Give it a random texture
            currentTrash.GetComponent<RawImage>().color = new Color(255, 255, 255, 255); // Start with alpha 0 (for coroutine)

            DetermineSize();

            currentTrash = Instantiate(Trash, new Vector3(x, y, 0), Quaternion.Euler(new Vector3(0,0,Random.Range(0,360))), gameObject.transform) as GameObject;

            // Increase counter
            trashCount++;

            wait = Random.Range(lowerSpawnSpan, upperSpawnSpan);
            yield return new WaitForSeconds(wait);
        }

        // Set custom sizes for each piece of trash
        void DetermineSize()
        {
            string n = currentTrash.GetComponent<RawImage>().texture.name;

            switch (n)
            {
                case "group":
                    currentTrash.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                    print("Big trash spawned!");
                    break;
                case "bottle":
                    currentTrash.transform.localScale = new Vector3(.5f, .5f, .5f);
                    print("Bottle spawned!");
                    break;
                case "chips":
                    currentTrash.transform.localScale = new Vector3(1, 1, 1);
                    print("Chips spawned!");
                    break;
            }
        }
    }
}
