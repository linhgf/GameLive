using System.Collections;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public AudioSource impactAudio;
    public float force;
    public float lifeTime = 10;
    protected Cinemachine.CinemachineImpulseSource source;
    protected Rigidbody rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = transform.position;
    }

    void Start() {
        force = 100 * Random.Range(1.3f, 1.7f);
    }

    /// <summary>
    /// 发射弓箭
    /// </summary>
    /// <param name="direction">
    /// 弓箭飞行方向
    /// </param>
    public virtual void Fire(Vector3 direction)
    {
        rb.AddForce(direction * force, ForceMode.Impulse);

        //触发镜头晃动
        source = GetComponent<Cinemachine.CinemachineImpulseSource>();
        source.GenerateImpulse(Camera.main.transform.forward);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {   
        if (collision.gameObject.name != "Player")
        {
            //播放碰撞声
            PlayImpactSound();

            //取消CD（continuous dynamic）检测模式 启用离散模式 避免占用
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rb.isKinematic = true;
            StartCoroutine(Countdown());
        }
    }

    public virtual void PlayImpactSound()
    {
        //计算距离 调整音量 远小近大
        Transform player = GameObject.Find("Player").transform;
        float distance = Vector3.Distance(transform.position, player.position);
        impactAudio.volume = Mathf.Clamp(2 / distance, 0, 1);
        impactAudio.Play();
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }


}
