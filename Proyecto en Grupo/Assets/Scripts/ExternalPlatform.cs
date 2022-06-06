using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalPlatform : MonoBehaviour
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
            //print("---Plataforma EXTERNA pisada. Volviendo al punto anterior");
            PrincipalNPC principalNpc = other.gameObject.GetComponent<PrincipalNPC>();
            principalNpc.GoToActualPlatform();
        }
    }
}
