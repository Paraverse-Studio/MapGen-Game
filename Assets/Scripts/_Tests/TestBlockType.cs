using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType 
{ 
    grass,
    dirt,
    treeTrunk,
    treeLeaves,
    props
}

[CreateAssetMenu(fileName = "TestBlockType", menuName = "SOs/Block/TestBlockType")]
public class TestBlockType : ScriptableObject
{
    public BlockType blockType;
    public Color blockColour;
    public SingleLayer layer;
    public Vector3 defaultScale = Vector3.one;


}
