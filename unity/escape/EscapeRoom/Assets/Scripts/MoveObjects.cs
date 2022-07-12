using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//this script is old code
// trying to get objects to float in front of the player 
// the inventory is just holding items now
public class MoveObjects : MonoBehaviour
{

    bool move = false;

    bool drag = false;

    int speed = 1;
    Vector2 currentMouse = Vector2.zero;
    Vector2 lastMouse = Vector2.zero;

    RaycastHit lastHit;


    // Start is called before the first frame update
    void Start()
    {
       // Screen.lockCursor = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            lastMouse = currentMouse;
           // currentMouse = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            move = true;
            //drag = true;
        } else if(Input.GetMouseButtonUp(0)){
            drag = false;
            move = false;

            Rigidbody rb = lastHit.transform.GetComponent<Rigidbody>();
            rb.useGravity = true;
 
        }
        else
        {
           // move = false;
        }
        //lastMouse =  new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        if (Input.GetKey(KeyCode.Escape))
        {
            Screen.lockCursor = false;
        }
            
    }

    void FixedUpdate()
    {
        if (move)
        {
            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            //layerMask = ~layerMask;

            Transform cameraTransform = Camera.main.transform;

            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(cameraTransform.position, cameraTransform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {
                lastHit = hit;
                Debug.DrawRay(cameraTransform.position, cameraTransform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
               /* Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                Vector2 newVel = lastMouse - currentMouse;
*/
                //rb.velocity = new Vector3(1, 1, 0);
                Debug.Log("Did Hit" +hit.transform.name);
                drag = true;
                move = false;
            }
            else
            {
                Debug.DrawRay(cameraTransform.position, cameraTransform.TransformDirection(Vector3.forward) * 1000, Color.white);
                Debug.Log("Did not Hit");
                drag = false;
            }
        }

        if (drag)
        {
            Transform cameraTransform = Camera.main.transform;

            currentMouse = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            Rigidbody rb = lastHit.transform.GetComponent<Rigidbody>();
            rb.useGravity = false;
            Vector3 vel = new Vector3(lastHit.transform.position.x - currentMouse.x, lastHit.transform.position.y - currentMouse.y, 0);
            Debug.Log("Dragging" + vel);
            rb.velocity += vel * 10* Time.deltaTime;
           // drag = false;
        }
        
    }

    void OnMouseDrag()
    {
        Debug.Log("Dragging");
        drag = true;
        //lastHit

       // rend.material.color -= Color.white * Time.deltaTime;
    }

}
