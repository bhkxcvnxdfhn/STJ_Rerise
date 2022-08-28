using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlanetArea", menuName = "New PlanetArea")]
public class PlanetAreaData : ScriptableObject
{
    [Header("�ԲӸ��")]
    public string areaName;
    [TextArea(7, 13)]
    public string description;

    [Header("�i�`������")]
    public List<SpacePartsFragData> spaceFrags;
}
