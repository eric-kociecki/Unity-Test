using UnityEngine;
using System.Collections.Generic;

public class Node : MonoBehaviour {
	//You will learn mores about these states in the next steps 
	public enum State { ACTIVE, OPEN, CLOSED, START, GOAL }; 
	
	private Vector3 pos;		        // The x,y,z coordinates of the node
	private float score = 0;		// The f score
	private Node parentNode;		// The best parent node that connects to this node (with lowest f score)
	private State state = State.ACTIVE;	// Default state - meaning not opened or closed
	private List<Node> connectedNodes = new List<Node>();	// a list of nodes this node can connect to
	private List<Node> possibleParents = new List<Node>();	// a list of possible parents for this node

	public void setPos(Vector3 newPos)
	{
		pos = newPos;
	}

	public Vector3 getPos() 
	{
		return pos;
	}

	public void setScore(float newScore)
	{
		score = newScore;
	}
	
	public float getScore() 
    {
        return score;
    }

	public void setParentNode(Node newParentNode)
	{
		parentNode = newParentNode;
	}

	public Node getParentNode()
	{
		return parentNode;
	}

	public void setState(State newState)
	{
		state = newState;
	}

	public State getState()
	{
		return state;
	}

	public void setConnectedNodes(List<Node> newConnectedNodes)
	{
		connectedNodes = newConnectedNodes;
	}

	public List<Node> getConnectedNodes()
	{
		return connectedNodes;
	}

	public void setPossibleParents(List<Node> newPossibleParents)
	{
		possibleParents = newPossibleParents;
	}

	public List<Node> getPossibleParents()
	{
		return possibleParents;
	}
}
