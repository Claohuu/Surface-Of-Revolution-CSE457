using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flap : MonoBehaviour
{
    public GameObject model;
    public Transform modelTransform;
    public GameObject leftWingContainer;
    public GameObject rightWingContainer;
    public bool flapping = false;
    public int speed = 5;
    public float flapTime = 0f;
    public Vector3 origin;
    public Quaternion leftWingStart;
    public Quaternion rightWingStart;

    void Start() {
        modelTransform = model.transform.Find("Model");
        origin = model.transform.position;

        if (leftWingContainer != null && rightWingContainer != null) {
            // Store the CONTAINER rotations (not the wing children)
            leftWingStart = leftWingContainer.transform.localRotation;
            rightWingStart = rightWingContainer.transform.localRotation;
        } else {
            Debug.LogError("Wing containers not assigned in Inspector!");
        }
    }

    public void StartFlapping() {
        flapping = true;
        flapTime = 0f;
    }

    void Update() {
        if (flapping && leftWingContainer != null && rightWingContainer != null) {
            flapTime += Time.deltaTime;

            // Wings flap UP and DOWN - rotate the CONTAINERS
            float wingFlap = Mathf.Sin(flapTime * speed * 3f) * 45f;

            // Rotate containers (parents) instead of wing children
            rightWingContainer.transform.localRotation = Quaternion.Euler(-wingFlap, 0, 0);
            leftWingContainer.transform.localRotation = Quaternion.Euler(wingFlap, 0, 0);

            // Stop after 2 seconds
            if (flapTime >= 2f) {
                flapping = false;
                flapTime = 0f;
                Reset();
            }
        }
    }

    void Reset() {
        flapping = false;
        flapTime = 0f;

        if (leftWingContainer != null && rightWingContainer != null) {
            leftWingContainer.transform.localRotation = leftWingStart;
            rightWingContainer.transform.localRotation = rightWingStart;
        }

        model.transform.position = origin;
    }
}