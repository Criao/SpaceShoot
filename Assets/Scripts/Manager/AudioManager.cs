using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 音频管理器 - 管理游戏音乐音量，响应设置变化
/// </summary>
public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource; // 缓存当前 AudioSource 组件，避免每次重复查找

    /// <summary>
    /// 对象实例化时调用，优先于 Start
    /// </summary>
    private void Awake()
    {
        // 获取挂载的 AudioSource 组件
        audioSource = GetComponent<AudioSource>();

        // 立即应用当前存档中的音量设置
        ApplyVolume();
    }

    /// <summary>
    /// GameObject 激活时调用
    /// </summary>
    private void OnEnable()
    {
        // 订阅 DataManager 的设置变化回调
        DataManager.SettingsChanged += ApplyVolume;

        // 再次应用当前值，保证状态同步
        ApplyVolume();
    }

    /// <summary>
    /// GameObject 禁用或销毁时调用
    /// </summary>
    private void OnDisable()
    {
        // 取消订阅，避免内存泄漏或空引用
        DataManager.SettingsChanged -= ApplyVolume;
    }

    /// <summary>
    /// 统一音量应用逻辑
    /// </summary>
    private void ApplyVolume()
    {
        // 如果 audioSource 丢失，尝试重新获取
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        // 如果仍然为空，则直接跳过
        if (audioSource == null) return;

        // 仅在 DataManager 实例存在时才读取设置值
        if (DataManager.Instance != null)
        {
            audioSource.volume = DataManager.Instance.musicSettingValue;
            Debug.Log("AudioManager 设置音量：" + audioSource.volume);
        }
    }
}

