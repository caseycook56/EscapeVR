using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OpenDraw : MonoBehaviour
{
    public bool openningDraw = false;
    public int speed =10;
    public float lengthZ = 0.005f;
    private float startZ;
    // Start is called before the first frame update
    void Start()
    {
        startZ = transform.position.z;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       /* if (openningDraw)
        {
            Debug.Log("Openning!!" + transform.position.z+ " " + startZ + lengthZ);
            if (transform.position.z < startZ+lengthZ)
            {
                //Debug.Log
                transform.position = new Vector3(transform.position.x,
                transform.position.y,
                transform.position.z + lengthZ * Time.deltaTime);
            }
            else
            {
                openningDraw = false;
            }

        }*/

    }

    public void Open()
    {
        if (GameManger.Instance.IsGriffinComplete() == false)
        {
            Debug.Log("Lets Open!");
            openningDraw = true;
            transform.localPosition = new Vector3(transform.localPosition.x,
                transform.localPosition.y, transform.localPosition.z + 4f);
            GameManger.Instance.CompleteGriffin();
        }
        
    }


}
