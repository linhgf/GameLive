using UnityEngine;

public class DayNigthController : MonoBehaviour
{
    public Material daySkybox;
    public Material nightSkybox;

    public Light directionalLight;
    public Color dayColor;
    public Color nightColor;

    bool isDay = false;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            isDay = !isDay;
            if(isDay)
            {
                DayMode();
            }
            else
            {
                NightMode();
            }
        }
    }

    void DayMode()
    {
        RenderSettings.skybox = daySkybox;
        directionalLight.color = dayColor;
    }

    void NightMode()
    {
        RenderSettings.skybox = nightSkybox;
        directionalLight.color = nightColor;
    }
}
