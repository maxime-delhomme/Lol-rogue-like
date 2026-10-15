using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField] private float _duration = 1f;
    [SerializeField] private float _moveSpeed = 1f;

    private TextMeshPro _text;
    private Camera _camera;
    private float _timer;

    private void Awake()
    {
        _text = GetComponent<TextMeshPro>();
        _camera = Camera.main;
    }
    
    public void Initialiser(int damage)
    {
        _text.text = damage.ToString();
        _timer = _duration;
    }

    private void Update()
    {
        transform.position += Vector3.up * _moveSpeed * Time.deltaTime;

        if (_camera != null )
        {
            transform.forward = _camera.transform.forward;
        }

        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
