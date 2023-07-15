using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;      	// AR 관련 클래스를 참조하기 위해 선언해주자
using UnityEngine.XR.ARSubsystems;



public class ARPlaceObject : MonoBehaviour
{
    private ARRaycastManager arRaycastManager;      // RaycastManager 참조
    private ARPlaneManager arPlaneManager;      // ARPlaneManager 참조
    private GameObject placeObject;
    private GameObject spawnedObject;       // 생성한 게임 오브젝트 저장할 변수 선언
    private bool isCreated = false;

    // Start is called before the first frame update
    void Start()
    {
        isCreated = true;
        arRaycastManager = GetComponent<ARRaycastManager>();
        arPlaneManager = GetComponent<ARPlaneManager>();
        placeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        placeObject.transform.localScale = Vector3.one * 0.05f;   
    }

    // Update is called once per frame
    void Update()
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        // Raycast를 실행하며, 그 결과값을 hits 변수에 담아준다.
        Vector2 position = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        if (!isCreated && arRaycastManager.Raycast(position, hits, TrackableType.PlaneWithinInfinity))
        {
            var hitPose = hits[0].pose;   // ray에 맞은 결과의 첫번째 정보를 변수로 선언

            if (spawnedObject == null)
            {
                isCreated = true;
                // 생성된 게임 오브젝트가 없으면 변수로 할당한 오브젝트를 생성하고 spawnObject에 담는다
                spawnedObject = Instantiate(placeObject, hitPose.position, hitPose.rotation);
            }
            else
            {
                
                // 생성된 오브젝트가 있다면, hitPose 위치 정보에 맞게 위치 좌표와 회전값을 대입하여 이동시킨다.
                spawnedObject.transform.position = hitPose.position;
                spawnedObject.transform.rotation = hitPose.rotation;

                foreach (var plane in arPlaneManager.trackables)
                {
                    // 오브젝트가 생성되었기 때문에 Plane 인스턴스 생성을 멈추게 한다.
                    plane.gameObject.SetActive(false);
                }
            }
        }
    }
}
