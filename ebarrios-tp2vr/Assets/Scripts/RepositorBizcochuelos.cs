
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RepositorBizcochuelos : MonoBehaviour
{
    [Header("Bizcochuelos")]
    [SerializeField] private GameObject prefabBizcochuelo;
    [SerializeField] private XRGrabInteractable bizcochueloInicial;

    [Header("Reposición")]
    [SerializeField, Min(0.05f)]
    private float distanciaReposicion = 0.35f;

    [SerializeField, Min(2)]
    private int maximoActivos = 8;

    private readonly HashSet<XRGrabInteractable> activos =
        new HashSet<XRGrabInteractable>();

    private XRGrabInteractable disponible;
    private Rigidbody cuerpoDisponible;
    private RigidbodyConstraints restriccionesOriginales;

    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;
    private Transform contenedor;

    private bool fueAgarrado;
    private bool reposicionPendiente;
    private float siguienteRevision;

    private void Start()
    {
        if (prefabBizcochuelo == null || bizcochueloInicial == null)
        {
            Debug.LogError("Faltan referencias en " + name);
            enabled = false;
            return;
        }

        posicionInicial = bizcochueloInicial.transform.position;
        rotacionInicial = bizcochueloInicial.transform.rotation;
        contenedor = bizcochueloInicial.transform.parent;

        activos.Add(bizcochueloInicial);

        if (!Preparar(bizcochueloInicial))
            enabled = false;
    }

    private bool Preparar(XRGrabInteractable bizcochuelo)
    {
        Rigidbody cuerpo = bizcochuelo.GetComponent<Rigidbody>();

        if (cuerpo == null)
        {
            Debug.LogError("Bizcochuelo sin Rigidbody: " + name);
            return false;
        }

        disponible = bizcochuelo;
        cuerpoDisponible = cuerpo;

        restriccionesOriginales = cuerpo.constraints;
        cuerpo.constraints = RigidbodyConstraints.FreezeAll;

        fueAgarrado = false;
        reposicionPendiente = false;

        disponible.selectEntered.AddListener(AlAgarrar);
        disponible.selectExited.AddListener(AlSoltar);

        return true;
    }

    private void AlAgarrar(SelectEnterEventArgs args)
    {
        if (cuerpoDisponible == null)
            return;

        cuerpoDisponible.constraints = restriccionesOriginales;
        fueAgarrado = true;
    }

    private void AlSoltar(SelectExitEventArgs args)
    {
        if (disponible == null || cuerpoDisponible == null)
            return;

        float distanciaCuadrada =
            (disponible.transform.position - posicionInicial).sqrMagnitude;

        float limite = distanciaReposicion * distanciaReposicion;

        if (distanciaCuadrada >= limite)
            return;

        // Si no llegó a salir del horno, vuelve a su lugar.
        fueAgarrado = false;

        disponible.transform.SetPositionAndRotation(
            posicionInicial,
            rotacionInicial
        );

        cuerpoDisponible.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void Update()
    {
        if (reposicionPendiente)
        {
            // Revisar cada medio segundo, no constantemente.
            if (Time.time >= siguienteRevision)
            {
                siguienteRevision = Time.time + 0.5f;
                IntentarReponer();
            }

            return;
        }

        if (!fueAgarrado || disponible == null)
            return;

        float distanciaCuadrada =
            (disponible.transform.position - posicionInicial).sqrMagnitude;

        float limite = distanciaReposicion * distanciaReposicion;

        if (distanciaCuadrada < limite)
            return;

        DesvincularDisponible();

        disponible = null;
        cuerpoDisponible = null;
        fueAgarrado = false;
        reposicionPendiente = true;

        IntentarReponer();
    }

    private void IntentarReponer()
    {
        // Quitar del registro los objetos destruidos.
        activos.RemoveWhere(objeto => objeto == null);

        // No superar el límite configurado.
        if (activos.Count >= maximoActivos)
            return;

        GameObject nuevo = Instantiate(
            prefabBizcochuelo,
            posicionInicial,
            rotacionInicial,
            contenedor
        );

        XRGrabInteractable interactable =
            nuevo.GetComponent<XRGrabInteractable>();

        if (interactable == null)
        {
            Debug.LogError(
                "El prefab no tiene XR Grab Interactable: " + name
            );

            Destroy(nuevo);
            enabled = false;
            return;
        }

        if (!Preparar(interactable))
        {
            Destroy(nuevo);
            enabled = false;
            return;
        }

        activos.Add(interactable);
    }

    private void DesvincularDisponible()
    {
        if (disponible == null)
            return;

        disponible.selectEntered.RemoveListener(AlAgarrar);
        disponible.selectExited.RemoveListener(AlSoltar);
    }

    private void OnDestroy()
    {
        DesvincularDisponible();
    }
}
