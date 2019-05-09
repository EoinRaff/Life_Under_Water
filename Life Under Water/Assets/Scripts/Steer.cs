using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steer : MonoBehaviour
{
    public Transform centerOfMass;
    public GameObject trashExplosion;

    // Update is called once per frame
    void Update()
    {
        //gameObject.transform.Rotate(centerOfMass.localPosition);
        Vector3 position = new Vector3();
        position.x = Mathf.Lerp(transform.position.x, centerOfMass.position.x, Time.deltaTime);
        position.x = Mathf.Clamp(position.x, -2, 2);
        position.z = transform.position.z; // -centerOfMass.position.x;
        position.y = 0;
        gameObject.transform.position = position;// = Quaternion.Euler(position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "trash")
        {
            for (int i = 0; i < 20; i++)
            {
                Instantiate(trashExplosion, other.transform.position, Random.rotation);
            }
            Destroy(other.gameObject);
            AnimationCTRL.Instance.PlayReaction();
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
