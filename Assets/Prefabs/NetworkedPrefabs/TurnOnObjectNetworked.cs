using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TurnOnObjectNetworked : NetworkBehaviour {

    [Command]
    void CmdEnableObject(GameObject TurnThisOn)
    {
        TurnThisOn.SetActive(true);
    }

    [Command]
    void CmdDisableObject(GameObject TurnThisOn)
    {
        TurnThisOn.SetActive(false);
    }
}
