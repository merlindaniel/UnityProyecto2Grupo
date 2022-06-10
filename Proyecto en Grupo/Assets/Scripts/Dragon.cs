using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Dragon : MonoBehaviour
{
    private Animator animator;
    private AnimatorOverrideController animatorOverrideController;

    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Fly", true);

        SelectTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
            transform.LookAt(target);
        else
            SelectTarget();
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
