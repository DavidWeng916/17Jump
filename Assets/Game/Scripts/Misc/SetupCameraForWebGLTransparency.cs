using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SetupCameraForWebGLTransparency : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;

        var universalAdditionalCameraData = GetComponent<UniversalAdditionalCameraData>();
        universalAdditionalCameraData.renderPostProcessing = false;
    }
}
