using System.Collections;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
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

    public virtual void Fire(Vector3 direction)
    {
        rb.AddForce(direction * force, ForceMode.Impulse);
        source = GetComponent<Cinemachine.CinemachineImpulseSource>();
        source.GenerateImpulse(Camera.main.transform.forward);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {   
        if (collision.gameObject.name != "Player")
        {
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rb.isKinematic = true;
            StartCoroutine(Countdown());
        }
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }


}
