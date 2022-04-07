using UnityEngine;
using UnityEngine.UI;

public abstract class ToggleBase : MonoBehaviour
{
    [SerializeField] private Image _checkSprite;
    private Image CheckSprite => _checkSprite;
    
    [SerializeField] private Sprite _spriteFalse;
    private Sprite SpriteFalse => _spriteFalse;
    
    [SerializeField] private Sprite _spriteTrue;
    private Sprite SpriteTrue => _spriteTrue;
    
    public abstract void ToggleValue();

    protected void UpdateCheckSprite(bool value)
    {
        CheckSprite.sprite = value ? SpriteTrue : SpriteFalse;
    }
}