using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    public AudioSource efxSource;
    public AudioSource musicSource;

    public static SoundManager instance = null;

    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;

	// Use this for initialization
	void Awake () {

        if (instance == null)
            instance = this;
        else if (instance)
            Destroy(gameObject);

	}

    public void PlaySingle ( AudioClip clip)
    {
        efxSource.clip = clip;

        efxSource.Play();
    }

    // params sirve para pasar cualquier cantidad de parámetros del tipo especificado separados por comas
    public void RandomizeSfx(params AudioClip [] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        // Hacemos una pequeña variación en el tono del sonido
        efxSource.pitch = randomPitch;

        efxSource.clip = clips[randomIndex];
        efxSource.Play();
    }


}
