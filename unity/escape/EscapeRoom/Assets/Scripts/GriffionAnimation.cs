using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GriffionAnimation : MonoBehaviour
{

    bool animation = false;
    public Animator griffin;
    // Start is called before the first frame update
    void Start()
    {
        //griffin = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Doughnut_Holdable"))
        {
            Debug.Log("Collide with Dount!");
            griffin.SetTrigger("Activate");
        }
    }
}
