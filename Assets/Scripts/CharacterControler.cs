using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControler : MonoBehaviour
{
    public float moveSpeed;

    public float turnSpeed;

    

    private Rigidbody rigidbody;
    
    private CapsuleCollider capsuleCollider;
    private float forwardInput;
    private float turnInput;


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void ProcessActions()
    {

        
        if (turnInput != 0f)
        {
            float angle = Mathf.Clamp(turnInput, -1f, 1f) * turnSpeed;
            transform.Rotate(Vector3.up, Time.fixedDeltaTime * angle);
        }

        // Movement
        Vector3 move = transform.forward * Mathf.Clamp(forwardInput, -1f, 1f) *
            moveSpeed * Time.fixedDeltaTime;

        Debug.Log("b" + turnInput);
        Debug.Log("c" + move);
        rigidbody.MovePosition(transform.position + move);
    }

    private void FixedUpdate()
    {
        ProcessActions();
    }

    private void Update()
    {
        // Get input values
        int vertical = Mathf.RoundToInt(Input.GetAxis("Vertical"));
        int horizontal = Mathf.RoundToInt(Input.GetAxis("Horizontal"));

        forwardInput = vertical;
        turnInput= horizontal;

        Debug.Log("a"+forwardInput);
    }

}
