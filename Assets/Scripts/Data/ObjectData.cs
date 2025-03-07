using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    Info,
    JumpPad,
    MovingObject
}

[CreateAssetMenu(fileName = "Object", menuName = "New Object")]
public class ObjectData : ScriptableObject
{
    [Header("Info")]
    public string objectName;
    public string objectDescription;
    public ObjectType EObjectType;

    [Header("MoveMent")]
    public bool canMave;
    public float moveSpeed;
    public bool loopMovement;

    [Header("JumpPad")]
    public bool canJump;
    public float jumpPower;
}
