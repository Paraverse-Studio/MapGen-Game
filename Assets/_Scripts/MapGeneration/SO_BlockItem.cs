using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
    grass,
    dirt,
    water,
    foundation,
    bridge,
    propPart
}


[CreateAssetMenu(fileName = "SO_BlockItem", menuName = "SOs/Block/SO_BlockItem")]
public class SO_BlockItem : ScriptableObject
{
    [System.Serializable]
    public struct RandomizeRotation 
    {
        public bool x;
        public bool y;
        public bool z;
    }

    [Header("—————  Basics  ————— ")]
    public BlockType blockType;
    public SingleLayer layer;

    [Header("—————  Modifications  —————")]
    public Vector3 defaultScale = Vector3.one;
    public Vector3 defaultOffset = Vector3.zero;

    public RandomizeRotation rotationRandomization = new RandomizeRotation();

    [Header("—————  Physical  —————")]
    public GameObject[] prefabVariations;

    [Header("—————  Props  —————")]
    public float foliageSpawnChance;
    public GameObject[] blockFoliage;

}
