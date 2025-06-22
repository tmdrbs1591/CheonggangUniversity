using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotEnemy : EnemyBase
{
    private void Start()
    {
        base.Start();
    }
    private void Update()
    {
        base.Update();
    }
    protected override IEnumerator Cor_Attack()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;

        dangerLineRenderer.enabled = true;

        dangerLine.SetActive(true);
        yield return new WaitForSeconds(1f);
        dangerLine.SetActive(false);

        Debug.Log("����!");
        // ��Ÿ�� ����
        currentCoolTime = attackCoolTime;

        // ���� �� ��� �ð�
        yield return new WaitForSeconds(1f);

        isAttacking = false;
    }

}
