using System;
using System.Net;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EasyUI.Toast;
using System.Threading.Tasks;

public class GetMyPetInfo : MonoBehaviour
{
    public TMP_Text petName;
    public TMP_Text rank;
    public GameObject hungry;
    public GameObject energy;
    public GameObject happy;
    public GameObject clean;
    public GameObject prevButton;
    public GameObject nextButton;
    public GameObject empty;
    public GameObject scrollView;
    private int index = 0;
    private int totalCnt = 3;
    private GetMyPetResponseBody body;
    public GameObject backgroundPanel;
    public GameObject sleepButton;
    public GameObject walkButton;
    public GameObject feedButton;
    public GameObject showerButton;
    Sprite bedroomBackground;
    Sprite homeBackground;
    Sprite showerBackground;
    Sprite feedBackground;
    Sprite walkBackground;

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
    // Start is called before the first frame update
    void Start()
    {
        homeBackground = Resources.Load<Sprite>("homeBackground");
        bedroomBackground = Resources.Load<Sprite>("bedroomBackground");
        walkBackground = Resources.Load<Sprite>("walkBackground");
        showerBackground = Resources.Load<Sprite>("showerBackground");
        feedBackground = Resources.Load<Sprite>("feedBackground");
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
            body = JsonUtility.FromJson<GetMyPetResponseBody>(json);
            if (body.list.Length == 0)
            {
                petName.text = "???";
                rank.gameObject.SetActive(false);
                empty.SetActive(true);
                scrollView.SetActive(false);
            }
            else
            {
                empty.SetActive(false);
                scrollView.SetActive(true);
                ChangePetInfo(body.list[index]);
            }
            
            if (body.list.Length <= 1)
            {
                prevButton.SetActive(false);
                nextButton.SetActive(false);
            }
            else
            {
                prevButton.SetActive(true);
                nextButton.SetActive(true);
            }
            totalCnt = body.list.Length;
        }
        catch (Exception e)
        {
            Toast.Show(e.ToString());
        }
    }

    string GetRank(int rank)
    {
        if (rank == 1)
        {
            return "Lv. 학사";
        }
        else if (rank == 1)
        {
            return "Lv. 석사";
        }
        else
        {
            return "Lv. 박사";
        }
    }

    private void ChangePetInfo(MyPet item)
    {
        petName.text = item.name;
        rank.text = GetRank(item.rank);
        hungry.transform.Find("Text").gameObject.GetComponent<TMP_Text>().text = "배고픔 (" + item.hungry + "%)";
        hungry.transform.Find("Slider").gameObject.GetComponent<Slider>().value = item.hungry / 100F;
        energy.transform.Find("Text").gameObject.GetComponent<TMP_Text>().text = "활력 (" + item.sleep + "%)";
        energy.transform.Find("Slider").gameObject.GetComponent<Slider>().value = item.sleep / 100F;
        happy.transform.Find("Text").gameObject.GetComponent<TMP_Text>().text = "행복도 (" + item.happy + "%)";
        happy.transform.Find("Slider").gameObject.GetComponent<Slider>().value = item.happy / 100F;
        clean.transform.Find("Text").gameObject.GetComponent<TMP_Text>().text = "청결도 (" + item.clean + "%)";
        clean.transform.Find("Slider").gameObject.GetComponent<Slider>().value = item.clean / 100F;
    }

    public void ClickNextButton()
    {
        index = (index + 1) % totalCnt;
        ChangePetInfo(body.list[index]);
    }
    public void ClickPrevButton()
    {
        index = (index - 1 + totalCnt) % totalCnt;
        ChangePetInfo(body.list[index]);
    }

    public async void Feed()
    {
        backgroundPanel.GetComponent<Image>().sprite = feedBackground;
        feedButton.transform.Find("Text").GetComponent<TMP_Text>().text = "밥먹는 중";
        DisableAllButtons();
        await Task.Delay(3000);
        backgroundPanel.GetComponent<Image>().sprite = homeBackground;
        backgroundPanel.transform.Find("PetName").GetComponent<TMP_Text>().color = Color.black;
        backgroundPanel.transform.Find("Rank").GetComponent<TMP_Text>().color = Color.black;
        feedButton.transform.Find("Text").GetComponent<TMP_Text>().text = "밥먹기";
        EnabledAllButtons();
    }

    public async void Sleep()
    {
        backgroundPanel.GetComponent<Image>().sprite = bedroomBackground;
        backgroundPanel.transform.Find("PetName").GetComponent<TMP_Text>().color = Color.white;
        backgroundPanel.transform.Find("Rank").GetComponent<TMP_Text>().color = Color.white;
        sleepButton.transform.Find("Text").GetComponent<TMP_Text>().text = "잠자는 중";
        DisableAllButtons();
        await Task.Delay(3000);
        backgroundPanel.GetComponent<Image>().sprite = homeBackground;
        backgroundPanel.transform.Find("PetName").GetComponent<TMP_Text>().color = Color.black;
        backgroundPanel.transform.Find("Rank").GetComponent<TMP_Text>().color = Color.black;
        sleepButton.transform.Find("Text").GetComponent<TMP_Text>().text = "잠자기";
        EnabledAllButtons();
    }

    public async void Walk()
    {
        backgroundPanel.GetComponent<Image>().sprite = walkBackground;
        // backgroundPanel.transform.Find("PetName").GetComponent<TMP_Text>().color = Color.white;
        // backgroundPanel.transform.Find("Rank").GetComponent<TMP_Text>().color = Color.white;
        walkButton.transform.Find("Text").GetComponent<TMP_Text>().text = "산책 중";
        DisableAllButtons();
        await Task.Delay(3000);
        backgroundPanel.GetComponent<Image>().sprite = homeBackground;
        backgroundPanel.transform.Find("PetName").GetComponent<TMP_Text>().color = Color.black;
        backgroundPanel.transform.Find("Rank").GetComponent<TMP_Text>().color = Color.black;
        walkButton.transform.Find("Text").GetComponent<TMP_Text>().text = "산책하기";
        EnabledAllButtons();
    }

    public async void Shower()
    {
        backgroundPanel.GetComponent<Image>().sprite = showerBackground;
        showerButton.transform.Find("Text").GetComponent<TMP_Text>().text = "목욕 중";
        DisableAllButtons();
        await Task.Delay(3000);
        backgroundPanel.GetComponent<Image>().sprite = homeBackground;
        backgroundPanel.transform.Find("PetName").GetComponent<TMP_Text>().color = Color.black;
        backgroundPanel.transform.Find("Rank").GetComponent<TMP_Text>().color = Color.black;
        showerButton.transform.Find("Text").GetComponent<TMP_Text>().text = "목욕하기";
        EnabledAllButtons();
    }

    private void DisableAllButtons()
    {
        feedButton.GetComponent<Button>().enabled = false;
        sleepButton.GetComponent<Button>().enabled = false;
        walkButton.GetComponent<Button>().enabled = false;
        showerButton.GetComponent<Button>().enabled = false;
        prevButton.GetComponent<Button>().enabled = false;
        nextButton.GetComponent<Button>().enabled = false;
    }

    private void EnabledAllButtons()
    {
        feedButton.GetComponent<Button>().enabled = true;
        sleepButton.GetComponent<Button>().enabled = true;
        walkButton.GetComponent<Button>().enabled = true;
        showerButton.GetComponent<Button>().enabled = true;
        prevButton.GetComponent<Button>().enabled = true;
        nextButton.GetComponent<Button>().enabled = true;
    }
}
