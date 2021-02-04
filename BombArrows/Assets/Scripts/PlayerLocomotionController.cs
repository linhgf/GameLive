using System.Collections;
using UnityEngine;

public class PlayerLocomotionController : MonoBehaviour
{
    private Animator _animator;
    private PlayerMovementInputController _movement;

    private Vector2 smoothDeltaPosition = Vector2.zero;
    public Vector2 velocity = Vector2.zero;
    public float magnitude = 0.25f;

    public bool shouldMove;
    public bool shouldTurn;
    public float turn;

    public GameObject look;

    public GameObject arrow;

    public Transform arrowBone;
    public GameObject arrowPrefab;
    public GameObject bombPrefab;

    public Camera cam;
    public LayerMask playerMask;
  
    private void OnEnable()
    {
        _movement = GetComponent<PlayerMovementInputController>();
        _animator = GetComponent<Animator>();
    }

    public void Update()
    {
        Vector3 worldDeltaPosition = _movement.nextPosition - transform.position;

        //Map to local space
        float dX = Vector3.Dot(transform.right, worldDeltaPosition);
        float dY = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dX, dY);

        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        if (Time.deltaTime > 1e-5f)
        {
            velocity = smoothDeltaPosition / Time.deltaTime;
        }

        shouldMove = _movement._move.x != 0 || _movement._move.y != 0;

        bool isAiming = (_movement.aimValue == 1f);

        if (isAiming)
        {
            if ((_movement.fireValue == 1f))
            {
                _animator.SetTrigger("Fire");
                arrow.SetActive(false);
                StartCoroutine(ThrowProjectile(arrowPrefab));
            }
            else if ((_movement.bombValue == 1f))
            {
                _animator.SetTrigger("Fire");
                arrow.SetActive(false);
                StartCoroutine((ThrowProjectile(bombPrefab)));
            }


            if (_animator.GetCurrentAnimatorStateInfo(2).IsName("Fire"))
            {
                arrow.SetActive(false);
            }
            else
            {
                arrow.SetActive(true);
            }

         
            _movement.fireValue = 0f;
            _movement.bombValue = 0f;

        }
        _animator.SetBool("IsAiming", isAiming);
        _animator.SetBool("IsMoving", shouldMove);
        _animator.SetFloat("VelocityX", Mathf.Lerp(_animator.GetFloat("VelocityX"),_movement._move.x, .2f));
        _animator.SetFloat("VelocityY", Mathf.Lerp(_animator.GetFloat("VelocityY"), _movement._move.y, .2f));

    }

    private void OnAnimatorMove()
    {
        //Update the position based on the next position;
        transform.position = _movement.nextPosition;
    }

    [Range( 0, 1f)]
    public float distanceToGround;

    private void OnAnimatorIK(int layerIndex)
    {
        if (_animator && layerIndex == 2)
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, _animator.GetFloat("IKLeftFootWeight"));
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, _animator.GetFloat("IKLeftFootWeight"));
            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, _animator.GetFloat("IKRightFootWeight"));
            _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, _animator.GetFloat("IKRightFootWeight"));

            int mask = ~playerMask.value;

            //Left Foot
            RaycastHit hit;
            Ray ray = new Ray(_animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
            if (Physics.Raycast(ray, out hit, distanceToGround + 1f, mask))
            {
                if (hit.transform.CompareTag("Walkable"))
                {
                    Vector3 footPosition = hit.point;
                    footPosition.y += distanceToGround;
                    _animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                    _animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hit.normal));
                }
            }

            //Right Foot
            ray = new Ray(_animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
            if (Physics.Raycast(ray, out hit, distanceToGround + 1f, mask))
            {
                if (hit.transform.CompareTag("Walkable"))
                {
                    Vector3 footPosition = hit.point;
                    footPosition.y += distanceToGround;
                    _animator.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                    _animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, hit.normal));
                }
            }
        }
    }

    [SerializeField]
    private Transform fireTransform;

    Vector3 calculateDirection(float force)
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 maxTarget = ray.origin + ray.direction * force;
        var heading = maxTarget - fireTransform.position;
        var distance = heading.magnitude;
        var direction = heading / distance;

        int mask = ~ playerMask.value;
        RaycastHit hitPoint;
        if( Physics.Raycast(ray.origin, ray.direction, out hitPoint, force, mask))
        {   
            heading = hitPoint.point - fireTransform.position;
            distance = heading.magnitude;
            direction = heading / distance;
        }

        Debug.DrawRay(ray.origin, direction * 10, Color.yellow, 2);

        return direction;
    }

    IEnumerator ThrowProjectile(GameObject prefab)
    {
        GameObject projectile = Instantiate(prefab);
        ArrowProjectile arrowProjectile = projectile.GetComponent<ArrowProjectile>();

        Vector3 direction = calculateDirection(arrowProjectile.force);

        projectile.transform.position = fireTransform.position + direction;
        projectile.transform.forward = direction;

        //Wait for the position to update
        yield return null;

        arrowProjectile.Fire(direction);
    }
}
