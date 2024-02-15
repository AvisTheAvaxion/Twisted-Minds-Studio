using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Useables : MonoBehaviour
{
    [SerializeField] string useableName;
    [SerializeField] string description;

    public abstract void Use();

    public string GetName()
    {
        return useableName;
    }

    public string GetDescription()
    {
        return description;
    }
}
