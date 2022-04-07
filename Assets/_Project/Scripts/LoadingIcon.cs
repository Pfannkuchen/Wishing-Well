using UnityEngine;
using UnityEngine.UI;

public class LoadingIcon : MonoBehaviour
{
    [SerializeField] private Image _loadingBar;
    private Image LoadingBar => _loadingBar;
    
    [SerializeField] private Transform _rotationIndicator;
    private Transform RotationIndicator => _rotationIndicator;
    
    public void Animate(float progress)
    {
        RotationIndicator.Rotate(0f, 1f, 0f);

        if(LoadingBar != null) LoadingBar.fillAmount = progress;
    }
}