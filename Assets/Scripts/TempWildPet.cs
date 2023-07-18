public static class TempWildPet
{
    public static string mainURL = "http://172.10.5.110:80/";
    public static string wildPetShowURL = mainURL + "map/show";
    public static string petId = "64b13dce9a0458cf3b1e8cfd";
    public static string locationId = "64b13a5c29beab0a894b8980";
}

[System.Serializable]
public class Response
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
