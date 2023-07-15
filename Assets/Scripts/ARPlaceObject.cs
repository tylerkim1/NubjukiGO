using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;      	// AR ���� Ŭ������ �����ϱ� ���� ����������
using UnityEngine.XR.ARSubsystems;



public class ARPlaceObject : MonoBehaviour
{
    private ARRaycastManager arRaycastManager;      // RaycastManager ����
    private ARPlaneManager arPlaneManager;      // ARPlaneManager ����
    private GameObject placeObject;
    private GameObject spawnedObject;       // ������ ���� ������Ʈ ������ ���� ����
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
        // Raycast�� �����ϸ�, �� ������� hits ������ ����ش�.
        Vector2 position = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        if (!isCreated && arRaycastManager.Raycast(position, hits, TrackableType.PlaneWithinInfinity))
        {
            var hitPose = hits[0].pose;   // ray�� ���� ����� ù��° ������ ������ ����

            if (spawnedObject == null)
            {
                isCreated = true;
                // ������ ���� ������Ʈ�� ������ ������ �Ҵ��� ������Ʈ�� �����ϰ� spawnObject�� ��´�
                spawnedObject = Instantiate(placeObject, hitPose.position, hitPose.rotation);
            }
            else
            {
                
                // ������ ������Ʈ�� �ִٸ�, hitPose ��ġ ������ �°� ��ġ ��ǥ�� ȸ������ �����Ͽ� �̵���Ų��.
                spawnedObject.transform.position = hitPose.position;
                spawnedObject.transform.rotation = hitPose.rotation;

                foreach (var plane in arPlaneManager.trackables)
                {
                    // ������Ʈ�� �����Ǿ��� ������ Plane �ν��Ͻ� ������ ���߰� �Ѵ�.
                    plane.gameObject.SetActive(false);
                }
            }
        }
    }
}
