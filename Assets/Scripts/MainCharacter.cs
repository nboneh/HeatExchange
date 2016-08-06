using UnityEngine;
using System.Collections;

public class MainCharacter : MonoBehaviour {

    public Collider floatColider;
    public Camera followCamera;
    float moveSpeed = 2;
    float floatingCycleAngle = 0f;
    float floatingHeight = .7f;
    float floatingY = 0f;

     float minimumCameraX = -360F;
     float maximumCameraX = 360F;
     float minimumCameraY = -60F;
     float maximumCameraY = 60F;

    float cameraRotationX = 0;
    float cameraRotationY = 0;
    Vector3 prevMousePos;

    Collider currentFloor = null;
	// Use this for initialization
	void Start () {
       cameraRotationX = followCamera.transform.rotation.y;
       // cameraRotationY = followCamera.transform.rotation.x;
     

        prevMousePos = Input.mousePosition;
        followCamera.transform.LookAt(transform.position);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("w"))
            GetComponent<Rigidbody>().velocity = Vector3.forward * moveSpeed;

        if (Input.GetKeyDown("s"))
            GetComponent<Rigidbody>().velocity = -Vector3.forward * moveSpeed;

        if (Input.GetKeyDown("a"))
            GetComponent<Rigidbody>().velocity = -Vector3.right * moveSpeed;

        if (Input.GetKeyDown("d"))
            GetComponent<Rigidbody>().velocity = Vector3.right * moveSpeed;


        Float(Time.deltaTime);
        UpdateCamera();
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

            floatingCycleAngle += t * 90;
            floatingY = Mathf.Sin(floatingCycleAngle * (Mathf.PI / 180.0f) * 2) * .07f;
            float floatingRoll = Mathf.Sin(floatingCycleAngle * (Mathf.PI / 180.0f)) * 5;
            float floatingYaw = Mathf.Sin(floatingCycleAngle * (Mathf.PI / 180.0f) * 2) * 2;

            if (floatingCycleAngle > 360)
            {
                floatingCycleAngle = floatingCycleAngle % 360;
            }

            if (currentFloor.Raycast(ray, out hit, 2.0f * maxHeight))
            {
                transform.position = new Vector3(x, hit.point.y+ floatingHeight + floatingY, z);
                transform.rotation = Quaternion.Euler(new Vector3(floatingRoll, rb.rotation.y, floatingYaw));
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            } 
        }
    }

    void UpdateCamera()
    {
        if (prevMousePos == null)
        {
            prevMousePos = Input.mousePosition;
            return;
        }

       Vector3 delta = Input.mousePosition - prevMousePos;

        cameraRotationX += delta.x/15.0f;
        cameraRotationY += delta.y/15.0f;

        prevMousePos = Input.mousePosition;

        followCamera.transform.rotation = Quaternion.Euler(new Vector3(cameraRotationY, cameraRotationX, followCamera.transform.rotation.z));

        float x =transform.position.x;
        float y = transform.position.y - floatingY;
        float z = transform.position.z;
        Vector3 pos = new Vector3(x, y, z);
        followCamera.transform.position = new Vector3(x, y, z); 
        followCamera.transform.Translate( -10 * followCamera.transform.forward);

        followCamera.transform.LookAt(pos);
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
