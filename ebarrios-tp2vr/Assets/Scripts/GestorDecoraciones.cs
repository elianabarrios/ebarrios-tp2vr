using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

//Visual y config
public enum TipoDecoracion
{
    Ninguna,
    Granas,
    Velita,
    Duraznos,
    CerezasYChocolate
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class GestorDecoraciones : MonoBehaviour
{
    public TipoDecoracion DecoracionCargada { get; private set; }
        = TipoDecoracion.Ninguna;

    [Header("Zona de aplicacion")]
    [SerializeField] private Collider zonaAplicacion;

    [Header("Materiales")]
    [SerializeField] private Material[] materialesGranas;
    [SerializeField] private Material materialDurazno;
    [SerializeField] private Material materialCereza;
    [SerializeField] private Material materialVirutas;

    [Header("Indicador anterior")]
    [SerializeField] private GameObject indicadorCarga;

    [Header("Relleno visual de la cuchara")]
    [SerializeField] private Transform cucharaVisual;
    [SerializeField]
    private Vector3 desplazamientoRelleno =
        new Vector3(0f, 0.009f, 0f);
    [SerializeField] private float escalaRelleno = 1f;

    [Header("Alturas sobre el bizcochuelo")]
    [SerializeField] private float alturaSuperficie = 1.16f;
    [SerializeField] private float alturaCentroCerezas = 1.22f;

    [Header("Retorno del aplicador")]
    [SerializeField] private Transform puntoReposoAplicador;

    [SerializeField, HideInInspector]
    private GameObject prefabVelita;

    private XRGrabInteractable agarreAplicador;
    private Rigidbody cuerpoAplicador;
    private Coroutine retornoActivo;

    //Dependencias , recipientes, comandas y xrsockets

    private GameObject visualGranas;
    private GameObject visualDuraznos;
    private GameObject visualCerezas;

    [Header("Sockets de la torta")]
    [SerializeField] private XRSocketInteractor socketInferior;
    [SerializeField] private XRSocketInteractor socketSuperior;

    private void Awake()
    {
        agarreAplicador = GetComponent<XRGrabInteractable>();
        cuerpoAplicador = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if (agarreAplicador == null) return;

        agarreAplicador.selectEntered.AddListener(AlAgarrarAplicador);
        agarreAplicador.selectExited.AddListener(AlSoltarAplicador);
    }

    private void OnDisable()
    {
        if (agarreAplicador != null)
        {
            agarreAplicador.selectEntered.RemoveListener(AlAgarrarAplicador);
            agarreAplicador.selectExited.RemoveListener(AlSoltarAplicador);
        }

        if (retornoActivo != null)
        {
            StopCoroutine(retornoActivo);
            retornoActivo = null;
        }
    }

    private void Start()
    {
        CrearRellenosVisuales();
        ActualizarRellenoVisual();
        ColocarAplicadorEnBase();
    }


    public void CargarDecoracion(TipoDecoracion decoracion)
    {
        if (decoracion == TipoDecoracion.Velita)
            return;

        if (decoracion == TipoDecoracion.Ninguna)
        {
            VaciarCuchara();
            return;
        }

        if (agarreAplicador == null || !agarreAplicador.isSelected)
            return;

        DecoracionCargada = decoracion;
        ActualizarRellenoVisual();

        Debug.Log("Decoracion cargada: " + decoracion);
    }

    private void VaciarCuchara()
    {
        DecoracionCargada = TipoDecoracion.Ninguna;
        ActualizarRellenoVisual();
    }

    private void CrearRellenosVisuales()
    {
        if (cucharaVisual == null) {return;}

        Transform raiz = new GameObject("RellenosCuchara_Runtime").transform;
        raiz.SetParent(cucharaVisual, false);
        raiz.localPosition = desplazamientoRelleno;
        raiz.localScale = Vector3.one * escalaRelleno;

        visualGranas = new GameObject("RellenoGranas");
        visualGranas.transform.SetParent(raiz, false);

        for (int i = 0; i < 12; i++)
        {
            float angulo = i * Mathf.PI * 2f / 12f;

            CrearPieza(
                visualGranas.transform,
                PrimitiveType.Cube,
                new Vector3(Mathf.Cos(angulo) * 0.02f,
                            0f,
                            Mathf.Sin(angulo) * 0.025f),
                new Vector3(0.006f, 0.003f, 0.003f),
                ObtenerMaterialGrana(i));
        }
        visualDuraznos = new GameObject("RellenoDuraznos");
        visualDuraznos.transform.SetParent(raiz, false);

        for (int i = 0; i < 3; i++)
        {
            CrearPieza(
                visualDuraznos.transform,
                PrimitiveType.Sphere,
                new Vector3((i - 1) * 0.014f, 0f, i * 0.01f),
                new Vector3(0.014f, 0.006f, 0.012f),
                materialDurazno);
        }
        visualCerezas = new GameObject("RellenoCerezasChocolate");
        visualCerezas.transform.SetParent(raiz, false);

        CrearPieza(
            visualCerezas.transform,
            PrimitiveType.Sphere,
            new Vector3(0f, 0.005f, 0f),
            Vector3.one * 0.018f,
            materialCereza);

        for (int i = 0; i < 4; i++)
        {
            float angulo = i * Mathf.PI / 2f;

            CrearPieza(
                visualCerezas.transform,
                PrimitiveType.Cube,
                new Vector3(Mathf.Cos(angulo) * 0.017f,
                            0f,
                            Mathf.Sin(angulo) * 0.017f),
                new Vector3(0.007f, 0.003f, 0.003f),
                materialVirutas);
        }
    }

    private void ActualizarRellenoVisual()
    {
        if (indicadorCarga != null)
            indicadorCarga.SetActive(false);

        if (visualGranas != null)
            visualGranas.SetActive(DecoracionCargada == TipoDecoracion.Granas);

        if (visualDuraznos != null)
            visualDuraznos.SetActive(DecoracionCargada == TipoDecoracion.Duraznos);

        if (visualCerezas != null)
            visualCerezas.SetActive(
                DecoracionCargada == TipoDecoracion.CerezasYChocolate);
    }


    private void OnTriggerStay(Collider otro)
    {
        if (zonaAplicacion == null || otro != zonaAplicacion)
            return;

        if (agarreAplicador == null || !agarreAplicador.isSelected)
            return;

        if (DecoracionCargada == TipoDecoracion.Ninguna)
            return;

        XRSocketInteractor socket =
            socketSuperior != null && socketSuperior.hasSelection
                ? socketSuperior : socketInferior;

        if (socket == null || !socket.hasSelection ||
            socket.interactablesSelected.Count == 0)
            return;

        Transform bizcochuelo =
            socket.interactablesSelected[0].transform;

        bool aplicada = false;

        switch (DecoracionCargada)
        {
            case TipoDecoracion.Granas:
                aplicada = CrearGranas(bizcochuelo);
                break;

            case TipoDecoracion.Duraznos:
                aplicada = CrearDuraznos(bizcochuelo);
                break;

            case TipoDecoracion.CerezasYChocolate:
                aplicada = CrearCerezas(bizcochuelo);
                break;
        }

        if (aplicada)
            VaciarCuchara();
    }

    private bool CrearGranas(Transform torta)
    {
        if (torta.Find("GranasVisual") != null ||
            ObtenerMaterialGrana(0) == null)
            return false;

        Transform grupo = new GameObject("GranasVisual").transform;
        grupo.SetParent(torta, false);

        for (int i = 0; i < 16; i++)
        {
            Vector2 p = Random.insideUnitCircle * 0.38f;

            CrearPieza(grupo, PrimitiveType.Cube,
                new Vector3(p.x, alturaSuperficie, p.y),
                new Vector3(0.06f, 0.05f, 0.025f),
                ObtenerMaterialGrana(i),
                Random.Range(0f, 180f));
        }

        return true;
    }

    private bool CrearDuraznos(Transform torta)
    {
        if (torta.Find("DuraznosVisual") != null ||
            materialDurazno == null)
            return false;

        Transform grupo = new GameObject("DuraznosVisual").transform;
        grupo.SetParent(torta, false);

        Vector2[] posiciones =
        {
            new Vector2(-0.35f, 0.25f),
            new Vector2(0.35f, 0.25f),
            new Vector2(-0.35f, -0.25f),
            new Vector2(0.35f, -0.25f)
        };

        for (int i = 0; i < posiciones.Length; i++)
        {
            CrearPieza(grupo, PrimitiveType.Sphere,
                new Vector3(posiciones[i].x, alturaSuperficie, posiciones[i].y),
                new Vector3(0.26f, 0.07f, 0.14f),
                materialDurazno,
                i * 45f);
        }

        return true;
    }

    private bool CrearCerezas(Transform torta)
    {
        if (torta.Find("CerezasChocolateVisual") != null ||
            materialCereza == null || materialVirutas == null)
            return false;

        Transform grupo = new GameObject("CerezasChocolateVisual").transform;
        grupo.SetParent(torta, false);

        Vector2[] cerezas =
        {
            new Vector2(-0.30f, 0.20f),
            new Vector2(0.30f, 0.20f),
            new Vector2(0f, -0.30f)
        };

        foreach (Vector2 p in cerezas)
        {
            CrearPieza(grupo, PrimitiveType.Sphere,
                new Vector3(p.x, alturaCentroCerezas, p.y),
                Vector3.one * 0.12f,
                materialCereza);
        }

        for (int i = 0; i < 5; i++)
        {
            Vector2 p = Random.insideUnitCircle * 0.40f;

            CrearPieza(grupo, PrimitiveType.Cube,
                new Vector3(p.x, alturaSuperficie + 0.01f, p.y),
                new Vector3(0.09f, 0.07f, 0.025f),
                materialVirutas,
                i * 35f);
        }

        return true;
    }

    private Material ObtenerMaterialGrana(int indice)
    {
        if (materialesGranas == null || materialesGranas.Length == 0)
            return null;

        for (int i = 0; i < materialesGranas.Length; i++)
        {
            Material m = materialesGranas[
                (indice + i) % materialesGranas.Length];

            if (m != null)
                return m;
        }

        return null;
    }

    private GameObject CrearPieza(
        Transform padre,
        PrimitiveType tipo,
        Vector3 posicion,
        Vector3 escala,
        Material material,
        float giroY = 0f)
        {
            GameObject pieza = GameObject.CreatePrimitive(tipo);
            pieza.transform.SetParent(padre, false);
            pieza.transform.localPosition = posicion;
            pieza.transform.localScale = escala;
            pieza.transform.localRotation = Quaternion.Euler(0f, giroY, 0f);

            Renderer render = pieza.GetComponent<Renderer>();
            if (render != null && material != null)
                render.sharedMaterial = material;

            Collider col = pieza.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
                Destroy(col);
            }

            return pieza;
        }
    private void AlAgarrarAplicador(SelectEnterEventArgs args)
    {
        if (retornoActivo != null)
        {
            StopCoroutine(retornoActivo);
            retornoActivo = null;
        }

        if (cuerpoAplicador == null)
            return;

        cuerpoAplicador.isKinematic = false;
        cuerpoAplicador.useGravity = false;
    }

    private void AlSoltarAplicador(SelectExitEventArgs args)
    {
        if (args.isCanceled || agarreAplicador == null ||
            agarreAplicador.isSelected)
            return;
        VaciarCuchara();
        if (retornoActivo != null)
            StopCoroutine(retornoActivo);

        retornoActivo = StartCoroutine(DevolverAplicador());
    }

    private IEnumerator DevolverAplicador()
    {
        yield return new WaitForFixedUpdate();

        if (agarreAplicador != null && !agarreAplicador.isSelected)
            ColocarAplicadorEnBase();

        retornoActivo = null;
    }

    private void ColocarAplicadorEnBase()
    {
        if (puntoReposoAplicador == null || cuerpoAplicador == null)
            return;

        if (!cuerpoAplicador.isKinematic)
        {
            cuerpoAplicador.linearVelocity = Vector3.zero;
            cuerpoAplicador.angularVelocity = Vector3.zero;
        }

        cuerpoAplicador.isKinematic = true;
        cuerpoAplicador.useGravity = false;

        transform.SetPositionAndRotation(
            puntoReposoAplicador.position,
            puntoReposoAplicador.rotation);
    }
    
}