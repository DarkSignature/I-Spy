using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool canMove;

    // Start is called before the first frame update
    void Start()
    {
        canMove = true;    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LockMovement()
    {
        canMove = false;
    }
}
