using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        if (transform.position.y < -1000) // Destruir la plataforma entera si esta muy abajo
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "NPC")
        {
            JumpingNPC jumpingNPC = other.gameObject.GetComponent<JumpingNPC>();

            if (finalPlatform)
            {
                other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
            else if (nextPlatform != null)
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
        else if (other.gameObject.tag == "Projectile")
        {
            // Destroy(other.gameObject);
        }
    }

    // Plataformas: A -> B -> C
    // Si se destruye B, queda:
    // A -> C
    private void OnDestroy() 
    {
        List<Platform> platforms = FindObjectsOfType<Platform>().ToList();
        platforms
            .Find(p => p.GetNextPlatform().GetInstanceID() == gameObject.GetInstanceID())
            .SetNextPlatform(nextPlatform);
    }

    public GameObject GetNextPlatform()
    {
        return nextPlatform;
    }

    public void SetNextPlatform(GameObject nextPlatform)
    {
        this.nextPlatform = nextPlatform;
    }
}
