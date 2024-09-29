using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinTop : MonoBehaviour
{
    // Speed of the rotation around the Y-axis
    public float rotationSpeed = 100f;

    // Amplitude and speed of the bobbing motion
    public float bobbingAmplitude = 0.2f;  // How far it moves up and down
    public float bobbingSpeed = 1f;        // How fast it moves up and down

    // Initial position to start bobbing from
    private Vector3 initialPosition;

    void Start()
    {
        // Store the initial position of the cube
        initialPosition = transform.position;
    }

    void Update()
    {
        // Rotate the cube around the Y-axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Bobbing motion using a sine wave
        float newY = initialPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmplitude;
        transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);
    }
}
