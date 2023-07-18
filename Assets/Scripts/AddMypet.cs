using UnityEngine;
using UnityEngine.Networking;
using System.Net;
using System.IO;

public static class AddMypet
{
    public static string mainURL = "http://172.10.5.110:80/";
    public static string addMyPetURL = mainURL + "grow/add";
    public static string userId = "64b4b85f33ca7a0e8e58d925";
    public static AddMypetResponse getdata;

    public static AddMypetResponse AddToMyPet(string petId, string locationId) {
        MyPet newMyPet = new MyPet {
            // Set the properties of newMyPet here, e.g.,
            userId = userId,
            petId = petId,
            locationId = locationId,
            hungry = 70,
            sleep = 70,
            happy = 70,
            clean = 70,
        };
        
        string str = JsonUtility.ToJson(newMyPet);
        var bytes = System.Text.Encoding.UTF8.GetBytes(str);

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(addMyPetURL);
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

        getdata = JsonUtility.FromJson<AddMypetResponse>(json);
        return getdata;
    }
}

[System.Serializable]
public class AddMypetResponse
{
    public string userId;
    public string petId;
    public string locationId;
    public int hungry;
    public int sleep;
    public int happy;
    public int clean;
}

[System.Serializable]
public class MyPet
{
    public string userId;
    public string petId;
    public string locationId;
    public int hungry;
    public int sleep;
    public int happy;
    public int clean;
}