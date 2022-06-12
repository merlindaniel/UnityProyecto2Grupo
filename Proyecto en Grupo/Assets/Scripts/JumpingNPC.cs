using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class JumpingNPC : MonoBehaviour
{
    Rigidbody rbNPC;
    GameObject respawnPlatform, currentPlatform, nextPlatform;

    bool finished; //Si el NPC llego a la meta
    float time;
    bool isSetToJump;
    
    float timeUntilJump;
    public float timeUntilJumpRangeMin = 0.5f;
    public float timeUntilJumpRangeMax = 5f;

    // Saltar manualmente / Respawnear al NPC
    public bool manualJump = false, respawn = false;

    bool isOnAPlatform;

    bool beingChased;

    private Dictionary<int, float> errors;

    private void Awake()
    {
        errors = new Dictionary<int, float>();
        isOnAPlatform = false;
        finished = false;
        isSetToJump = false;
        beingChased = false;
        ResetTimeUntilJump();

        respawnPlatform = GameObject.FindGameObjectWithTag("Respawn");
        GoToSpawn();
    }

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        rbNPC = GetComponent<Rigidbody>();
        GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -100)
        {
            GoToSpawn();
        }

        // Preparar salto
        if (isSetToJump)
        {
            time += Time.deltaTime;
            if (time >= 0.1) // Mirar a la plataforma
            {
                rbNPC.isKinematic = false;
                LookNextPlatform();
            }
            if (time >= timeUntilJump && isOnAPlatform) // Solo saltar si esta sobre la plataforma
            {
                SetToJump(false);
                time = 0;
                ResetTimeUntilJump();
                GetComponent<JumpingAI>().PredictAndExecute();
            }
        }

        // ---- Manual Inputs ----
        if (manualJump)
        {
            LookNextPlatform();
            GetComponent<JumpingAI>().PredictAndExecute();
            manualJump = false;
        }

        if (respawn)
        {
            rbNPC.isKinematic = true;
            GoToSpawn();
            respawn = false;
        }
    }

    // ---------------- Jump / Teleport NPC  ----------------

    public void GoToSpawn()
    {
        SetToJump(false);
        SetBeingChased(false);
        float actSpawnHeight = respawnPlatform.GetComponent<Collider>().bounds.size.y;
        float actSpawnSizeX = respawnPlatform.GetComponent<Collider>().bounds.size.x;
        float actSpawnSizeZ = respawnPlatform.GetComponent<Collider>().bounds.size.z;
        transform.position = new Vector3(respawnPlatform.transform.position.x + Random.Range(-actSpawnSizeX/2, actSpawnSizeX/2), respawnPlatform.transform.position.y + (actSpawnHeight / 2f) + (GetNpcHeight() / 2f) + 1, respawnPlatform.transform.position.z + Random.Range(-actSpawnSizeZ/2, actSpawnSizeZ/2));
    }

    public void LookNextPlatform()
    {
        if (nextPlatform != null)
        {
            transform.LookAt(nextPlatform.transform.position);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }

    public void jumpRelative(float forceX, float forceY, float forceZ)
    {
        rbNPC.AddRelativeForce(new Vector3(forceX, forceY, forceZ), ForceMode.Impulse);
    }

    // ---------------- Collisions ----------------

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.GetComponent<Platform>() != null)
        {
            isOnAPlatform = true;

            // Si no es el respawn, apuntar el error de salto
            if (other.gameObject.tag != "Respawn" && !errors.ContainsKey(other.gameObject.GetInstanceID())) 
            {
                float error = CalculateError(other);
                errors.Add(other.gameObject.GetInstanceID(), error);
            }
        }
    }

    private float CalculateError(Collision plataformCollider)
    {
        Vector3 platformPosition = plataformCollider.transform.position;
        float distance = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(platformPosition.x - transform.position.x), 2f) + Mathf.Pow(Mathf.Abs(platformPosition.z - transform.position.z), 2f));

        return distance;
    }

    private void OnCollisionExit(Collision other) {
        if (other.gameObject.GetComponent<Platform>() != null)
        {
            isOnAPlatform = false;
        }
    }

    // ---------------- Getters / Setters ----------------

    public bool IsSetToJump()
    {
        return isSetToJump;
    }

    public bool IsFinished()
    {
        return finished;
    }

    public GameObject GetNextPlatform()
    {
        return nextPlatform;
    }

    public float GetNpcHeight()
    {
        return GetComponent<Collider>().bounds.size.y;
    }

    public bool isBeingChased()
    {
        return beingChased;
    }

    public void SetToJump(bool value)
    {
        isSetToJump = value;
        time = 0f;
    }

    public void SetFinished(bool finish)
    {
        this.finished = finish;
        rbNPC.isKinematic = finish;
        
        if (finished)
        {
            float meanError = CalculateMeanError();
            print("NPC:  " + gameObject.GetInstanceID() + " FINISHED. MEAN ERROR: " + meanError);
            GameObject textGameObject = new GameObject("Text");
            TextMesh t = textGameObject.AddComponent<TextMesh>();
            t.text = meanError.ToString();
            t.fontSize = 30;
            t.characterSize = 0.2f;
            t.transform.position = transform.position + (Vector3.up * GetNpcHeight()) + (Vector3.left * 1.25f);
            t.transform.parent = transform;
            t.color = Color.black;
        }
    }

    private float CalculateMeanError()
    {
        if (errors.Count > 0)
            return errors.Sum(e => e.Value) / errors.Count;

        return 0;
    }

    public void SetNextPlatform(GameObject nextPlatform)
    {
        this.nextPlatform = nextPlatform;
    }

    private void ResetTimeUntilJump()
    {
        timeUntilJump = Random.Range(timeUntilJumpRangeMin, timeUntilJumpRangeMax);
    }

    public void SetBeingChased(bool value)
    {
        beingChased = value;
    }
}
