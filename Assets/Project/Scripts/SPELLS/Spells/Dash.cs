using UnityEngine;

public class Dash : MonoBehaviour, ISpell
{
    private SpellData _data;
    private Rigidbody _rigidbody;
    private Transform _caster;
    private Character _character;

    private Vector3 _targetPosition;
    private Vector3 _direction;
    private float _dashDuration;
    private float _dashElapsedTime;

    public void Initialiser(SpellCastContext context)
    {
        _data = context._data;

        _caster = context._caster;

        _rigidbody = context._caster.GetComponent<Rigidbody>();

        _targetPosition = context._targetPosition;

        Vector3 direction = _targetPosition - context._caster.position;
        direction.y = 0f;

        _dashDuration = Vector3.Distance(_caster.position, _targetPosition) / _data._speed;

        _dashElapsedTime = 0f;

        if (direction.sqrMagnitude <= 0.01f)
        {
            Destroy(gameObject);
            return;
        }

        _direction = direction.normalized;

        _character = context._caster.GetComponent<Character>();
        _rigidbody = context._caster.GetComponent<Rigidbody>();

        if (_character.IsDashing)
        {
            Destroy(gameObject);
            return;
        }

        _character.StartDash();
    }

    private void FixedUpdate()
    {
        if (_character == null || _rigidbody == null)
            return;

        _dashElapsedTime += Time.fixedDeltaTime;

        float remainingDistance = Vector3.Distance(_rigidbody.position, _targetPosition);

        if (_dashElapsedTime >= _dashDuration || remainingDistance <= 0.05f)
        {
            TerminerDash();
            return;
        }

        _rigidbody.linearVelocity = new Vector3(_direction.x * _data._speed, _rigidbody.linearVelocity.y, _direction.z * _data._speed);
    }

    private void TerminerDash()
    {
        
        _rigidbody.linearVelocity = new Vector3(0f, _rigidbody.linearVelocity.y, 0f);

        _character.EndDash();

        Destroy(gameObject);
    }
}
