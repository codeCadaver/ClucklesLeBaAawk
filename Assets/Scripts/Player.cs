using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private float _gravity = 1f;
    [SerializeField] private float _floatGravity = 0.2f;
    [SerializeField] private float _jumpHeight = 10f;
    [SerializeField] private float _attackDelay = 0.2f;
    [SerializeField] private float _speed = 10f;
    [SerializeField] private int _maxHealth = 3;
    [SerializeField] private Transform _startPosition;
    [SerializeField] private GameObject _smokeFX;

    private int _deathTriggerHash = Animator.StringToHash("DeathTrigger");
    private int _idleTriggerHash = Animator.StringToHash("IdleTrigger");
    private int _isGroundedHash = Animator.StringToHash("isGrounded");
    private int _leftPunchHash = Animator.StringToHash("PunchLeft");
    private int _punchHash = Animator.StringToHash("Punch");
    private int _speedHash = Animator.StringToHash("Speed");
    private int _shouldFlapHash = Animator.StringToHash("shouldFlap");

    private Animator _animator;
    private bool _canAttack = true;
    private bool _canFlap = false;
    private bool _canMove = true;
    private bool _isFlapping = false;
    private CharacterController _character;
    private float _currentGravity;
    private float _currentSpeed;
    private float _yVelocity;
    private int _numPunches;
    private Quaternion _currentRotation;
    private Vector3 _hitPoint;
    private Vector3 _velocity;
    private Vector3 _wallSurfaceNormal;

    public int Health { get; set; }
    public bool IsAlive { get; private set; }
    
    void Start()
    {
        _animator = GetComponent<Animator>();
        _character = GetComponent<CharacterController>();
        _currentSpeed = _speed;
        Reset();
    }

    void Update()
    {
        Attack();
        Jump();
        Flap();
        Movement();
    }

    private void Movement()
    {
        if (!_canMove) return;
        
        float h = Input.GetAxisRaw("Horizontal");
        _velocity = Vector3.right * (h * _speed);
        
        // Change Direction
        if (h != 0)
        {
            _currentRotation = Quaternion.Euler(new Vector3(0, 90 * Mathf.Sign(h), 0));
            _character.transform.rotation = _currentRotation;
        }
        
        // walk animation
        _animator.SetFloat(_speedHash, Mathf.Abs(h));

        _velocity.y = _yVelocity;
        _character.Move(_velocity * Time.deltaTime);
        _animator.SetBool(_isGroundedHash, _character.isGrounded);
    }

    private void Jump()
    {
        if (_character.isGrounded)
        {
            _canFlap = false;
            _animator.SetBool(_shouldFlapHash, !_character.isGrounded);
            // _canDoubleJump = true;
            // _canWallJump = false;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _yVelocity = _jumpHeight;
                StartCoroutine(FlapRoutine());
            }
        }
        else
        {
            SetGravity();
        }
    }

    private void Flap()
    {
        if (Input.GetKeyDown(KeyCode.Space)  && _canFlap)
        {
            _animator.SetBool(_shouldFlapHash, !_character.isGrounded);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            _animator.SetBool(_shouldFlapHash, false);
        }
    }

    IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(_attackDelay);
        _canAttack = true;
        ResumeRun();
    }

    IEnumerator FlapRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        _canFlap = true;
    }

    private void Attack()
    {
        if (!_character.isGrounded) return;

        if (Input.GetButtonDown("Fire1") && _canAttack)
        {
            _canMove = false;
            if (_numPunches <= 0)
            {
                _animator.SetTrigger(_punchHash);
                _numPunches += 1;
            }
            else
            {
                _animator.SetTrigger(_leftPunchHash);
                _numPunches = 0;
                _canAttack = false;
            }
            StartCoroutine(AttackRoutine());
        }
    }

    private void SetGravity()
    {
        if (_isFlapping && !_character.isGrounded)
        {
            _currentGravity = _floatGravity;
        }
        else
        {
            _currentGravity = _gravity;
        }

        _yVelocity -= _currentGravity * Time.deltaTime;
    }

    private void IsFlapping()
    {
        _isFlapping = true;
    }

    private void NotFlapping()
    {
        _isFlapping = false;
    }

    private void ResetCombo()
    {
        _numPunches = 0;
    }

    private void ResumeRun()
    {
        _canMove = true;
        _currentSpeed = _speed;
    }

    public void Damage(int damageAmount)
    {
        Health -= damageAmount;
        if (Health <= 0)
        {
            _canMove = false;
            _velocity = Vector3.zero;
            IsAlive = false;
            _character.enabled = false;
            Death();
        }
    }

    private void Death()
    {
        // Death Sequence
        if (_character.isGrounded)
        {
            _animator.SetTrigger(_deathTriggerHash);
        }
        else
        {
            Reset();
        }
    }

    private void ShowSmokeFX()
    {
        if (IsAlive)
        {
            GameObject smokeFX = Instantiate(_smokeFX, transform.position, Quaternion.identity);
        }
        else
        {
            var redSmokeFX = Instantiate(Resources.Load<GameObject>("RedSmokeParticleFX"), transform.position, Quaternion.identity);
        }
    }

    // Called in Death Animation
    private void Reset()
    {
        IsAlive = true;
        transform.position = _startPosition.position;
        ShowSmokeFX();
        _character.enabled = true;
        _canMove = true;
        Health = _maxHealth;
        _character.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        _animator.SetTrigger(_idleTriggerHash);
    }
}
