using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class PickUpBehaviour : MonoBehaviour {

    public float maxTimeBeforelanding = 5;
    [Header ("Animation Blend")]
    public Animator anim;
    public bool falling = true;
    [Range (0,1)]
    public float fallPercent = 0;

    public float maxDistance;
    Vector3 fallTarget;
    public LayerMask rayMask;

    public UnityEvent OnLanding;

    [Header ("Components")]
    public GameObject chute;
    public float shrinkingTime = 0.5f;
    float shrinkTimer =1;
    Vector3 oldscale;
    public GameObject box;
    Rigidbody rb;

    private void Start()
    {
        SetMaxDistance();
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (falling)
        {
            if (maxTimeBeforelanding < 0)
            {
                transform.position = fallTarget;
                OnLanding.Invoke();
                shrinkTimer = 1;
                fallPercent = 0;
                falling = false;
                oldscale = chute.transform.localScale;
                rb.isKinematic = true;
            }
            else
            {
                maxTimeBeforelanding -= Time.deltaTime;
            }
            CalcDistanceTo01();
            anim.SetFloat("Blend", fallPercent);
            anim.speed = fallPercent + 0.25f;
        }
        else
        {
            if (shrinkTimer >= 0)
            {
                shrinkTimer -= Time.deltaTime / shrinkingTime;
                chute.transform.localScale = Vector3.Lerp(Vector3.zero, oldscale, shrinkTimer);
                
            }
            else
            {
                chute.SetActive(false);
            }
        }
    }
    void SetMaxDistance()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 10000, rayMask))
        {
            maxDistance = Vector3.Distance(hit.point, transform.position);
            fallTarget = hit.point;
        }
    }

    void CalcDistanceTo01()
    {
        fallPercent = Vector3.Distance(transform.position, fallTarget) / maxDistance;
        if (fallPercent <= 0.005)
        {
            OnLanding.Invoke();
            shrinkTimer = 1;
            fallPercent = 0;
            falling = false;
            oldscale = chute.transform.localScale;
            rb.isKinematic = true;
        }
    }
}
