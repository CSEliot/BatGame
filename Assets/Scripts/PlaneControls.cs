using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaneControls : Photon.MonoBehaviour {

    private Rigidbody r;

    public float MinimumHoverRotSpeedX;
    public float MinimumHoverRotSpeedZ;
    private bool isFlapping;

    public float TargetRotZ;
    public float TargetRotX;

    public float RotSpeedX;
    public float RotSpeedZ;
    public float RotSpeedY;
    public float MovSpeedX;
    public float MovSpeedY;
    public float MovSpeedZ;

    private Vector3 currentRot;
    private Vector3 rotSpeedXVec;
    private Vector3 rotSpeedZVec;
    private Vector3 currentSpd;
    private Vector3 movSpeedXVec;
    private Vector3 movSpeedYVec;
    
    Vector3 m_LastPosition;
    PhotonView m_PhotonView;
    PhotonTransformView m_TransformView;
    Animator m_Animator;

    public GameObject CameraToSpawn;
    private GameObject myCam;

    private bool moveRight;
    private bool moveLeft;
    private bool moveUp;
    private bool moveDown;
    private bool moveStop;

    private bool isMoth;

    private List<GameObject> otherPlayers;
    public float EchoDelayModifier = 0.1f;

    private cakeslice.Outline[] myOutlines;


    // Use this for initialization
    void Start () {

        otherPlayers = new List<GameObject>();

        r = GetComponent<Rigidbody>();
        m_PhotonView = GetComponent<PhotonView>();
        m_TransformView = GetComponent<PhotonTransformView>();
        m_Animator = GetComponentInChildren<Animator>();

        currentRot = r.rotation.eulerAngles;
        currentSpd = r.velocity;
        rotSpeedXVec = new Vector3(RotSpeedX, 0f, 0f);
        rotSpeedZVec = new Vector3(0, 0f, RotSpeedZ);
        movSpeedXVec = new Vector3(MovSpeedX, 0f, 0f);
        movSpeedYVec = new Vector3(0f, MovSpeedY, 0f);

        if(m_PhotonView.isMine != true)
        {
            tag = "NetPlayer";
        } else
        {
            tag = "Player";
            myCam = Instantiate(CameraToSpawn) as GameObject;
        }
        CBUG.Do("My tag is: " + tag);

        isFlapping = true;

        moveRight = false;
        moveLeft = false;
        moveUp = false;
        moveDown = false;
        moveStop = false;

        isMoth = name.Contains("Moth");
        CBUG.Do("I am a " + (isMoth ? "Moth!" : "Bat!"));

        myOutlines = this.GetComponentsInChildren<cakeslice.Outline>();
    }

    // Update is called once per frame
    void Update() {

        if(isFlapping && (   
            Mathf.Abs(currentRot.x) < MinimumHoverRotSpeedX ||
            Mathf.Abs(currentRot.z) < MinimumHoverRotSpeedZ ) 
        ) {
            isFlapping = false;
            m_Animator.SetBool("Flap", isFlapping);
        }
        if(!isFlapping && (
            Mathf.Abs(currentRot.x) > MinimumHoverRotSpeedX ||
            Mathf.Abs(currentRot.z) > MinimumHoverRotSpeedZ ) 
        ) {
            isFlapping = true;
            m_Animator.SetBool("Flap", isFlapping);
        }

        if (m_PhotonView.isMine != true)
            return;

        if (Input.GetAxis("Horizontal") > 0)
        {
            moveRight = true;
            moveLeft = false;
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            moveRight = false;
            moveLeft = true;
        }
        else
        {
            moveRight = false;
            moveLeft = false;
        }
        if (Input.GetAxis("Vertical") > 0)
        {
            moveUp = true;
            moveDown = false;
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            moveUp = false;
            moveDown = true;
        }
        else
        {
            moveUp = false;
            moveDown = false;
        }

        if(Input.GetAxis("Stop") > 0 && isMoth && moveStop == false)
        {
            CBUG.Do("Stopping!");
            moveStop = true;
            m_Animator.SetBool("Stop", moveStop);
        } else if (Input.GetAxis("Stop") == 0 && moveStop == true )
        {
            CBUG.Do("Resuming Moving!");
            moveStop = false;
            m_Animator.SetBool("Stop", moveStop);
        }

    }

    private void FixedUpdate()
    {

        if (m_PhotonView.isMine != true)
            return;

        currentSpd = transform.forward * MovSpeedZ;
        if(moveRight)
        {
            if (currentRot.z >= -TargetRotZ)
            {
                currentRot -= rotSpeedZVec;
            }
            currentRot += new Vector3(0f, RotSpeedY, 0f);
        }
        else if (moveLeft)
        {
            if (currentRot.z <= TargetRotZ)
                currentRot += rotSpeedZVec;
            currentRot -= new Vector3(0f, RotSpeedY, 0f);
        }
        else
        {
            if (currentRot.z > 0f)
            {
                currentRot -= rotSpeedZVec;
                currentRot -= new Vector3(0f, RotSpeedY, 0f);
            }
            if (currentRot.z < 0f)
            {
                currentRot += rotSpeedZVec;
                currentRot += new Vector3(0f, RotSpeedY, 0f);
            }
        }
        if (moveUp)
        {
            if (currentRot.x >= -TargetRotX)
                currentRot -= rotSpeedXVec;
        }
        else if (moveDown)
        {
            if (currentRot.x <= TargetRotX)
                currentRot += rotSpeedXVec;
        }
        else
        {
            if (currentRot.x > 0f)
                currentRot -= rotSpeedXVec;
            if (currentRot.x < 0f)
                currentRot += rotSpeedXVec;
        }
        if (moveStop)
        {
            r.isKinematic = true;
            r.isKinematic = false;
            currentSpd = Vector3.zero;
        }

        r.rotation = Quaternion.Euler(currentRot);
        r.velocity = currentSpd;
    }

    [PunRPC]
    void KillSelf ()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        CBUG.Do("Trigger tag Name:" + other.tag + " from " + name);
        CBUG.Do("Trigger obj Name:" + other.name + " from " + name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CBUG.Do("Collider tag Name:" + collision.gameObject.tag + " from " + name);
        CBUG.Do("Collider obj Name:" + collision.gameObject.name + " from " + name);
    }

    private void Screech ()
    {
        GameObject[] netPlayers = GameObject.FindGameObjectsWithTag("NetPlayer");
        otherPlayers = new List<GameObject>(netPlayers);
        foreach (GameObject player in otherPlayers)
        {
            float distance = Mathf.Abs((player.transform.position - transform.position).sqrMagnitude);
            Tools.DelayFunction(player.GetComponent<PlaneControls>().ShowMyOutline, distance * EchoDelayModifier);
        }

        foreach (GameObject player in otherPlayers)
        {
            float distance = Mathf.Abs((player.transform.position - transform.position).sqrMagnitude);
            Tools.DelayFunction(player.GetComponent<PlaneControls>().ShowMyOutline, (distance * EchoDelayModifier) + (distance / EchoDelayModifier));
        }
    }

    private void ShowMyOutline()
    {
        foreach (cakeslice.Outline line in myOutlines)
        {
            line.eraseRenderer = false;
        }
        Tools.DelayFunction(HideMyOutline, 0.2f);
    }

    private void HideMyOutline()
    {
        foreach (cakeslice.Outline line in myOutlines)
        {
            line.eraseRenderer = true;
        }
    }
}
