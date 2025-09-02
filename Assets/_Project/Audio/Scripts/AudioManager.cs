using Cobra.DesignPattern;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioObject audioObjectPrefab;

    public AudioClip Footstep;
    public AudioClip FragExplosion;
    public AudioClip FragThrow;
    public AudioClip Hit;
    public AudioClip MenuBack;
    public AudioClip MenuHover;
    public AudioClip MenuSelect;
    public AudioClip PlayerJoin;
    public AudioClip Shoot;
    public AudioClip Slide;
    public AudioClip SneakFootstep;

    public void PlayOneShotAtLocation(Vector3 pos, float volume, Vector2 pitch, AudioClip clip)
    {
        AudioObject spawned = Instantiate(audioObjectPrefab, pos, transform.rotation);
        spawned.SetValues(clip, volume, Random.Range(pitch.x, pitch.y));
    }

    public void PlayFootstep(Vector3 pos, float volume, Vector2 pitch)
    {
        PlayOneShotAtLocation(pos, volume, pitch, Footstep);
    }

    public void PlayFragExplosion(Vector3 pos, float volume, Vector2 pitch)
    {
        PlayOneShotAtLocation(pos, volume, pitch, FragExplosion);
    }

    public void PlayFragThrow(Vector3 pos, float volume, Vector2 pitch)
    {
        PlayOneShotAtLocation(pos, volume, pitch, FragThrow);
    }

    public void PlayHit(Vector3 pos, float volume, Vector2 pitch)
    {
        PlayOneShotAtLocation(pos, volume, pitch, Hit);
    }

    public void PlayMenuBack(Vector3 pos, float volume, Vector2 pitch)
    {
        PlayOneShotAtLocation(pos, volume, pitch, MenuBack);
    }

    public void PlayMenuHover(Vector3 pos, float volume, Vector2 pitch)
    {
        PlayOneShotAtLocation(pos, volume, pitch, MenuHover);
    }

    public void PlayMenuSelect(Vector3 pos, float volume, Vector2 pitch)
    {
        PlayOneShotAtLocation(pos, volume, pitch, MenuSelect);
    }

    public void PlayPlayerJoin(Vector3 pos, float volume, Vector2 pitch)
    {
        PlayOneShotAtLocation(pos, volume, pitch, PlayerJoin);
    }

    public void PlayShoot(Vector3 pos, float volume, Vector2 pitch)
    {
        PlayOneShotAtLocation(pos, volume, pitch, Shoot);
    }

    public void PlaySlide(Vector3 pos, float volume, Vector2 pitch)
    {
        PlayOneShotAtLocation(pos, volume, pitch, Slide);
    }

    public void PlaySneakFootstep(Vector3 pos, float volume, Vector2 pitch)
    {
        PlayOneShotAtLocation(pos, volume, pitch, SneakFootstep);
    }
}
