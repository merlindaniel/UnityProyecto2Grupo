using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
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
            JumpingNPC jumpingNPC = other.gameObject.GetComponent<JumpingNPC>();

            if (nextPlatform != null)
            {
                if (jumpingNPC.GetNextPlatform().GetInstanceID() == gameObject.GetInstanceID())   //Comprobamos que la siguiente plataforma es la que acaba de pisar el NPC
                {
                    jumpingNPC.SetNextPlatform(nextPlatform);

                    // Predecir y saltar
                    other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    jumpingNPC.isSetToJump = true;
                }
            }
        }
    }
}
