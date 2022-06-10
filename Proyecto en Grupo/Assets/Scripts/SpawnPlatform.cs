using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlatform : MonoBehaviour
{
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
            PrincipalNPC principalNpc = other.gameObject.GetComponent<PrincipalNPC>();
            JumpingAI jumpingAI = other.gameObject.GetComponent<JumpingAI>();
            principalNpc.SetNextPlatform(nextPlatformPrediction);
            jumpingAI.PredictAndJumpToNextPlatform();
        }
    }
}
