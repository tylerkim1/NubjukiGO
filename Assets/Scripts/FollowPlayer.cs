using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Examples;

public class FollowPlayer : MonoBehaviour
{
    public AbstractMap map;  // Your Mapbox Unity map
    public LocationStatus locationStatus;  // Your LocationStatus component

    private Vector2d lastPlayerPosition;
    private float minUpdateDistance = 0.0001f;  // Set your desired minimum update distance here
    private int initialZoom = 17;

    private void Start()
    {
        Vector2d playerPosition = new Vector2d(locationStatus.GetLocationLat(), locationStatus.GetLocationLong());
        map.Initialize(playerPosition, initialZoom);
        lastPlayerPosition = playerPosition;
        StartCoroutine(UpdateMapRoutine());
    }

    private IEnumerator UpdateMapRoutine()
    {
        while (true)
        {
            Vector2d playerPosition = new Vector2d(locationStatus.GetLocationLat(), locationStatus.GetLocationLong());

            // Update the map only if the player has moved more than the minimum update distance
            if (Vector2d.Distance(lastPlayerPosition, playerPosition) > minUpdateDistance)
            {
                map.UpdateMap(playerPosition, map.Zoom);
                lastPlayerPosition = playerPosition;
            }

            yield return new WaitForSeconds(5);  // Wait for 1 second
        }
    }
}
