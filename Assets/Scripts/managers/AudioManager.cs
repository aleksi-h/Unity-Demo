using UnityEngine;
using System.Collections;

public class AudioManager : Singleton<AudioManager> {

    public AudioClip buildingSelected;
    public AudioClip buildingPickedUp;
    public AudioClip buildingPlanted;
    public AudioClip buildingUpgraded;

    private Transform myTransform;
    private float volume = 1;
    private float pitch = 1;

    public override void Awake() {
        base.Awake();
        myTransform = transform;
    }

    public void PlayOnce(AudioClip clip) {
        GameObject go = new GameObject("AudioClip: " + clip.name);
        go.transform.parent = myTransform;

        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
        Destroy(go, clip.length);
    }
}