using UnityEngine;

public enum TipoDecoracion
{
    Ninguna,
    Granas,
    Velita,
    Duraznos,
    CerezasYChocolate
}

public class GestorDecoraciones : MonoBehaviour
{
    public TipoDecoracion DecoracionCargada { get; private set; }
        = TipoDecoracion.Ninguna;

    public void CargarDecoracion(TipoDecoracion decoracion)
    {
        DecoracionCargada = decoracion;

        Debug.Log("Decoración cargada: " + decoracion);
    }
}
