using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;
 
public class Pokeball : MonoBehaviour {
    [SerializeField]
    private float throwSpeed = 150f;
    private float speed;
    private float lastMouseX, lastMouseY;

    public Camera cam;
    public GameObject panel; // Assign your Panel object in the inspector
 
    private bool thrown, holding;
 
    private Rigidbody _rigidbody;
    private Vector3 newPosition;
    public GameObject toastObject; // Assign your Toast UI object in the inspector
    public TextMeshProUGUI captureText; // Assign the Text component of the Toast object
    public TextMeshProUGUI petname;
    public RawImage img;
    public TextMeshProUGUI petrank;
    public TextMeshProUGUI hungry;
    public TextMeshProUGUI energy;
    public TextMeshProUGUI happy;
    public TextMeshProUGUI clean;
    public TextMeshProUGUI locationText;
    
    public AudioManager audioManager;

    // This function shows a toast message
    public void ShowToast(string message, int duration)
    {
        captureText.text = message;
        toastObject.SetActive(true);
        StartCoroutine(DisableToastAfterTime(duration));
    }

    public void ShowPanel()
    {
        audioManager.StopBGM();
        WildPetInfo.GetData();
        petname.text = WildPetInfo.getdata.pet.name;
        StartCoroutine(GetTexture(img, WildPetInfo.getdata.pet.rank));
        petrank.text = WildPetInfo.getdata.pet.rank.ToString();
        petrank.text = "희귀도. " + getRank(WildPetInfo.getdata.pet.rank);
        if (WildPetInfo.getdata.pet.rank == 1) {
            hungry.text = "배고픔: 70";
            energy.text = "활력: 70";
            happy.text = "행복도: 70";
            clean.text = "청결도: 70";
        } else if (WildPetInfo.getdata.pet.rank == 2) {
            hungry.text = "배고픔: 60";
            energy.text = "활력: 60";
            happy.text = "행복도: 60";
            clean.text = "청결도: 60";
        } else if(WildPetInfo.getdata.pet.rank == 3) {
            hungry.text = "배고픔: 50";
            energy.text = "활력: 50";
            happy.text = "행복도: 50";
            clean.text = "청결도: 50";
        }
        locationText.text = WildPetInfo.getdata.location.location + "에서 잡았습니다!";
        AddMypet.AddToMyPet(WildPetInfo.petId, WildPetInfo.locationId);
        panel.SetActive(true);
    }

