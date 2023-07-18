using UnityEngine;
using UnityEngine.Networking;
using System.Net;
using System.IO;

public static class WildPetInfo
{
    public static string mainURL = "http://172.10.5.110:80/";
    public static string wildPetShowURL = mainURL + "map/show";
    // public static string petId = "64b13dce9a0458cf3b1e8cfd";
    // public static string wildPetId = "64b4e9cc33ca7a0e8e58d928";
    public static string petId = "64b13dc39a0458cf3b1e8cfb";
    public static string locationId = "64b1396929beab0a894b8974";
    public static WildpetResponse getdata;

    public static WildpetResponse GetData() {
        WildPet newWildPet = new WildPet {
            petId = WildPetInfo.petId,
            locationId = WildPetInfo.locationId
        };
        
        string str = JsonUtility.ToJson(newWildPet);
        var bytes = System.Text.Encoding.UTF8.GetBytes(str);

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(wildPetShowURL);
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

        getdata = JsonUtility.FromJson<WildpetResponse>(json);
        return getdata;
    }
}

[System.Serializable]
public class WildpetResponse
{
    public Pet pet;
    public Location location;
}

[System.Serializable]
public class Location
{
    public string longitude;
    public string latitude;
    public string location;
}

[System.Serializable]
public class Pet
{
    public string name;
    public int rank;
    public string[] habitat;
    public int[] probability;
}

[System.Serializable]
public class WildPet
{
    public string petId;
    public string locationId;
}
