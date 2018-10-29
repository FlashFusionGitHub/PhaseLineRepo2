using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public int musicBoxIndex;
    private void Start()
    {
        if (FindObjectOfType<MusicBox>())
        FindObjectOfType<MusicBox>().loadMusic(musicBoxIndex);
    }
}
