using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
    grass,
    dirt,
    water,
    foundation,
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

    public BlockType blockType;
    public Color blockColour;
    public SingleLayer layer;
    public Vector3 defaultScale = Vector3.one;
    public Vector3 defaultOffset = Vector3.zero;

    public RandomizeRotation rotationRandomization = new RandomizeRotation();

    public GameObject[] prefabVariations;

}
