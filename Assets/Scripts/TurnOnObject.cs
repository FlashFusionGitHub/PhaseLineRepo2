using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Simple class that can turn objects on or off*/
public class TurnOnObject : MonoBehaviour {

    public void turnOnObject(GameObject TurnThisOn)
    {
        TurnThisOn.SetActive(true);
    }

    public void turnOffObject(GameObject TurnThisOff)
    {
        TurnThisOff.SetActive(false);
    }
}
