using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors: Frank Alfano
// Date Created: 9/15/22
// Date Last Editted: 9/15/22

// The purpose of this is to calculate the edge collider points for the bounds of the arena
// This way the arena can be circular and not square

// Code referenced from this post: https://answers.unity.com/questions/1201097/invert-a-circle-collider-2d.html

public class CalculateArenaBounds : MonoBehaviour {
	[SerializeField] [Min(2)] public int NumEdges;
	[SerializeField] [Min(1)] public float Radius;

	private void OnValidate ( ) {
		// Get the edge collider on the arena bounds
		EdgeCollider2D edgeCollider = GetComponent<EdgeCollider2D>( );
		Vector2[ ] points = new Vector2[NumEdges + 1];

		// For each edge index, find the new point on the circle so it outlines the arena bounds
		for (int i = 0; i < NumEdges; i++) {
			float angle = 2 * Mathf.PI * i / NumEdges;
			float x = Radius * Mathf.Cos(angle);
			float y = Radius * Mathf.Sin(angle);

			points[i] = new Vector2(x, y);
		}

		points[NumEdges] = points[0];
		edgeCollider.points = points;
	}
}