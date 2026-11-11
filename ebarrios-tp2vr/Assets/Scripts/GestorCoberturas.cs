using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
public class GestorCoberturas : MonoBehaviour
{
    [Header("XRSockets")]
    [SerializeField] private XRSocketInteractor socketInferior;
    [SerializeField] private XRSocketInteractor socketSuperior;

    [Header("Recipientes")]
    [SerializeField] private XRGrabInteractable coberturaFrutilla;
    [SerializeField] private XRGrabInteractable coberturaNaranja;

    [Header("Materiales")]
    [SerializeField] private Material materialFrutilla;
    [SerializeField] private Material materialNaranja;

    public string CoberturaActual { get; private set; } = "Ninguna";

    private Material materialActual;

    private void OnTriggerEnter(Collider otro)
    {
        XRGrabInteractable recipiente =
            otro.GetComponentInParent<XRGrabInteractable>();

        if (recipiente == null || !recipiente.isSelected)
            return;

        if (socketInferior == null || !socketInferior.hasSelection)
            return;

        if (recipiente == coberturaFrutilla)
        {
            materialActual = materialFrutilla;
            CoberturaActual = "Frutilla";
        }
        else if (recipiente == coberturaNaranja)
        {
            materialActual = materialNaranja;
            CoberturaActual = "Naranja";
        }
        else
        {
            return;
        }

        AplicarEnSocket(socketInferior);
        AplicarEnSocket(socketSuperior);
    }

    private void Update()
    {
        if (materialActual == null)
            return;
        AplicarEnSocket(socketInferior);
        AplicarEnSocket(socketSuperior);
    }

    private void AplicarEnSocket(XRSocketInteractor socket)
    {
        if (socket == null || !socket.hasSelection)
            return;

        Transform bizcochuelo =
            socket.interactablesSelected[0].transform;
        Transform capaExistente =
            bizcochuelo.Find("CoberturaVisual");

        GameObject capa;

        if (capaExistente == null)
        {
            capa = GameObject.CreatePrimitive(
                PrimitiveType.Cylinder
            );

            capa.name = "CoberturaVisual";
            capa.transform.SetParent(bizcochuelo, false);

            capa.transform.localPosition =
                new Vector3(0f, 1.03f, 0f);

            capa.transform.localScale =
                new Vector3(1.04f, 0.10f, 1.04f);

            capa.GetComponent<Collider>().enabled = false;
        }
        else
        {
            capa = capaExistente.gameObject;
        }

        Renderer renderer = capa.GetComponent<Renderer>();

        if (renderer.sharedMaterial != materialActual)
            renderer.sharedMaterial = materialActual;
    }
}
