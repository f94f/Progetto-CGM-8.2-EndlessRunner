using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;
    
    [SerializeField]
    private float speed;
    [SerializeField]
    private float jump;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rb.velocity = Vector3.forward * speed;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("jump");
            rb.AddForce(Vector3.up * jump, ForceMode.Impulse);
            
            
        }
    }
}
