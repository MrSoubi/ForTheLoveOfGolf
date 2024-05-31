using UnityEngine;

public class GravitySource : MonoBehaviour {

	[SerializeField]
	float m_Gravity = Physics.gravity.y;

	public virtual Vector3 GetGravity (Vector3 position) {
		return new Vector3(0, m_Gravity, 0);
	}

	void OnEnable () {
		CustomGravity.Register(this);
	}

	void OnDisable () {
		CustomGravity.Unregister(this);
	}
}