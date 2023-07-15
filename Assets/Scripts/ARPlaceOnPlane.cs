using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;      	// AR 관련 클래스를 참조하기 위해 선언해주자
using UnityEngine.XR.ARSubsystems;



public class ARPlaceOnPlane : MonoBehaviour
{
    private ARRaycastManager arRaycastManager;      // RaycastManager 참조
    private ARPlaneManager arPlaneManager;      // ARPlaneManager 참조
    public GameObject placeObject;
    private GameObject spawnedObject = null;       // 생성한 게임 오브젝트 저장할 변수 선언
    private bool isCreated = false;

    // Start is called before the first frame update
    void Start()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        arPlaneManager = GetComponent<ARPlaneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCreated && arPlaneManager.enabled && arRaycastManager.enabled)
        {
            DetectGround();
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(placeObject, placeObject.transform.position, placeObject.transform.rotation);
                isCreated = true;
            }
            else
            {
                spawnedObject.transform.position = placeObject.transform.position;
                spawnedObject.transform.rotation = placeObject.transform.rotation;
                if (arPlaneManager.enabled)
                {
                    arRaycastManager.enabled = false;
                    arPlaneManager.enabled = false;
                }
            }
            
        }
    }

    void DetectGround()
    {
        Vector2 screenSize = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        List<ARRaycastHit> hitInfos = new List<ARRaycastHit>();

        if (arRaycastManager.Raycast(screenSize, hitInfos, TrackableType.Planes))
        {
            placeObject.transform.position = hitInfos[0].pose.position;
            placeObject.transform.rotation = hitInfos[0].pose.rotation;
            placeObject.transform.position += placeObject.transform.up * 0.1f;
        }
    }
}
