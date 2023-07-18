using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThrowBall : MonoBehaviour
{
    public void GoToThrowBallScene()
    {
        SceneManager.LoadScene("ThrowBall_1");
    }
}
