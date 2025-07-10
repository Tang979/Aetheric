using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SliderButtonLoadingStart : MonoBehaviour
{
    public Slider slider;
    public float sliderSpeed = 0.5f;

    private void OnEnable()
    {
        if (slider != null)
        {
            slider.value = 0;
            StartCoroutine(RunSlider());
        }
    }

    private void OnDisable()
    {
        if (slider != null)
            slider.value = 0;
    }

    IEnumerator RunSlider()
    {
        while (slider.value < slider.maxValue)
        {
            slider.value += sliderSpeed * Time.deltaTime;
            yield return null;
        }

        slider.value = slider.maxValue;

        // Nếu muốn load scene ở đây, có thể thêm gọi LoadScene tại đây
        // SceneManager.LoadScene("Game");
    }
}
