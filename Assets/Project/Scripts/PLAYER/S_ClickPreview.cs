using Unity.VisualScripting;
using UnityEngine;

public class S_ClickPreview : MonoBehaviour
{
    float m_currentScale = 0;

    [SerializeField,Range(5,100)]
    int m_decraseSpeed = 5;

    [SerializeField]
    Material m_pointerMaterial;
    Color m_pointerMaterialAlpha;

    bool m_canScale = false;

    private void OnEnable()
    {
        Character.OnPointerClick += OnClick;
    }

    private void OnDisable()
    {
        Character.OnPointerClick -= OnClick;
    }

    private void OnClick(Vector3 _position)
    {

        _position.y += 0.1f;
        this.gameObject.transform.position = _position ;

        m_canScale = true;
        m_currentScale = 1;


         m_pointerMaterialAlpha = new Vector4(m_pointerMaterial.color.r,
            m_pointerMaterial.color.g,
            m_pointerMaterial.color.b,
            m_currentScale);

        m_pointerMaterial.color = m_pointerMaterialAlpha;

        this.gameObject.transform.localScale = Vector3.one;
    }

    private void Update()
    {
        if (!m_canScale)
            return;
        if (m_currentScale <= 0)
        {
            m_currentScale = 0;
            m_canScale = !m_canScale;
        }
        else
        {
            m_currentScale -= 0.01f * (m_decraseSpeed / 5);
        }

        m_pointerMaterial.color = m_pointerMaterialAlpha;

        Vector4 v4 = new Vector4(
            m_pointerMaterial.color.r,
           m_pointerMaterial.color.g,
           m_pointerMaterial.color.b,
           m_currentScale
           );
        m_pointerMaterialAlpha = v4;

        this.gameObject.transform.localScale = new Vector3(m_currentScale, m_currentScale, m_currentScale);
    }
}
