using UnityEngine;
using System.Collections;

public class Pan : MonoBehaviour
{

    int numOfAnimals = 0;
    private AudioSource source;

    // Use this for initialization
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void Reset()
    {
        numOfAnimals = 0;   
    }
    public int GetNumOfAnimals ()
    {
        return numOfAnimals;
    }

    // Update is called once per frame
    void Update()
    {
        if (numOfAnimals < 0)
            numOfAnimals = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Animal")
        {
            if (!source.isPlaying)
            {
                source.Play();
            }
            numOfAnimals++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Animal")
            numOfAnimals--;
    }
}
