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
        if (other.gameObject.tag == "NPC")
        {
            PrincipalNPC principalNpc = other.gameObject.GetComponent<PrincipalNPC>();
            //if (principalNpc.isLearing())
            //{
            //    if (nextPlatform != null)
            //    {
            //        if (principalNpc.GetNextPlatform().GetInstanceID() == gameObject.GetInstanceID())//Comprobamos que la siguiente plataforma es la que acaba de pisar el NPC
            //        {
            //            other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            //            other.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            //            principalNpc.GoToActualPlatform();
            //        }
            //        else
            //        {
            //            principalNpc.isJumping = false;
            //        }
                    
            //    }
            //    else
            //    {
            //        principalNpc.SetFinished(true);
            //        principalNpc.isJumping = false;
            //    }
            //}
            //else
            //{
            if (nextPlatform != null)
            {
                //print("---Plataforma interna pisada");
                if (principalNpc.GetNextPlatform().GetInstanceID() == gameObject.GetInstanceID())   //Comprobamos que la siguiente plataforma es la que acaba de pisar el NPC
                {
                    other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    other.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    principalNpc.ChangeNextPlatform(nextPlatform);
                    principalNpc.GoToActualPlatform();//Lo posicionamos en el centro de la plataforma
                }
                else
                {
                    principalNpc.isJumping = false;
                }
            }
            else
            {
                principalNpc.SetFinished(true);
                principalNpc.isJumping = false;
            }
            
            
        }
    }
}
