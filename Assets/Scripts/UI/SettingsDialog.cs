using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 设置对话框 - 管理音乐和音效音量设置
/// </summary>
public class SettingsDialog : DialogBase
{
    private AudioSource musicSource; // 音乐音源
    [SerializeField] private Slider musicSlider; // 音乐音量滑块
    [SerializeField] private Slider sfxSlider; // 音效音量滑块

    /// <summary>
    /// 初始化滑块监听器
    /// </summary>
    private void Start()
    {
        musicSource = GameObject.Find("Audio Manager").GetComponent<AudioSource>();

        // 音乐音量滑块监听
        musicSlider.onValueChanged.AddListener((value) =>
        {
            musicSource.volume = musicSlider.value;
            DataManager.Instance.musicSettingValue = musicSlider.value;
            DataManager.Instance.SaveMusicSetting();
        });

        // 音效音量滑块监听
        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener((value) =>
            {
                DataManager.Instance.SFXSettingValue = Mathf.Clamp01(value);
                DataManager.Instance.SaveMusicSetting();
            });
        }
    }

    /// <summary>
    /// 打开对话框时加载当前设置值
    /// </summary>
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
