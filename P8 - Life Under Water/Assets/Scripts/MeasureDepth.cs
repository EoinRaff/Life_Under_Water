using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class MeasureDepth : MonoBehaviour
{
    public delegate void NewTriggerPoints(List<Vector2> triggerPoints);
    public static event NewTriggerPoints OnTriggerPoints = null;

    public MultiSourceManager multiSource;
    public Texture2D depthTexture;

    // Cutoffs
    [Range(0, 1.0f)]
    public float depthSensitivity = 1.0f;

    [Range(-10.0f, 10.0f)]
    public float wallDepth = -10.0f;

    [Header("Top and Bottom")]
    [Range(-1.0f, 1.0f)]
    public float topCutOff = 1.0f;
    [Range(-1.0f, 1.0f)]
    public float bottomCutOff = -1.0f;

    [Header("Left and Right")]
    [Range(-1.0f, 1.0f)]
    public float leftCutOff = -1.0f;
    [Range(-1.0f, 1.0f)]
    public float rightCutOff = 1.0f;

    // Depth Data
    [Header("")]
    [SerializeField]
    private int downSampleFactor = 1;

    private ushort[] depthData = null;
    private CameraSpacePoint[] cameraSpacePoints = null;
    private ColorSpacePoint[] colorSpacePoints = null;
    private List<ValidPoint> validPoints = null;
    private List<Vector2> triggerPoints = null;

    // Kinect
    private KinectSensor sensor = null;
    private CoordinateMapper mapper = null;
    private Camera mainCamera = null;

    // Blob Analysis
    private int[] labelMap = null;
    private static readonly float[,] kernel = {{0,0,1,0,0},
                                               {0,1,1,1,0},
                                               {1,1,1,1,1},
                                               {0,1,1,1,0},
                                               {0,0,1,0,0}};
    private static int kernelSize;
    private static int borderSize;
    private Color[] colors = { Color.red, Color.green, Color.blue };

    private readonly Vector2Int depthResolution = new Vector2Int(512, 424);
    private Rect rect;

    float startTime, elapsedTime;
    float interval;

    private void Awake()
    {
        print("Awake");
        startTime = Time.time;
        interval = 2f;

        kernelSize = kernel.GetLength(0);
        borderSize = kernelSize / 2;
        sensor = KinectSensor.GetDefault();
        mapper = sensor.CoordinateMapper;
        mainCamera = Camera.main;

        int arraySize = depthResolution.x * depthResolution.y;

        cameraSpacePoints = new CameraSpacePoint[arraySize];
        colorSpacePoints = new ColorSpacePoint[arraySize];

        labelMap = new int[1920 * 1080];

    }

    private void Update()
    {

        validPoints = DepthToColor();

        triggerPoints = FilterToTrigger(validPoints);

        if (OnTriggerPoints != null && triggerPoints.Count != 0)
        {
            OnTriggerPoints(triggerPoints);
        }

        elapsedTime = Time.time - startTime;

        if (Input.GetKeyDown(KeyCode.Space)|| elapsedTime % interval < 1)
        {
            print("Update Texture");
            rect = CreatRect(validPoints);

            depthTexture = CreateTexture(triggerPoints);
        }
        //Debugging
        //rect = CreatRect(validPoints);

        //depthTexture = CreateTexture(triggerPoints);
        //MedianFilter(depthTexture);
    }

    private void OnGUI()
    {
        //GUI.Box(rect, "");

        if (triggerPoints == null)
            return;
        /*
        foreach (Vector2 point in triggerPoints)
        {
            Rect rect = new Rect(point, new Vector2(10, 10));
            GUI.Box(rect, "");
        }
        */
    }

    private List<ValidPoint> DepthToColor()
    {
        List<ValidPoint> validPoints = new List<ValidPoint>();

        // Get Depth Data From Kinect
        depthData = multiSource.GetDepthData();

        //Map Depth Data to Camera & Color Space
        mapper.MapDepthFrameToCameraSpace(depthData, cameraSpacePoints);
        mapper.MapDepthFrameToColorSpace(depthData, colorSpacePoints);

        //Filter
        for (int i = 0; i < depthResolution.x / downSampleFactor; i++)
        {
            for (int j = 0; j < depthResolution.y / downSampleFactor; j++)
            {
                //Down Sampling
                int sampleIndex = j * depthResolution.x + i;
                sampleIndex *= downSampleFactor; 
                
                // Cutoff Tests
                if (cameraSpacePoints[sampleIndex].X < leftCutOff)
                    continue;

                if (cameraSpacePoints[sampleIndex].X > rightCutOff)
                    continue;

                if (cameraSpacePoints[sampleIndex].Y < bottomCutOff)
                    continue;

                if (cameraSpacePoints[sampleIndex].Y > topCutOff)
                    continue;

                // Create Valid Point
                ValidPoint newPoint = new ValidPoint(colorSpacePoints[sampleIndex], cameraSpacePoints[sampleIndex].Z);

                // Depth Test
                if (cameraSpacePoints[sampleIndex].Z >= wallDepth)
                {
                    newPoint.withinWallDepth = true;
                }

                // Add to List
                validPoints.Add(newPoint);
            }
        }

        return validPoints;
    }

    private List<Vector2> FilterToTrigger(List<ValidPoint> validPoints)
    {
        List<Vector2> triggerPoints = new List<Vector2>();

        foreach (ValidPoint point in validPoints)
        {
            if (!point.withinWallDepth)
            {
                if (point.z < wallDepth * depthSensitivity)
                {
                    Vector2 screenPoint = ScreenToCamera(new Vector2(point.colorSpace.X, point.colorSpace.Y));
                    triggerPoints.Add(screenPoint);
                }
            }
        }
        return triggerPoints;
    }

    private Texture2D CreateTexture(List<ValidPoint> validPoints)
    {
        Texture2D newTexture = new Texture2D(1920, 1080, TextureFormat.Alpha8, false);

        for (int x = 0; x < 1920; x++)
        {
            for (int y = 0; y < 1080; y++)
            {
                newTexture.SetPixel(x, y, Color.black);
            }
        }

        foreach(ValidPoint point in validPoints)
        {
            newTexture.SetPixel((int)point.colorSpace.X, (int)point.colorSpace.Y, Color.white);
        }

        newTexture.Apply();

        return newTexture;
    }

    private Texture2D CreateTexture(List<Vector2> validPoints)
    {
        Texture2D newTexture = new Texture2D(1920, 1080);

        for (int x = 0; x < 1920; x++)
        {
            for (int y = 0; y < 1080; y++)
            {
                newTexture.SetPixel(x, y, Color.black);
            }
        }

        foreach (Vector2 point in validPoints)
        {
            newTexture.SetPixel((int)point.x, (int)point.y, Color.white);
        }

        Color[] pixels = newTexture.GetPixels();

        pixels = Dilate(pixels, newTexture.width, newTexture.height);
        pixels = Erode(pixels, newTexture.width, newTexture.height);

        //Label(pixels);

        for (int x = 0; x < 1920; x++)
        {
            for (int y = 0; y < 1080; y++)
            {
                //pixels[y * 1920 + x] = colors[labelMap[y * 1920 + x]];
            }
        }

        newTexture.SetPixels(pixels);
        newTexture.Apply();

        return newTexture;
    }

    private void MedianFilter(Texture2D tex)
    {
        Color[] pix = tex.GetPixels();
        Color[] pix2 = tex.GetPixels();

        for (int y = 1; y < tex.height - 1; y++)
        {
            for (int x = 1; x < tex.width - 1; x++)
            {
                float sum = 0;
                for (int ky = 0; ky <= 4; ky++)
                {
                    for (int kx = 0; kx <= 4; kx++)
                    {
                        sum += pix[(y + ky - 1) * tex.width + (x + kx - 1)].r;
                    }
                }
                if (sum >= 5)
                {
                    pix2[y * tex.width + x] = Color.white;

                }
                else
                {
                    pix2[y * tex.width + x] = Color.black;

                }

            }
        }

        tex.SetPixels(pix2);
        tex.Apply();
    }

    private Color[] Dilate(Color[] pix, int width, int height)
    {
        Color[] newPixels = new Color[width * height];
        for (int y = borderSize; y < height - borderSize; y++)
        {
            for (int x = borderSize; x < width - borderSize; x++)
            {
                float sum = 0;
                for (int ky = 0; ky <= kernelSize-1; ky++)
                {
                    for (int kx = 0; kx <= kernelSize - 1; kx++)
                    {
                        sum += pix[(y + ky - borderSize) * width + (x + kx - borderSize)].r * kernel[ky, kx];
                    }
                }
                if (sum >= 1)
                {
                    newPixels[y * width + x] = Color.white;
                }
                else
                { 
                    newPixels[y * width + x] = Color.black;
                }
            }
        }
        return newPixels;
    }
    private Color[] Erode(Color[] pix, int width, int height)
    {
        Color[] newPixels = new Color[width * height];

        for (int y = borderSize; y < height - borderSize; y++)
        {
            for (int x = borderSize; x < width - borderSize; x++)
            {
                float sum = 0;
                float ksum = 0;
                for (int ky = 0; ky <= kernelSize - 1; ky++)
                {
                    for (int kx = 0; kx <= kernelSize-1; kx++)
                    {
                        sum += pix[(y + ky - borderSize) * width + (x + kx - borderSize)].r * kernel[ky, kx];
                        ksum += kernel[ky, kx];
                    }
                }
                if (sum >= ksum)
                {
                    newPixels[y * width + x] = Color.white;
                }
                else
                {
                    newPixels[y * width + x] = Color.black;
                }
            }
        }
        return newPixels;
    }

    #region Rect Creation
    private Rect CreatRect(List<ValidPoint> points)
    {
        if(points.Count == 0)
            return new Rect();

        // Get Corners of Rect
        Vector2 topLeft = GetTopLeft(points);
        Vector2 bottomRight = GetBottomRight(points);

        // Translate to Viewport
        Vector2 screenTopLeft = ScreenToCamera(topLeft);
        Vector2 screenBottomRight = ScreenToCamera(bottomRight);

        // Rect Dimensions
        int width = (int)(screenBottomRight.x - screenTopLeft.x);
        int height = (int)(screenBottomRight.y - screenTopLeft.y);

        // Create Rect
        Vector2 size = new Vector2(width, height);
        Rect rect = new Rect(screenTopLeft, size);

        return rect;


    }

    private Vector2 GetTopLeft(List<ValidPoint> points)
    {
        Vector2 topLeft = new Vector2(int.MaxValue, int.MaxValue);

        foreach (ValidPoint point in points)
        {
            if (point.colorSpace.X < topLeft.x)
                topLeft.x = point.colorSpace.X;

            if (point.colorSpace.Y < topLeft.y)
                topLeft.y = point.colorSpace.Y;
        }
        return topLeft;
    }

    private Vector2 GetBottomRight(List<ValidPoint> points)
    {
        Vector2 bottomRight = new Vector2(int.MinValue, int.MinValue);

        foreach (ValidPoint point in points)
        {
            if (point.colorSpace.X > bottomRight.x)
                bottomRight.x = point.colorSpace.X;

            if (point.colorSpace.Y > bottomRight.y)
                bottomRight.y = point.colorSpace.Y;
        }
        return bottomRight;
    }
    
    private Vector2 ScreenToCamera(Vector2 screenPosition)
    {
        //REPLACE 1920 and 1080 with SCREEN DIMENSIONS
        Vector2 normalizedScreen = new Vector2(Mathf.InverseLerp(0, 1920, screenPosition.x), Mathf.InverseLerp(0, 1080, screenPosition.y));

        Vector2 screenPoint = new Vector2(normalizedScreen.x * mainCamera.pixelWidth, normalizedScreen.y * mainCamera.pixelHeight);

        return screenPoint;
    }
    #endregion

    #region BLOB Analysis

    private Texture2D Threshold()
    {
        //generate a binary image where BLOBs are pixels with depth 
        Texture2D newTexture = new Texture2D(1920, 1080, TextureFormat.Alpha8, false);

        for (int x = 0; x < 1920; x++)
        {
            for (int y = 0; y < 1080; y++)
            {
                newTexture.SetPixel(x, y, Color.black);
            }
        }

        foreach (Vector2 point in triggerPoints)
        {
            newTexture.SetPixel((int)point.x, (int)point.y, Color.white);
        }

        newTexture.Apply();

        return newTexture;

    }

    void Label(Color[] pixels)
    {
        int label = 1;
        for (int y = 1; y < 1080 - 1; y++)
        {
            for (int x = 1; x < 1920 - 1; x++)
            {
                if (pixels[y * 1920 + x] == Color.white)
                {
                    Grassfire(pixels, y, x, label);
                    label++;
                }
            }
        }
    } // Label

    /*
    foreach (ValidPoint point in validPoints)
    {
        if (!point.withinWallDepth)
        {
            if (point.z < wallDepth * depthSensitivity)
            {
                Vector2 screenPoint = ScreenToCamera(new Vector2(point.colorSpace.X, point.colorSpace.Y));

                triggerPoints.Add(screenPoint);
            }
        }
    }*/
    
    void Grassfire(Color[] pixels, int y, int x, int label)
    {
        labelMap[y * 1920 + x] = label; // maps the area of the image based on labels only

        if (y > 0 && pixels[(y - borderSize) * 1920 + x] == Color.white)
            Grassfire(pixels, y - 1, x, label);
        if (pixels[(y + 1) * 1920 + x] == Color.white)
            Grassfire(pixels, y + 1, x, label);
        if (pixels[y * 1920 + (x - 1)] == Color.white)
            Grassfire(pixels, y, x - 1, label);
        if (pixels[y * 1920 + (x + 1)] == Color.white)
            Grassfire(pixels, y, x + 1, label);
    }


    private Vector2 CenterOfMass()
    {
        Vector2 centerOfMass = new Vector2();

        return centerOfMass;
    }
    #endregion

}

public class ValidPoint
{
    public ColorSpacePoint colorSpace;
    public float z = 0.0f;

    public bool withinWallDepth = false;

    public ValidPoint(ColorSpacePoint colorSpace, float z)
    {
        this.colorSpace = colorSpace;
        this.z = z;
    }
}