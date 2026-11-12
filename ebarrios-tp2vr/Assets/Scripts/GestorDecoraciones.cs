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

    [Header("Indicador visual")]
    [SerializeField] private GameObject indicadorCarga;


    [Header("Pisos de la torta")]
    [SerializeField] private XRSocketInteractor socketInferior;
    [SerializeField] private XRSocketInteractor socketSuperior;

    [Header("Materiales de granas")]
    [SerializeField] private Material[] materialesGranas = new Material[4];

    [Header("Decoración de cumpleaños")]
    [SerializeField] private GameObject prefabVelita;

    [Header("Decoración de duraznos")]
    [SerializeField] private Material materialDurazno;

    [Header("Decoración de Selva Negra")]
    [SerializeField] private Material materialCereza;
    [SerializeField] private Material materialVirutas;






    public void CargarDecoracion(TipoDecoracion decoracion)
    {
        DecoracionCargada = decoracion;

        Debug.Log("Decoración cargada: " + decoracion);

        if (indicadorCarga != null)
            indicadorCarga.SetActive(true);

    }


    private void OnTriggerStay(Collider otro)
    {
        if (otro != zonaAplicacion)
            return;

        XRGrabInteractable agarre =
            GetComponent<XRGrabInteractable>();

        if (agarre == null || !agarre.isSelected)
            return;

        if (DecoracionCargada == TipoDecoracion.Ninguna)

            return;

        if (indicadorCarga != null)
            indicadorCarga.SetActive(false);


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
        DecoracionCargada = TipoDecoracion.Ninguna;

        if (DecoracionCargada == TipoDecoracion.Granas)
        {
            CrearGranas(bizcochuelo);
        }

        else if (DecoracionCargada == TipoDecoracion.Velita)
        {
            CrearVelita(bizcochuelo);
        }

        else if (DecoracionCargada == TipoDecoracion.Duraznos)
        {
            CrearDuraznos(bizcochuelo);
        }

        else if (DecoracionCargada == TipoDecoracion.CerezasYChocolate)
        {
            CrearCerezasYChocolate(bizcochuelo);
        }




    }

    private void CrearGranas(Transform bizcochuelo)
    {
        if (bizcochuelo.Find("GranasVisual") != null)
            return;

        if (materialesGranas == null || materialesGranas.Length == 0)
            return;

        GameObject contenedor = new GameObject("GranasVisual");
        contenedor.transform.SetParent(bizcochuelo, false);

        for (int i = 0; i < 16; i++)
        {
            GameObject grana = GameObject.CreatePrimitive(
                PrimitiveType.Cube);

            grana.name = "Grana";
            grana.transform.SetParent(contenedor.transform, false);

            Vector2 punto = Random.insideUnitCircle * 0.38f;

            grana.transform.localPosition =
                new Vector3(punto.x, 1.16f, punto.y);

            grana.transform.localScale =
                new Vector3(0.06f, 0.05f, 0.025f);

            grana.transform.localRotation = Quaternion.Euler(
                0f, Random.Range(0f, 180f), 0f);

            Renderer renderer = grana.GetComponent<Renderer>();
            renderer.sharedMaterial =
                materialesGranas[i % materialesGranas.Length];

            Collider collider = grana.GetComponent<Collider>();
            collider.enabled = false;
            Destroy(collider);
        }
    }

    private void CrearVelita(Transform bizcochuelo)
    {
        if (prefabVelita == null)
        {
            Debug.LogWarning("Falta asignar el prefab de la velita.");
            return;
        }

        if (bizcochuelo.Find("VelitaVisual") != null)
            return;

        Vector3 posicion = bizcochuelo.TransformPoint(
            new Vector3(0f, 1.12f, 0f));

        GameObject velita = Instantiate(
            prefabVelita,
            posicion,
            Quaternion.identity);

        velita.name = "VelitaVisual";

        velita.transform.localScale = Vector3.one * 0.65f;

        velita.transform.SetParent(bizcochuelo, true);
    }


    private void CrearDuraznos(Transform bizcochuelo)
    {
        if (bizcochuelo.Find("DuraznosVisual") != null)
            return;

        if (materialDurazno == null)
            return;

        GameObject contenedor = new GameObject("DuraznosVisual");
        contenedor.transform.SetParent(bizcochuelo, false);

        Vector3[] posiciones =
        {
        new Vector3(-0.35f, 1.16f, 0.25f),
        new Vector3(0.35f, 1.16f, 0.25f),
        new Vector3(-0.35f, 1.16f, -0.25f),
        new Vector3(0.35f, 1.16f, -0.25f)
    };

        for (int i = 0; i < posiciones.Length; i++)
        {
            GameObject durazno =
                GameObject.CreatePrimitive(PrimitiveType.Sphere);

            durazno.name = "GajitoDurazno";
            durazno.transform.SetParent(contenedor.transform, false);

            durazno.transform.localPosition = posiciones[i];
            durazno.transform.localScale =
                new Vector3(0.26f, 0.07f, 0.14f);

            durazno.transform.localRotation =
                Quaternion.Euler(0f, i * 45f, 0f);

            durazno.GetComponent<Renderer>().sharedMaterial =
                materialDurazno;

            Collider collider = durazno.GetComponent<Collider>();
            collider.enabled = false;
            Destroy(collider);
        }
    }


    private void CrearCerezasYChocolate(Transform bizcochuelo)
    {

        if (bizcochuelo.Find("CerezasChocolateVisual") != null)
            return;

        if (materialCereza == null || materialVirutas == null)
        {
            Debug.LogWarning("Faltan materiales de Selva Negra.");
            return;
        }

        GameObject contenedor = new GameObject("CerezasChocolateVisual");
        contenedor.transform.SetParent(bizcochuelo, false);

        Vector3[] posicionesCerezas =
        {
        new Vector3(-0.30f, 1.56f, 0.20f),
        new Vector3(0.30f, 1.56f, 0.20f),
        new Vector3(0f, 1.56f, -0.30f)
    };

        foreach (Vector3 posicion in posicionesCerezas)
        {
            GameObject cereza =
                GameObject.CreatePrimitive(PrimitiveType.Sphere);

            cereza.name = "CerezaVisual";
            cereza.transform.SetParent(contenedor.transform, false);
            cereza.transform.localPosition = posicion;

            cereza.transform.localScale =
                new Vector3(0.12f, 0.8f, 0.12f);

            cereza.GetComponent<Renderer>().sharedMaterial =
                materialCereza;

            Destroy(cereza.GetComponent<Collider>());
        }

        Vector3[] posicionesVirutas =
        {
        new Vector3(-0.40f, 1.17f, -0.10f),
        new Vector3(0.35f, 1.17f, -0.30f),
        new Vector3(0.10f, 1.17f, 0.40f),
        new Vector3(-0.15f, 1.17f, 0.05f),
        new Vector3(0.40f, 1.17f, 0.30f)
    };

        for (int i = 0; i < posicionesVirutas.Length; i++)
        {
            GameObject viruta =
                GameObject.CreatePrimitive(PrimitiveType.Cube);

            viruta.name = "VirutaChocolate";
            viruta.transform.SetParent(contenedor.transform, false);

            viruta.transform.localPosition = posicionesVirutas[i];
            viruta.transform.localScale =
                new Vector3(0.09f, 0.07f, 0.025f);

            viruta.transform.localRotation =
                Quaternion.Euler(0f, i * 35f, 0f);

            viruta.GetComponent<Renderer>().sharedMaterial =
                materialVirutas;

            Destroy(viruta.GetComponent<Collider>());
        }
    }




}
