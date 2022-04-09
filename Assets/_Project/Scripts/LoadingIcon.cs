using UnityEngine;
using UnityEngine.UI;

public class LoadingIcon : MonoBehaviour
{
    [SerializeField] private Image _loadingBar;
    private Image LoadingBar => _loadingBar;
    
    [SerializeField] private Image _rotationIndicator;
    private Image RotationIndicator => _rotationIndicator;

    private RectTransform _localTransform;
    private RectTransform LocalTransform => _localTransform ??= GetComponent<RectTransform>();

    private float _currentProgress;
    private float _targetProgress;

    private void Start()
    {
        transform.SetParent(InterfaceManager.Instance.transform);
    }

    private void Update()
    {
        if (_currentProgress >= 1f) return;

        _currentProgress = Mathf.Lerp(_currentProgress, _targetProgress, Time.deltaTime * 10f);
        if(LoadingBar != null) LoadingBar.fillAmount = _currentProgress;
        if (_currentProgress >= 0.99f)
        {
            Hide();
        }
    }

    public void Show()
    {
        LoadingBar.enabled = true;
        RotationIndicator.enabled = true;
        enabled = true;
    }

    public void Hide()
    {
        LoadingBar.enabled = false;
        RotationIndicator.enabled = false;
        enabled = false;
    }

    public void SetWorldPosition(Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        LocalTransform.position = screenPos;
        
        if(RotationIndicator != null) RotationIndicator.transform.Rotate(0f, 0f, 2f);
        
        //Debug.Log($"{gameObject.name} -> LoadingIcon.SetWorldPosition() -> screenPos = {screenPos}, LocalTransform.position = {LocalTransform.position}");
    }

    public void SetProgress(float progress)
    {
        //Debug.Log($"progress = {progress}");
        _targetProgress = progress;
    }
}