using UnityEngine;

public class Fireball : MonoBehaviour, ISpell
{
    private SpellData _data;
    private Character _caster;
    private Vector3 _direction;
    private Vector3 _startPosition;
    [SerializeField] private LayerMask _obstacleLayers;
    private int _range;
    public void Initialiser(SpellCastContext context)
    {
        _data = context._data;
        _caster = context._caster.GetComponent<Character>();
        _direction = context._direction;
        _startPosition = transform.position;
        _range = _caster.CalculateSpellRange(_data._range);
    }

    private void Update()
    {
        transform.position += _direction * _data._speed * Time.deltaTime;

        float distance = Vector3.Distance(_startPosition, transform.position);

        if (distance >= _range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Ennemi enemy = other.GetComponent<Ennemi>();

        if (enemy != null)
        {
            Impact();
            Destroy(gameObject);
            return;
        }

        if ((_obstacleLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            Destroy(gameObject);
        }
    }

    private void Impact()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _data._impactRadius);

        foreach (Collider collider in colliders)
        {
            Ennemi enemy = collider.GetComponent<Ennemi>();

            if (enemy == null)
            { 
                continue; 
            }

            int damage = _caster.CalculateSpellDamage(_data._damage);

            enemy.TakeDamage(damage);

            _caster.ApplyLifeSteal(damage);
        }
    }


}
