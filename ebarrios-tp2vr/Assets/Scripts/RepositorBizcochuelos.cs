using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RepositorBizcochuelos : MonoBehaviour
{
    [Header("Bizcochuelos")]
    [SerializeField] private GameObject prefabBizcochuelo;
    [SerializeField] private XRGrabInteractable bizcochueloInicial;

    [Header("Reposición")]
    [SerializeField] private float distanciaReposicion = 0.35f;

    private XRGrabInteractable bizcochueloEnBandeja;
    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;
    private Transform contenedor;
    private bool fueAgarrado;

    void Start()
    {
        if (prefabBizcochuelo == null || bizcochueloInicial == null)
        {
            Debug.LogError("Faltan referencias en RepositorBizcochuelos.");
            enabled = false;
            return;
        }

        posicionInicial = bizcochueloInicial.transform.position;
        rotacionInicial = bizcochueloInicial.transform.rotation;
        contenedor = bizcochueloInicial.transform.parent;

        bizcochueloEnBandeja = bizcochueloInicial;
    }

    
    void Update()
    {
        if (bizcochueloEnBandeja == null)
            return;

        if (bizcochueloEnBandeja.isSelected)
            fueAgarrado = true;

        if (!fueAgarrado)
            return;

        float distancia = Vector3.Distance(
            bizcochueloEnBandeja.transform.position,
            posicionInicial);

        if (distancia < distanciaReposicion)
            return;

        GameObject nuevoBizcochuelo = Instantiate(
            prefabBizcochuelo,
            posicionInicial,
            rotacionInicial,
            contenedor);

        bizcochueloEnBandeja =
            nuevoBizcochuelo.GetComponent<XRGrabInteractable>();

        fueAgarrado = false;
    }
}
