using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ZoneControllerNetworked : MonoBehaviour {

    public List<CaptureZoneActorNetworked> zones;

    // Use this for initialization
    void Start()
    {
        zones = GetComponentsInChildren<CaptureZoneActorNetworked>().ToList();
    }
}
