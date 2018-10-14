using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour {

    public GameObject lobby;

    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void DestoryLobby()
    {
        Destroy(lobby);
    }
}
