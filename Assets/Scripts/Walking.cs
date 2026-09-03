using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walking : MonoBehaviour {

    public GameObject model;
    public Transform modelTransform;
    public GameObject leftLegContainer;
    public GameObject rightLegContainer;
    public GameObject leftWingContainer;
    public GameObject rightWingContainer;
    public bool walking = false;
    public int speed = 5;
    public float walkTime = 0f;
    public Vector3 origin;
    public Quaternion leftLegStart;
    public Quaternion rightLegStart;
    public Quaternion leftWingStart;
    public Quaternion rightWingStart;

    void Start() {
        modelTransform = model.transform.Find("Model");
        origin = model.transform.position;

        // Store leg rotations
        leftLegStart = leftLegContainer.transform.localRotation;
        rightLegStart = rightLegContainer.transform.localRotation;

        // Store wing rotations
        leftWingStart = leftWingContainer.transform.localRotation;
        rightWingStart = rightWingContainer.transform.localRotation;
    }

    public void StartWalking() {
        walking = true;
        walkTime = 0f;
    }

    void Update() {
        if (walking) {
            walkTime += Time.deltaTime;

            // Alternating leg swing front-to-back (Z-axis rotation)
            float legSwing = Mathf.Sin(walkTime * speed * 3f) * 30f;

            // Rotate entire leg containers on Z-axis
            rightLegContainer.transform.localRotation = Quaternion.Euler(0, 0, -legSwing);
            leftLegContainer.transform.localRotation = Quaternion.Euler(0, 0, legSwing);

            // Rotate wings/arms OPPOSITE to legs (when right leg goes forward, left arm goes forward)
            rightWingContainer.transform.localRotation = Quaternion.Euler(0, 0, legSwing);
            leftWingContainer.transform.localRotation = Quaternion.Euler(0, 0, -legSwing);

            // Body bob up and down
            float bobAmount = Mathf.Abs(Mathf.Sin(walkTime * speed * 3f)) * 0.2f;
            model.transform.position = new Vector3(
                origin.x,
                origin.y + bobAmount,
                origin.z
            );

            // Stop after 2 seconds
            if (walkTime >= 2f) {
                walking = false;
                walkTime = 0f;
                Reset();
            }
        }
    }

    void Reset() {
        walking = false;
        walkTime = 0f;
        leftLegContainer.transform.localRotation = leftLegStart;
        rightLegContainer.transform.localRotation = rightLegStart;
        leftWingContainer.transform.localRotation = leftWingStart;
        rightWingContainer.transform.localRotation = rightWingStart;
        model.transform.position = origin;
    }
}
