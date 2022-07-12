using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerControls : MonoBehaviour
{

    // for character movement
    public float moveSpeed = 5;
    private float gravity = 9.8f;
    private float velocity = 0;

    private CharacterController cc;
    private bool mirror =false;


    private Transform cameraTransform;
    private Collider lastHit;
    private int floorMask = 1 << 9;
    private int objectMask = 1 << 8;


    // private bool trayPuzzle = false;
    private GameObject dountObject;
    private GameObject drawObject;
    private GameObject safeDoor;
    private OpenDraw draw;
    private GameObject teleport;

    // Ram here
    public string safeNumber;

    // this keeps track of all the numbers
    private List<string> safeArray = new List<string>();
    public string safeCode;

    private bool prevSwap = false;

    /*   private bool safePuzzle = false;
       private bool riverPuzzle;*/

    private Dictionary<string, float> completeRiver = new Dictionary<string, float>();
    private Dictionary<string, float> currentRiver = new Dictionary<string, float>();

    private string[] hints = {"Griffins are always hungry.",
        "A grid or numerals holds the order.",
        "The number of people are the answer.",
        "Light will reveal the answer unlock your troubles"};


    // if player is holding an object
    private bool isHolding = false;

    // which item is the player holding
    private GameObject holding;

    // Start is called before the first frame update

    public GameObject hand;
    private Rigidbody rb;


    public Animator safeAnimator;
    public Animator doorwayAnimator;
    private GameObject safe;

    private GameObject lastHighlight;
    private List<Color> lastMaterials = new List<Color>();
    private Renderer lastRenderer;

    private GameObject riverComplete;
    private GameObject riverIncomplete;
    private bool torchRotated = false;
    private GameObject torch;
    private GameObject doorway;
    private int lampTimes;
    private int defaultLayer = 0 << 8;

    // Mirror gameObjects
    private GameObject Mirror1;
    private GameObject Mirror2;
    private GameObject Mirror3;
    private GameObject Mirror4;

    // Nothing but sounds in here
    public AudioSource hexSound;
    private Dictionary<string, GameObject> highlights = new Dictionary<string, GameObject>();
    public TextMeshProUGUI hint;

    private GameObject batteryMount1;
    private GameObject batteryMount2;
    private GameObject keyMount;
    private GameObject battery2;

    private bool placeBattery1 = false;
    private bool placeBattery2 = false;
    private bool placeKey = false;

    public Laser laser;

    private Dictionary<string, GameObject> cipherHiglight = new Dictionary<string, GameObject>();



    void Start()
    {

        LoadObjects();
        
        // Ram here
        safeNumber = "";
        safeCode = "84371";
        lampTimes = 0;

        // Setting the battery near the lamp to be false
        battery2 = GameObject.Find("/Battery2");
        battery2.SetActive(false);

        // Setting the mirror's visibility to transparent
        Mirror1 = GameObject.Find("/Mirror1");
        Mirror2 = GameObject.Find("/Mirror2");
        Mirror3 = GameObject.Find("/Mirror3");
        Mirror4 = GameObject.Find("/Mirror4");

        Mirror1.SetActive(false);
        Mirror2.SetActive(false);
        Mirror3.SetActive(false);
        Mirror4.SetActive(false);

        // Setting the mirrors to low visibility before the river puzzle is solved
        //changeMirrorVisibility(0.2f);

        //riverPuzzle = false
        AddRiverRotations();
        

    }

    // Update is called once per frame
    void Update()
    {

        // always draw ray cast line in the debug screen
        Debug.DrawRay(cameraTransform.position, cameraTransform.TransformDirection(Vector3.forward), Color.green);

        MovePlayer();


        RaycastHit hit;

        HighlightObjects();

        // if player clicks mouse then move objects if it can
        if (Input.GetMouseButtonDown(0))
        {
            // Ray Cast checks which object is clicked
            if (Physics.Raycast(cameraTransform.position, cameraTransform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, objectMask))
            {
                Debug.Log("Hit: " + hit.collider.name);
                if (hit.collider.gameObject.CompareTag("Hexagon") && GameManger.Instance.IsRiverComplete() == false)
                {
                    Debug.Log("river");
                    RiverPuzzle(hit);
                }
                // Ram here
                else if (hit.collider.gameObject.CompareTag("NumberPad"))
                {
                    SafePuzzle(hit);

                    // Ram's gone from here

                    //selects tray
                }
                else if (hit.collider.gameObject.CompareTag("Tray"))
                {
                    TrayPuzzle(hit);

                }
                else if (hit.collider.gameObject.CompareTag("Cipher"))
                {
                    Cipher(hit);
                }
                else if (hit.collider.gameObject.CompareTag("Mirror"))
                {
                    Mirror(hit);
                }
                else if (hit.collider.gameObject.CompareTag("Lamp"))
                {
                    lampPuzzle(hit);
                }
                else if (hit.collider.gameObject.CompareTag("Hint"))
                {
                    ShowHint();
                }
                else if (hit.collider.gameObject.CompareTag("Mount"))
                {
                    Mount(hit);
                }


                // if player should hold the object
                if (isHolding == false)
                {
                    HoldingItem(hit);

                }
                else
                {
                    Debug.Log("Holding an item, can't pick another.");
                }

                lastHit = hit.collider;
            }
            else
            {
                Debug.Log("Didnt hit anything!");
            }

            if (Physics.Raycast(cameraTransform.position, cameraTransform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {

                if (hit.collider.gameObject.CompareTag("Mirror"))
                {
                    Debug.Log("mirror!");
                    Mirror(hit);
                }
            }
        }

        //drop item if player right clicks
        if (Input.GetMouseButtonDown(1))
        {
            DropObject();
        }

        // shows where teh player is going to teleport too
        if (Input.GetKey(KeyCode.G))
        {
            if (Physics.Raycast(cameraTransform.position, cameraTransform.TransformDirection(Vector3.forward), out hit, 10))
            {
                Debug.Log("Show Teleport");
                if (hit.collider.gameObject.CompareTag("Floor"))
                {
                    TeleportCircle(hit);
                }
            }
            else
            {
                Debug.Log("Dont Show Teleport");
            }



        }

        //Teleporting the player if player presses G
        if (Input.GetKeyUp(KeyCode.G))
        {
            if (Physics.Raycast(cameraTransform.position, cameraTransform.TransformDirection(Vector3.forward), out hit, 10, floorMask))
            {
                Debug.Log("Teleport!");
                if (hit.collider.gameObject.CompareTag("Floor"))
                {

                }
                Teleport(hit);
            }
            else
            {
                Debug.Log("Dont teleport!");
            }
        }

        //activate torch by pressing E
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (holding != null && holding.CompareTag("Torch_Holdable"))
            {

                RotateTorch();
            }
        }

        if (Input.GetKeyUp(KeyCode.M))
        {
            if (mirror == true)
            {
                Mirror1.SetActive(false);
                Mirror2.SetActive(false);
                Mirror3.SetActive(false);
                Mirror4.SetActive(false);
                mirror = false;
            }else
            {
                mirror = true;
                Mirror1.SetActive(true);
                Mirror2.SetActive(true);
                Mirror3.SetActive(true);
                Mirror4.SetActive(true);
            }
        
        }
    }

    private void Mount(RaycastHit hit)
    {
        if (GameManger.Instance.IsControlPanelComplete() == false && holding != null)
        {
            if (holding.CompareTag("Battery_Holdable"))
            {
                if (placeBattery1 == false)
                {
                    batteryMount1.SetActive(true);
                    placeBattery1 = true;
                    GameObject battery = holding;
                    DropObject();
                    battery.SetActive(false);
                }
                else if (placeBattery2 == false)
                {
                    batteryMount2.SetActive(true);
                    placeBattery2 = true;
                    GameObject battery = holding;
                    DropObject();
                    battery.SetActive(false);
                }
            } else if (holding.CompareTag("Key_Holdable"))
            {
                if (placeKey == false)
                {
                    keyMount.SetActive(true);
                    placeKey = true;
                    GameObject battery = holding;
                    DropObject();
                    battery.SetActive(false);
                }

            }

            if (placeBattery1 && placeBattery2 && placeKey)
            {
                GameManger.Instance.CompleteControlPanel();
                laser.StartCastLaser();
                // unlock laser
            }
        }

    }

    public void ShowHint()
    {
        hint.text = hints[UnityEngine.Random.Range(0, 3)];
    }

    private void RotateTorch()
    {
        if (torchRotated)
        {
            torch.transform.localPosition = new Vector3(0, 0f, 0);
            torch.transform.localRotation = Quaternion.Euler(-0, 0, 0);
            torch.transform.localPosition = new Vector3(0, -0.5f, 0);
            torchRotated = false;
        }
        else
        {
            torch.transform.localPosition = new Vector3(0, 0f, 0);
            torch.transform.localRotation = Quaternion.Euler(90, 0, 0);
            torch.transform.localPosition = new Vector3(0f, -0.1f, -0.75f);
            torchRotated = true;

        }
    }


    private void HighlightObjects()
    {
        RaycastHit hit;



        if (Physics.Raycast(cameraTransform.position, cameraTransform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, objectMask))
        {

            if (hit.collider.gameObject.CompareTag("Cipher")){
                /*string name = "HighlightCipher";

                string[] currentCipherName = hit.collider.name.Split('_');
                int currentNumber = int.Parse(currentCipherName[0]);
                name = name + currentNumber;

                if (cipherHiglight.ContainsKey(name))
                {
                    cipherHiglight[name].SetActive(true);

                    if (lastHighlight != null && lastHighlight.name.Equals(name) == false)
                    {

                        if (lastHighlight.CompareTag("Cipher1") == false)
                        {
                            highlights[lastHighlight.name].SetActive(false);
                        }
                        else
                        {
                            string name1 = "HighlightCipher";

                            string[] currentCipherName1 = hit.collider.name.Split('_');
                            int currentNumber1 = int.Parse(currentCipherName1[0]);
                            name1 = name1 + currentNumber1;

                            cipherHiglight[name1].SetActive(false);
                        }

                    }
                    lastHighlight = hit.collider.gameObject;

                }*/

            }
            
            
            if (highlights.ContainsKey(hit.collider.name))
            {
                //Debug.Log("Highlight: " + hit.collider.name);
                highlights[hit.collider.name].SetActive(true);
                if (lastHighlight != null && lastHighlight.name.Equals(hit.collider.name) == false)
                {

                    if (lastHighlight.CompareTag("Cipher") == false)
                    {
                        highlights[lastHighlight.name].SetActive(false);
                    }
                    else
                    {
                       /* string name1 = "HighlightCipher";

                        string[] currentCipherName1 = hit.collider.name.Split('_');
                        int currentNumber1 = int.Parse(currentCipherName1[0]);
                        name1 = name1 + currentNumber1;

                        cipherHiglight[name1].SetActive(false);*/
                    }

                }

                lastHighlight = hit.collider.gameObject;
            }

        }
        else
        {
            //Debug.Log("Dont highlight");
            if (lastHighlight != null)
            {
                Debug.Log("Unhighlight");
                if (highlights.ContainsKey(lastHighlight.name))
                {
                    highlights[lastHighlight.name].SetActive(false);
                }
                lastHighlight = null;

            }

        }

        if (Physics.Raycast(cameraTransform.position, cameraTransform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.CompareTag("Mirror"))
            {
                if (highlights.ContainsKey(hit.collider.name))
                {
                    Debug.Log("Highlight: " + hit.collider.name);
                    highlights[hit.collider.name].SetActive(true);
                    if (lastHighlight != null && lastHighlight.name.Equals(hit.collider.name) == false)
                    {
                        highlights[lastHighlight.name].SetActive(false);
                    }

                    lastHighlight = hit.collider.gameObject;
                }


            }
        }
        else
        {
           
            if (lastHighlight != null&& lastHighlight.CompareTag("Mirror"))
            {
                Debug.Log("Unhighlight mirror");
                if (highlights.ContainsKey(lastHighlight.name))
                {
                    highlights[lastHighlight.name].SetActive(false);
                }
                lastHighlight = null;

            }

        }

    }




        

    private void HighlightColour(RaycastHit hit)
    {
        Debug.Log("Lets highlight" + hit.collider.name);
        GameObject currentObject = hit.collider.gameObject;
        // Debug.Log("Lets highlight");

        lastRenderer = currentObject.GetComponent<Renderer>();

        if (lastRenderer != null)
        {
            lastHighlight = currentObject;
            Material[] currentMaterials = lastRenderer.materials;
            Debug.Log(currentMaterials);

            foreach (Material eachMaterial in currentMaterials)
            {

                lastMaterials.Add(eachMaterial.color);
            }

            foreach (Material eachMaterial in currentMaterials)
            {

                eachMaterial.SetColor("_Color", new Color(179, 0, 255));
            }

        }

    }


    //Teleport Player
    private void Teleport(RaycastHit hit)
    {
        teleport.SetActive(false);
        Debug.Log("Hit numbers " + hit.point);
        Vector3 teleportVector = new Vector3(hit.point.x, transform.position.y, hit.point.z);

        // unsure if the correct way to update players poistion, 
        //sometimes it glitchs out need to fix later
        transform.position = teleportVector;
        // rb.MovePosition()
        // cc.Move(teleport);
        Debug.Log("Teleport!" + teleportVector);
    }

    private void DropObject()
    {
        if (isHolding == true)
        {
            holding.transform.parent = null;

            Rigidbody rb = holding.transform.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;
            holding = null;
            isHolding = false;
            torchRotated = false;
        }
    }

    private void Holditem(GameObject item, Vector3 lp , float x, float y, float z)
    {
        //new Vector3(1.3f, -1.75f, 1.2f)
        item.transform.parent = hand.transform;
        item.transform.localPosition = lp;
        item.transform.Rotate(x, y, z);
    }

    // Checks if the puzzle is complete or not
    public Boolean IsRiverComplete(Dictionary<string,float> currentRiver)
    {
        foreach(KeyValuePair<string,float> entry in currentRiver)
        {
            if (entry.Value != completeRiver[entry.Key])
            {
                Debug.Log("currentRiver " + entry.Key + " " + entry.Value + " completeRiver " + completeRiver[entry.Key]);
                return false;
            }
                
        }

        return true;
    }

    // Convert the eulerAngles (with values like 0.2131232) to multiples of 60
    public static float convertAngles(float eulerAngles)
    {
        float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
        if (result < 0)
        {
            result += 360f;
        }
        return result;
    }

    private void MovePlayer()
    {
        // Moves the player

        //Gets wasd input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // from the input moves the player
        Vector3 direction = transform.right * horizontal + transform.forward * vertical;
        cc.Move(direction * Time.deltaTime * moveSpeed);

        // Gravity
        // makes sure the player is connected the the floor
        if (cc.isGrounded)
        {
            velocity = 0;
        }
        else
        {
            velocity -= gravity * Time.deltaTime;
            cc.Move(new Vector3(0, velocity, 0));
        }

    }

    private void changeMirrorVisibility(float alphaValue)
    {
        for(int i = 1; i < 5; i++)
        {
            Material m = GameObject.Find("Mirror" + i).GetComponent<Renderer>().material;
            Color oldColor = m.color;
            Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alphaValue);
            m.color = newColor;
        }
    }



    private void AddRiverRotations()
    {

        // The dictionary containing the correct rotation
        // values for completing the river puzzle
        completeRiver.Add("river_end.002", 120);
        completeRiver.Add("river_end.003", 120);
        completeRiver.Add("river_end.004", 300);
        completeRiver.Add("river_straight.002", 300);
        completeRiver.Add("river_straight.001", 60);
        completeRiver.Add("river_straight.003", 0);
        completeRiver.Add("river_straight.007", 120);
        completeRiver.Add("river_corner.004", 180);
        completeRiver.Add("river_corner.003", 240);
        completeRiver.Add("river_corner.001", 300);
        completeRiver.Add("river_corner.005", 300);
        completeRiver.Add("river_intersectionB.002", 0);
        completeRiver.Add("river_intersectionD.002", 0);
        completeRiver.Add("river_cornerSharp.002", 180);
        completeRiver.Add("river_cornerSharp.001", 60);
        completeRiver.Add("river_start.001", 300);
        completeRiver.Add("building_water.001", 120);

        // The dictionary containing the values for
        // the current state of the river puzzle
        currentRiver.Add("river_end.002", 0);
        currentRiver.Add("river_end.003", 0);
        currentRiver.Add("river_end.004", 0);
        currentRiver.Add("river_straight.002", 0);
        currentRiver.Add("river_straight.001", 0);
        currentRiver.Add("river_straight.003", 0);
        currentRiver.Add("river_straight.007", 0);
        currentRiver.Add("river_corner.004", 0);
        currentRiver.Add("river_corner.003", 0);
        currentRiver.Add("river_corner.001", 0);
        currentRiver.Add("river_corner.005", 0);
        currentRiver.Add("river_intersectionB.002", 0);
        currentRiver.Add("river_intersectionD.002", 0);
        currentRiver.Add("river_cornerSharp.002", 0);
        currentRiver.Add("river_cornerSharp.001", 0);
        currentRiver.Add("river_start.001", 0);
        currentRiver.Add("building_water.001", 0);

    }

    private void LoadObjects()
    {
        // players components
        cc = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();

        cameraTransform = Camera.main.transform;

        //load objects in scene
        dountObject = GameObject.FindGameObjectWithTag("Doughnut_Holdable");
        drawObject = GameObject.FindGameObjectWithTag("Draw");
        safeDoor = GameObject.FindGameObjectWithTag("SafeDoor");
        hand = GameObject.FindGameObjectWithTag("Hand");
        teleport = GameObject.FindGameObjectWithTag("Teleport");
        safe = GameObject.FindGameObjectWithTag("SafeAnimate");
        riverComplete = GameObject.FindGameObjectWithTag("RiverComplete");
        riverIncomplete = GameObject.FindGameObjectWithTag("RiverIncomplete");
        torch = GameObject.FindGameObjectWithTag("Torch_Holdable");
        doorway = GameObject.FindGameObjectWithTag("Door");


        riverComplete.SetActive(false);

        safeAnimator = safe.GetComponent<Animator>();
        doorwayAnimator = doorway.GetComponent<Animator>();

        teleport.SetActive(false);


        draw = drawObject.GetComponent<OpenDraw>();

        GameObject[] highlightArray = GameObject.FindGameObjectsWithTag("Highlight");
        Debug.Log(highlightArray);

       

        foreach(GameObject eachHighlight in highlightArray)
        {
            Debug.Log(eachHighlight.name);
            Debug.Log(eachHighlight.transform.parent.name);
           // eachHighlight.transform.position = new Vector3(0, 0, 0);
            string parent = eachHighlight.transform.parent.name;
            highlights.Add(parent, eachHighlight);
            eachHighlight.SetActive(false);
        }


        keyMount = GameObject.FindGameObjectWithTag("Key_Mount");
        keyMount.SetActive(false);
        batteryMount1 = GameObject.FindGameObjectWithTag("Battery_Mount_1");
        batteryMount1.SetActive(false);
        batteryMount2 = GameObject.FindGameObjectWithTag("Battery_Mount_2");
        batteryMount2.SetActive(false);

        GameObject[] cipherArray = GameObject.FindGameObjectsWithTag("Highlight_Cipher");
        Debug.Log("Cipher:" + cipherArray);



        foreach (GameObject eachHighlight in cipherArray)
        {
            Debug.Log(eachHighlight.name);
            Debug.Log(eachHighlight.transform.parent.name);
            // eachHighlight.transform.position = new Vector3(0, 0, 0);
           // string parent = eachHighlight.transform.parent.name;
            cipherHiglight.Add(eachHighlight.name, eachHighlight);
            eachHighlight.SetActive(false);
        }



        //highlights = 
    }

    private void RiverPuzzle(RaycastHit hit)
    {
        // Rotate hexagon by 60 degrees
        hit.collider.gameObject.transform.Rotate(0, 60, 0);
        hexSound = hit.collider.gameObject.transform.parent.GetComponent<AudioSource>();
        hexSound.Play();

        Mirror1.SetActive(true);
        Mirror2.SetActive(true);
        Mirror3.SetActive(true);
        Mirror4.SetActive(true);

        // Updates the dictionary with the new angle value
        currentRiver[hit.collider.name] = convertAngles(hit.collider.gameObject.transform.localEulerAngles.y);
        // If the user rotates the hexagon to correct position
        // then, puzzle completion is checked
        if (currentRiver[hit.collider.name] == completeRiver[hit.collider.name])
        {
            if (IsRiverComplete(currentRiver))
            {
                Debug.Log("River Puzzle completed succcessfully!");
                riverComplete.SetActive(true);
                riverIncomplete.SetActive(false);
                GameManger.Instance.CompleteRiver();
                //changeMirrorVisibility(2f);
                Mirror1.SetActive(true);
                Mirror2.SetActive(true);
                Mirror3.SetActive(true);
                Mirror4.SetActive(true);
            }
        }
    }

    private void lampPuzzle(RaycastHit hit)
    {
        Debug.Log("hit collider in lamp is" + hit.collider.name);
        lampTimes += 1;
        float lampLight = hit.collider.gameObject.transform.GetChild(0).GetComponent<Light>().range;
        bool isLightOn = lampLight > 0;

        if (hit.collider.name == "Lamp")
        {
            hexSound = hit.collider.gameObject.GetComponent<AudioSource>();
            hexSound.Play();
        }
        else
        {
            hexSound = hit.collider.gameObject.transform.parent.GetComponent<AudioSource>();
            hexSound.Play();
        }

        if (isLightOn)
            hit.collider.gameObject.transform.GetChild(0).GetComponent<Light>().range = 0;
        else
            hit.collider.gameObject.transform.GetChild(0).GetComponent<Light>().range = (float)1.69;

        GameObject doorHinge = GameObject.Find("/room 2/wallDoorway/doorHinge");
        doorHinge.transform.Rotate(0, -120, 0);
        Debug.Log("Door object is" + doorHinge);
        if (lampTimes == 6)
        {
            Debug.Log("\n\n\n\n\n\nLamp Puzzle completed");
            battery2.SetActive(true);
        }
    }


    private void SafePuzzle(RaycastHit hit)
    {
        // if safe puzzle not completed
        if (GameManger.Instance.IsSafeComplete() == false)
        {
            string[] buttonNumber = hit.collider.name.Split(' ');
            hexSound = hit.collider.gameObject.GetComponent<AudioSource>();
            hexSound.Play();
            safeNumber += buttonNumber[1];
            safeArray.Add(buttonNumber[1]);
            Debug.Log("Safenumber " + safeNumber);

            // if more than 5 numbers are entered
            if (safeArray.Count > 4)
            {
                safeArray.Reverse();
                // checks the last 5 numbers entered
                string current = safeArray[4] + safeArray[3] + safeArray[2] + safeArray[1] + safeArray[0];

                // if the last numbers entered equal safeCode
                if (current.Equals(safeCode))
                {
                    Debug.Log("Safe Open :)");
                    hexSound = hit.collider.gameObject.transform.parent.GetComponent<AudioSource>();
                    hexSound.Play();
                    safeAnimator.SetTrigger("OpenSafe");
                    // need to change this becuase the rotation hits things
                    // safeDoor.transform.Rotate(0, -150, 0);

                    GameManger.Instance.CompleteSafe();
                }
                else
                {
                    Debug.Log("Safe code wrong :(");
                }
                safeArray.Reverse();
            }
        }
    }

    private void TeleportCircle(RaycastHit hit)
    {
        teleport.SetActive(true);
        teleport.transform.position = hit.point;
    }



    // Logic for Griffin and Tray Puzzle
    private void TrayPuzzle(RaycastHit hit)
    {
        if (holding.CompareTag("Doughnut_Holdable") && GameManger.Instance.IsGriffinComplete() == false)
        {
            DropObject();
            StartCoroutine(waiter(hit, (float)10.5));
        }
           
       
    }

    // Wait for a specific time before the tray sound comes to play
    private IEnumerator waiter(RaycastHit hit, float seconds) 
    {
        // Checks if player is holding a Doughnut
       
        Debug.Log("Dount and tray puzzle complete!" + hit.collider.name);

        // Moves Doughnut to position of tray
        dountObject.transform.position = hit.collider.gameObject.transform.position;
        GameManger.Instance.CompleteGriffin();

        //The Logic for the Draw of the the Griffin Box 
        draw.Open();
        hexSound = hit.collider.gameObject.GetComponent<AudioSource>();
        yield return new WaitForSecondsRealtime(seconds);
        hexSound.Play();

        //Drop the Doughnut in players hand
       
        
        
    }

    private void HoldingItem(RaycastHit hit)
    {
        if (hit.collider.gameObject.tag.Contains("_Holdable"))
        {
            Debug.Log("Lets hold! " + hit.collider.gameObject.name);

            holding = hit.collider.gameObject;

            Rigidbody rb = hit.collider.gameObject.transform.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeAll;
            hit.collider.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
           
            isHolding = true;
            if (hit.collider.gameObject.CompareTag("Torch_Holdable"))
            {
                // Holditem(hit.collider.gameObject, new Vector3(0.25f, -0.75f, 0.25f), 0, 0, 0);
                Holditem(hit.collider.gameObject, new Vector3(0f, -0.5f, 0f), 0, 0, 0);
            }
            else if (hit.collider.gameObject.CompareTag("Doughnut_Holdable"))
            {
                Holditem(hit.collider.gameObject, new Vector3(0, 0f, 0), -90, 0, 0);
            }
            else if (hit.collider.gameObject.CompareTag("Battery_Holdable"))
            {
                Holditem(hit.collider.gameObject, new Vector3(0, 0f, 0), 0, 0, 0);
                hit.collider.gameObject.transform.localScale = new Vector3(5, 5, 5);
            }
            else
            {
                // default setting to place the object in players hand
                Holditem(hit.collider.gameObject, new Vector3(0, 0, 0), 0, 0, 0);
            }
        }
        else
        {
            Debug.Log("Can't Hold " + hit.collider.gameObject.name);
        }
    }

    private void Cipher(RaycastHit hit)
    {
        //Debug.Log("Let's cipher!!");
        if (lastHit != null && lastHit.CompareTag("Cipher") && prevSwap==false)
        {

            //swap position

            // get the current index postion of the cicphers
            string[] currentCipherName = hit.collider.name.Split('_');
            int currentNumber = int.Parse(currentCipherName[0]);

            string[] buttonNumber = lastHit.name.Split('_');
            int lastNumber = int.Parse(buttonNumber[0]);

            // each block moves 3 from original
            float z = 2.8f;

            // swaps the positions
            if (currentNumber != lastNumber)
            {
                hexSound = hit.collider.gameObject.transform.parent.GetComponent<AudioSource> ();
                hexSound.Play();
                // get transform positon of both cipher blocks
                float currentX = hit.collider.gameObject.transform.localPosition.x;
                float currentY = hit.collider.gameObject.transform.localPosition.y;
                float currentZ = hit.collider.gameObject.transform.localPosition.z;

                float lastX = lastHit.gameObject.transform.localPosition.x;
                float lastY = lastHit.gameObject.transform.localPosition.y;
                float lastZ = lastHit.gameObject.transform.localPosition.z;
                Debug.Log("Swaping Positions: " + hit.collider.name + " and " + lastHit.name);

                // swaps positions of the ciphers blocks
                hit.collider.gameObject.transform.localPosition =
                    new Vector3(currentX, currentY, currentZ + z * (currentNumber - lastNumber));
                lastHit.gameObject.transform.localPosition =
                    new Vector3(lastX, lastY, lastZ - z * (currentNumber - lastNumber));

                // change the index postion of the cipher
                string lastName = lastHit.name;
                string currentName = hit.collider.name;

                hit.collider.name = lastName;
                lastHit.name = currentName;

                prevSwap = true;
            }
            else
            {
                prevSwap = false;
                Debug.Log("Ciphers same block not swaping");
            }
        }
        else
        {
            prevSwap = false;
            Debug.Log("Last object not a cipher");
        }
    }

    private void Mirror(RaycastHit hit)
    {
        string name = hit.collider.name;
        
        if (name == "Mirror1")
        {
            if ((int) hit.collider.gameObject.transform.localRotation.eulerAngles.y == 0)
            {
                hit.collider.gameObject.transform.Rotate(0, 240, 0);
            } else
            {
                hit.collider.gameObject.transform.Rotate(0, 60, 0);
            }
        }

        if (name == "Mirror2")
        {
            float xPos = hit.collider.gameObject.transform.position.x;
            float yPos = hit.collider.gameObject.transform.position.y;
            float zPos = hit.collider.gameObject.transform.position.z;
            //float step = (float) 0.5;
            float step;

            if (xPos > 0.3)
            {
                // step = (float) -0.5;
                step = -2;
            } else //if (xPos < -1.7)
            {
                step = (float) 0.5;
            }
            
            Vector3 newPos = new Vector3(xPos+step, yPos, zPos);
            hit.collider.gameObject.transform.position = newPos;
            
        }

        if (name == "Mirror3")
        {
            if ((int) hit.collider.gameObject.transform.localRotation.eulerAngles.y == 99)
            {
                hit.collider.gameObject.transform.Rotate(0, 240, 0);
            } else
            {
                hit.collider.gameObject.transform.Rotate(0, 60, 0);
            }
        }

        if (name == "Mirror4")
        {
            float xPos = hit.collider.gameObject.transform.position.x;
            float yPos = hit.collider.gameObject.transform.position.y;
            float zPos = hit.collider.gameObject.transform.position.z;
            //float step = (float) 0.5;
            Vector3 newPos;

            if ((int) xPos == -2 && (int) yPos == 2 && (int) zPos == -3)
            {
                // step = (float) -0.5;
                if ((int) hit.collider.gameObject.transform.localRotation.eulerAngles.x != 13)
                {
                    hit.collider.gameObject.transform.Rotate(57, 0, 0);
                    newPos = new Vector3(xPos, yPos, zPos);
                } else
                {
                    newPos = new Vector3(xPos, yPos, zPos + 2);
                }
            } else if ((int) xPos == -2 && (int) yPos == 2 && (int) zPos == -1)
            {
                newPos = new Vector3(xPos+3, yPos, zPos);
            } else if ((int) xPos == 0 && (int) yPos == 2 && (int) zPos == -1)
            {
                if ((int)hit.collider.gameObject.transform.localRotation.eulerAngles.x == 13)
                {
                    hit.collider.gameObject.transform.Rotate(-57, 0, 0);
                    newPos = new Vector3(xPos, yPos, zPos);
                }
                else
                {
                    newPos = new Vector3(xPos, yPos, zPos - 2);
                }
                //newPos = new Vector3(xPos, yPos, zPos-2);
            }
            else
            {
                newPos = new Vector3(xPos - 3, yPos, zPos);
            }


            //Vector3 newPos = new Vector3(xPos, yPos, zPos);
            hit.collider.gameObject.transform.position = newPos;

        }
    }

 }
