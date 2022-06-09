using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlatform : MonoBehaviour
{

    public GameObject nextPlatformLearing;
    public GameObject nextPlatformPrediction;

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
            // other.gameObject.transform.LookAt(new Vector3(nextPlatformPrediction.transform.position.x, 0, nextPlatformPrediction.transform.position.z));
            PrincipalNPC principalNpc = other.gameObject.GetComponent<PrincipalNPC>();
            principalNpc.isJumping = false;
        }
    }
}
