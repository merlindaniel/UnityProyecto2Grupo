using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Dragon : MonoBehaviour
{
    private Animator animator;
    private AnimatorOverrideController animatorOverrideController;
    private Rigidbody rb;

    public GameObject fireball;

    [SerializeField] Transform target;

    public float maxDistance = 50f;
    public float maxVelocityY = 50f, maxVelocityX = 10f, maxVelocityZ = 80f;

    public float impulseZ = 80f;
    public float impulseY = 500f;

    public bool allowMovement = true;
    public bool allowRotation = true;
    
    public int maxFireballsAtOnce = 1;
    public float attackFrequencySeconds = 10f;
    private float attackTimer = 0f;

    private float timeUntilY = 1f;
    private float timerY = 0f;

    private float timeUntilX = 2f;
    private float timerX = 0f;

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
        if (target != null && !target.GetComponent<JumpingNPC>().isBeingChased())
            target = null;

        // Si no hay target o este ya ha terminado el circuito, seleccionar otro target
        if (target == null || target.GetComponent<JumpingNPC>().IsFinished())
            SelectTarget();

        // Permitir movimiento / rotación
        if (!rb.isKinematic && !allowRotation && !allowMovement)
            rb.isKinematic = true;
        else if (rb.isKinematic && (allowRotation || allowMovement))
            rb.isKinematic = false;

        if (target != null)
        {
            // Rotar dragon
            if (allowRotation)        
                Rotation();
            
            // Mover dragon
            if (allowMovement)
                Movement();

            // Lanzar bolas de fuego
            attackTimer += Time.deltaTime;
            if (attackTimer > attackFrequencySeconds)
            {
                attackTimer = 0f;
                FireballAttack();
            }
        }
        else // Si no hay target, no se mueve ni rota
        {
            allowMovement = false;
            allowRotation = false;
        }
    }

    private void Rotation()
    {
        transform.LookAt(target);
    }

    private void Movement()
    {
        Vector3 targetPositionXZ = new Vector3(target.position.x, 0f, target.position.z);
        float distanceXZ = Mathf.Abs(Vector3.Distance(targetPositionXZ, new Vector3(transform.position.x, 0, transform.position.z)));
        float distanceY = Mathf.Abs(transform.position.y - target.position.y);

        timerY += Time.deltaTime;
        if (timerY > timeUntilY)
        {
            timerY = 0f;
            // Impulsar en eje Y
            if (transform.position.y < target.position.y - maxDistance || rb.velocity.y < -maxVelocityY)
            {
                rb.AddForce(Vector3.up * impulseY * (1 + (distanceY / 100) * (rb.velocity.y < 0 ? 1 + (-rb.velocity.y / 100) : 1)), ForceMode.Impulse);
            }
        }

        timerX += Time.deltaTime;
        if (timerX > timeUntilX)
        {
            timerX = 0f;
            // Impulsar en eje Z local, segun la distanciaXZ
            if (distanceXZ > maxDistance)
            {
                rb.AddRelativeForce(Vector3.forward * impulseZ * (1 + (distanceXZ / 100)), ForceMode.Impulse);
            }
            else if (distanceXZ < -maxDistance)
            {
                rb.AddRelativeForce(Vector3.back * impulseZ * (distanceXZ / 100), ForceMode.Impulse);
            }
        }
    }

    void FireballAttack()
    {
        for (int i = 0; i < maxFireballsAtOnce; i++)
        {
            SpawnFireball();
        }
    }

    void SpawnFireball()
    {
        Vector3 fireballSpawnPosition = transform.position + transform.forward * 1.5f + (transform.up * (GetComponent<Renderer>().bounds.size.y * 0.6f) * Random.Range(1, 2f) + transform.right * Random.Range(-10, 10f));
        GameObject instanceFireball = Instantiate(fireball, fireballSpawnPosition, transform.rotation);
        FireballAI fireballAI = instanceFireball.GetComponent<FireballAI>();
        fireballAI.SetTarget(target);
        fireballAI.SetToLaunch();
    }

    void SelectTarget()
    {
        // Seleccionar un NPC aleatorio que no haya terminado el circuito
        List<JumpingNPC> npcs = FindObjectsOfType<JumpingNPC>().ToList();
        JumpingNPC[] notFinishedNpcs = npcs.FindAll(npc => !npc.IsFinished()).ToArray();
        if (notFinishedNpcs.Length > 0)
        {
            target = notFinishedNpcs[Random.Range(0, notFinishedNpcs.Length)].transform;
            target.GetComponent<JumpingNPC>().SetBeingChased(true);
        }
        else
            target = null;
    }
}
