//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using UnityEngine;

public class BlockStone : Block
{
	public BlockStone () : base ()
	{
        colorUV = new Vector2((Random.value / 10) + 0.1f,
                              (Random.value / 10) + 0.1f);
	}

	public override string ToString()
	{
		return "block stone";
	}
}

