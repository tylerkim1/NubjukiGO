using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Examples;

public class EventSpawner : MonoBehaviour

{
    [SerializeField] float rotationSpeed = 50f;
    [SerializeField] float amplitude = 2.0f;
    [SerializeField] float frequency = 0.50f;
    LocationStatus playerLocation;
    public Vector2d eventPos;
    public int eventID;

    MenuUIManager menuUIManager;
    EventManager eventManager;
    // Start is called before the first frame update
    void Start()
    {
        menuUIManager = GameObject.Find("Canvas").GetComponent<MenuUIManager>();
        eventManager = GameObject.Find("EventManager").GetComponent<EventManager>();
        Debug.Log("Exist");
        Debug.Log("Is " + gameObject.name + " active?: " + gameObject.activeInHierarchy);
        Debug.Log("Does " + gameObject.name + " have a collider?: " + (GetComponent<Collider>() != null));
        // Graphic의 경우 Raycast Target 설정 확인
        //Debug.Log("Is " + gameObject.name + "'s raycast target enabled?: " + GetComponent<UnityEngine.UI.Graphic>().raycastTarget);
    }


    // Update is called once per frame
    void Update()
    {
        // 마우스 좌클릭 또는 화면 터치가 시작될 때
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Clicked!1");
            RaycastHit hit;
            // 카메라에서 마우스 위치로 Ray를 쏩니다.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Ray가 어떤 Collider와 충돌했는지 확인합니다.
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Clicked!2");
                Debug.Log(hit.transform.name);
                // hit.transform.name 또는 hit.transform.tag를 사용하여 특정 오브젝트를 식별할 수 있습니다.
                // 예를 들어, hit.transform.name이 "MyObjectName"인 경우에 패널을 띄우고 싶다면:
                if (hit.transform.name == "MapboxPin" || hit.transform.name == "MapboxPin(Clone)")
                {
                    Debug.Log("Clicked!");
                    playerLocation = GameObject.Find("Canvas").GetComponent<LocationStatus>();
                    var currentPlayerLocation = new GeoCoordinatePortable.GeoCoordinate(playerLocation.GetLocationLat(), playerLocation.GetLocationLong());
                    var eventLocation = new GeoCoordinatePortable.GeoCoordinate(eventPos[0], eventPos[1]);
                    var distance = currentPlayerLocation.GetDistanceTo(eventLocation);
                    Debug.Log("Distance is: " + distance);
                    if (distance < 40 )
                    {
                        menuUIManager.DisplayCatchPanel(1);
                    }
                    else
                    {
                        menuUIManager.DisplayToFarPanel();
                    }
                }
            }
        }
    }
    //public void FloatAndRotatePointer()
    //{
    //transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    //transform.position = new Vector3(transform.position.x, (Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude) * 2 + 10, transform.position.z);
    //}
    private void OnMouseDown()
    {

        //Debug.Log("Clicked!");
       
    }
}
