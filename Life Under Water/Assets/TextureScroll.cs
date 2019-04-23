using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureScroll : MonoBehaviour
{
    public float scrollSpeed;
    public Vector2 direction;
    private Renderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float offset = Time.time * scrollSpeed;
        renderer.material.SetTextureOffset("_MainTex", direction * offset);
        renderer.material.SetTextureOffset("_BumpMap", direction * offset);
        renderer.material.SetTextureOffset("_SpecMap", direction * offset);
    }
}
