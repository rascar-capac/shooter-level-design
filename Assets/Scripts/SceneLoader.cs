using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string[] ScenesToLoadAtStart;

    void Start()
    {
#if UNITY_EDITOR
        return;
#endif
        foreach (string s in ScenesToLoadAtStart)
        {
            SceneManager.LoadScene(s, LoadSceneMode.Additive);
        }
    }

}
