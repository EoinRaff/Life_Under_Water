using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageViewer : MonoBehaviour
{
    public MeasureDepth measureDepth;
    public MultiSourceManager multiSource;

    public RawImage rawImage;
    public RawImage rawDepth;

    void Update()
    {
        /// This throws an error but still works - look into it
        
        rawImage.texture = MultiSourceManager.Instance.GetColorTexture();
//        rawImage.texture = multiSource.GetColorTexture();

        rawDepth.texture = measureDepth.depthTexture;
    }
}
