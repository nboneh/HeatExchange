using UnityEngine;
using System.Collections;

public class MainCharacter : MonoBehaviour {

    public Collider floatColider;
    public Camera followCamera;
    float moveSpeed = 2;
    float floatingCycleAngle = 0f;
    float floatingHeight = .7f;
    float floatingY = 0f;

    float landStretchMaxVelocity = 0f;
    float landStretchSpeed = 40f;
    float landStretchVelocity = 0f;
    int landStretchState = 0;
    bool shouldJumpAfterLandStrecth = false;
    float jumpVel;



    Vector3 initialScale;

    int forwardMovement = 0;
    int sideMovement = 0;

    float momentumAngle = 0;
    float maxMomentumAngle = 15;

     float minimumCameraY = -30F;
     float maximumCameraY = 30F;

    float cameraRotationX = 0;
    float cameraRotationY = 15;

    Collider currentFloor = null;
   
    bool flippedMode = false;
    bool spaceHeld = false;
    float jumpCounter = 0f;
	// Use this for initialization
	void Start () {
        initialScale = GetComponent<Rigidbody>().transform.localScale; 
    }
	
	// Update is called once per frame
	void Update () {
        float t = Time.deltaTime;
        Float(t);
        Movement(t);
        updateLandStretch(t);
        JumpInput(t);
        UpdateCamera(t);
    }

    void JumpInput(float t)
    {
        if (currentFloor != null)
        {
            if (Input.GetKeyDown("space")){
                spaceHeld = true;
            }
            if (spaceHeld)
            {
                jumpCounter += t;
            }

            if ((Input.GetKeyUp("space") || jumpCounter > .2f) && spaceHeld)
            {
                if(jumpCounter > .2f)
                     jumpCounter = .2f;
                spaceHeld = false;
                shouldJumpAfterLandStrecth = true;
                jumpVel = jumpCounter * 15 + 2.2f;
                setLandStretch(jumpVel); 
            }
        }
    }

    void Jump()
    {
       currentFloor = null;
       Rigidbody rb = GetComponent<Rigidbody>();
       rb.velocity = new Vector3(rb.velocity.x, jumpVel, rb.velocity.z);
    }
 
    void Movement(float t)
    {
        if (Input.GetKeyDown("w") && forwardMovement == 0)
        {
            forwardMovement = 1;
        }

        else if (Input.GetKeyDown("s") && forwardMovement == 0)
        {
            forwardMovement = -1;
        }

        else if (Input.GetKeyUp("w") && forwardMovement == 1)
        {
            forwardMovement = 0;
        }

        else if (Input.GetKeyUp("s") && forwardMovement == -1)
        {
            forwardMovement = 0;
        }

        if (forwardMovement != 0)
        {
            float speed = moveSpeed;
            if (sideMovement != 0)
                speed *= Mathf.Sqrt(2) / 2;
            transform.position += forwardMovement * followCamera.transform.forward * speed * t;
        }

        if (Input.GetKeyDown("a") && sideMovement == 0)
        {
            sideMovement = 1;
        }

        else if (Input.GetKeyDown("d") && sideMovement == 0)
        {
            sideMovement = -1;
        }

        else if (Input.GetKeyUp("a") && sideMovement == 1)
        {
            sideMovement = 0;
        }

        else if (Input.GetKeyUp("d") && sideMovement == -1)
        {
            sideMovement = 0;
        }

        if (sideMovement != 0)
        {
            float speed = moveSpeed;
            if (forwardMovement != 0)
                speed *= Mathf.Sqrt(2) / 2;
            transform.position -= sideMovement * followCamera.transform.right * speed * t;
        }

        if (forwardMovement != 0 || sideMovement != 0)
        {
            momentumAngle += t * moveSpeed * 80;
            if (momentumAngle >= maxMomentumAngle)
                momentumAngle = maxMomentumAngle;

            float cameraYaw = followCamera.transform.eulerAngles.y;
            float mainYaw = transform.rotation.eulerAngles.y;

            if (forwardMovement == 1 && sideMovement == 0)
            {
                cameraYaw += 0;
            } else if (forwardMovement == 1 && sideMovement == -1)
            {
                cameraYaw += 45;
            } else if (forwardMovement == 0 && sideMovement == -1)
            {
                cameraYaw += 90;
            } else if (forwardMovement == -1 && sideMovement == -1)
            {
                cameraYaw += 135;
            }
            else if (forwardMovement == -1 && sideMovement == 0)
            {
                cameraYaw += 180;
            }
            else if (forwardMovement == -1 && sideMovement == 1)
            {
                cameraYaw += 225;
            }
            else if (forwardMovement == 0 && sideMovement == 1)
            {
                cameraYaw += 270;
            }
            else if (forwardMovement == 1 && sideMovement == 1)
            {
                cameraYaw += 315;
            }

            cameraYaw = cameraYaw % 360;


            float cameraYawLow = cameraYaw - 360;
            float cameraYawHigh = cameraYaw + 360;

            float low = Mathf.Abs(cameraYawLow - mainYaw);
            float mid = Mathf.Abs(cameraYaw - mainYaw);
            float high = Mathf.Abs(cameraYawHigh - mainYaw);

            if (low < mid && low < high)
                cameraYaw = cameraYawLow;
            else if (high < low && high < mid)
                cameraYaw = cameraYawHigh;


            if (Mathf.Abs(cameraYaw - mainYaw) < .01f)
            {
                // Do Nothing
            } else if (mainYaw < cameraYaw)
            {
                mainYaw += moveSpeed * t * 150;
                if (mainYaw >= cameraYaw)
                {
                    mainYaw = cameraYaw;
                }
            }
            else if (mainYaw > cameraYaw)
            {
                mainYaw -= moveSpeed * t * 150;
                if (mainYaw <= cameraYaw)
                {
                    mainYaw = cameraYaw;
                }
            }
            transform.rotation = Quaternion.Euler(new Vector3(momentumAngle, mainYaw, transform.rotation.eulerAngles.z));
        } else
        {
            momentumAngle -= t * moveSpeed * 80;
            if (momentumAngle <= 0)
                momentumAngle = 0;

            transform.rotation = Quaternion.Euler(new Vector3(momentumAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
        }

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
                if (momentumAngle < .01f)
                    transform.rotation = Quaternion.Euler(new Vector3(floatingRoll, transform.rotation.eulerAngles.y, floatingYaw));
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            } 
        } 
        Stretch(GetComponent<Rigidbody>().velocity.y);
    }

