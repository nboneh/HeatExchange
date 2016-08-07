using UnityEngine;
using System.Collections;

public class Fan : MonoBehaviour {

    // Use this for initialization
    float fanSpeed = 1080;
    bool stoppedFan = false;
	void Start () {
	 
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.timeScale == 0) return;
        if (stoppedFan) return;
        float t = Time.deltaTime;
        transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y + t * fanSpeed, transform.localRotation.eulerAngles.z));
    }

    public void StopFan()
    {
        stoppedFan = true;
    }

    public void StartFan()
    {
        stoppedFan = false;
    }
}
