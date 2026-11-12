using UnityEngine;
using TMPro;

using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class GestorComandas : MonoBehaviour
{
    [Header("Interfaz")]
    [SerializeField] private TMP_Text textoComanda;


    [Header("Preparación")]
    [SerializeField] private XRSocketInteractor socketInferior;

    [SerializeField] private XRSocketInteractor socketSuperior;

    [SerializeField] private GestorCoberturas gestorCoberturas;


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


    private int indiceComandaActual = 0;

    private bool pedidoValidado = false;


    private void Start()
    {
        MostrarComanda();
    }

    private void MostrarComanda()
    {
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

        // Verifica cobertura
        if (gestorCoberturas == null ||
            gestorCoberturas.CoberturaActual != coberturaRequerida)
        {
            textoComanda.text += "\n\nLa cobertura no coincide con la receta."; ;
            return;
        }

        // Comprueba que se cumplan los requisitos 
        textoComanda.text += "\n\n¡Pedido correcto!";

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

        MostrarComanda();
    }
}
