using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// This code is the based on the following forum, but I tweaked a lot.
// https://forum.unity.com/threads/how-to-setup-people-occlusion.691789/
public class PostProcessHelper : MonoBehaviour
{
    [SerializeField] private ARCameraManager m_cameraManager = null;

    public AROcclusionManager occlusionManager;

    private int camW;
    private int camH;

    private static PostProcessHelper instance;
    public static PostProcessHelper GetInstance() => instance;
    
    void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        m_cameraManager.frameReceived += OnCameraFrameReceived;
    }

    private void OnDisable()
    {
        m_cameraManager.frameReceived -= OnCameraFrameReceived;
    }

    private void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        XRCameraImage cameraImage;
        m_cameraManager.TryGetLatestImage(out cameraImage);
        camW = cameraImage.width;
        camH = cameraImage.height;

        cameraImage.Dispose();
        if(camW != 0)
        {
            m_cameraManager.frameReceived -= OnCameraFrameReceived;
        }

        /* // If you want to do something with camera feed, you can tweak the following the code.
        if (m_cameraFeedTexture == null || m_cameraFeedTexture.width != cameraImage.width || m_cameraFeedTexture.height != cameraImage.height)
        {
            m_cameraFeedTexture = new Texture2D(cameraImage.width, cameraImage.height, TextureFormat.RGBA32, false);
            // m_cameraFeedTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
        }

        CameraImageTransformation imageTransformation = Input.deviceOrientation == DeviceOrientation.LandscapeRight ? CameraImageTransformation.MirrorY : CameraImageTransformation.MirrorX;
        XRCameraImageConversionParams conversionParams = new XRCameraImageConversionParams(cameraImage, TextureFormat.RGBA32, imageTransformation);

        NativeArray<byte> rawTextureData = m_cameraFeedTexture.GetRawTextureData<byte>();

        try
        {
            unsafe
            {
                cameraImage.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
            }
        }
        finally
        {
            cameraImage.Dispose();
        }

        m_cameraFeedTexture.Apply();
        m_material.SetTexture("_CameraFeed", testTexture);
        */
    }

    public void UpdateShaderProperty(Material mat)
    {
        mat.SetFloat("_UVMultiplierLandScape", CalculateUVMultiplierLandScape(camW, camH));
        mat.SetFloat("_UVMultiplierPortrait", CalculateUVMultiplierPortrait(camW, camH));

        if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
        {
            mat.SetFloat("_UVFlip", 0);
            mat.SetInt("_ONWIDE", 1);
        }
        else if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
        {
            mat.SetFloat("_UVFlip", 1);
            mat.SetInt("_ONWIDE", 1);
        }
        else
        {
            mat.SetInt("_ONWIDE", 0);
        }

        mat.SetTexture("_OcclusionDepth", occlusionManager.humanDepthTexture);
        mat.SetTexture("_OcclusionStencil", occlusionManager.humanStencilTexture);
    }

    private float CalculateUVMultiplierLandScape(int camw, int camh)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraTextureAspect = (float)camw / (float)camh;
        return screenAspect / cameraTextureAspect;
    }

    private float CalculateUVMultiplierPortrait(int camw, int camh)
    {
        float screenAspect = (float)Screen.height / (float)Screen.width;
        float cameraTextureAspect = (float)camw / (float)camh;
        return screenAspect / cameraTextureAspect;
    }
}
