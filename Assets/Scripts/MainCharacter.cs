using UnityEngine;
using System.Collections;

public class MainCharacter : MonoBehaviour {

    public Collider floatColider;
    float walkSpeed = 2;
    float floatingAngle;

    Collider currentFloor = null;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("w"))
            GetComponent<Rigidbody>().velocity = Vector3.forward * walkSpeed;

        if (Input.GetKeyDown("s"))
            GetComponent<Rigidbody>().velocity = -Vector3.forward * walkSpeed;

        if (Input.GetKeyDown("a"))
            GetComponent<Rigidbody>().velocity = -Vector3.right * walkSpeed;

        if (Input.GetKeyDown("d"))
            GetComponent<Rigidbody>().velocity = Vector3.right * walkSpeed;


        Float(Time.deltaTime);
    }

    void Float(float t)
    {
         if (currentFloor != null)
         {
            float maxHeight = 25;
            Rigidbody rb = GetComponent<Rigidbody>();
            float x = rb.position.x;
            float z = rb.position.z;
            RaycastHit hit;
            Ray ray = new Ray(new Vector3(x, maxHeight, z), Vector3.down);
            if (currentFloor.Raycast(ray, out hit, 2.0f * maxHeight))
            {
                rb.position = new Vector3(x, hit.point.y+1f, z);
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            } 
        }
    }


    void OnTriggerEnter(Collider other)
    {
        currentFloor = other;
    }

    void OnTriggerExit(Collider other)
    {
        currentFloor = null;
    }

    void OnColisionEnter(Collision Col)
    {
    }
}
