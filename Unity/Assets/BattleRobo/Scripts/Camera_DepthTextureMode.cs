using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Camera_DepthTextureMode : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private void Start()
    {
        cam.depthTextureMode = DepthTextureMode.Depth;
    }
}
