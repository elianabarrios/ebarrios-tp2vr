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
            return;

        indiceComandaActual++;
        pedidoValidado = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
