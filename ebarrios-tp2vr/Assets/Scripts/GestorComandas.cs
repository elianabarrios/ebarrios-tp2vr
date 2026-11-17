using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactors;



public class GestorComandas : MonoBehaviour
{
    [Header("Interfaz")]
    [SerializeField] private TMP_Text textoComanda;
    [SerializeField] private TMP_Text textoBotonEntregar;
    [SerializeField] private TMP_Text textoEstadoComanda;


    [Header("Preparación")]
    [SerializeField] private XRSocketInteractor socketInferior;

    [SerializeField] private XRSocketInteractor socketSuperior;

    [SerializeField] private GestorCoberturas gestorCoberturas;

    [SerializeField] private GestorPisos gestorPisos;


    private void MostrarEstado(string mensaje)
    {
        if (textoEstadoComanda != null)
            textoEstadoComanda.text = mensaje;
    }

    [Header("Pedidos")]
    private string[] nombresTortas =
    {
        "Torta de cumpleaños",
        "Torta de crema y durazno",
        "Selva Negra"
    };

    [Header("Requisitos de las comandas")]

    private readonly SaborBizcochuelo[] saboresRequeridos =
    {
    SaborBizcochuelo.Vainilla,
    SaborBizcochuelo.Vainilla,
    SaborBizcochuelo.Chocolate
};

    private readonly int[] pisosRequeridos =
    {
    1,
    1,
    2
};

    private readonly string[] coberturasRequeridas =
    {
    "Frutilla",
    "Crema",
    "Crema"
};

    private static int indiceComandaActual = 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ReiniciarPartida()
    {
        indiceComandaActual = 0;
    }

    private bool pedidoValidado = false;

    private bool partidaTerminada = false;



    private void Start()
    {
        MostrarComanda();
    }

    private void MostrarComanda()
    {
        MostrarEstado("");
        if (gestorPisos != null)
        {
            gestorPisos.ConfigurarPedidoDeDosPisos(
                pisosRequeridos[indiceComandaActual] == 2);
        }

        if (textoComanda == null)
        {
            Debug.LogWarning("Falta asignar TextoComanda.");
            return;
        }
        ;


        textoComanda.text =
            "<size=65%>Ramoncito: Tortitas a Domicilio</size>\n" +
            "<size=60%>COMANDA N.º " +
            (indiceComandaActual + 1).ToString("000") +
            "</size>\n\n" +
            "<b>" + nombresTortas[indiceComandaActual] + "</b>\n\n" +
            "<size=60%>PARA RETIRAR</size>";


    }



