    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.AI;

    public class Enemies : MonoBehaviour
    {
        public Slider healthSlider;
        public int maxHealth;
        public int curHealth;

        private NavMeshAgent navMeshAgent;
        public GameObject player;
        public Animator animator;
        public float distance;

        private bool isAttacks = false;
        public int minDamage;
        public int maxDamage;
        public PlayerData data;

        private bool hasDied = false;


        private void Start() {
            healthSlider.maxValue = maxHealth;
            curHealth = maxHealth;
            healthSlider.value = curHealth;

            navMeshAgent = GetComponent<NavMeshAgent>();
            data = FindObjectOfType<PlayerData>();
        }

        public void Update() {
            if(player != null)
            {
                distance = Vector3.Distance(player.transform.position, transform.position);
                if(distance > 15)
                {
                    animator.SetTrigger("Orc Idle");
                    navMeshAgent.enabled = false;
                    isAttacks = false;
                }
                else if(distance < 15 && distance > 1)
                {
                    animator.SetTrigger("Orc Walk");
                    navMeshAgent.enabled = true;
                    navMeshAgent.SetDestination(player.transform.position);
                    isAttacks = false;
                }
                else if(distance < 1)
                {
                    if(!isAttacks)
                    {
                        navMeshAgent.enabled = false;
                        animator.SetTrigger("Attack");
                        StartCoroutine(Attack());
                    }        
                }
            }
        }

        public void TakeDamage(int damage)
        {
            if(!hasDied)
            {
                curHealth -= damage;
                healthSlider.value = curHealth;
                if(curHealth <= 0)
                {
                    hasDied = true;
                    animator.SetTrigger("Dye Orc");
                    PlayerData.orcsDiedTotal += 1; 
                    Destroy(gameObject, 5f);
                }
            }
        }

        IEnumerator Attack()
        {
            if(!isAttacks)
            {
                isAttacks = true;
                yield return new WaitForSeconds(1f);
                data.TakeDamage(Random.Range(minDamage, maxDamage));
                isAttacks = false;
            }
        }
    }
