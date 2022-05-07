using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayCorrect : MonoBehaviour
{
    /// <summary>
    /// Make the object stay unrotated in world space
    /// </summary>
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
