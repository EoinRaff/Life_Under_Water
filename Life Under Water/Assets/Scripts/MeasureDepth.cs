using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class MeasureDepth : Singleton<MeasureDepth>
{
    public delegate void NewTriggerPoints(List<Vector2> triggerPoints);
    public static event NewTriggerPoints OnTriggerPoints = null;
    
    public MultiSourceManager multiSource;
    public Texture2D depthTexture;

    [HideInInspector]
    public KinectData kinectData;

    #region Cutoffs
    [SerializeField]
    [Range(0, 1.0f)]
    private float depthSensitivity = 1.0f;
    [SerializeField]
    [Range(-10.0f, 10.0f)]
    private float floorDepth = -10.0f;

    [Header("Top and Bottom")]
    [SerializeField]
    [Range(-1.0f, 1.0f)]
    private float topCutOff = 1.0f;
    [SerializeField]
    [Range(-1.0f, 1.0f)]
    private float bottomCutOff = -1.0f;

    [Header("Left and Right")]
    [SerializeField]
    [Range(-1.0f, 1.0f)]
    private float leftCutOff = -1.0f;
    [SerializeField]
    [Range(-1.0f, 1.0f)]
    private float rightCutOff = 1.0f;
    #endregion

    #region Depth Data
    private ushort[] depthData = null;
    private CameraSpacePoint[] cameraSpacePoints = null;
    private ColorSpacePoint[] colorSpacePoints = null;
    private List<ValidPoint> validPoints = null;
    private List<Vector2> triggerPoints = null;
    private int downSampleFactor = 8;
    #endregion

    #region Kinect & Camera
    private KinectSensor sensor = null;
    private CoordinateMapper mapper = null;
    private Camera mainCamera = null;
    private readonly Vector2Int depthResolution = new Vector2Int(512, 424);
    #endregion

    #region UI
    private Rect boundingBox, centerOfMassRect;
    #endregion

    #region Encapsulation
    public Vector2 CenterOfMass { get; private set; }
    public List<Vector2> TriggerPoints { get => triggerPoints; private set => triggerPoints = value; }
    public float DepthSensitivity { get => depthSensitivity; set => depthSensitivity = value; }
    public float FloorDepth { get => floorDepth; set => floorDepth = value; }
    public float TopCutOff { get => topCutOff; set => topCutOff = value; }
    public float BottomCutOff { get => bottomCutOff; set => bottomCutOff = value; }
    public float LeftCutOff { get => leftCutOff; set => leftCutOff = value; }
    public float RightCutOff { get => rightCutOff; set => rightCutOff = value; }
    #endregion

    private new void Awake()
    {
        base.Awake();

        sensor = KinectSensor.GetDefault();
        mapper = sensor.CoordinateMapper;
        mainCamera = Camera.main;

        int arraySize = depthResolution.x * depthResolution.y;

        cameraSpacePoints = new CameraSpacePoint[arraySize];
        colorSpacePoints = new ColorSpacePoint[arraySize];
        kinectData = new KinectData(arraySize/downSampleFactor);

    }

    private void Update()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        validPoints = DepthToColor();

        triggerPoints = FilterToTrigger(validPoints);

        CenterOfMass = CalculateCenterOfMass(triggerPoints);
        centerOfMassRect = new Rect(CenterOfMass, new Vector2(50, 50));


        if (OnTriggerPoints != null && triggerPoints.Count != 0)
        {
            OnTriggerPoints(triggerPoints);
        }

        kinectData.triggerPoints = triggerPoints.ToArray();
        kinectData.centerOfMass = CenterOfMass;
        kinectData.bottomRight = GetBottomRight(validPoints);
        kinectData.topLeft = GetTopLeft(validPoints);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            boundingBox = CreatRect(validPoints);

            depthTexture = CreateTexture(validPoints);
        }
    }

    private void OnGUI()
    {
        GUI.Box(boundingBox, "");
        GUI.Box(centerOfMassRect, "CoM");
        

        if (triggerPoints == null)
            return;

        foreach (Vector2 point in triggerPoints)
        {
            Rect rect = new Rect(point, new Vector2(10, 10));
            GUI.Box(rect, "");
        }
    }

    private List<ValidPoint> DepthToColor()
    {
        List<ValidPoint> validPoints = new List<ValidPoint>();

        // Get Depth Data From Kinect
        depthData = multiSource.GetDepthData();
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
                if (cameraSpacePoints[sampleIndex].Z >= floorDepth)
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
                if (point.z < floorDepth * depthSensitivity)
                {
                    Vector2 screenPoint = ScreenToCamera(new Vector2(point.colorSpace.X, point.colorSpace.Y), mainCamera);

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
                newTexture.SetPixel(x, y, Color.clear);
            }
        }

        foreach (ValidPoint point in validPoints)
        {
            newTexture.SetPixel((int)point.colorSpace.X, (int)point.colorSpace.Y, Color.black);
        }

        newTexture.Apply();

        return newTexture;
    }

    private Vector2 CalculateCenterOfMass(List<Vector2> points)
    {
        Vector2 centerOfMass = new Vector2(0, 0);
        int count = 0;

        foreach (Vector2 point in points)
        {
            count++;
            centerOfMass += point;
        }
        centerOfMass /= count;
        return centerOfMass;
    }

    public void SetOffset(int x, int y)
    {
        kinectData.offset = new Vector2(x, y);
    }

    #region Rect Creation
    private Rect CreatRect(List<ValidPoint> points)
    {
        if (points.Count == 0)
            return new Rect();

        // Get Corners of Rect
        Vector2 topLeft = GetTopLeft(points);
        Vector2 bottomRight = GetBottomRight(points);

        // Translate to Viewport
        Vector2 screenTopLeft = ScreenToCamera(topLeft, mainCamera);
        Vector2 screenBottomRight = ScreenToCamera(bottomRight, mainCamera);

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

    private static Vector2 ScreenToCamera(Vector2 screenPosition, Camera cam)
    {
        //REPLACE 1920 and 1080 with SCREEN DIMENSIONS
        Vector2 normalizedScreen = new Vector2(Mathf.InverseLerp(0, 1920, screenPosition.x), Mathf.InverseLerp(0, 1080, screenPosition.y));

        Vector2 screenPoint = new Vector2(normalizedScreen.x * cam.pixelWidth, normalizedScreen.y * cam.pixelHeight);

        return screenPoint;
    }
    #endregion
}

[System.Serializable]
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
