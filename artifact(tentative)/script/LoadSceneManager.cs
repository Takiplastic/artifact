using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadSceneManager : MonoBehaviour
{
    public void MoveToStartScene()
    {
        SceneManager.LoadScene("Start");
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
