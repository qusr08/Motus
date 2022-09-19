using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors: Frank Alfano
// Date Created: 9/16/22
// Date Last Editted: 9/16/22

// *** DUMMY CLASS This is definitely gonna change later just wanted to have some starting point

public class EnemyController : MonoBehaviour {
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private Transform target;
	[SerializeField] private float shootTime;

	protected Vector3 acceleration;
	protected Vector3 velocity;
	protected Vector3 desiredVelocity;
	protected Vector3 steeringForce;

	protected float maxSpeed;
	protected float maxForce;
	protected float mass = 1.0f;
	protected float rad;
	protected float separationDistance = 10.0f;

	private float timer;

	private void Start ( ) {
		timer = shootTime;
	}

	private void Update ( ) {
		if (target != null) {
			// transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(transform.position.y - target.position.y, transform.position.x - target.position.y) + 90f);

			timer -= Time.deltaTime;
			if (timer <= 0) {
				BulletController.SpawnBullet(bulletPrefab, transform.position, (target.position - transform.position).normalized, BulletType.ENEMY);
				timer = shootTime;
			}

			ApplyForce(acceleration);

			velocity += acceleration * Time.deltaTime;

			velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

			transform.position += velocity;

			acceleration = Vector3.zero;

			transform.forward = velocity.normalized;
		}
	}

	// Have the player lose health
	// int damage: The amount of health to make the player lose
	public void TakeDamage (int damage) {
		// TO DO: Implement this later
	}

	// Update the acceleration of the enemy using all the different forces
	public void ApplyForce(Vector3 force)
	{
		acceleration += force / mass;
	}

	public Vector3 CalculateFuturePosition(float futureTime)
	{
		Vector3 futurePosition = transform.position + velocity * futureTime;

		return futurePosition;
	}

	public Vector3 StayInBounds(Vector3 center)
	{
		Vector3 returnForce = Vector3.zero;

		if (transform.position.x > 74 || transform.position.x < -74 || transform.position.z > 74 || transform.position.z < -74)
		{
			returnForce = Seek(center) * 250;

			return returnForce;
		}
		else if (transform.position.x > 60 || transform.position.x < -60 || transform.position.z > 60 || transform.position.z < -60)
		{
			returnForce = Seek(center);

			return returnForce;
		}
		else
		{
			return Vector3.zero;
		}
	}

	public Vector3 Wander(float futuretime, float radius)
	{
		Vector3 futurePos = CalculateFuturePosition(futuretime);

		float angle = Random.Range(0, 360);
		float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius + futurePos.x;
		float z = Mathf.Sin(angle * Mathf.Deg2Rad) * radius + futurePos.z;

		return (Seek(new Vector3(x, 0, z)));
	}

	protected Vector3 Seek(Vector3 targetPos)
	{
		desiredVelocity = targetPos - transform.position;

		desiredVelocity = desiredVelocity.normalized * maxSpeed;

		steeringForce = desiredVelocity - velocity;

		return steeringForce;
	}

	public Vector3 Flee(Vector3 targetPos)
	{
		desiredVelocity = Seek(targetPos);

		desiredVelocity *= -1;

		return desiredVelocity;
	}
}

// Bullet pattern planning (PROLLY WILL CHANGE)

// BulletPattern
// List<ahiwdba>
//		{BulletType, Angle, Delay}
//		{BulletType, Angle, Delay}
//		{.....

// ShootPattern()
//		for all list items:
//			SpawnBullet(...)