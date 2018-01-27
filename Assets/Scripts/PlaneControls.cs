using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaneControls : MonoBehaviour {

    private Rigidbody r;

    public float TargetRotZ;
    public float TargetRotX;
    public float RotSpeedZ;
    public float RotSpeedX;
    public float MovSpeedX;
    public float MovSpeedY;
    public float LeftWall;
    public float RightWall;
    private Vector3 currentRot;
    private Vector3 rotSpeedXVec;
    private Vector3 rotSpeedZVec;
    private Vector3 currentSpd;
    private Vector3 movSpeedXVec;
    private Vector3 movSpeedYVec;

    public Text TextToFade;
    public GameObject EndGameText;

    private bool canFade;
    public string FriendCode;

    Vector3 m_LastPosition;
    PhotonView m_PhotonView;
    PhotonTransformView m_TransformView;
    Animator m_Animator;

    // Use this for initialization
    void Start () {
        r = GetComponent<Rigidbody>();
        currentRot = r.rotation.eulerAngles;
        currentSpd = r.velocity;
        rotSpeedXVec = new Vector3(RotSpeedX, 0f, 0f);
        rotSpeedZVec = new Vector3(0, 0f, RotSpeedZ);
        movSpeedXVec = new Vector3(MovSpeedX, 0f, 0f);
        movSpeedYVec = new Vector3(0f, MovSpeedY, 0f);
        fogEnd = RenderSettings.fogEndDistance;
        currentFogEnd = 0f;

        opening = true;
        closing = false;

        fadeTextOut = false;
        fadeTextIn = false;
        textFadedIn = false;
        textFadedOut = false;

        TextToFade.CrossFadeColor(Color.clear, 0f, true, true, true);
        canFade = false;
        if (Application.absoluteURL.Contains(FriendCode) || Application.isEditor)
            canFade = true;
    }

    // Update is called once per frame
    void Update() {

        if (m_PhotonView.isMine != true)
            return;

    }

    private void FixedUpdate()
    {

        if (m_PhotonView.isMine != true)
            return;

        currentSpd = Vector3.zero;
        if(Input.GetAxis("Horizontal") > 0)
        {
            if (currentRot.z >= -TargetRotZ)
                currentRot -= rotSpeedZVec;
            if (transform.position.x <= RightWall)
                currentSpd += movSpeedXVec;
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            if (currentRot.z <= TargetRotZ)
                currentRot += rotSpeedZVec;
            if (transform.position.x >= LeftWall)
                currentSpd -= movSpeedXVec;
        }
        else
        {
            if (currentRot.z > 0f)
                currentRot -= rotSpeedZVec;
            if (currentRot.z < 0f)
                currentRot += rotSpeedZVec;
        }
        if (Input.GetAxis("Vertical") > 0)
        {
            if (currentRot.x >= -TargetRotX)
                currentRot -= rotSpeedXVec;
            if (transform.position.y <= Ceiling)
                currentSpd += movSpeedYVec;
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            if (currentRot.x <= TargetRotX)
                currentRot += rotSpeedXVec;
            if (transform.position.y >= Floor)
                currentSpd -= movSpeedYVec;
        }
        else
        {
            if (currentRot.x > 0f)
                currentRot -= rotSpeedXVec;
            if (currentRot.x < 0f)
                currentRot += rotSpeedXVec;
        }
        r.rotation = Quaternion.Euler(currentRot);
        r.velocity = currentSpd;
    }
}
