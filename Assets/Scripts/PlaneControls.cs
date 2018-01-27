using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaneControls : MonoBehaviour {

    private Rigidbody r;

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

    // Use this for initialization
    void Start () {
        r = GetComponent<Rigidbody>();
        m_PhotonView = GetComponent<PhotonView>();
        m_TransformView = GetComponent<PhotonTransformView>();
        m_Animator = GetComponent<Animator>();

        currentRot = r.rotation.eulerAngles;
        currentSpd = r.velocity;
        rotSpeedXVec = new Vector3(RotSpeedX, 0f, 0f);
        rotSpeedZVec = new Vector3(0, 0f, RotSpeedZ);
        movSpeedXVec = new Vector3(MovSpeedX, 0f, 0f);
        movSpeedYVec = new Vector3(0f, MovSpeedY, 0f);
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

        currentSpd = transform.forward * MovSpeedZ;
        if(Input.GetAxis("Horizontal") > 0)
        {
            if (currentRot.z >= -TargetRotZ)
            {
                currentRot -= rotSpeedZVec;
            }
            currentRot += new Vector3(0f, RotSpeedY, 0f);
        }
        else if (Input.GetAxis("Horizontal") < 0)
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
        if (Input.GetAxis("Vertical") > 0)
        {
            if (currentRot.x >= -TargetRotX)
                currentRot -= rotSpeedXVec;
        }
        else if (Input.GetAxis("Vertical") < 0)
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
        r.rotation = Quaternion.Euler(currentRot);
        r.velocity = currentSpd;
    }
}