    public void EntregarPedido()
    {


        if (partidaTerminada)
        {
            indiceComandaActual = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }


        if (pedidoValidado)
        {
            SiguienteComanda();
            return;
        }

        MostrarComanda();

        SaborBizcochuelo saborRequerido =
        saboresRequeridos[indiceComandaActual];

        int cantidadPisos =
            pisosRequeridos[indiceComandaActual];

        // Chequea si hay un bizcochuelo
        if (socketInferior == null || !socketInferior.hasSelection)
        {
            MostrarEstado("Falta colocar el bizcochuelo.");
            return;
        }

        // Verifca sabor
        Transform objeto = socketInferior.interactablesSelected[0].transform;
        Bizcochuelos bizcochuelo = objeto.GetComponent<Bizcochuelos>();

        if (bizcochuelo == null ||
            bizcochuelo.Sabor != saborRequerido)
        {
            MostrarEstado("El sabor del primer piso es incorrecto.");
            return;
        }

        // Verifica cantidad de pisos
        if (cantidadPisos == 1 && socketSuperior != null &&
            socketSuperior.hasSelection)
        {
            MostrarEstado("Esta torta debe tener un solo piso.");
            return;
        }

        if (cantidadPisos == 2)
        {
            if (socketSuperior == null || !socketSuperior.hasSelection)
            {
                MostrarEstado("Falta colocar el segundo piso.");
                return;
            }

            Transform objetoSuperior =
                socketSuperior.interactablesSelected[0].transform;

            Bizcochuelos bizcochueloSuperior =
                objetoSuperior.GetComponent<Bizcochuelos>();

            if (bizcochueloSuperior == null ||
                bizcochueloSuperior.Sabor != saborRequerido)
            {
                MostrarEstado("El sabor del segundo piso es incorrecto.");
                return;
            }
        }


        if (!ValidarCoberturaEnPisos())
        {
            MostrarEstado("Revisá la cobertura de los pisos.");
            return;
        }


        if (!ValidarDecoraciones())
        {
            MostrarEstado("La decoración no coincide con la receta.");
            return;
        }


        // Comprueba que se cumplan los requisitos 

        MostrarEstado("¡Pedido correcto!\nPresioná Siguiente para continuar.");


        pedidoValidado = true;
        if (textoBotonEntregar != null)
            textoBotonEntregar.text = "Siguiente";

    }
    public void SiguienteComanda()
    {
        if (!pedidoValidado)
            return;

        if (indiceComandaActual >= nombresTortas.Length - 1)
        {
            partidaTerminada = true;

            if (textoBotonEntregar != null)
                textoBotonEntregar.text = "Reiniciar";

            MostrarEstado("");

            textoComanda.text =
                "Ramoncito: Tortitas a Domicilio\n\n" +
                "¡FELICITACIONES!\n\n" +
                "Completaste las 3 comandas.\n" +
                "¡Ganaste!";

            return;
        }

        indiceComandaActual++;
        pedidoValidado = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    private bool ValidarDecoraciones()
    {
        XRSocketInteractor socket =
            socketSuperior != null && socketSuperior.hasSelection
                ? socketSuperior
                : socketInferior;

        if (socket == null || !socket.hasSelection)
            return false;

        Transform bizcochuelo =
            socket.interactablesSelected[0].transform;

        bool tieneGranas =
            bizcochuelo.Find("GranasVisual") != null;

        bool tieneVelita =
            bizcochuelo.Find("VelitaVisual") != null;

        bool tieneDuraznos =
            bizcochuelo.Find("DuraznosVisual") != null;

        bool tieneCerezas =
            bizcochuelo.Find("CerezasChocolateVisual") != null;

        switch (indiceComandaActual)
        {
            case 0: // Cumpleaños
                return (tieneGranas || tieneVelita)
                    && !tieneDuraznos
                    && !tieneCerezas;

            case 1: // Crema y durazno
                return tieneDuraznos
                    && !tieneGranas
                    && !tieneVelita
                    && !tieneCerezas;

            case 2: // Selva Negra
                return tieneCerezas
                    && !tieneGranas
                    && !tieneVelita
                    && !tieneDuraznos;

            default:
                return false;
        }
    }


    private bool ValidarCoberturaEnPisos()
    {
        if (gestorCoberturas == null)
            return false;

        if (socketInferior == null || !socketInferior.hasSelection)
            return false;

        // Cumpleaños lleva frutilla.
        // Crema y durazno, y Selva Negra llevan crema.
        string coberturaEsperada =
            indiceComandaActual == 0 ? "Frutilla" : "Crema";

        Transform pisoInferior =
            socketInferior.interactablesSelected[0].transform;

        // Comprobar el material del primer piso.
        if (!gestorCoberturas.TieneCobertura(
            pisoInferior, coberturaEsperada))
            return false;

        // Selva Negra necesita dos pisos con crema.
        if (indiceComandaActual == 2)
        {
            if (socketSuperior == null || !socketSuperior.hasSelection)
                return false;

            Transform pisoSuperior =
                socketSuperior.interactablesSelected[0].transform;

            if (!gestorCoberturas.TieneCobertura(
                pisoSuperior, coberturaEsperada))
                return false;
        }

        return true;
    }



}
