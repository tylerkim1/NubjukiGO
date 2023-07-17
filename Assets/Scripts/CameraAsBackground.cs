using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraAsBackground : MonoBehaviour
{
    public RawImage image;
    private WebCamTexture cam;
    private AspectRatioFitter arf;

    // Start is called before the first frame update
    IEnumerator Start()
    {   
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam)) {
            // Do your stuff
            arf = GetComponent<AspectRatioFitter>();
            image = GetComponent<RawImage>();
            if (image == null) {
                Debug.LogError("RawImage component is not found");
                yield break;
            }

            if(WebCamTexture.devices.Length > 0) {
                cam = new WebCamTexture(Screen.width, Screen.height);
                image.texture = cam;
                cam.Play();
            } else Debug.Log("No webcam detected");
        } else {
            // Exit or show a dialog that the webcam is needed
            Debug.Log("No Permission");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (cam.width < 100) {
            return;
        }
        
        float cwNeeded = -cam.videoRotationAngle;
        if(cam.videoVerticallyMirrored)
            cwNeeded += 180f;

        image.rectTransform.localEulerAngles = new Vector3(0f, 0f, cwNeeded);

        float vidioRatio = (float)cam.width / (float)cam.height;
        arf.aspectRatio = vidioRatio;

        if(cam.videoVerticallyMirrored) image.uvRect = new Rect(1, 0, -1, 1);
        else image.uvRect = new Rect(0, 0, 1, 1);
    }
}
