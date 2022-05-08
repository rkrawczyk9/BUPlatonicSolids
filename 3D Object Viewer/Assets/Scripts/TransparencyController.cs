using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransparencyController : MonoBehaviour
{
    [SerializeField] private string onText;
    [SerializeField] private string offText;
    [SerializeField] private Text buttonText;

    /// <summary>
    /// whether or not faces should be transparent
    /// </summary>
    private bool transparent;
    public bool Transparent
    {
        get
        {
            return transparent;
        }
    }

    private void Start()
    {
        transparent = false;
        buttonText.text = offText;
    }

    /// <summary>
    /// Toggle the transpareancy on all existing faces
    /// </summary>
    public void ToggleTransparency()
    {
        // update text
        if (buttonText.text == offText)
            buttonText.text = onText;
        else
            buttonText.text = offText;
        
        // Toggle transparency for every face in scene
        transparent = !transparent;

        Face[] temp = FindObjectsOfType<Face>();

        foreach (Face obj in temp)
        {
            obj.ToggleTransparency();
        }
    }
}
