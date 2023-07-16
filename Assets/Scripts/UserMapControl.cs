using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.Location;


public class UserMapControl : MonoBehaviour
{
    public AbstractMap map;
    ILocationProvider _locationProvider;
    

    // Start is called before the first frame update
    void Start()
    {
        _locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
    }

    // Update is called once per frame
    public void ReturnToUserLocation(AbstractMap map)
    {
        var location = _locationProvider.CurrentLocation;
        map.UpdateMap(location.LatitudeLongitude, map.Zoom);
    }

}
