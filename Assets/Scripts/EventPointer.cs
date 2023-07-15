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
    // Start is called before the first frame update
    void Start()
    {
        menuUIManager = GameObject.Find("Canvas").GetComponent<MenuUIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        FloatAndRotatePointer();
    }
    public void FloatAndRotatePointer()
    {
        transform.Rotate(Vector3.up, rotationSpeed* Time.deltaTime);
        transform.position = new Vector3(transform.position.x, (Mathf.Sin(Time.fixedTime*Mathf.PI*frequency)*amplitude)*2+10, transform.position.z);
    }
    private void OnMouseDown()
    {
        playerLocation = GameObject.Find("Canvas").GetComponent<LocationStatus>();
        var currentPlayerLocation = new GeoCoordinatePortable.GeoCoordinate(playerLocation.GetLocationLat(), playerLocation.GetLocationLong());
        var eventLocation = new GeoCoordinatePortable.GeoCoordinate(eventPos[0], eventPos[1]);
        var distance = currentPlayerLocation.GetDistanceTo(eventLocation);
        Debug.Log("Distance is: " + distance);
        if(distance < 300)
        {
            menuUIManager.DisplayCatchPanel(1);
        }
        else
        {
            menuUIManager.DisplayToFarPanel();
        }
    }
}
