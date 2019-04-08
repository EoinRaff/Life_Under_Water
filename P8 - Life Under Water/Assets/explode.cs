using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class explode : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        Vector3 direction = new Vector3(Random.Range(-0.5f, 0.5f), 1f, -1).normalized;

        float magnitude = Random.Range(2, 3);
        rb.AddForce(direction * magnitude, ForceMode.Impulse);
    }

    private void Update()
    {
        if (transform.position.y <= 0)
        {
            Destroy(gameObject);
        }
    }
}
