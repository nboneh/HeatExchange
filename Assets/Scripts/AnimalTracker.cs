using UnityEngine;
using System.Collections;

public class AnimalTracker : MonoBehaviour {

    public MainCharacter main;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = main.transform.position;
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Animal")
        {
            main.SetCanPickUpAnimal(other.gameObject.GetComponent<Animal>());
        }
    }

    void OnTriggerExit(Collider other)
    {
       if (other.gameObject.tag == "Animal")
            main.SetCannotPickUpAnimal(other.gameObject.GetComponent<Animal>());
    }
}
