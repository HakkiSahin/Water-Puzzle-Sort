using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{

    public Slider volSlider;
    float vol;
    // Start is called before the first frame update
    void Start()
    {
        volSlider.value = AudioListener.volume;


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeVolume()
    {
        AudioListener.volume = volSlider.value;
        vol = volSlider.value;
    }


    public void MuteVolume()
    {
        AudioListener.volume = 0;
    }

    public void OpenVolume()
    {
        AudioListener.volume = vol;
    }
}
