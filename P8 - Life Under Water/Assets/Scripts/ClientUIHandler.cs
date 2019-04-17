using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using TMPro;

public class ClientUIHandler : MonoBehaviour
{
    public TextMeshProUGUI statusText;

    public TMP_InputField ipInput;
    public TMP_InputField portInput;
    public TMP_InputField offsetX, offsetY;

    public enum Slider
    {
        Sensitivity, Depth, Top, Bottom, Left, Right
    };
    private Slider activeSlider;

    private IPAddress ip;
    private int port, X, Y;


    void Awake()
    {
        statusText.text = "";
    }

    public bool TryParseInputValues()
    {
        // Validate IP Address
        if (!IPAddress.TryParse(ipInput.text, out ip))
        {
            statusText.text = "IP Address not valid.";
            return false;
        }
        // Validate Port as Integer
        if (!int.TryParse(portInput.text, out port))
        {
            statusText.text = "Port not valid.";
            return false;
        }

        // Validate Port is in correct range
        if (port < 1024 || port > 65535)
        {
            statusText.text = "Port not valid. \nPlease choose port between 1024 and 65535.";
            return false;
        }

        statusText.text = "IP Address & Port Accepted.";
        return true;
    }

    public void ConnectToServer()
    {
        if (!TryParseInputValues())
            return;

        KinectClient.Instance.ConnectToServer(ip, port);
        statusText.text = string.Format("Connected to {0} at port {1}", ip, port);
    }

    public void ConnectToLocalHost()
    {
        ip = IPAddress.Parse("127.0.0.1");
        port = 11000;

        KinectClient.Instance.ConnectToServer(ip, port);
        statusText.text = string.Format("Connected to localhost at port " + port);
    }

    public void SetKinectDataOffset()
    {
        if (!int.TryParse(offsetX.text, out X))
            return; 
        if (!int.TryParse(offsetY.text, out Y))
            return;
        print(string.Format("Set Data Offset to {0}, {1}", X, Y));
        MeasureDepth.Instance.SetOffset(X, Y);
    }

    public void SetActiveSlider(string s)
    {
        print("Setting active Slider as" + s);
        switch (s)
        {
            case "Sensitivity":
                activeSlider = Slider.Sensitivity;
                break;
            case "Depth":
                activeSlider = Slider.Depth;
                break;
            case "Top":
                activeSlider = Slider.Top;
                break;
            case "Bottom":
                activeSlider = Slider.Bottom;
                break;
            case "Left":
                activeSlider = Slider.Left;
                break;
            case "Right":
                activeSlider = Slider.Right;
                break;
            default:
                break;
        }
    }

    public void SetMeasurementParameters(float value)
    {
        print("Changing Value for Slider" + activeSlider + " to " + value);
        switch (activeSlider)
        {
            case Slider.Sensitivity:
                MeasureDepth.Instance.DepthSensitivity = value;
                break;
            case Slider.Depth:
                MeasureDepth.Instance.FloorDepth = value;
                break;
            case Slider.Top:
                MeasureDepth.Instance.TopCutOff = value;
                break;
            case Slider.Bottom:
                MeasureDepth.Instance.BottomCutOff = value;
                break;
            case Slider.Left:
                MeasureDepth.Instance.LeftCutOff = value;
                break;
            case Slider.Right:
                MeasureDepth.Instance.RightCutOff = value;
                break;
            default:
                break;
        }
    }

}