    IEnumerator GetTexture(RawImage img, int rank)
    {
        string imgUrl = "https://imgur.com/d9rMHfO.png";
        if (rank == 1) {
            imgUrl = "https://imgur.com/YfygwQt.png";
        } else if (rank == 2) {
            imgUrl = "https://imgur.com/Ihmm87C.png";
        } else if(rank == 3) {
            imgUrl = "https://imgur.com/nzGQvWG.png";
        }

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(imgUrl);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        Stream responseStream = response.GetResponseStream();
        byte[] buffer = new byte[16 * 1024];
        using (MemoryStream ms = new MemoryStream())
        {
            int read;
            while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            byte[] imageBytes = ms.ToArray();

            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);

            img.texture = texture;
        }
        yield return null;
    }

    private string getRank(int rank)
    {
        if (rank == 1) return "common";
        else if (rank == 2) return "rare";
        else return "epic";
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }

    IEnumerator DisableToastAfterTime(int time)
    {
        yield return new WaitForSeconds(time);
        toastObject.SetActive(false);
    }
 
    void Start() {
        _rigidbody = GetComponent<Rigidbody> ();
        Reset ();
        HidePanel();
        toastObject.SetActive(false);  // 비활성화 코드 추가
    }
 
    void Update() {
        if (holding)
            OnTouch ();
    
        if (thrown)
            return;

        // Detect if it is touch or mouse click
        bool isTouchDevice = Input.touchCount > 0;

        // Use touch for mobile, mouse click for editor
        if(isTouchDevice) {
            Touch touch = Input.GetTouch(0);

            ProcessTouch(touch.position, touch.phase);
        }
        else {
            if(Input.GetMouseButtonDown(0)) {
                ProcessTouch(Input.mousePosition, TouchPhase.Began);
            }
            if(Input.GetMouseButtonUp(0)) {
                ProcessTouch(Input.mousePosition, TouchPhase.Ended);
            }
            if(Input.GetMouseButton(0)) {
                ProcessTouch(Input.mousePosition, TouchPhase.Moved);
            }
        }
    }
 
    public void Reset(){
        CancelInvoke ();
        transform.position = cam.ViewportToWorldPoint (new Vector3 (0.5f, 0.2f, cam.nearClipPlane * 7.5f));
        newPosition = transform.position;
        thrown = holding = false;
 
        _rigidbody.useGravity = false;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.Euler (0f, 90f, 0f);
        transform.SetParent (cam.transform);
        missed = false;
    }
 
    void OnTouch() {
        Vector3 position;

        bool isTouchDevice = Input.touchCount > 0;

        if (isTouchDevice) {
            position = Input.GetTouch(0).position;
        } else {
            position = Input.mousePosition;
        }
        
        position.z = cam.nearClipPlane * 7.5f;
        newPosition = cam.ScreenToWorldPoint (position);

        transform.localPosition = Vector3.Lerp (transform.localPosition, newPosition, 50f * Time.deltaTime);
    }

    void ProcessTouch(Vector2 position, TouchPhase phase) {
        if (phase == TouchPhase.Began) {
            Ray ray = cam.ScreenPointToRay (position);
            RaycastHit hit;

            if (Physics.Raycast (ray, out hit, 100f)) {
                if (hit.transform == transform) {
                    holding = true;
                    transform.SetParent (null);
                }
            }
        } else if (phase == TouchPhase.Ended && holding) {
            if (lastMouseY < position.y) {
                ThrowBall (position);
            }
            holding = false;
            thrown = true;
        }

        lastMouseX = position.x;
        lastMouseY = position.y;
    }
 
    void ThrowBall(Vector2 mousePos) {
        _rigidbody.useGravity = true;
 
        float differenceY = (mousePos.y - lastMouseY) / Screen.height * 100;
        speed = throwSpeed * Mathf.Sqrt(differenceY / Screen.height) * 120;
 
        float x = (mousePos.x / Screen.width) - (lastMouseX / Screen.width);

        // Check if it is touch device or not
        bool isTouchDevice = Input.touchCount > 0;
        
        if (isTouchDevice) {
            x = Mathf.Abs (Input.GetTouch (0).position.x - lastMouseX) / Screen.width * 100 * x;
        } else {
            x = Mathf.Abs (mousePos.x - lastMouseX) / Screen.width * 100 * x;
        }
 
        Vector3 direction = new Vector3 (x, 0f, 1f);
        direction = cam.transform.TransformDirection (direction);
 
        _rigidbody.AddForce((direction * speed / 2f) + (Vector3.up * speed));
 
        holding = false;
        thrown = true;
 
        Invoke ("Reset", 5.0f);
    }

    bool missed = false;

    void OnCollisionEnter(Collision collision) {
        GetComponent<AudioSource>().Play();
        if (collision.transform.tag == "Pet" && !missed) {
            GameObject pet = collision.transform.gameObject;
            StartCoroutine(CatchingPhase(0.5f, pet));
        } else if (collision.transform.tag != "Pet"){
            missed = true;
        }
    }

    IEnumerator CatchingPhase(float chance, GameObject pet) {
        bool caught = (Random.Range (0f, 1f) < chance);
        // bool caught = false;
        // bool caught = true;

        pet.SetActive (false);

        _rigidbody.AddForce(Vector3.up * 2.0f);
        yield return new WaitForSeconds (0.25f);
        _rigidbody.isKinematic = true;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        yield return new WaitForSeconds (0.25f);
        _rigidbody.isKinematic = false;
        yield return new WaitForSeconds (1f);
        if (caught) {
            ShowToast(pet.name + "\n를 잡았습니다!", 3);
            yield return new WaitForSeconds (1f);
            Debug.Log (pet.name + " has been captured!");
            ShowPanel();
            this.gameObject.SetActive(false);
        } else {
            Reset();
            ShowToast(pet.name + "\n가 탈출했습니다!", 3);
            Debug.Log (pet.name + " has escaped!");
            pet.SetActive (true);
        }
        yield break;
    }
    
}