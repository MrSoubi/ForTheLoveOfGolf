using Unity.VisualScripting;
using UnityEngine;

public class AccelerationZone : MonoBehaviour 
{
	[Header("Settings")]
	[SerializeField, Min(0f)]
	float acceleration = 10f, speed = 10f;
	[SerializeField] private bool giveShoot;

    [SerializeField] private AudioSource sfx;

    void OnTriggerEnter (Collider other) {
		Rigidbody body = other.attachedRigidbody;
		if (body) 
		{
			if (giveShoot) body.GetComponent<PC_MovingSphere>().AddShootCharges(1);
            if (sfx != null) sfx.Play();
            Accelerate(body);
		}
	}

	void OnTriggerStay (Collider other) {
		Rigidbody body = other.attachedRigidbody;
		if (body) 
		{
			Accelerate(body);
		}
	}

	void Accelerate(Rigidbody body) {
		Vector3 velocity = transform.InverseTransformDirection(body.linearVelocity);
		if (velocity.y >= speed) {
			return;
		}

		if (acceleration > 0f) {
			velocity.y = Mathf.MoveTowards(
				velocity.y, speed, acceleration * Time.deltaTime
			);
		}
		else {
			velocity.y = speed;
		}

		body.linearVelocity = transform.TransformDirection(velocity);
		if (body.TryGetComponent(out MovingSphere sphere)) {
			sphere.PreventSnapToGround();
		}
	}
}