using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
    grass,
    dirt,
    water,
    propPart
}

[CreateAssetMenu(fileName = "SO_BlockItem", menuName = "SOs/Block/SO_BlockItem")]
public class SO_BlockItem : ScriptableObject
{
    public BlockType blockType;
    public Color blockColour;
    public SingleLayer layer;
    public Vector3 defaultScale = Vector3.one;


}
