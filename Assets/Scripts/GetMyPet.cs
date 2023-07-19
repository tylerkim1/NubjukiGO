using EasyUI.Toast;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GetMyPet : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject firstPet;
    public GameObject noFirstPet;
    public GameObject secondPet;
    public GameObject noSecondPet;
    public GameObject thirdPet;
    public GameObject noThirdPet;
    Sprite rank2PetImage;
    Sprite rank3PetImage;
    List<MyPet> myPetList = new List<MyPet>();
    [System.Serializable]
    public class GetMyPetRequestBody
    {
        public string userId;
    }
    [System.Serializable]
    public class MyPet
    {
        public string _id;
        public string userId;
        public string petId;
        public string locationId;
        public int hungry;
        public int sleep;
        public int happy;
        public int clean;
        public string name;
        public int rank;
    }
    [System.Serializable]
    public class GetMyPetResponseBody
    {
        public MyPet[] list;
    }
    [System.Serializable]
    public class DeletePetRequestBody
    {
        public string myPetId;
    }
    void Start()
    {
        rank2PetImage = Resources.Load<Sprite>("rank2Nupzuki");
        rank3PetImage = Resources.Load<Sprite>("rank3Nupzuki");
        string url = "http://172.10.5.110/grow/all";
        GetMyPetRequestBody data = new GetMyPetRequestBody();
        data.userId = UserInfo.id;
        string str = JsonUtility.ToJson(data);
        var bytes = System.Text.Encoding.UTF8.GetBytes(str);

        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = bytes.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                stream.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string json = reader.ReadToEnd();
            reader.Close();
            GetMyPetResponseBody body = JsonUtility.FromJson<GetMyPetResponseBody>(json);
            for (int i = 0; i < body.list.Length; i++)
            {
                myPetList.Add(body.list[i]);
            }
            UpdateUI();
        }
        catch(Exception e)
        {
            Toast.Show(e.ToString());
        }

    }
    private string GetRank(int rank)
    {
        if (rank == 1)
        {
            return "common";
        }
        else if (rank == 2)
        {
            return "rare";
        }
        else
        {
            return "epic";
        }
    }

    private void UpdateUI()
    {
        GameObject[] gameObjects = { firstPet, secondPet, thirdPet };
        GameObject[] noGameObjects = { noFirstPet, noSecondPet, noThirdPet };
        for (int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].SetActive(false);
            noGameObjects[i].SetActive(true);
        }
        int length = Math.Min(myPetList.Count, 3);
        for (int i = 0; i < length; i++)
        {
            MyPet item = myPetList[i];
            GameObject gameObject = gameObjects[i];
            gameObject.SetActive(true);
            if (item.rank == 2)
            {
                gameObject.transform.Find("Image").gameObject.GetComponent<Image>().sprite = rank2PetImage;
            }
            else if (item.rank == 3)
            {
                gameObject.transform.Find("Image").gameObject.GetComponent<Image>().sprite = rank3PetImage;
            }

            gameObject.transform.Find("Top").Find("PetName").gameObject.GetComponent<TMP_Text>().text = item.name;
            gameObject.transform.Find("Top").Find("Rank").gameObject.GetComponent<TMP_Text>().text = "Rank. " + GetRank(item.rank);
            gameObject.transform.Find("Middle").Find("Hungry").Find("Text").gameObject.GetComponent<TMP_Text>().text = "포만감 (" + item.hungry + "%)";
            gameObject.transform.Find("Middle").Find("Hungry").Find("Slider").gameObject.GetComponent<Slider>().value = item.hungry / 100F;
            gameObject.transform.Find("Middle").Find("Energy").Find("Text").gameObject.GetComponent<TMP_Text>().text = "활력 (" + item.sleep + "%)";
            gameObject.transform.Find("Middle").Find("Energy").Find("Slider").gameObject.GetComponent<Slider>().value = item.sleep / 100F;
            gameObject.transform.Find("Bottom").Find("Happy").Find("Text").gameObject.GetComponent<TMP_Text>().text = "행복도 (" + item.happy + "%)";
            gameObject.transform.Find("Bottom").Find("Happy").Find("Slider").gameObject.GetComponent<Slider>().value = item.happy / 100F;
            gameObject.transform.Find("Bottom").Find("Clean").Find("Text").gameObject.GetComponent<TMP_Text>().text = "청결도 (" + item.clean + "%)";
            gameObject.transform.Find("Bottom").Find("Clean").Find("Slider").gameObject.GetComponent<Slider>().value = item.clean / 100F;
            noGameObjects[i].SetActive(false);
        }
    }

    public void DeletePet(int index)
    {
        string url = "http://172.10.5.110/map/delete";
        string id = myPetList[index]._id;
        DeletePetRequestBody data = new DeletePetRequestBody { myPetId = id };
        string str = JsonUtility.ToJson(data);
        var bytes = System.Text.Encoding.UTF8.GetBytes(str);
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = bytes.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                stream.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string json = reader.ReadToEnd();
            reader.Close();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        catch (Exception e)
        {
            Toast.Show(e.ToString());
        }
    }
}
