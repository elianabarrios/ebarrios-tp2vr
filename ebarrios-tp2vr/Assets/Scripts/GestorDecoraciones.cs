using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

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


    [Header("Zona de aplicación")]
    [SerializeField] private Collider zonaAplicacion;

    [Header("Pisos de la torta")]
    [SerializeField] private XRSocketInteractor socketInferior;
    [SerializeField] private XRSocketInteractor socketSuperior;


    public void CargarDecoracion(TipoDecoracion decoracion)
    {
        DecoracionCargada = decoracion;

        Debug.Log("Decoración cargada: " + decoracion);
    }


    private void OnTriggerEnter(Collider otro)
    {
        if (otro != zonaAplicacion)
            return;

        XRGrabInteractable agarre =
            GetComponent<XRGrabInteractable>();

        if (agarre == null || !agarre.isSelected)
            return;

        if (DecoracionCargada == TipoDecoracion.Ninguna)
            return;

        XRSocketInteractor socket =
            socketSuperior != null && socketSuperior.hasSelection
                ? socketSuperior
                : socketInferior;

        if (socket == null || !socket.hasSelection)
            return;

        Transform bizcochuelo =
            socket.interactablesSelected[0].transform;

        Debug.Log(
            "Aplicando " + DecoracionCargada +
            " sobre " + bizcochuelo.name);
    }

}
