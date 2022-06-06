using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternalPlatform : MonoBehaviour
{
    public GameObject nextPlatform;

    public bool finalPlatform = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "NPC" && nextPlatform != null)
        {
            other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            PrincipalNPC principalNpc = other.gameObject.GetComponent<PrincipalNPC>();
            principalNpc.changeNextPlatform(nextPlatform);
            principalNpc.isJumping = false;
            other.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
        else if (other.gameObject.tag == "NPC" && nextPlatform == null)
        {
            PrincipalNPC principalNpc = other.gameObject.GetComponent<PrincipalNPC>();
            principalNpc.setFinished();
            principalNpc.isJumping = false;
        }
    }
}
