using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombArrow : ArrowProjectile
{
    public float radius = 2;
    public float power = 500;
    [Range(1, 10)] 
    public int blinkFrequency = 1;
    public float warmupTime = .5f;
    public List<Renderer> bombRenderers;
    public Material explosiveMaterial;
    public GameObject explosionParticle;
    public LayerMask destructibleLayer;

    List<Material> defaultMaterials;
    Destructable solidObject;
    
    bool triggered = false;
    bool warm = false;

    public override void Fire(Vector3 direction)
    {
        defaultMaterials = new List<Material>();
        foreach(var bomb in bombRenderers)
            defaultMaterials.Add(bomb.material);
        base.Fire(direction);
    }
    
    /// <summary>
    /// 检测碰撞，当碰撞体不是人物时，进入爆炸状态
    /// </summary>
    /// <param name="collision">
    /// 碰撞体
    /// </param>

    protected override void OnCollisionEnter(Collision collision)
    {   
        if(!triggered &&  collision.gameObject.name != "Player")
        {
            solidObject = collision.gameObject.GetComponent<Destructable>();
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rb.isKinematic = true;
            WaitAndExplode();
            triggered = !triggered;
        }
    }

    void WaitAndExplode()
    {
        StartCoroutine(DetonateCoroutine());
    }

    /// <summary>
    ///实现弓箭闪烁、弓箭爆炸闪光特效、爆炸功能
    /// </summary>
    IEnumerator DetonateCoroutine()
    {
        float elapsedTime = 0;
        float frameCount = 0;

        while (elapsedTime <= lifeTime)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            frameCount ++;

            //Math.PingPong实现每20帧进行一次闪烁
            var blink = Mathf.PingPong(frameCount * Time.timeScale, 10 / blinkFrequency);

            //弓箭闪烁
            if(blink == 10 / blinkFrequency)
                ToggleMaterial();

            //爆炸前的闪光特效
            if(elapsedTime >= lifeTime - warmupTime && !warm)
                PrewarmExplosion();   

            //实现爆炸
            if(elapsedTime >= lifeTime)
                Detonate();
        }
    }

    /// <summary>
    /// 弓箭随机闪烁
    /// </summary>
    void ToggleMaterial()
    {
        if(explosiveMaterial != null)
        {
            var randomN = Random.Range(0, bombRenderers.Count);
            bombRenderers[randomN].material = bombRenderers[randomN].material == defaultMaterials[randomN] ? explosiveMaterial : defaultMaterials[randomN];
        }
    } 

    /// <summary>
    /// 爆炸闪烁光
    /// </summary>
    void PrewarmExplosion()
    {   
        warm = true;
        if (explosionParticle != null)
        {   
            var particle = Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
            particle.SetActive(true);
            Destroy(particle, 3);
        }
    } 

    /// <summary>
    /// 弓箭进行爆炸
    /// </summary>
    void Detonate()
    {
        Vector3 explosionPos = transform.position;

        if(solidObject != null)
        {
            solidObject.Explode();
        }

        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius, destructibleLayer);
        
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                var realPower = power /Time.timeScale;
                rb.AddExplosionForce(realPower, explosionPos, radius, 1);
            }
        }

        if(source != null)
        { 
            source.GenerateImpulse();
        }

        Destroy(gameObject);
    }


}
