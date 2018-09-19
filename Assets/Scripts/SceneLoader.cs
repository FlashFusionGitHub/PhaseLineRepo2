using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;
using UnityEngine.UI;

public enum Scenes { MENU = 0, SPLITSCREEN, ONLINE, SETTINGS };

public class SceneLoader : MonoBehaviour {

    public InputDevice m_controller;

    public Text m_textBox;

    public GameObject lobby;

    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void DestoryLobby()
    {
        Destroy(lobby);
    }

    public IEnumerator FlashText(string warning)
    {
        while(true)
        {
            m_textBox.text = warning;

            yield return new WaitForSeconds(0.5f);
            
            m_textBox.text = "";
            
            break;
        }
    }
}
