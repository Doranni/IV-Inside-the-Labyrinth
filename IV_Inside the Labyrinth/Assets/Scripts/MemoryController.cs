using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryController : MonoBehaviour
{
    [SerializeField] private float attitude;
    Rigidbody memoryRB;

    // Start is called before the first frame update
    void Start()
    {
        memoryRB = GetComponentInChildren<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    // TODO: rotating and shaking (animation)
    // TODO: moving away fron player or to the player when close (friendly/agressive/neutral).
    // TODO: getting pieces of the story with each memory
}
