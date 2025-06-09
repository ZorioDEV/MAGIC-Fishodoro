using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = PlayerPrefs.GetFloat("AlarmVolume", 0.5f);
        slider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float volume)
    {
        PlayerPrefs.SetFloat("AlarmVolume", volume);
        PlayerPrefs.Save();
    }
}