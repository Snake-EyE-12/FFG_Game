using UnityEngine;

public class AudioObject : MonoBehaviour
{
    [SerializeField] private AudioSource source;

    public void SetValues(AudioClip clip, float volume = .8f, float velocity = 1)
    {
        source.clip = clip;
        source.volume = volume;
        source.pitch = velocity;
        source.PlayOneShot(clip);
        Destroy(gameObject, clip.length);
    }
}
