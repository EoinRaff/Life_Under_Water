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
        rawImage.texture = multiSource.GetColorTexture();

        //rawDepth.texture = measureDepth.depthTexture;
    }
}
