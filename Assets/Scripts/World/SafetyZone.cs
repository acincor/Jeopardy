using UnityEngine;
using System.Collections.Generic;
public class SafetyZone : MonoBehaviour
{
    public static readonly List<SafetyZone> All = new List<SafetyZone>();
    [Range(0f, 1f)]
    public float danger;
    public enum Phase
    {
        Stable,
        Pressure,
        Critical,
        Collapse
    }
    void OnDisable()
    {
        All.Remove(this);
    }
    void OnEnable()
    {
        All.Add(this);
    }
    public Phase CurrentPhase { get; private set; }

    public void AddDanger(float rate)
    {
        danger = Mathf.Clamp01(danger + rate * Time.deltaTime);
        Debug.Log($"danger = {danger}");
        UpdatePhase();
    }

    void UpdatePhase()
    {
        Phase newPhase =
            danger < 0.3f ? Phase.Stable :
            danger < 0.6f ? Phase.Pressure :
            danger < 0.85f ? Phase.Critical :
            Phase.Collapse;

        if (newPhase != CurrentPhase)
        {
            Phase old = CurrentPhase;
            CurrentPhase = newPhase;
        }
    }
}
