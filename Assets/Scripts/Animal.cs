using UnityEngine;
using System.Collections;

public class Animal : MonoBehaviour {


    Vector3 initPosition;
    Quaternion initRot;
    private AudioSource source;


    
	// Use this for initialization
	void Start () {
        source = GetComponent<AudioSource>();
        initPosition = transform.position;
        initRot = transform.rotation;

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Reset()
    {
        transform.position = initPosition;
        transform.rotation = initRot;
    }

    public void PlaySound()
    {
        if(!source.isPlaying)
             source.Play();
    }
}
