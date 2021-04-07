using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private Rigidbody2D m_rb;
    private Animator m_anim;

    private int light_combo;//轻攻击连击
    [SerializeField]
    private float light_pasue;
    [SerializeField]
    private float light_strength;
    private int heavy_combo;//重攻击连击
    [SerializeField]
    private float heavy_pasue;
    [SerializeField]
    private float heavy_strength;
    [SerializeField]
    private float shake_duration;
    private string attack_state;//当前攻击状态
    private bool isAttack;
    [SerializeField]
    private float interval;//攻击间隔设置
    [SerializeField]
    private AudioSource light_attack_fx;
    private float attack_timer;//攻击计时器

    [SerializeField]
    private float attack_force;//控制角色攻击时发生的位移

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        m_rb = GetComponent<Rigidbody2D>();
        m_anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
    }

    private void FixedUpdate() {
        Move();
    }

    void Move()
    {
        if(!isAttack){
            float inputX =  Input.GetAxis("Horizontal");

            if(inputX * transform.localScale.x > 0){
                transform.localScale = new Vector2(-transform.localScale.x, 1);
            }
            
            m_anim.SetFloat("toRun", Mathf.Abs(inputX));

            m_rb.velocity = new Vector2(inputX * speed, m_rb.velocity.y);
        }
        else{
            if(attack_state.Equals("heavy_attack"))
                m_rb.velocity = new Vector2(-transform.localScale.x * attack_force * 0.1f, m_rb.velocity.y);
            else
                m_rb.velocity = new Vector2(-transform.localScale.x * attack_force, m_rb.velocity.y);
        }
        
    }

    void Attack(){
        if(Input.GetMouseButtonDown(0) && !isAttack){
            attack_state = "light_attack";
            light_attack_fx.Play();
            isAttack = true;
            //连击数加1
            light_combo++;
            if(light_combo > 3)
                light_combo = 1;
            attack_timer = interval;
            m_anim.SetTrigger("LightAttack");
            m_anim.SetInteger("LightAttackCombo", light_combo);
        }

        //重攻击
        if(Input.GetMouseButtonDown(1) && !isAttack){
            attack_state = "heavy_attack";
            isAttack = true;
            //连击数加1
            heavy_combo++;
            if(heavy_combo > 3)
                heavy_combo = 1;
            attack_timer = interval;
            m_anim.SetTrigger("HeavyAttack");
            m_anim.SetInteger("HeavyAttackCombo", heavy_combo);
        }

        //计时器 判断是否进行连击动作
        if(attack_timer != 0){
            attack_timer -= Time.deltaTime;
            if(attack_timer <= 0){
                //将combo重置为0
                light_combo = 0;
                heavy_combo = 0;
                attack_timer = 0;
            }
        }
    }

    /// <summary>
    /// 阶段攻击结束 将isAttack设为false，即可进行下次攻击，由动画调用
    /// </summary>
    public void AttackOver(){
        isAttack = false;
    }

    /// <summary>
    /// 判断敌人是否进入攻击范围
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag.Equals("Enemy")){
            Vector2 direction = other.transform.position - transform.position;
            other.GetComponent<EnemyController>().GetHit(direction);
            
            if(attack_state == "light_attack"){
                AttackSense.GetInstance().ShakeCamera(shake_duration, light_strength);
                AttackSense.GetInstance().HitPause(light_pasue);
            }

            else if(attack_state == "heavy_attack"){
                AttackSense.GetInstance().ShakeCamera(shake_duration, heavy_strength);
                AttackSense.GetInstance().HitPause(heavy_pasue);
            }
            
        }
    }

}
