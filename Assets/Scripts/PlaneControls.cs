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

    private cakeslice.Outline[] myOutlines;

    private List<GameObject> otherPlayersToEcho;
    public float EchoDelayModifier = 0.01f;
    public float ChirpCooldown =5f;
    private float chirpTime = 0f;
    public float EchoOutlineTimeSpan = 0.1f;
    public float ReturnEchoDelay = 1f;

    public bool isTest = false;


    string time_currentGameEndsIn_keyString = "time_currentGameEndsIn";
    string time_nextGameStartsIn_keyString = "time_nextGameStartsIn";
    string roundStarted_keyString = "RoundStarted";

    private float timeSpan_batMothCooldown = 3f;
    private float lastBatMothChange;

    public AudioClip[] BatNoises;
    public AudioClip[] MothNoises;

    public AudioSource flapSource;
    public AudioSource pingSource;

    public Vector3 SpawnLocation;
    public Quaternion SpawnRotation;

    // Use this for initialization
    void Start () {

        otherPlayersToEcho = new List<GameObject>();

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

        isFlapping = true;

        moveRight = false;
        moveLeft = false;
        moveUp = false;
        moveDown = false;
        moveStop = false;

        isMoth = name.Contains("Moth");
        //CBUG.Do("I am a " + (isMoth ? "Moth!" : "Bat!"));

        myOutlines = this.GetComponentsInChildren<cakeslice.Outline>();

        flapSource.Stop();
        if (isMoth)
        {
            HideMyOutline();
            flapSource.clip = MothNoises[0];
        }
        else
        {
            flapSource.clip = BatNoises[1];
        }
        flapSource.Play();

        if (m_PhotonView.isMine)
        {
            if(isMoth)
            {
                GameObject.Find("BatUI").GetComponent<Image>().enabled = false;
                GameObject.Find("SnackUI").GetComponent<Image>().enabled = true;
                GameObject.Find("BatSonarUI").GetComponent<Image>().enabled = false;
                CBUG.Do("IsMoth!");
            }
            else
            {
                GameObject.Find("BatUI").GetComponent<Image>().enabled = true;
                GameObject.Find("SnackUI").GetComponent<Image>().enabled = false;
                GameObject.Find("BatSonarUI").GetComponent<Image>().enabled = true;
                CBUG.Do("IsBat!");
            }
        }

        lastBatMothChange = Time.time;
    }

    // Update is called once per frame
    void Update() {

        if(isFlapping && (   
            Mathf.Abs(currentRot.x) < MinimumHoverRotSpeedX ||
            Mathf.Abs(currentRot.z) < MinimumHoverRotSpeedZ ) 
        ) {
            isFlapping = false;
            m_Animator.SetBool("Flap", isFlapping);
            if(isMoth == false) {
                flapSource.Pause();
            }
        }
        if(!isFlapping && (
            Mathf.Abs(currentRot.x) > MinimumHoverRotSpeedX ||
            Mathf.Abs(currentRot.z) > MinimumHoverRotSpeedZ ) 
        ) {
            isFlapping = true;
            m_Animator.SetBool("Flap", isFlapping);
            if(isMoth == false) {
                flapSource.Play();
            }
        }

        if (isTest)
            return;


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
            //CBUG.Do("Stopping!");
            moveStop = true;
            m_Animator.SetBool("Stop", moveStop);
            flapSource.Pause();
        } else if (Input.GetAxis("Stop") == 0 && isMoth && moveStop == true )
        {
            //CBUG.Do("Resuming Moving!");
            moveStop = false;
            flapSource.Play();
            m_Animator.SetBool("Stop", moveStop);
        }



        if (!isMoth && Time.time - chirpTime > ChirpCooldown)
        {
            GameObject.Find("BatSonarUI").GetComponent<Image>().enabled = true;
        }

        if (Input.GetAxis("Chirp") > 0 && !isMoth  && Time.time - chirpTime > ChirpCooldown)
        {
            Chirp();
            chirpTime = Time.time;
        }


        if((isMoth && Input.GetButtonDown("Batify") && Time.time - lastBatMothChange > timeSpan_batMothCooldown) || Input.GetKeyDown(KeyCode.N))
        {
            var roomProperties = PhotonNetwork.room.CustomProperties;
            //if round started
            if (roomProperties.ContainsKey(roundStarted_keyString) && (bool)roomProperties[roundStarted_keyString] == false)
            {
                lastBatMothChange = Time.time;
                Batify();
            }
        }

        if ((isMoth == false && Input.GetButtonDown("Mothify") && Time.time - lastBatMothChange > timeSpan_batMothCooldown) || Input.GetKeyDown(KeyCode.N))
        {
            var roomProperties = PhotonNetwork.room.CustomProperties;
            //if round started
            if (roomProperties.ContainsKey(roundStarted_keyString) && (bool)roomProperties[roundStarted_keyString] == false)
            {
                lastBatMothChange = Time.time;
                Mothify();
            }
        }

    }

    [PunRPC]
    public void Batify()
    {
        if (m_PhotonView.isMine)
        {
            Destroy(myCam);
            PhotonNetwork.Destroy(gameObject);
            var rotation = new Vector3(0f, transform.rotation.eulerAngles.y, 0f);
            GameObject newPlayerObject = PhotonNetwork.Instantiate("Bat", SpawnLocation, SpawnRotation, 0);
        }
    }

    [PunRPC]
    public void Mothify()
    {
        if (m_PhotonView.isMine)
        {
            Destroy(myCam);
            PhotonNetwork.Destroy(gameObject);
            var rotation = new Vector3(0f, transform.rotation.eulerAngles.y, 0f);
            GameObject newPlayerObject = PhotonNetwork.Instantiate("Moth", SpawnLocation, SpawnRotation, 0);
        }
    }

    private void FixedUpdate()
    {

        if (m_PhotonView.isMine != true)
            return;

        if (isTest)
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

    void OnDestroy() {
        CBUG.Do("I was eaten!");
        var roomProperties = PhotonNetwork.room.CustomProperties;
        //if round started
        if (isMoth && roomProperties.ContainsKey(roundStarted_keyString) && (bool)roomProperties[roundStarted_keyString] == true) {
            pingSource.PlayOneShot(BatNoises[3]);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //CBUG.Do("Trigger tag Name Other:" + other.tag + " I am: " + tag);
        //CBUG.Do("Trigger obj Name Other:" + other.name + " I am: " + name);

        if (isMoth && other.name.Contains("Bat") || other.name.Contains("Eat"))
        {
            isMoth = false;
            CBUG.Do("BATIFY THE PLAYER!!");
            Batify();
            pingSource.PlayOneShot(BatNoises[2]);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //CBUG.Do("Collider tag Name Other:" + collision.gameObject.tag + " I am!~ " + tag);
        //CBUG.Do("Collider obj Name Other:" + collision.gameObject.name + " I am!~ " + name);
    }

    private void Chirp ()
    {
        GameObject.Find("BatSonarUI").GetComponent<Image>().enabled = false;
        m_PhotonView.RPC("BatChirped", PhotonTargets.All);
        GameObject[] netPlayers = GameObject.FindGameObjectsWithTag("NetPlayer");
        otherPlayersToEcho = new List<GameObject>(netPlayers);
        foreach (GameObject player in otherPlayersToEcho)
        {
            float distance = Mathf.Abs((player.transform.position - transform.position).magnitude);
            Tools.DelayFunction(player.GetComponent<PlaneControls>().ShowMyOutline, distance * EchoDelayModifier);
        }

        //foreach (GameObject player in otherPlayersToEcho)
        //{
        //    float distance = Mathf.Abs((player.transform.position - transform.position).magnitude);
        //    Tools.DelayFunction(player.GetComponent<PlaneControls>().ShowMyOutline, (distance * EchoDelayModifier) + (ReturnEchoDelay / distance) * EchoDelayModifier);
        //}
    }

    private void ShowMyOutline()
    {
        pingSource.PlayOneShot(MothNoises[1]);
        foreach (cakeslice.Outline line in myOutlines)
        {
            line.eraseRenderer = false;
        }
        Tools.DelayFunction(HideMyOutline, EchoOutlineTimeSpan);
    }

    private void HideMyOutline()
    {
        foreach (cakeslice.Outline line in myOutlines)
        {
            line.eraseRenderer = true;
        }
    }

    [PunRPC]
    private void BatChirped()
    {
        pingSource.PlayOneShot(BatNoises[0]);
    }
}
