using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAnimationController : MonoBehaviour {

	public Animator[] childAnimators;
	public bool findAnimatorsButton;
	public void callTrigger(string trig)
	{
		foreach (Animator anim in childAnimators)
		{
			anim.SetTrigger (trig);
		}
	}

	public void findAnimators ()
	{
		childAnimators = GetComponentsInChildren<Animator> ();
	}

	void OnDrawGizmos()
	{
		if (findAnimatorsButton) 
		{
			findAnimators ();
			findAnimatorsButton = false;
		}
	}
}
