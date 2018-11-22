using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    /*The music mangager class, if the MusicBoc compnent doesnt exist then music wont play*/
    public int musicBoxIndex;
    private void Start()
    {
        if (FindObjectOfType<MusicBox>())
        FindObjectOfType<MusicBox>().loadMusic(musicBoxIndex);
    }
}
