using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections; 

public class VoxelButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    private Vector3 midaOriginal;

    [Header("Configuració de Text")]
    public TextMeshProUGUI textBoto;
    public Color colorNormal = Color.white;
    public Color colorSobre = new Color(1f, 0.85f, 0f);

    void Start()
    {
        midaOriginal = transform.localScale;
        PosarColorNormal();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(this.gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (EventSystem.current.currentSelectedGameObject != this.gameObject)
        {
            PosarColorNormal();
        }
    }

    public void OnSelect(BaseEventData eventData) 
    {
        AudioManager.instance.PlayButtonSelect();
        PosarColorSobre(); 
    }
    public void OnDeselect(BaseEventData eventData) { PosarColorNormal(); }

    public void OnPointerDown(PointerEventData eventData) 
    { 
        EnfonsarBoto();
        AudioManager.instance.PlayButtonClick();
    }
    public void OnPointerUp(PointerEventData eventData) { RestaurarBoto(); }

    public void OnSubmit(BaseEventData eventData)
    {
        if (eventData is PointerEventData) return; 
        StartCoroutine(EfecteClicTeclat());
    }

    private IEnumerator EfecteClicTeclat()
    {
        EnfonsarBoto();
        AudioManager.instance.PlayButtonClick();
        yield return new WaitForSecondsRealtime(0.1f);
        RestaurarBoto();
    }

    private void EnfonsarBoto() { transform.localScale = midaOriginal * 0.9f; }
    private void RestaurarBoto() { transform.localScale = midaOriginal; }
    private void PosarColorSobre() { if (textBoto != null) textBoto.color = colorSobre; }
    private void PosarColorNormal() { if (textBoto != null) textBoto.color = colorNormal; }
}