    void UpdateCamera(float t)
    {

        cameraRotationX += Input.GetAxis("Mouse X") * 3.0f;
         cameraRotationY -= Input.GetAxis("Mouse Y");

        if (cameraRotationY <= minimumCameraY)
            cameraRotationY = minimumCameraY;
        else if (cameraRotationY >= maximumCameraY)
            cameraRotationY = maximumCameraY;

        followCamera.transform.rotation = Quaternion.Euler(new Vector3(cameraRotationY, cameraRotationX, followCamera.transform.rotation.eulerAngles.z));

        float x =transform.position.x;
        float y = transform.position.y - floatingY;
        float z = transform.position.z;
        Vector3 pos = new Vector3(x, y, z);
        followCamera.transform.position = new Vector3(x, y, z); 
        followCamera.transform.Translate( -30 * followCamera.transform.forward);

        followCamera.transform.LookAt(pos);
    }

    void OnTriggerEnter(Collider other)
    {
        currentFloor = other;
        jumpCounter = 0;
        setLandStretch(-GetComponent<Rigidbody>().velocity.y);
    }

    void OnTriggerExit(Collider other)
    {
        currentFloor = null;
    }

    void setLandStretch(float velocity)
    {

        landStretchState = 1;
        landStretchMaxVelocity = velocity;
    }

    void updateLandStretch(float t)
    {
        if (landStretchState == 1)
        {
            if (landStretchVelocity < landStretchMaxVelocity)
            {
                landStretchVelocity += landStretchSpeed * t;
            } else {
                landStretchVelocity = landStretchMaxVelocity;
                landStretchState = -1;
                if (shouldJumpAfterLandStrecth)
                {
                    shouldJumpAfterLandStrecth = false;
                    Jump();
                    landStretchVelocity = 0;
                    landStretchState = 0;
                }
            }
        } else if (landStretchState == -1){
            if (landStretchVelocity >= 0)
            {
                landStretchVelocity -= landStretchSpeed * t;
            }
            else
            {
                landStretchVelocity = 0;
                landStretchState = 0;
            }
        }
        if(Mathf.Abs(landStretchVelocity) > .05f)
          Stretch(landStretchVelocity);
    }
    


    void Stretch(float velocity)
    {
        float stretch = velocity / 200.0f;
        GetComponent<Rigidbody>().transform.localScale = new Vector3(initialScale.x + stretch / 2, initialScale.y - stretch, initialScale.z + stretch / 2);
    }

   
    void OnColisionEnter(Collision Col)
    {
    }
}
