using UnityEngine;
using TMPro;

public class ContadorFPS : MonoBehaviour
{
    private TMP_Text textoFPS;
    [SerializeField] private TMP_Text textoEstadoAplicador;
    [SerializeField] private GestorDecoraciones aplicador;
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
        if (textoEstadoAplicador != null && aplicador != null)
        {
            string decoracion = aplicador.DecoracionCargada.ToString();

            textoEstadoAplicador.text = decoracion == "Ninguna"
                ? "Aplicador: Sin decoración"
                : "Aplicador: " + decoracion;
        }

        tiempoAcumulado = 0f;
        fotogramas = 0;
    }
}
