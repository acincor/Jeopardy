using UnityEngine;

public class BossTestMove : MonoBehaviour
{
    CharacterController cc;
    public float speed = 5f;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0f, v);
        cc.Move(move * speed * Time.deltaTime);
    }
}
