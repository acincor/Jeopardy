using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{
    protected WorkerController target;
    protected NavMeshAgent agent;
    public BossState bossState;
    protected Danger systemDanger;
    protected GameMode gameMode;
    protected CharacterController controller;
    public float baseMoveSpeed = 5f;
    public float baseRotationSpeed = 10f;
    public float dangerMultiplier = 1f;
    public float mistakeMultiplier = 1f;
    public float moveSpeed => baseMoveSpeed * dangerMultiplier * mistakeMultiplier;
    public float rotationSpeed => baseRotationSpeed * dangerMultiplier * mistakeMultiplier;
    private Coroutine mistakeCoroutine;
    public bool isInZone = false;
    public static readonly List<BossController> All = new List<BossController>();
    void OnDisable()
    {
        All.Remove(this);
    }
    void OnEnable()
    {
        All.Add(this);
    }
    public void Caught() {
        StartCoroutine(Recover(1f));
    }
    private IEnumerator Recover(float time)
    {
        controller.enabled = false;
        yield return new WaitForSeconds(time);
        controller.enabled = true;
    }
    public virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
        agent.updatePosition = false;
        agent.updateRotation = false;
        systemDanger = GameObject.FindObjectOfType<Danger>();
        gameMode = GameObject.FindObjectOfType<GameMode>();
    }
    public virtual void SetState(BossState boss) {
        bossState = boss;
    }
    // ⚠ Danger = 0.0 ~ 1.0
    public void ApplyDanger(float danger)
    {
        if(danger < 0.3f)
            return;
        // 0.3 → 1x, 1.0 → ~2.3x
        float t = Mathf.Clamp01(danger / 0.3f);
        dangerMultiplier = 1f + t * 0.4f;
    }
    // Boss misjudgment
    public void TriggerMistake(float duration)
    {
        if (mistakeCoroutine != null)
            StopCoroutine(mistakeCoroutine);

        mistakeMultiplier = 0.7f; // 30% 操作失误
        mistakeCoroutine = StartCoroutine(ResetMistake(duration));
    }
    private IEnumerator ResetMistake(float time)
    {
        yield return new WaitForSeconds(time);
        mistakeMultiplier = 1f;
        mistakeCoroutine = null;
    }
}
