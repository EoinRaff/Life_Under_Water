using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steer : MonoBehaviour
{
    public Transform centerOfMass;
    public GameObject trashExplosion;

    void Update()
    {
        if (GameManager.Instance.Interactive)
        {
            Vector3 position = new Vector3();
            position.x = Mathf.Lerp(transform.position.x, centerOfMass.position.x, Time.deltaTime);
            position.x = Mathf.Clamp(position.x, -2, 2);
            position.z = transform.position.z;
            position.y = 0;
            gameObject.transform.position = position;
        }
        else
        {
            // ANIMATE RAFT
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "trash")
        {
            for (int i = 0; i < 20; i++)
            {
                Instantiate(trashExplosion, other.transform.position, Random.rotation);
            }
            GameManager.Instance.GetComponent<AudioSource>().Play();
            Destroy(other.gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "trash")
        {
            print("TRASH");
            collision.gameObject.GetComponent<AudioSource>().Play();
        }
    }
}
