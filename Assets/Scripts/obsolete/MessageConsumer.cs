using UnityEngine;
using System.Collections;

public class MessageConsumer : MonoBehaviour {

	public Color SelectedColor;
	public Color DefaultColor;
	private Material mat;

	void Start() {
		mat = renderer.material;
	}

	void OnTouchDown() {
		mat.color = SelectedColor;
	}
	void OnTouchUp() {
		mat.color = DefaultColor;
	}
	void OnTouchStay() {
	}
	void OnTouchExit() {
	}
}
