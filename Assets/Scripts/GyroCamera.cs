using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GyroCamera : MonoBehaviour
{
    public string url = "http://172.10.5.110:80/map/show";
    string petId = "64b13dce9a0458cf3b1e8cfd";
    string locationId = "64b13a5c29beab0a894b8980";
    private Gyroscope gyro;
    private bool gyroSupported;
    private Quaternion rotFix;

    [SerializeField]
    private Transform worldObj;
    private float startY;

    [SerializeField]
    private GameObject[] zoomObj;
    private GameObject currentZoomObj;

    // Start is called before the first frame update
    void Start() {
        gyroSupported = SystemInfo.supportsGyroscope;

        GameObject camParent = new GameObject ("camParent");
        camParent.transform.position = transform.position;
        transform.parent = camParent.transform;
        SetPetToShown();

        if (gyroSupported) {
            gyro = Input.gyro;
            gyro.enabled = true;

            camParent.transform.rotation = Quaternion.Euler(90f, 180f, 0f);
            rotFix = new Quaternion (0, 0, 1, 0);
        }
    }

    void SetPetToShown() {
        WildPet newWildPet = new WildPet {
            petId = petId,
            locationId = locationId
        };
        string str = JsonUtility.ToJson(newWildPet);
        var bytes = System.Text.Encoding.UTF8.GetBytes(str);

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.ContentType = "application/json";
        request.ContentLength = bytes.Length;

        using(var stream = request.GetRequestStream()) {
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            stream.Close();
        }

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string json = reader.ReadToEnd();

        var getdata = JsonUtility.FromJson<Response>(json);
        Debug.Log(getdata);

        zoomObj = new GameObject[3];
        zoomObj[0] = GameObject.Find("Nupzuki");  
        zoomObj[1] = GameObject.Find("Nupzuki2");
        zoomObj[2] = GameObject.Find("Nupzuki3");
        int curInt = 0;
        if (getdata.pet.name == "새내기 넙죽이") {
            curInt = 0;
        } else if (getdata.pet.name == "화석 넙죽이") {
            curInt = 1;
        } else if(getdata.pet.name == "교수 넙죽이") {
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
        if(gyroSupported && startY == 0) {
            ResetGyroRotation();
        }
        if (gyro != null) {
            transform.localRotation = gyro.attitude * rotFix;
        } else {
            Debug.Log("Gyro is null");
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
            currentZoomObj.transform.localPosition = new Vector3(0f, currentZoomObj.transform.localPosition.y, Mathf.Clamp(z, 8.5f, 8.5f));
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