using UnityEngine;
using System.Collections;

public class DistortionMobile : MonoBehaviour {

  public float Scale = 1f;
  public RenderTextureFormat RenderTextureFormat;
  public FilterMode FilterMode = FilterMode.Point;
  public LayerMask CullingMask;

  private RenderTexture renderTexture;
	// Use this for initialization

  void Start()
  {
    Invoke("Initialize", 0.1f);
  }

  private void Initialize()
  {
    //Screen.SetResolution(1980, 1024, true);
    var goCamera = new GameObject("RenderTextureCamera");
    var cameraInstance = goCamera.AddComponent<Camera>();
    var cam = Camera.main;
    cameraInstance.CopyFrom(cam);
    cameraInstance.depth++;
    cameraInstance.cullingMask = CullingMask;
    goCamera.transform.parent = cam.transform;
    renderTexture = new RenderTexture((int)(Screen.width * Scale), (int)(Screen.height * Scale), 32, RenderTextureFormat);
    renderTexture.DiscardContents();
    renderTexture.filterMode = FilterMode.Point;
    cameraInstance.targetTexture = renderTexture;

    Shader.SetGlobalTexture("_GrabTextureMobile", renderTexture);
  }
}
