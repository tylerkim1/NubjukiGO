using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TouchHelper
{
#if UNITY_EDITOR
    public static bool Touch2 => Input.GetMouseButtonDown(1);
    public static bool IsDown => Input.GetMouseButtonDown(0);
    public static bool IsUp => Input.GetMouseButtonUp(0);
    public static Vector2 TouchPosition => Input.mousePosition;

#else
    public static bool Touch2 => Input.touchCount == 2 && (Input.GetTouch(1).phase == TouchPhase.Began);
    public static bool IsDown => Input.GetTouch(0).phase == TouchPhase.Began;
    public static bool IsUp => Input.GetTouch(0).phase == TouchPhase.Ended;
    public static Vector2 TouchPosition => Input.GetTouch(0).position;
#endif
}


public class BoxController : MonoBehaviour
{
    private const float CameraDistance = 7.5f;
    public float positionY = 0.4f;
    public GameObject[] prefab;

    private Camera mainCamera;
    private GameObject HoldingOjbect;
    private Vector3 InputPosition;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
#if !UNITY_EDITOR
        if (Input.touchCount == 0) return;
#endif
        InputPosition = TouchHelper.TouchPosition;

        if (TouchHelper.Touch2)
        {
            Reset();
            return;
        }
        if (HoldingOjbect)
        {
            if (TouchHelper.IsUp)
            {
                OnPut(InputPosition);
                HoldingOjbect = null;
                return;
            }
            Move(InputPosition);
            return;
        }

        if (!TouchHelper.IsDown) return;
        if (Physics.Raycast(mainCamera.ScreenPointToRay(InputPosition), out var hits, mainCamera.farClipPlane))
        {
            if (hits.transform.gameObject.tag.Equals("Player"))
            {
                HoldingOjbect = hits.transform.gameObject;
                OnHold();
            }
        }
    }
    private void OnPut(Vector3 pos)
    {
        HoldingOjbect.GetComponent<Rigidbody>().useGravity = true;
        HoldingOjbect.transform.SetParent(null);
    }

    private void Move(Vector3 pos)
    {
        pos.z = mainCamera.nearClipPlane * CameraDistance;
        HoldingOjbect.transform.position = Vector3.Lerp(HoldingOjbect.transform.position, mainCamera.ScreenToWorldPoint(pos), Time.deltaTime * 7f);
    }

    private void OnHold()
    {
        HoldingOjbect.GetComponent<Rigidbody>().useGravity = false;

        HoldingOjbect.transform.SetParent(mainCamera.transform);
        HoldingOjbect.transform.rotation = Quaternion.identity;
        HoldingOjbect.transform.position = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, positionY, mainCamera.nearClipPlane * CameraDistance));
    }

    private void Reset()
    {
        var pos = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, positionY, mainCamera.nearClipPlane * CameraDistance));
        var obj = Instantiate(prefab[0], pos, Quaternion.identity, mainCamera.transform);
        var rigidbody = obj.GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }
}