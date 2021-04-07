using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    private Animator m_anim;
    private AnimatorStateInfo info;
    private Rigidbody2D m_rb;

    [SerializeField]
    private float beat_back_force;
    [SerializeField]
    private Animator hit_fx;
    private bool isHit;
    private Vector2 hit_direction;
    // Start is called before the first frame update
    void Start()
    {
        m_anim = GetComponent<Animator>();
        m_rb = GetComponent<Rigidbody2D>();
        hit_fx = transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        info = m_anim.GetCurrentAnimatorStateInfo(0);
        if(isHit){
            m_rb.velocity = hit_direction.normalized * beat_back_force;
            if(info.normalizedTime > 0.6f){
                isHit = false;
            }
        }
    }

    public void GetHit(Vector2 direction){
        hit_direction = direction;
        hit_fx.SetTrigger("toHurt");
        if(direction.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);

        isHit = true;
        m_anim.SetTrigger("toHurt");
    }
    
}
