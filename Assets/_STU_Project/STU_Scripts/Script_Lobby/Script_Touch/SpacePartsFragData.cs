using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SpacePartsFragData", menuName = "New SpacePartsFrag")]
public class SpacePartsFragData : ScriptableObject
{
    [Header("����")]
    public bool unlock;
    public Sprite lockSprite;
    public Sprite unlockSprite;

    public Sprite showSprite
    {
        get 
        {
            if(unlock == true)
                return unlockSprite;
            else
                return lockSprite;
        }
    }

    [Header("����")]
    public string description;
}
