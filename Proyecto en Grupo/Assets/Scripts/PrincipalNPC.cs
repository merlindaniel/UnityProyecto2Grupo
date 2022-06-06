using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrincipalNPC : MonoBehaviour
{
    GameObject respawn;
    GameObject nextPlatform;
    bool finished;
    public bool isJumping;

    Rigidbody rigidbody;


    private void Awake()
    {
        respawn = GameObject.FindGameObjectWithTag("Respawn");

        nextPlatform = respawn.GetComponent<Platform>().nextPlatform;
        transform.position = new Vector3(respawn.transform.position.x, respawn.transform.position.y + 10, respawn.transform.position.z);
    }

    // Start is called before the first frame update
    void Start()
    {
        finished = false;
        isJumping = false;
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void goToSpawn()
    {
        isJumping = true; //Spawnea en al aire. Tomaremos esto como un salto
        //Transform spawnTransform = GameObject.FindGameObjectWithTag("Respawn").transform;
        //transform.position = new Vector3(spawnTransform.position.x, spawnTransform.position.y + 10, spawnTransform.position.z);
        //nextPlatform = GameObject.FindGameObjectWithTag("Respawn").GetComponent<Platform>().nextPlatform;

        nextPlatform = respawn.GetComponent<Platform>().nextPlatform;
        transform.position = new Vector3(respawn.transform.position.x, respawn.transform.position.y + 10, respawn.transform.position.z);
    }

    public void changeNextPlatform(GameObject nextPlatform)
    {
        this.nextPlatform = nextPlatform;
        if(nextPlatform!= null)
            transform.LookAt(nextPlatform.transform.position);
    }

    public void jumpRelative(float forceX, float forceY, float forceZ)
    {
        isJumping = true;
        rigidbody.AddRelativeForce(new Vector3(forceX, forceY, forceZ), ForceMode.Impulse);
    }


    public void setFinished()
    {
        finished = true;
        nextPlatform = null;
        rigidbody.isKinematic = true;
    }

    public bool isFinished()
    {
        return finished;
    }

    public GameObject getNextPlatform()
    {
        return nextPlatform;
    }
}
