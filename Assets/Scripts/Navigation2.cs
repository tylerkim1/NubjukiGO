using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigation2 : MonoBehaviour
{
    public void Navigate(string scene)
    {
        SceneManager.LoadScene(scene);

    }
}
