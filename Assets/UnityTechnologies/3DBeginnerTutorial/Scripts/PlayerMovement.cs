using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float turnSpeed = 20f;
    
    public Image StaminaBar;
    public float Stamina, MaxStamina;
    public float RunCost;
    public float ChargeRate;
    public float MovementSpeed = 1f;

    private Coroutine recharge;

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    AudioSource m_AudioSource;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;


    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool("IsWalking", isWalking);

        if (isWalking)
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
        else
        {
            m_AudioSource.Stop();
        }

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);

    }
    void Update() {
        if(Input.GetKey("f") && Stamina > 0) { 
            MovementSpeed = 1.5f;
            Debug.Log("Run");
            Stamina -= RunCost * Time.deltaTime;
            if(Stamina < 0) Stamina = 0;
            StaminaBar.fillAmount = Stamina / MaxStamina;
            if(recharge != null) StopCoroutine(recharge);
            recharge = StartCoroutine(RechargeStamina());
        }

        if(Input.GetKeyUp("f") || Stamina <= 0) {
            MovementSpeed = 1;
        }
    }
    
    private IEnumerator RechargeStamina() {
        yield return new WaitForSeconds(1f);

        while(Stamina < MaxStamina) {
            Stamina += ChargeRate / 10f;
            if(Stamina > MaxStamina) Stamina = MaxStamina;
            StaminaBar.fillAmount = Stamina / MaxStamina;
            yield return new WaitForSeconds(.1f);
            if(recharge != null) StopCoroutine(recharge);
            recharge = StartCoroutine(RechargeStamina());
        }
    }
    
    void OnAnimatorMove()
    {
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude * MovementSpeed);
        m_Rigidbody.MoveRotation(m_Rotation);
    }

}