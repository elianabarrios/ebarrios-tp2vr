
using System.Collections;
using UnityEngine;
using Unity.XR.CoreUtils;

public class OrientacionInicialVR : MonoBehaviour
{
    [SerializeField] private Transform objetivo;

    private IEnumerator Start()
    {
        yield return null;
        yield return null;

        if (objetivo == null)
            yield break;

        XROrigin origen = GetComponent<XROrigin>();

        if (origen == null || origen.Camera == null)
            yield break;

        Vector3 direccion =
            objetivo.position - origen.Camera.transform.position;
        direccion.y = 0f;

        if (direccion.sqrMagnitude < 0.001f)
            yield break;

        origen.MatchOriginUpCameraForward(
            Vector3.up,
            direccion.normalized
        );
    }
}
