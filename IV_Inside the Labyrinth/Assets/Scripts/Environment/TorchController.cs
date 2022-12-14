using UnityEngine;

public class TorchController : MonoBehaviour
{
    [SerializeField] private Light torchLight;
    [SerializeField] private SphereCollider torchEffectRange;

    public void ChangeIntensity(float lightValue, float rangeValue)
    {
        torchLight.intensity = lightValue;
        torchEffectRange.radius = rangeValue;
    }
}
