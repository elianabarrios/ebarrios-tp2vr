using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GestorVelas : MonoBehaviour
{
    [SerializeField] private XRSocketInteractor socketInferior;
    [SerializeField] private XRSocketInteractor socketSuperior;
    [SerializeField] private float altura = 1.12f;
    [SerializeField] private float distancia = 0.25f;

    private XRGrabInteractable agarre;
    private Rigidbody cuerpo;
    private Vector3 inicio;
    private Quaternion rotacionInicio;

    private void Awake()
    {
        agarre = GetComponent<XRGrabInteractable>();
        cuerpo = GetComponent<Rigidbody>();

        inicio = transform.position;
        rotacionInicio = transform.rotation;
    }

    private void OnEnable()
    {
        agarre.selectExited.AddListener(AlSoltar);
    }

    private void OnDisable()
    {
        agarre.selectExited.RemoveListener(AlSoltar);
    }

    private void AlSoltar(SelectExitEventArgs args)
    {
        if (!args.isCanceled)
            StartCoroutine(ColocarVelita());
    }

    private IEnumerator ColocarVelita()
    {
        yield return new WaitForFixedUpdate();

        if (agarre.isSelected)
            yield break;

        XRSocketInteractor socket =
            socketSuperior != null && socketSuperior.hasSelection
            ? socketSuperior : socketInferior;

        if (socket != null && socket.hasSelection)
        {
            Transform torta = socket.interactablesSelected[0].transform;

            Vector3 destino = torta.TransformPoint(
                new Vector3(0f, altura, 0f));

            if (Vector3.Distance(transform.position, destino) <= distancia
                && torta.Find("VelitaVisual") == null)
            {
                GameObject grupo = new GameObject("VelitaVisual");
                grupo.transform.SetParent(torta, false);

                transform.SetPositionAndRotation(
                    destino, Quaternion.identity);

                transform.SetParent(grupo.transform, true);

                cuerpo.isKinematic = true;
                cuerpo.useGravity = false;
                agarre.enabled = false;
                enabled = false;
                yield break;
            }
        }

        cuerpo.isKinematic = true;
        cuerpo.useGravity = false;

        transform.SetPositionAndRotation(inicio, rotacionInicio);
    }
}
