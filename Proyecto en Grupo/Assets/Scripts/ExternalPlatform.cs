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
            print("---Plataforma EXTERNA pisada. Volviendo al punto anterior");
            //Descartamos el salto y volvemos al punto anterior
            JumpingNPC principalNpc = other.gameObject.GetComponent<JumpingNPC>();
            principalNpc.SetFinished(true);
            principalNpc.SetFinished(false);
            principalNpc.GoToSpawn();
        }
    }
}
