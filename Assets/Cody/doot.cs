using UnityEngine;
using System.Collections;

public class doot : MonoBehaviour {


	public GameObject playEvent1;


    public GameObject start;
    public GameObject play;
    public GameObject playtwo;


	void Start()
	{
        play.SetActive(false);

        if (playtwo != null)
            playtwo.SetActive(false);

	}

	void OnTriggerEnter(Collider col)

	{
        //Debug.Log("i hit this bitch");
		if (col.gameObject.tag == "next")
		{
            play.SetActive(true);

            if (playtwo != null)
                playtwo.SetActive(true);

		}
	}
}