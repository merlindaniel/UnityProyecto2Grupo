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
                // if (jumpingNPC.GetNextPlatform().GetInstanceID() == gameObject.GetInstanceID())   //Comprobamos que la siguiente plataforma es la que acaba de pisar el NPC
                // {
                    // Predecir y saltar
                    if (!jumpingNPC.IsSetToJump())
                    {
                        jumpingNPC.SetToJump(true);
                        jumpingNPC.SetNextPlatform(nextPlatform);
                        other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    }
                // }
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
        Platform p = FindObjectsOfType<Platform>()
            .ToList()
            .Find(p => p.GetNextPlatform().GetInstanceID() == gameObject.GetInstanceID());
        
        if (p != null)
        {
            p.SetNextPlatform(nextPlatform);
        }    
    }

    public GameObject GetNextPlatform()
    {
        return nextPlatform;
    }

    public void SetNextPlatform(GameObject nextPlatform)
    {
        this.nextPlatform = nextPlatform;
    }

    public void DrawLine(Color color)
    {
        if (nextPlatform != null)
        {
            Vector3 target = transform.TransformDirection(nextPlatform.transform.position);
            Debug.DrawLine(transform.position, target, color);
        }
    }
}
