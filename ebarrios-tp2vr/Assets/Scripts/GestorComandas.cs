using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactors;



public class GestorComandas : MonoBehaviour
{
    [Header("Interfaz")]
    [SerializeField] private TMP_Text textoComanda;


    [Header("Preparación")]
    [SerializeField] private XRSocketInteractor socketInferior;

    [SerializeField] private XRSocketInteractor socketSuperior;

    [SerializeField] private GestorCoberturas gestorCoberturas;

    [SerializeField] private GestorPisos gestorPisos;


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

        textoComanda.text =
            "Tortas Ramón\n" +
            "COMANDA #" + (indiceComandaActual + 1).ToString("000") + "\n" +
            nombresTortas[indiceComandaActual] + "\n" +
            "Pedido para retirar";
    }

    public void EntregarPedido()
    {


        if (partidaTerminada)
            return;

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

        string coberturaRequerida =
            coberturasRequeridas[indiceComandaActual];

        // Chequea si hay un bizcochuelo
        if (socketInferior == null || !socketInferior.hasSelection)
        {
            textoComanda.text += "\n\nFalta colocar el bizcochuelo.";
            return;
        }

        // Verifca sabor
        Transform objeto = socketInferior.interactablesSelected[0].transform;
        Bizcochuelos bizcochuelo = objeto.GetComponent<Bizcochuelos>();

        if (bizcochuelo == null ||
            bizcochuelo.Sabor != saborRequerido)
        {
            textoComanda.text += "\n\nEl sabor del primer piso es incorrecto.";
            return;
        }

        // Verifica cantidad de pisos
        if (cantidadPisos == 1 && socketSuperior != null &&
            socketSuperior.hasSelection)
        {
            textoComanda.text += "\n\nEsta torta debe tener un solo piso.";
            return;
        }

        if (cantidadPisos == 2)
        {
            if (socketSuperior == null || !socketSuperior.hasSelection)
            {
                textoComanda.text += "\n\nFalta colocar el segundo piso.";
                return;
            }

            Transform objetoSuperior =
                socketSuperior.interactablesSelected[0].transform;

            Bizcochuelos bizcochueloSuperior =
                objetoSuperior.GetComponent<Bizcochuelos>();

            if (bizcochueloSuperior == null ||
                bizcochueloSuperior.Sabor != saborRequerido)
            {
                textoComanda.text += "\n\nEl sabor del segundo piso es incorrecto.";
                return;
            }
        }

        // Verifica cobertura
        if (gestorCoberturas == null ||
            gestorCoberturas.CoberturaActual != coberturaRequerida)
        {
            textoComanda.text += "\n\nLa cobertura no coincide con la receta."; ;
            return;
        }


        if (!ValidarCoberturaEnPisos())
        {
            textoComanda.text +=
                "\n\nRevisá la cobertura de los pisos.";
            return;
        }


        if (!ValidarDecoraciones())
        {
            textoComanda.text +=
                "\n\nLa decoración no coincide con la receta.";
            return;
        }


        // Comprueba que se cumplan los requisitos 

        textoComanda.text +=
            "\n\n¡Pedido correcto!\nPulsá ENTREGAR otra vez para continuar.";


        pedidoValidado = true;

    }
    public void SiguienteComanda()
    {
        if (!pedidoValidado)
            return;

        if (indiceComandaActual >= nombresTortas.Length - 1)
        {
            partidaTerminada = true;

            textoComanda.text =
                "Tortas Ramón\n\n" +
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
        if (socketInferior == null || !socketInferior.hasSelection)
            return false;

        Transform pisoInferior =
            socketInferior.interactablesSelected[0].transform;

        if (pisoInferior.Find("CoberturaVisual") == null)
            return false;

        if (indiceComandaActual == 2)
        {
            if (socketSuperior == null || !socketSuperior.hasSelection)
                return false;

            Transform pisoSuperior =
                socketSuperior.interactablesSelected[0].transform;

            if (pisoSuperior.Find("CoberturaVisual") == null)
                return false;
        }

        return true;
    }


}
