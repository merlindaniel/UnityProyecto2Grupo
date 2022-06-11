using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Dragon : MonoBehaviour
{
    private Animator animator;
    private AnimatorOverrideController animatorOverrideController;

    public GameObject fireball;

    private Transform target;

    public float maxDistance = 100f;

    public float impulseZ = 80f;
    public float impulseY = 500f;

    public float attackFrequencySeconds = 10f;
    private float attackTimer = 0f;

    public bool allowMovement = true;
    public bool allowRotation = true;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Fly", true);
        rb = GetComponent<Rigidbody>();

        GetComponent<Renderer>().materials[1].color = new Color(Random.value, Random.value, Random.value);

        SelectTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (allowRotation)        
        {
            if (rb.isKinematic)
                rb.isKinematic = false;
            Rotation();
        }

        if (allowMovement)
        {
            if (rb.isKinematic)
                rb.isKinematic = false;

            Movement();
        }
        
        if (!rb.isKinematic && !allowRotation && !allowMovement)
            rb.isKinematic = true;
        
        if (target != null)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer > attackFrequencySeconds)
            {
                attackTimer = 0f;
                FireBallAttack();
            }
        }
    }

    private void Rotation()
    {
        if (target != null)
            transform.LookAt(target);
        else
            SelectTarget();
    }

    private void Movement()
    {
        if (target == null)
            SelectTarget();

        if (target == null) return; // No hay npcs

        float distanceY = Mathf.Abs(transform.position.y - target.position.y);
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y >= 0 ? Mathf.Min(rb.velocity.y, 10) : Mathf.Max(rb.velocity.y, -10), rb.velocity.z);
        if (transform.position.y < target.position.y + 20)
        {
            rb.AddForce(Vector3.up * impulseY, ForceMode.Impulse);
        }
        // else
        // {
        //     rb.AddForce(Vector3.down * impulseY * (1 + distanceY / 100), ForceMode.Impulse);
        // }

        Vector3 targetPositionXZ = new Vector3(target.position.x, 0f, target.position.z);
        float distanceXZ = Mathf.Abs(Vector3.Distance(targetPositionXZ, new Vector3(transform.position.x, 0, transform.position.z)));
        if (Mathf.Abs(rb.velocity.z) > 50)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
        }
        
        if (distanceXZ > maxDistance - 20)
        {
            rb.AddRelativeForce(Vector3.forward * impulseZ * (1 + (distanceXZ / 100)), ForceMode.Impulse);
        }
        else
        {
            rb.AddRelativeForce(Vector3.back * impulseZ * (1 + (distanceXZ / 100)), ForceMode.Impulse);
        }
    }

    void FireBallAttack()
    {
        Vector3 fireballSpawnPosition = transform.position + transform.forward * 1.5f + transform.up * 1.5f;
        GameObject instanceFireball = Instantiate(fireball, fireballSpawnPosition, transform.rotation);
        FireballAI fireballAI = instanceFireball.GetComponent<FireballAI>();
        fireballAI.SetTarget(target);
        fireballAI.SetToLaunch();
    }

    void SelectTarget()
    {
        // Seleccionar un objetivo
        JumpingNPC[] npcs = FindObjectsOfType<JumpingNPC>();
        if (npcs.Length > 0)
        {
            target = npcs[Random.Range(0, npcs.Length)].transform;
            print("TARGET -> " + target);
        }
    }
}
