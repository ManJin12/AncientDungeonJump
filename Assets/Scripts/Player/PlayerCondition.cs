using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition : MonoBehaviour
{
    public UICondition uICondition;
    private PlayerController controller;
    Condition health { get { return uICondition.health; } }
    Condition stamina { get { return uICondition.stamina; } }

    public bool Run = false;
    public float staminaRecoveryCooldown = 5f;
    private float lastStaminaUseTime;
    public bool isStaminaDepleted = false; // ���¹̳��� 0���� ����


    private void Start()
    {
        controller = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if(!isStaminaDepleted)
        {
            if (controller.isDash && controller.isMoving)
            {
                // ��� ��: ���¹̳� �Ҹ�
                UseStamina(5.0f * Time.deltaTime);
                Run = true;

                if (stamina.curValue <= 0)
                {
                    isStaminaDepleted = true;
                    Run = false;
                    lastStaminaUseTime = Time.time; // ���������� ���¹̳��� ����� �ð� ����
                }
            }
            else
            {
                Run = false;
            }
        }

        if(!Run)
        {
            stamina.Add(stamina.passiveValue * Time.deltaTime);

            if(isStaminaDepleted)
            {
                if(Time.time - lastStaminaUseTime > staminaRecoveryCooldown)
                {
                    isStaminaDepleted = false;
                }
            }
        }
    }

    public void SpeedUp(float addSpeed, float duration)
    {
        StartCoroutine(SpeedBoostCoroutine(addSpeed, duration));
    }

    private IEnumerator SpeedBoostCoroutine(float speedMultiplier, float duration)
    {
        float originalSpeed = controller.moveSpeed;
        controller.moveSpeed *= speedMultiplier;

        yield return new WaitForSeconds(duration);

        controller.moveSpeed = originalSpeed;
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void AddStamina(float amount)
    {
        stamina.Add(amount);
    }

    public void UseStamina(float amount)
    {
        stamina.Subtract(amount);
    }
}
