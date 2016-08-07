using UnityEngine;
using System.Collections;

public class MainCharacter : MonoBehaviour {

    public Collider floatColider;
    public Camera followCamera;
    public ParticleSystem wind;
    public ParticleSystem lightning;
    float moveSpeed = 5;
    float floatingCycleAngle = 0f;
    float floatingHeight = 1f;
    float floatingY = 0f;

    float landStretchMaxVelocity = 0f;
    float landStretchSpeed = 40f;
    float landStretchVelocity = 0f;
    int landStretchState = 0;
    bool shouldJumpAfterLandStrecth = false;
    float jumpVel;

    float flipAngle = 0;
    float flipAngleRate = 150f;
    ParticleSystem particleWind;
    ParticleSystem particleLightning;

    Vector3 initialPosition;
    Quaternion initialRotation;
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
    bool flipping = false;
    float flipY = 0;
    bool spaceHeld = false;
    bool dead = false;
    float jumpCounter = 0f;
    float maxBatteryPower = 30f;
    float batteryPower = 0f;

    float batteryDrainedCounter = 0f;
    float batteryDrainedCounterMax = 2f;

    public AudioClip highJumpSound;
    public AudioClip lowJumpSound;
    public AudioClip transformSound;
    public AudioClip landSound;
    public AudioClip shortOutSound;

    public AudioSource source;
    public AudioSource chargingSound;

    public Fan fan;
    public Animator animator;

    bool charging = false;

    GameObject animalThatCanBePickedUp = null;

    // Use this for initialization
    void Start () {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
        batteryPower = maxBatteryPower;
    }

    public void reset()
    {
        animalThatCanBePickedUp = null;
        batteryPower = maxBatteryPower;
        floatingCycleAngle = 0f;
        flippedMode = false;
        flipping = false;
        flipY = 0;
        spaceHeld = false;
        jumpCounter = 0f;

        floatingY = 0f;

        flipAngle = 0;

        cameraRotationX = 0;
        cameraRotationY = 15;

        landStretchVelocity = 0f;
        landStretchState = 0;
        shouldJumpAfterLandStrecth = false;

        if (particleWind != null)
        {
           Destroy(particleWind.gameObject);
           particleWind = null;
        }

        if(particleLightning != null)
        {
            Destroy(particleLightning.gameObject);
            particleLightning = null;
        }

        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;
        currentFloor = null;


         forwardMovement = 0;
         sideMovement = 0;
        momentumAngle = 0;
        dead = false;

        batteryDrainedCounter = 0f;

        GetComponent<Rigidbody>().freezeRotation = true;

        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

        charging = false;

        animator.enabled = true;

        fan.StartFan();

    }

    public bool IsDead()
    {
        return dead;
    }

    public float GetMaxBatteryPower()
    {
        return maxBatteryPower;
    }
    public float GetBatteryPower()
    {
        return batteryPower;
    }

    public bool IsCharging()
    {
        return charging;
    }

    // Update is called once per frame
    void Update() {
        if (Time.timeScale == 0) return;

        if (transform.position.y < -2)
            dead = true;
        float t = Time.deltaTime;

        if (batteryPower > 0)
        {
            if (charging)
            {
                batteryPower += t*5;
                if(batteryPower >= maxBatteryPower)
                {
                    batteryPower = maxBatteryPower;
                }
            }
            else
            {
                batteryPower -= t;
                if (batteryPower <= 0)
                {
                    source.PlayOneShot(shortOutSound);
                    batteryPower = 0;
                    GetComponent<Rigidbody>().freezeRotation = false;
                }
            }
        }
        else
        {
            fan.StopFan();
            animator.enabled = false;
            batteryDrainedCounter += t;
            if (batteryDrainedCounter >= batteryDrainedCounterMax)
                dead = true;
            return;
        }


        GetComponent<Rigidbody>().velocity = new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);


        if (flipping)
        {
            Flip(t);
            return;
        }
        CheckFlip();
        Float(t);
        Movement(t);
        UpdateLandStretch(t);
        JumpInput(t);
        UpdateCamera(t);

        if(particleWind != null)
        {
      
            particleWind.transform.position = new Vector3(transform.position.x, transform.position.y - .5f, transform.position.z);
            if(currentFloor != null) { 
                Destroy(particleWind.gameObject);
                particleWind = null;
            }
        }

