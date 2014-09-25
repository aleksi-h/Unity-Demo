using UnityEngine;
using System.Collections;

public class Bouncy : MonoBehaviour {
	public float frequency;
	public float amplitude;
	private float phase;
	void Start() {
		phase = Random.Range( 0, Mathf.PI );
	}

	void Update() {
		transform.Rotate( new Vector3(20,10,30)*Time.deltaTime, Space.Self );
		transform.Translate( new Vector3(0, ( amplitude*(Mathf.Sin(phase + 2*Mathf.PI*frequency*Time.time) - Mathf.Sin( phase + 2*Mathf.PI*frequency*(Time.time - Time.deltaTime))) ), 0), Space.World);
	}

}
