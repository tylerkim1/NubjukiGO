namespace Mapbox.Unity.Map
{
	using System.Collections;
	using Mapbox.Unity.Location;
	using UnityEngine;
	using Mapbox.Utils;

	public class InitializeMapWithLocationProvider : MonoBehaviour
	{
		[SerializeField]
		AbstractMap _map;
		private Vector3 _mousePositionPrevious;
		private bool _isMouseDragging;

		ILocationProvider _locationProvider;
    
		private void Awake()
		{
			// Prevent double initialization of the map. 
			_map.InitializeOnStart = false;
		}

		protected virtual IEnumerator Start()
		{
			yield return null;
			_locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
			_locationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated; ;
		}

		void LocationProvider_OnLocationUpdated(Unity.Location.Location location)
		{
			_locationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;
			_map.Initialize(location.LatitudeLongitude, _map.AbsoluteZoom);
		}
		void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				_mousePositionPrevious = Input.mousePosition;
				_isMouseDragging = true;
			}
			else if (Input.GetMouseButtonUp(0))
			{
				_mousePositionPrevious = Input.mousePosition;
				_isMouseDragging = false;
			}
			if (_isMouseDragging)
			{
				Vector3 direction = _mousePositionPrevious - Input.mousePosition;
				_mousePositionPrevious = Input.mousePosition;

				Vector2d latitudeLongitudeDelta = new Vector2d(-direction.y, direction.x) * 0.01;

				Vector2d newLatLong = _map.CenterLatitudeLongitude + latitudeLongitudeDelta;
				_map.UpdateMap(newLatLong, _map.Zoom);
			}
		}

	}
}
