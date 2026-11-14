using UnityEngine;
using TMPro;

public class ContadorFPS : MonoBehaviour
{
    private TMP_Text textoFPS;
    private float tiempoAcumulado;
    private int fotogramas;

    private void Awake()
    {
        textoFPS = GetComponent<TMP_Text>();
    }


    void Update()
    {
        tiempoAcumulado += Time.unscaledDeltaTime;
        fotogramas++;

        if (tiempoAcumulado < 0.5f)
            return;

        float fps = fotogramas / tiempoAcumulado;

        textoFPS.text = "FPS: " + Mathf.RoundToInt(fps);

        tiempoAcumulado = 0f;
        fotogramas = 0;
    }
}
