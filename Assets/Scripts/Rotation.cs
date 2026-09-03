using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class Rotation : MonoBehaviour
{

    public GameObject model;
    public Transform rotation;
    public bool rotating = false;
    public int speed = 20;
    public Vector3 origin = new Vector3(-4.41838884f, 9.61855698f, -0.574756682f);
    public Quaternion startRotation;
    public float rotationAmount = 0f;
    

    void Start()
    {
        rotation = model.transform.Find("Model");
        origin = model.transform.position;
        startRotation = model.transform.localRotation;

    }

    public void StartRotation() {
        rotating = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (rotating) {
            model.transform.Rotate(0, speed * Time.deltaTime, 0); 

            rotationAmount += speed * Time.deltaTime;


            if (rotationAmount >= 360f) {
                rotationAmount = 0f;
                rotating = false;
                Reset();
            }
        }
    }

    

    void Reset() {

        rotating = false;
        rotationAmount = 0f;
        model.transform.SetPositionAndRotation(origin, startRotation);
    }
}
