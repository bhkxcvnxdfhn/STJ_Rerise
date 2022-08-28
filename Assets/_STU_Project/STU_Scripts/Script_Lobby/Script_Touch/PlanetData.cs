using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Planet", menuName = "New Planet")]
public class PlanetData : ScriptableObject
{
    [Header("����")]
    public bool unlock;
    public Sprite lockSprite;
    public Sprite unlockSprite;

    [Header("�ԲӸ��")]
    public string planetName;
    [TextArea(7, 13)]
    public string description;

    [Header("�ϰ�")]
    public int levelIndex;
    public int planetIndex;
    public List<PlanetAreaData> planetArea;
}
