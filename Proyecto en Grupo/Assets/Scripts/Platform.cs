using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public GameObject nextPlatform;

    public bool finalPlatform = false;
    //ESTO ESTA EN DESUSO ACUALMENTE EN LA ESCENA SCENEIATEST
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "NPC" && nextPlatform != null) {
            PrincipalNPC principalNpc = other.gameObject.GetComponent<PrincipalNPC>();
            if (principalNpc.GetNextPlatform().GetInstanceID() == gameObject.GetInstanceID())   //Comprobamos que la siguiente plataforma es la que acaba de pisar el NPC
                principalNpc.ChangeNextPlatform(nextPlatform);
            principalNpc.isJumping = false;
        } else if (other.gameObject.tag == "NPC" && nextPlatform == null)
        {
            PrincipalNPC principalNpc = other.gameObject.GetComponent<PrincipalNPC>();
            principalNpc.SetFinished(true);
            principalNpc.isJumping = false;
        }
    }
}
