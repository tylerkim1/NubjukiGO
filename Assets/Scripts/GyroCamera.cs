using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GyroCamera : MonoBehaviour
{
    private Gyroscope gyro;
    private bool gyroSupported;
    private Quaternion rotFix;
    private Quaternion initialOrientation;
    private bool initialOrientationSet = false;

    [SerializeField]
    private Transform worldObj;
    private float startY;

    [SerializeField]
    private GameObject[] zoomObj;
    private GameObject currentZoomObj;
    
    float rotationSpeed = 90;
    Vector3 currentEulerAngles;

    // Start is called before the first frame update
    void Start() {
        gyroSupported = SystemInfo.supportsGyroscope;

        GameObject camParent = new GameObject ("camParent");
        camParent.transform.position = transform.position;
        transform.parent = camParent.transform;
        SetPetToShown();
        ResetGyroRotation();

        if (gyroSupported) {
            gyro = Input.gyro;
            gyro.enabled = true;

            camParent.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            rotFix = new Quaternion (0, 0, 1, 0);
        } else {
            // 카메라가 특정 방향을 바라보도록 설정합니다.
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);  // 이 각도를 적당히 조절하세요.
        }
    }

    void SetPetToShown() {
        WildPetInfo.GetData();

        zoomObj = new GameObject[3];
        zoomObj[0] = GameObject.Find("Nupzuki");  
        zoomObj[1] = GameObject.Find("Nupzuki2");
        zoomObj[2] = GameObject.Find("Nupzuki3");
        int curInt = 0;
        if (WildPetInfo.getdata.pet.name == "새내기 넙죽이") {
            curInt = 0;
        } else if (WildPetInfo.getdata.pet.name == "화석 넙죽이") {
            curInt = 1;
        } else if(WildPetInfo.getdata.pet.name == "교수 넙죽이") {
            curInt = 2;
        }
        // Let's assume we have three objects, we initialize them here
        currentZoomObj = zoomObj[curInt];

        for (int i = 0; i < 3; i++) {
            if (i == curInt) continue;
            else zoomObj[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update() {
        // 자이로스코프가 지원되는 경우
        if (gyroSupported) {
            if (startY == 0) {
                ResetGyroRotation();
            }
            if (gyro != null) {
                transform.localRotation = gyro.attitude * rotFix;
            } else {
                Debug.Log("Gyro is null");
            }
        } 
        // 자이로스코프가 지원되지 않는 경우 (PC에서 실행하는 경우 등)
        else {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            currentEulerAngles += new Vector3(-mouseY, mouseX, 0);
            transform.localRotation = Quaternion.Euler(currentEulerAngles);
        }
    }

    public void ResetGyroRotation() {
        int x = Screen.width / 2;
        int y = Screen.height / 2;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3 (x, y));
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 500)) {
            Vector3 hitPoint = hit.point;
            hitPoint.y = 0;

            float z = Vector3.Distance(Vector3.zero, hitPoint);
            // Here we choose which object to use. I used 0 as an example.
            // currentZoomObj.transform.localPosition = new Vector3(0f, currentZoomObj.transform.localPosition.y, Mathf.Clamp(z, 0f, -2f));
            if (currentZoomObj != null) {
                currentZoomObj.transform.localPosition = new Vector3(0f, currentZoomObj.transform.localPosition.y, Mathf.Clamp(z, 0f, -2f));
            } else {
                Debug.LogError("currentZoomObj is null.");
            }
            // zoomObj.localPosition = new Vector3(0f, zoomObj.localPosition.y, Mathf.Clamp(z, 8.5f, 8.5f));
        }

        startY = transform.eulerAngles.y;
        worldObj.rotation = Quaternion.Euler (0f, startY, 0f);
    }
}

[System.Serializable]
public class getWildPetById
{
    public string petId;
    public string locationId;
}