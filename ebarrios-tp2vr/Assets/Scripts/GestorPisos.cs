using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class GestorPisos : MonoBehaviour
{
    [SerializeField] private XRSocketInteractor socketInferior;
    [SerializeField] private XRSocketInteractor socketSuperior;

    private void Awake()
    {
        if (socketSuperior != null)
            socketSuperior.socketActive = false;
    }

    void Update()
    {
        if (socketInferior == null || socketSuperior == null)
            return;
        //Habilitar el segundo piso una vez que esté el primero
        socketSuperior.socketActive = socketInferior.hasSelection;

        // oCULTAR los sockets 
        socketInferior.showInteractableHoverMeshes = !socketInferior.hasSelection;
        socketSuperior.showInteractableHoverMeshes = socketInferior.hasSelection && !socketSuperior.hasSelection;

    }
}
