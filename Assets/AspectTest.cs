using UnityEngine;

public class AspectTest : MonoBehaviour
{
    [SerializeField, Min(1f)] private float _targetScale = 800f;
    [SerializeField, Min(1f)] private Vector2 _input = new Vector2(400f, 300f);

    [SerializeField] private Vector2 _pixelScale;
    [SerializeField] private Vector2 _normalizedScale;
    
    void OnValidate()
    {
        _pixelScale = AspectHelper.GetPixelScale(_targetScale, _input);
        _normalizedScale = AspectHelper.GetNormalizedScale(_targetScale, _input);
    }
}