        if (particleLightning != null)
        {
            particleLightning.transform.position = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        }
    }

    void Flip(float t)
    {
        flipAngle += t * flipAngleRate;

        if (flippedMode == true && flipAngle >= 180)
        {
            flipAngle = 180;
            flipping = false;
        }

        else if (flippedMode == false && flipAngle >= 360)
        {
            flipAngle = 0;
            flipping = false;
        }

        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, flipAngle));
        transform.position = new Vector3(transform.position.x, flipY, transform.position.z);
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
    }

    void CheckFlip()
    {
        if(currentFloor != null && landStretchState == 0 && flipping == false)
        {
            if (Input.GetKeyDown("tab"))
            {
                flipping = true;
                source.PlayOneShot(transformSound);
                flippedMode = !flippedMode;
                flipY = transform.position.y;
            }
        }
    }
    void JumpInput(float t)
    {
        if (currentFloor != null && landStretchState == 0)
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

                if (flippedMode)
                {
                    jumpVel = jumpCounter * 23 + 5f;
                }
                else
                {
                    jumpVel = jumpCounter * 15 + 2.2f;
                }
                SetLandStretch(jumpVel); 
            }
        }
    }

    void Jump()
    {
        currentFloor = null;
        if (flippedMode)
        {
            particleWind = (ParticleSystem)Instantiate(wind, new Vector3(transform.position.x, transform.position.y - .5f, transform.position.z), Quaternion.Euler(new Vector3( 90, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z)));
            source.PlayOneShot(highJumpSound);
        } else
        {
            source.PlayOneShot(lowJumpSound);
        }
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
            float lastFloatingY = floatingY;
            floatingY = Mathf.Sin(floatingCycleAngle * (Mathf.PI / 180.0f) * 2) * .07f;
            float floatingRoll = Mathf.Sin(floatingCycleAngle * (Mathf.PI / 180.0f)) * 5;
            float floatingYaw = Mathf.Sin(floatingCycleAngle * (Mathf.PI / 180.0f) * 3) * 2;
            if (floatingCycleAngle > 360)
            {
                floatingCycleAngle = floatingCycleAngle % 360;
            }

            if (currentFloor.Raycast(ray, out hit, 2.0f * maxHeight))
            {
                transform.position = new Vector3(x, hit.point.y + floatingHeight + floatingY, z);
                if (momentumAngle < .01f)
                    transform.rotation = Quaternion.Euler(new Vector3(floatingRoll, transform.rotation.eulerAngles.y, flipAngle + floatingYaw));
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            }
        }
        Stretch(GetComponent<Rigidbody>().velocity.y);
      
    }

    void UpdateCamera(float t)
    {
        cameraRotationX += Input.GetAxis("Mouse X") * 3.0f;
         cameraRotationY -= Input.GetAxis("Mouse Y");

        Debug.Log("X: " + cameraRotationX);

        if (cameraRotationY <= minimumCameraY)
            cameraRotationY = minimumCameraY;
        else if (cameraRotationY >= maximumCameraY)
            cameraRotationY = maximumCameraY;

        cameraRotationX = cameraRotationX % 360;

        if(cameraRotationX > 90  && cameraRotationX < 180)
        {
            cameraRotationX += 180;
        }

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
        if (other.gameObject.tag == "Floor" )
        {
            currentFloor = other;
            jumpCounter = 0;
            float volume = -GetComponent<Rigidbody>().velocity.y / 9;

            source.PlayOneShot(landSound, volume);
            SetLandStretch(-GetComponent<Rigidbody>().velocity.y);
        }

        if (other.gameObject.tag == "ReactorCharger")
        {
            charging = true;
            particleLightning = (ParticleSystem)Instantiate(lightning, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.Euler(new Vector3( 270, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z)));
            chargingSound.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (currentFloor == other)
            currentFloor = null;

        if (other.gameObject.tag == "ReactorCharger")
        {
            charging = false;
            if (particleLightning != null)
            {
                Destroy(particleLightning.gameObject);
                particleLightning = null;
            }
            chargingSound.Stop();
        }
    }

    void SetLandStretch(float velocity)
    {

        landStretchState = 1;
        landStretchMaxVelocity = velocity;
    }

    void UpdateLandStretch(float t)
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

    void SetCanPickUpAnimal(GameObject animal)
    {
        if (animalThatCanBePickedUp != null)
            return;
        animalThatCanBePickedUp = animal;
    }

    void SetCannotPickUpAnimal(GameObject animal)
    {
        if (animalThatCanBePickedUp == animal)
        {
            animalThatCanBePickedUp = null;
        }
    }
   
    void OnColisionEnter(Collision Col)
    {
    }
}
