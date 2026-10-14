using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string masterParam = "MasterVol";
    [SerializeField] private string musicParam = "MusicVol";
    [SerializeField] private string sfxParam = "SFXVol";

    [Header("Players")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetMaster(float linear01) => SetVolume(masterParam, linear01);
    public void SetMusic(float linear01) => SetVolume(musicParam, linear01);
    public void SetSFX(float linear01) => SetVolume(sfxParam, linear01);

    void SetVolume(string param, float linear01)
    {
        float v = Mathf.Clamp(linear01, 0.0001f, 1f);
        float db = Mathf.Log10(v) * 20f;
        mixer.SetFloat(param, db);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (!clip || !sfxSource) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (!clip || !musicSource) return;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }
}

