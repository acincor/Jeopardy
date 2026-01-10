using UnityEngine;
using System.Collections.Generic;
using System;
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
    public Phase CurrentPhase { get; private set; }
    void OnDisable()
    {
        All.Remove(this);
    }
    void OnEnable()
    {
        All.Add(this);
    }
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
    private void OnTriggerEnter(Collider other)
    {
        if(Time.timeScale == 0f)
            return;
        Debug.Log($"enter = {other.tag}");
        if (other.CompareTag("Boss"))
        {
            BossController boss = other.GetComponentInParent<BossController>();
            boss.ApplyDanger(danger);
            boss.isInZone = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(Time.timeScale == 0f)
            return;
        if (other.CompareTag("Boss"))
        {
            BossController boss = other.GetComponentInParent<BossController>();
            boss.isInZone = false;
        }
    }
}
