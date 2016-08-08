using UnityEngine;
using System.Collections;

public class FloatTracker : MonoBehaviour
{

    public MainCharacter main;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = main.transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        main.TouchFloor(other);
    }

    void OnTriggerExit(Collider other)
    {
        main.ExitFloor(other);
    }
}
