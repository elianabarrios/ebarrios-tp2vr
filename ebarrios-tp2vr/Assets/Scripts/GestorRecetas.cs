using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PaginaReceta
{
    public GameObject titulo;
    public GameObject imagen;
    public GameObject ingredientes;
}

public class GestorRecetas : MonoBehaviour
{
    [Header("Páginas")]
    [SerializeField] private PaginaReceta[] recetas;

    [Header("Botones")]
    [SerializeField] private Button botonAnterior;
    [SerializeField] private Button botonSiguiente;

    private int paginaActual = 0;

    private void Start()
    {
        MostrarPagina();
    }

    public void PaginaSiguiente()
    {
        if (paginaActual >= recetas.Length - 1)
            return;

        paginaActual++;
        MostrarPagina();
    }

    public void PaginaAnterior()
    {
        if (paginaActual <= 0)
            return;

        paginaActual--;
        MostrarPagina();
    }

    private void MostrarPagina()
    {
        if (recetas == null || recetas.Length == 0)
            return;

        for (int i = 0; i < recetas.Length; i++)
        {
            bool mostrar = i == paginaActual;

            if (recetas[i].titulo != null)
                recetas[i].titulo.SetActive(mostrar);

            if (recetas[i].imagen != null)
                recetas[i].imagen.SetActive(mostrar);

            if (recetas[i].ingredientes != null)
                recetas[i].ingredientes.SetActive(mostrar);
        }

        if (botonAnterior != null)
            botonAnterior.interactable = paginaActual > 0;

        if (botonSiguiente != null)
            botonSiguiente.interactable =
                paginaActual < recetas.Length - 1;
    }
}