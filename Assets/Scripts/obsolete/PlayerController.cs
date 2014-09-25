	using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public float Speed;
	public float sensitivity;
	public GUIText countText;
	private Vector3 growthLeft;
	private int count;
	public Vector3 growthSpeed;
	private ParticleSystem PS;
	void Start() {
		PS = GetComponent<ParticleSystem>();
		count = 0;
		growthLeft = new Vector3(0,0,0);
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		countText.text = "Count " + count.ToString();

	}
	void FixedUpdate() {
		//Android
		//float moveHorizontal =  -Input.acceleration.x * sensitivity;//Input.GetAxis ( "Horizontal" );
		//float moveVertical = -Input.acceleration.z * sensitivity ;//Input.GetAxis ( "Vertical" );
		//PC
		float moveHorizontal = sensitivity * Input.GetAxis ( "Horizontal" );
		float moveVertical = sensitivity * Input.GetAxis ( "Vertical" );


		Vector3 movement = new Vector3(-moveHorizontal, 0.0f, -moveVertical);
		PS.emissionRate = count * 2;
		rigidbody.AddForce(movement * Speed * Time.deltaTime);
		if ( growthLeft.magnitude > 0.2f) {
			transform.localScale = transform.localScale + growthLeft* 0.1f;
			growthLeft = growthLeft - new Vector3(0.1f,0.1f,0.1f)*0.1f;
		}
	}
	void OnTriggerEnter ( Collider other) {
		//Destroy(other.gameObject);
		if ( other.gameObject.tag == "pickup" ) {
			other.gameObject.SetActive( false );
			growthLeft += growthSpeed;
			count++;
			countText.text = "Count " + count.ToString();
		}
	}
	
}