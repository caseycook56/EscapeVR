using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    int maxBounces = 5;
    private LineRenderer lr;
    [SerializeField]
    private Transform startPoint;
    [SerializeField]
    private bool reflectOnlyMirror;
    private bool complete;

    public Animator doorAnimator;
    private GameObject door;

    public bool shouldCastLaser=false;
    

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
       
        lr.SetPosition(0, startPoint.position);

        complete = false;

        door = GameObject.FindGameObjectWithTag("Door");
        doorAnimator = door.GetComponent<Animator>();
        gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
       
        CastLaser(transform.position, -transform.right);
        
       
    }

    public void StartCastLaser()
    {
        shouldCastLaser = true;
        gameObject.SetActive(true);
    }

    public void CastLaser(Vector3 position, Vector3 direction)
    {
        lr.SetPosition(0, startPoint.position);

        for (int i = 0; i < maxBounces; i++)
        {
            Ray ray = new Ray(position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 300, 1))
            {
                position = hit.point;
                direction = Vector3.Reflect(direction, hit.normal);
                lr.SetPosition(i + 1, hit.point);

                if (hit.transform.tag.Equals("Mirror")==false && reflectOnlyMirror)
                {
                    for (int j = (i + 1); j <= maxBounces; j++)
                    {
                        lr.SetPosition(j, hit.point);
                    }
                    break;
                }

                if (hit.transform.tag.Equals("Lock") == true && !complete)
                {
                    complete = true;
                    Debug.Log("great success!!!");
                    doorAnimator.SetTrigger("OpenDoor");
                    float speed = 10F;
                    GameObject.Find("doorHinge").transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 90, 0), Time.deltaTime * speed);
                    GameObject.Find("doorway.002").SetActive(false);
                    doorAnimator.Play("OpenDoor", 0, 0.0f);
                    GameManger.Instance.CompleteLazer();
                 

                }
            }
        }
    }
}
