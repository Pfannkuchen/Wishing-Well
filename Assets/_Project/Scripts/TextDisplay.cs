using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class TextDisplay : MonoBehaviour
{
    private bool _isVisible;
    public bool IsVisible => _isVisible;

    private TMP_Text _text;
    private TMP_Text Text => _text ??= GetComponent<TMP_Text>();
    

    public void Show(float duration, string content)
    {
        StopAllCoroutines();
        StartCoroutine(Transition(true, duration, content));
    }

    public void Hide(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(Transition(false, duration, ""));
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    private IEnumerator Transition(bool show, float duration, string content)
    {
        if (show)
        {
            Text.text = content;
            _isVisible = true;
        }
        
        float lerp = show ? 0f : 1f;
        
        //Debug.Log($"show = {show}, duration = {duration}, content = {content}");

        while ((lerp < 1f && show) || (lerp > 0f && !show))
        {
            lerp = Mathf.Clamp(lerp + Time.deltaTime / Mathf.Max(duration, 0.1f) * (show ? 1f: -1f), 0f, 1f);
            Text.color = new Color(1f, 1f, 1f, lerp);
            
            yield return null;
        }
        
        _isVisible = lerp > 0f;
    }
}
