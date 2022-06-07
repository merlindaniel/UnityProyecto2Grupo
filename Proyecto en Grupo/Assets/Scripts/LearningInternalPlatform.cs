using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearningInternalPlatform : MonoBehaviour
{
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
        if (other.gameObject.tag == "NPC")
        {
            PrincipalNPC principalNpc = other.gameObject.GetComponent<PrincipalNPC>();

            if (principalNpc.isLearing() && principalNpc.GetActualPlatform().GetInstanceID() != gameObject.GetInstanceID())
            {
                print("---Plataforma LEARING fisica pisada");
                principalNpc.SetFinished(true);
                principalNpc.SetFinished(false);
                principalNpc.ChangeNextPlatform(null);
            }
            principalNpc.isJumping = false;

        }
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "NPC")
    //    {
    //        PrincipalNPC principalNpc = other.gameObject.GetComponent<PrincipalNPC>();

    //        if (principalNpc.isLearing())
    //        {
    //            print("---Plataforma LEARING interna pisada");
    //            //principalNpc.SetFinished(true);
    //            //principalNpc.SetFinished(false);
    //            principalNpc.ChangeNextPlatform(null);

    //            //principalNpc.GoToActualPlatform();
    //        }
    //        //principalNpc.isJumping = false;

    //    }
    //}
}
