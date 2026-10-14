using UnityEngine;
using UnityEngine.Audio;

public static class OneShotAudio
{
    public static void Play(AudioClip clip, Vector3 position, AudioMixerGroup output = null, float volume = 1f)
    {
        if (!clip) return;

        GameObject go = new GameObject("OneShotAudio_" + clip.name);
        go.transform.position = position;

        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
        src.spatialBlend = 1f;
        src.volume = volume;

        if (output) src.outputAudioMixerGroup = output;

        src.clip = clip;
        src.Play();

        Object.Destroy(go, clip.length + 0.1f);
    }
}
