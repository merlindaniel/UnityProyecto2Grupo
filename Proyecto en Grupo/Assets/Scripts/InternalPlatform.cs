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
            print("---Plataforma interna pisada");
            PrincipalNPC principalNpc = other.gameObject.GetComponent<PrincipalNPC>();
            print("....NextPlatformId: " + principalNpc.GetNextPlatform().GetInstanceID() + ". thisID: " + gameObject.GetInstanceID());
            if (principalNpc.GetNextPlatform().GetInstanceID() == gameObject.GetInstanceID())//Comprobamos que la siguiente plataforma es la que acaba de pisar el NPC
            {
                other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                other.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                print("--Interrnal Platform: ChangeNextPlatform and go actual Platform");
                principalNpc.ChangeNextPlatform(nextPlatform);
                principalNpc.GoToActualPlatform();
            }
            principalNpc.isJumping = false;
        }
        else if (other.gameObject.tag == "NPC" && nextPlatform == null)
        {
            PrincipalNPC principalNpc = other.gameObject.GetComponent<PrincipalNPC>();
            principalNpc.SetFinished();
            principalNpc.isJumping = false;
        }
    }
}
