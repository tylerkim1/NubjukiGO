using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoMaporHome : MonoBehaviour
{
    // Start is called before the first frame update
    public void GotoMap() {
        SceneManager.LoadScene("Location-basedGame");
    }

    public void GotoHome() {
        SceneManager.LoadScene("Home");
    }
}
