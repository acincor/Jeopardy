using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneVisual : MonoBehaviour
{
    private SafetyZone zone;
    private MeshRenderer mesh;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        zone = GetComponentInParent<SafetyZone>();
    }
    
    private Color startColor = Color.green;
    private Color endColor = Color.red;
    
    void Update()
    {
        if(Time.timeScale == 0f)
            return;
        if(zone.danger < 0.3)
            return;
        Color currentColor = Color.Lerp(startColor, endColor, zone.danger);
        currentColor.a = mesh.material.color.a;
        mesh.material.color = currentColor;
    }
}
