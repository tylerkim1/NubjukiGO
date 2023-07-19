namespace Mapbox.Examples
{
	using UnityEngine;
	using Mapbox.Utils;
	using Mapbox.Unity.Map;
	using Mapbox.Unity.MeshGeneration.Factories;
	using Mapbox.Unity.Utilities;
	using System.Collections.Generic;

	public class SpawnOnMap : MonoBehaviour
	{
		[SerializeField]
		AbstractMap _map;

		[SerializeField]
		[Geocode]
		string[] _locationStrings;
		Vector2d[] _locations;

		[SerializeField]
		float _spawnScale = 100f;

		[SerializeField]
		public GameObject _markerPrefab;

		List<GameObject> _spawnedObjects;

		void Start()
		{
			//_locations = new Vector2d[_locationStrings.Length];
			_locations = new Vector2d[]
			{
				new Vector2d(36.373888, 127.356779), // 샌프란시스코의 위도와 경도
                new Vector2d(36.37403, 127.3654),
				new Vector2d(36.37357500089068, 127.35956859056166)
			};
			_spawnedObjects = new List<GameObject>();
			for (int i = 0; i < _locations.Length; i++)  // Use _locations.Length instead of _locationStrings.Length
			{
				var location = _locations[i];
				var instance = Instantiate(_markerPrefab);
				Debug.Log("instanceLog!!" + instance);
				instance.GetComponent<EventSpawner>().eventID = i + 1;
				instance.GetComponent<EventSpawner>().eventPos = location;
				Debug.Log("Loc" + instance.GetComponent<EventSpawner>().eventPos);
				instance.transform.localPosition = _map.GeoToWorldPosition(location, true);
				instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
				_spawnedObjects.Add(instance);
			}
		}

		private void Update()
		{
			int count = _spawnedObjects.Count;
			for (int i = 0; i < count; i++)
			{
				var spawnedObject = _spawnedObjects[i];
				var location = _locations[i];
				spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
				spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
			}
		}
	}
}