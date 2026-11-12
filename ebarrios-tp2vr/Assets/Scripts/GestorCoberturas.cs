using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
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
    private Vector3 posicionFrutilla, posicionNaranja;
    private Quaternion rotacionFrutilla, rotacionNaranja;
    private bool frutillaUsada, naranjaUsada;

    private void Awake()
    {
        posicionFrutilla = coberturaFrutilla.transform.position;
        posicionNaranja = coberturaNaranja.transform.position;
        rotacionFrutilla = coberturaFrutilla.transform.rotation;
        rotacionNaranja = coberturaNaranja.transform.rotation;
    }

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

        if (recipiente == coberturaFrutilla)
            frutillaUsada = true;
        else if (recipiente == coberturaNaranja)
            naranjaUsada = true;
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

    private void OnEnable()
    {
        coberturaFrutilla.selectExited.AddListener(SoltarFrutilla);
        coberturaNaranja.selectExited.AddListener(SoltarNaranja);
    }

    private void OnDisable()
    {
        coberturaFrutilla.selectExited.RemoveListener(SoltarFrutilla);
        coberturaNaranja.selectExited.RemoveListener(SoltarNaranja);
    }

    private void SoltarFrutilla(SelectExitEventArgs args)
    {
        if (!frutillaUsada) return;

        frutillaUsada = false;
        StartCoroutine(DevolverRecipiente(
            coberturaFrutilla, posicionFrutilla, rotacionFrutilla));
    }

    private void SoltarNaranja(SelectExitEventArgs args)
    {
        if (!naranjaUsada) return;

        naranjaUsada = false;
        StartCoroutine(DevolverRecipiente(
            coberturaNaranja, posicionNaranja, rotacionNaranja));
    }

    private IEnumerator DevolverRecipiente(
        XRGrabInteractable recipiente, Vector3 posicion, Quaternion rotacion)
    {
        yield return null;

        Rigidbody cuerpo = recipiente.GetComponent<Rigidbody>();
        if (cuerpo == null) yield break;

        cuerpo.isKinematic = true;
        cuerpo.linearVelocity = Vector3.zero;
        cuerpo.angularVelocity = Vector3.zero;
        cuerpo.position = posicion;
        cuerpo.rotation = rotacion;
        cuerpo.isKinematic = false;
    }
}
