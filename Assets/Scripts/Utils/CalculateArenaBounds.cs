using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors: Frank Alfano
// Date Created: 9/15/22
// Date Last Editted: 10/23/22

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

	/// <summary>
	/// Get a random Vector2 position inside the arena bounds
	/// </summary>
	/// <returns>The random position.</returns>
	public Vector2 GetRandomPositionInArena ( ) {
		// https://stackoverflow.com/questions/5837572/generate-a-random-point-within-a-circle-uniformly

		float theta = Random.Range(0f, 1f) * 2 * Mathf.PI;
		// Subtracting 2 from the total radius so the position has a little padding away from the walls of the arena
		return ((Radius - 2) * Random.Range(0f, 1f) * new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)));
	}
}