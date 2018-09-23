using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour {

    [SerializeField] Color m_originalColor;
    [SerializeField] Color m_newColor = Color.green;

    [SerializeField] float m_colorCycleSpeed = 2;

    [SerializeField] List<Material> m_affectedMaterials = new List<Material>();
    // Use this for initialization
	void Start () {
        MeshRenderer[] mrs = GetComponentsInChildren<MeshRenderer>();
        if (GetComponent<MeshRenderer>())
        m_affectedMaterials.Add(GetComponent<MeshRenderer>().material);
        foreach (MeshRenderer mr in mrs)
        {
            for (int i = mr.materials.Length -1; i >= 0; i--)
            {
                m_affectedMaterials.Add(mr.materials[i]);
            }
        }
        m_originalColor = m_affectedMaterials[0].color;
	}
	
	// Update is called once per frame
	void Update () {
        foreach (Material mat in m_affectedMaterials)
        {
            mat.color = Color.Lerp(mat.color, m_newColor, m_colorCycleSpeed * Time.deltaTime);
        }
	}

    public void ChangeColorTo(Color c)
    {
        m_newColor = c;
    }

    public void ResetColor()
    {
        m_newColor = m_originalColor;
    }
}
