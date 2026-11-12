using UnityEngine;

public enum SaborBizcochuelo
{
    Vainilla,
    Chocolate
}
public class Bizcochuelos : MonoBehaviour
{
    [Header("Características")]
    [SerializeField] private SaborBizcochuelo sabor;

    public SaborBizcochuelo Sabor => sabor;
}
