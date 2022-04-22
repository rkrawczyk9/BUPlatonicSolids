using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Camera.main is null)
            return;

        transform.LookAt(Camera.main.transform);
        transform.Rotate(Vector3.up, 180);
    }
}
