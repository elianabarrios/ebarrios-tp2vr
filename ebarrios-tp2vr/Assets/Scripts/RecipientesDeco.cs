using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RecipientesDeco : MonoBehaviour
{
    [Header("Deco disponible")]
    [SerializeField] private TipoDecoracion tipoDecoracion;

    private void OnTriggerEnter(Collider otro)
    {
        GestorDecoraciones aplicador =
            otro.GetComponentInParent<GestorDecoraciones>();

        if (aplicador == null)
            return;

        XRGrabInteractable interactuable =
            aplicador.GetComponent<XRGrabInteractable>();

        if (interactuable == null || !interactuable.isSelected)
            return;

        aplicador.CargarDecoracion(tipoDecoracion);
    }
}
