using UnityEngine;
using System.Collections;
 
//If you're using please put my name in the credit, or link my Youtube page. :)
 
public class Pokeball : MonoBehaviour {
    [SerializeField]
    private float throwSpeed = 150f;
    private float speed;
    private float lastMouseX, lastMouseY;
    public Camera cam;
 
    private bool thrown, holding;
 
    private Rigidbody _rigidbody;
    private Vector3 newPosition;
 
    void Start() {
        _rigidbody = GetComponent<Rigidbody> ();
        Reset ();
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
        // if (holding)
        //     OnTouch ();
 
        // if (thrown)
        //     return;
 
        // if(Input.touchCount == 1) {
        //     Touch touch = Input.GetTouch(0);
            
        //     if (touch.phase == TouchPhase.Began) {
        //         Ray ray = cam.ScreenPointToRay (touch.position);
        //         RaycastHit hit;
    
        //         if (Physics.Raycast (ray, out hit, 100f)) {
        //             if (hit.transform == transform) {
        //                 holding = true;
        //                 transform.SetParent (null);
        //             }
        //         }
        //     } else if (touch.phase == TouchPhase.Ended && holding) {
        //         if (lastMouseY < touch.position.y) {
        //             ThrowBall (touch.position);
        //         }
        //         holding = false;
        //         thrown = true;
        //     }

        //     lastMouseX = touch.position.x;
        //     lastMouseY = touch.position.y;
        // }
    }
 
    public void Reset(){
        CancelInvoke ();
        transform.position = cam.ViewportToWorldPoint (new Vector3 (0.5f, 0.2f, cam.nearClipPlane * 7.5f));
        newPosition = transform.position;
        thrown = holding = false;
 
        _rigidbody.useGravity = false;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.Euler (0f, 200f, 0f);
        transform.SetParent (cam.transform);
        missed = false;
    }
 
    void OnTouch() {
        Vector3 position;

        // Detect if it is touch or mouse click
        bool isTouchDevice = Input.touchCount > 0;

        if (isTouchDevice) {
            position = Input.GetTouch(0).position;
        } else {
            position = Input.mousePosition;
        }
        
        position.z = cam.nearClipPlane * 7.5f;
        newPosition = cam.ScreenToWorldPoint (position);

        transform.localPosition = Vector3.Lerp (transform.localPosition, newPosition, 50f * Time.deltaTime);
        // Vector3 mousePos = Input.GetTouch (0).position;
        // mousePos.z = cam.nearClipPlane * 7.5f;
 
        // newPosition = cam.ScreenToWorldPoint (mousePos);
 
        // transform.localPosition = Vector3.Lerp (transform.localPosition, newPosition, 50f * Time.deltaTime);
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
 
        // float x = (mousePos.x / Screen.width) - (lastMouseX / Screen.width);
        // x = Mathf.Abs (Input.GetTouch (0).position.x - lastMouseX) / Screen.width * 100 * x;
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
        if (collision.transform.tag == "Pet" && !missed) {
            GameObject pet = collision.transform.gameObject;
            StartCoroutine(CatchingPhase(0.5f, pet));
        } else if (collision.transform.tag != "Pet"){
            missed = true;
        }
    }

    IEnumerator CatchingPhase(float chance, GameObject pet) {
        bool caught = (Random.Range (0, 1) < chance);

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
            Debug.Log (pet + " has been captured!");
        } else {
            Debug.Log (pet + " has escaped!");
        }
        // pet.SetActive (true);
        yield break;
    }
}
