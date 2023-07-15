using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;      	// AR 관련 클래스를 참조하기 위해 선언해주자
using UnityEngine.XR.ARSubsystems;



public class ARObjectOnPlane : MonoBehaviour
{
    private ARRaycastManager arRaycastManager;      // RaycastManager 참조
    private ARPlaneManager arPlaneManager;      // ARPlaneManager 참조
    public GameObject placeObject;
    private GameObject spawnedObject = null;       // 생성한 게임 오브젝트 저장할 변수 선언
    private bool isCreated = false;
    float speed = 5f;
    float height = 0.1f;


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
            Vector2 screenSize = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            List<ARRaycastHit> hitInfos = new List<ARRaycastHit>();

            if (arRaycastManager.Raycast(screenSize, hitInfos, TrackableType.Planes))
            {
                placeObject.transform.position = hitInfos[0].pose.position;
                placeObject.transform.rotation = hitInfos[0].pose.rotation;
            }
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(placeObject, placeObject.transform.position, placeObject.transform.rotation);
                isCreated = true;
            }
            else
            {
                spawnedObject.transform.position = placeObject.transform.position;
                spawnedObject.transform.rotation = placeObject.transform.rotation;
                arRaycastManager.enabled = false;
                arPlaneManager.enabled = false;
            }

        }
        if (isCreated)
        {
            spawnedObject.transform.position = placeObject.transform.position;
            spawnedObject.transform.rotation = placeObject.transform.rotation;
            float newY = Mathf.Sin(Time.time * speed) * height + spawnedObject.transform.position.y;
            spawnedObject.transform.position = new Vector3(spawnedObject.transform.position.x, newY, spawnedObject.transform.position.z);
        }
    }
}
