using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class MusicManager : MonoBehaviour
{
    private const string PLATER_PREFS_MUSIC_VOLUME = "MusicVolume";

    public static MusicManager Instance {get; private set;}


    private AudioSource audioSource;
    private float volume = .3f;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();

        volume = PlayerPrefs.GetFloat(PLATER_PREFS_MUSIC_VOLUME, .3f);
    }

    public void ChangeVolume()
    {
        volume += .1f;
        if (volume > 1f)
        {
            volume = 0f;
        }
        audioSource.volume = volume;

        PlayerPrefs.SetFloat(PLATER_PREFS_MUSIC_VOLUME, volume);
        PlayerPrefs.Save(); //이전 정보 저장, 음악 소리
    }

    public float GetVolume()
    {
        return volume;
    }
}
