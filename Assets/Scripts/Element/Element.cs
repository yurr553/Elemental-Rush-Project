using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ElementType
{
    Hydrogen,
    Oxygen,
    Carbon,
    Nitrogen,
    Iron,
    Sulfur,
    Potassium

}

public enum CompoundType
{
    None,
    Water,
    CarbonDioxide,
    Methane,
    Salt
}

public class Element : MonoBehaviour
{
    public ElementType elementType;
    public CompoundType compoundType;
    public bool isMatched = false;

    public bool isCompound => compoundType != 0;

    public string ReactionID => elementType.ToString();
}
