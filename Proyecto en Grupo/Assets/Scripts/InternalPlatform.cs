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

            if (nextPlatform != null)
            {
                //print("---Plataforma interna pisada");
                if (principalNpc.GetNextPlatform().GetInstanceID() == gameObject.GetInstanceID())   //Comprobamos que la siguiente plataforma es la que acaba de pisar el NPC
                {
                    print("hola");
                    principalNpc.SetNextPlatform(nextPlatform);
                    //other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    //Lo posicionamos en el centro de la plataforma
                    // principalNpc.GoToActualPlatform(); 

                    // Predecir y saltar
                    other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    principalNpc.inNextPlatform = true;
                }
            }
        }
    }
}
