using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SimpleAnimate : MonoBehaviour {

    [System.Serializable]
    public class CursorPart
    {
        public Transform part;
        public Vector3 rotation;
        public Vector3 originalLocalPosition;
        public bool originSet;
        public float bobbingHeight;
        public float bobIntensity;
        public bool movingUp;
    }

    Vector3 originSize;
    float m_timer;
    public float timeToGrow = 0.5f;
    public CursorPart[] cursorParts;
    public float speedModifier;

    private void Start()
    {
        originSize = transform.localScale;
        transform.localScale = Vector3.zero;
        m_timer = 0;
    }
    // Update is called once per frame
    void Update () {

        if (m_timer <= 1)
        {
            m_timer += Time.deltaTime / timeToGrow;
            transform.localScale = Vector3.Lerp(Vector3.zero, originSize, m_timer);
        }

        foreach (CursorPart cp in cursorParts)
        {
            if (cp.part)
            {
               
                if (!cp.originSet)
                {
                    cp.originalLocalPosition = cp.part.localPosition;
                    cp.originSet = true;
                }
                else
                {
                    if (cp.movingUp)
                    {
                        if (cp.part.localPosition.y < cp.originalLocalPosition.y + cp.bobbingHeight)
                        {
                            cp.part.localPosition += new Vector3(0, cp.bobIntensity * speedModifier * Time.deltaTime, 0);
                        }
                        else
                        {
                            cp.movingUp = false;
                        }
                    }
                    else
                    {
                        if (cp.part.localPosition.y > cp.originalLocalPosition.y - cp.bobbingHeight)
                        {
                            cp.part.localPosition -= new Vector3(0, cp.bobIntensity * speedModifier * Time.deltaTime, 0);
                        }
                        else
                        {
                            cp.movingUp = true;
                        }
                    }
                }
              cp.part.Rotate(cp.rotation * speedModifier * Time.deltaTime);
            }
        }
	}

    private void OnDisable()
    {
        transform.localScale = Vector3.zero;
        m_timer = 0;
    }
}
