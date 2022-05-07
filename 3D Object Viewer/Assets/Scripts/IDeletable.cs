using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IDeletable : MonoBehaviour
{
    [Tooltip("The default material for this object")]
    [SerializeField] protected Material normalMaterial;
    [Tooltip("The material used when this object is cla")]
    [SerializeField] protected Material claimedMaterial;

    /// <summary>
    /// whether or not this object should lock its internal color changing
    /// </summary>
    protected bool colorLocked = false;
    
    /// <summary>
    /// Used to mark object as selected by selection system.
    /// </summary>
    /// <param name="b"></param>
    public virtual void Selected(bool b)
    {
        colorLocked = b;
    }

    /// <summary>
    /// Used to delete the object
    /// </summary>
    public abstract void Delete();

}
