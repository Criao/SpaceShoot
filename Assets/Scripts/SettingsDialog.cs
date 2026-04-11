using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsDialog : DialogBase
{
    private AudioSource musicSource;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        musicSource = GameObject.Find("Audio Manager").GetComponent<AudioSource>();
        musicSlider.onValueChanged.AddListener((value) =>
        {
            musicSource.volume = musicSlider.value;
            DataManager.Instance.musicSettingValue = musicSlider.value;
            DataManager.Instance.SaveMusicSetting();
        });

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener((value) =>
            {
                DataManager.Instance.SFXSettingValue = Mathf.Clamp01(value);
                DataManager.Instance.SaveMusicSetting();
            });
        }
    }

    public override void Open()
    {
        base.Open();
        musicSlider.value = DataManager.Instance.musicSettingValue;
        if (sfxSlider != null)
        {
            sfxSlider.value = DataManager.Instance.SFXSettingValue;
        }
    }


}
