using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sample
{
    public class GhostAI : MonoBehaviour
    {
        private Animator Anim;
        private CharacterController Ctrl;
        [SerializeField] GameManager GameManager;

        // Cache hash values
        private static readonly int IdleState = Animator.StringToHash("Base Layer.idle");
        private static readonly int MoveState = Animator.StringToHash("Base Layer.move");
        private static readonly int SurprisedState = Animator.StringToHash("Base Layer.surprised");
        private static readonly int AttackState = Animator.StringToHash("Base Layer.attack_shift");
        private static readonly int DissolveState = Animator.StringToHash("Base Layer.dissolve");
        private static readonly int AttackTag = Animator.StringToHash("Attack");

        // Dissolve
        [SerializeField] private SkinnedMeshRenderer[] MeshR;
        private float Dissolve_value = 1;
        private bool DissolveFlg = false;
        private const int maxHP = 3;
        private int HP = maxHP;

        // AI Settings
        [Header("AI Movement Settings")]
        [SerializeField] private float MoveSpeed = 3f;
        [SerializeField] private float ChaseSpeed = 5f;
        [SerializeField] private float RotationSpeed = 10f;

        [Header("AI Detection Settings")]
        [SerializeField] private float DetectionRange = 15f;
        [SerializeField] private float AttackRange = 2f;
        [SerializeField] private float LosePlayerDistance = 20f;

        [Header("AI Patrol Settings")]
        [SerializeField] private float PatrolRadius = 10f;
        [SerializeField] private float WaypointReachDistance = 1f;
        [SerializeField] private float IdleTime = 2f;

        [Header("Obstacle Settings")]
        [SerializeField] private float ObstacleCheckDistance = 1.5f;
        [SerializeField] private LayerMask ObstacleMask;

        // AI State
        private Transform PlayerTarget;
        private Vector3 targetPosition;
        private Vector3 spawnPoint;
        private float verticalVelocity;
        private float idleTimer;
        private float attackTimer;

        private enum AIState
        {
            Idle,
            Patrol,
            Chase,
            Attack
        }

        private AIState currentState = AIState.Idle;

        // 💥 YENİ: OnTriggerEnter Metodu (İstenen Tüm İşlemleri GameManager'a Devreder)
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Çarpışma anında hayaletin hareketini durdurmak isteyebiliriz.
                Ctrl.enabled = false;
                currentState = AIState.Idle;

                // Tüm oyun durdurma, panel gösterme, timer durdurma ve yazdırma 
                // işlemlerini GameManager'a devret.
                if (GameManager.Instance != null)
                {
                    // GameManager'daki GameOver metodu çağrılır.
                    GameManager.Instance.GameOver();
                }
            }
        }

        void Start()
        {
            Anim = GetComponent<Animator>();
            Ctrl = GetComponent<CharacterController>();
            spawnPoint = transform.position;

            // GameManager referansını al
            if (GameManager == null)
            {
                GameManager = GameManager.Instance;
            }

            // Oyuncu bul
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerTarget = player.transform;
                Debug.Log("AI Ghost: Target player found!");
            }
            else
            {
                Debug.LogWarning("AI Ghost: Player target NOT found! Add 'Player' tag to player object.");
            }

            // İlk hedef
            GetNewPatrolPoint();
            Debug.Log("AI Ghost initialized at position: " + transform.position);
        }

        void Update()
        {
            // Ölüm kontrolü
            if (HP <= 0 && !DissolveFlg)
            {
                StartDeath();
                return;
            }

            if (DissolveFlg)
            {
                UpdateDissolve();
                return;
            }

            // Zamanlayıcılar
            idleTimer -= Time.deltaTime;
            attackTimer -= Time.deltaTime;

            // State machine
            UpdateAI();

            // Yerçekimi
            ApplyGravity();
        }

        void UpdateAI()
        {
            float distanceToPlayer = PlayerTarget != null ? Vector3.Distance(transform.position, PlayerTarget.position) : 999f;

            // State geçişleri
            if (PlayerTarget != null && distanceToPlayer <= DetectionRange && currentState != AIState.Attack)
            {
                currentState = AIState.Chase;
            }
            else if (currentState == AIState.Chase && distanceToPlayer > LosePlayerDistance)
            {
                currentState = AIState.Patrol;
                GetNewPatrolPoint();
            }

            // State davranışları
            switch (currentState)
            {
                case AIState.Idle:
                    HandleIdle();
                    break;

                case AIState.Patrol:
                    HandlePatrol();
                    break;

                case AIState.Chase:
                    HandleChase();
                    break;

                case AIState.Attack:
                    HandleAttack();
                    break;
            }
        }

        void HandleIdle()
        {
            Anim.CrossFade(IdleState, 0.2f);

            if (idleTimer <= 0)
            {
                currentState = AIState.Patrol;
                GetNewPatrolPoint();
            }
        }

        void HandlePatrol()
        {
            Anim.CrossFade(MoveState, 0.2f);

            // Hedefe ulaştık mı?
            float distance = Vector3.Distance(transform.position, targetPosition);

            if (distance < WaypointReachDistance)
            {
                currentState = AIState.Idle;
                idleTimer = IdleTime;
                return;
            }

            // Hareket yönü
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0;

            // Engel varsa yeni hedef bul
            if (IsObstacleAhead(direction))
            {
                GetNewPatrolPoint();
                return;
            }

            // Yüzünü hedefe döndür
            LookAtDirection(direction);

            // HAREKET ET - CharacterController.Move ile
            Vector3 moveVector = direction * MoveSpeed * Time.deltaTime;
            Ctrl.Move(moveVector);

            Debug.DrawLine(transform.position, targetPosition, Color.green);
        }

        void HandleChase()
        {
            if (PlayerTarget == null)
            {
                currentState = AIState.Patrol;
                return;
            }

            Anim.CrossFade(MoveState, 0.2f);

            float distance = Vector3.Distance(transform.position, PlayerTarget.position);

            // Saldırı mesafesine geldik
            if (distance <= AttackRange && attackTimer <= 0)
            {
                currentState = AIState.Attack;
                attackTimer = 2f;
                return;
            }

            // Oyuncuya doğru hareket
            Vector3 direction = (PlayerTarget.position - transform.position).normalized;
            direction.y = 0;

            // Engel varsa etrafından dolaş
            if (IsObstacleAhead(direction))
            {
                direction = FindAvoidanceDirection(direction);
            }

            // Yüzünü oyuncuya döndür
            LookAtDirection(direction);

            // HAREKET ET
            Vector3 moveVector = direction * ChaseSpeed * Time.deltaTime;
            Ctrl.Move(moveVector);

            Debug.DrawLine(transform.position, PlayerTarget.position, Color.red);
        }

        void HandleAttack()
        {
            Anim.CrossFade(AttackState, 0.1f);

            // Oyuncuya bak
            if (PlayerTarget != null)
            {
                Vector3 direction = (PlayerTarget.position - transform.position).normalized;
                direction.y = 0;
                LookAtDirection(direction);
            }

            // Animasyon bittikten sonra
            if (attackTimer <= 0)
            {
                float distance = PlayerTarget != null ? Vector3.Distance(transform.position, PlayerTarget.position) : 999f;

                if (distance <= AttackRange * 1.5f)
                {
                    currentState = AIState.Attack;
                    attackTimer = 2f;
                }
                else if (distance <= DetectionRange)
                {
                    currentState = AIState.Chase;
                }
                else
                {
                    currentState = AIState.Patrol;
                    GetNewPatrolPoint();
                }
            }
        }

        //---------------------------------------------------------------------
        // MOVEMENT HELPERS
        //---------------------------------------------------------------------

        void LookAtDirection(Vector3 direction)
        {
            if (direction.magnitude < 0.1f) return;

            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        }

        bool IsObstacleAhead(Vector3 direction)
        {
            Vector3 rayStart = transform.position + Vector3.up * 0.5f;
            return Physics.Raycast(rayStart, direction, ObstacleCheckDistance, ObstacleMask);
        }

        Vector3 FindAvoidanceDirection(Vector3 blockedDirection)
        {
            // Sağ ve sol kontrol
            Vector3 right = Quaternion.Euler(0, 45, 0) * blockedDirection;
            Vector3 left = Quaternion.Euler(0, -45, 0) * blockedDirection;

            if (!IsObstacleAhead(right))
                return right;
            if (!IsObstacleAhead(left))
                return left;

            // Daha geniş açılar
            right = Quaternion.Euler(0, 90, 0) * blockedDirection;
            left = Quaternion.Euler(0, -90, 0) * blockedDirection;

            if (!IsObstacleAhead(right))
                return right;
            if (!IsObstacleAhead(left))
                return left;

            return -blockedDirection; // Geri dön
        }

        void GetNewPatrolPoint()
        {
            // Spawn etrafında rastgele nokta
            Vector2 randomCircle = Random.insideUnitCircle * PatrolRadius;
            targetPosition = spawnPoint + new Vector3(randomCircle.x, 0, randomCircle.y);

            // Y pozisyonunu ayarla
            targetPosition.y = spawnPoint.y;

            Debug.Log("AI Ghost: New patrol target set at " + targetPosition);
        }

        // 👻 GÜNCELLENDİ: Y eksenindeki hareketi engeller.
        void ApplyGravity()
        {
            if (!Ctrl.enabled) return;

            // Hayalet her zaman havada süzülüyormuş gibi davranır.
            // verticalVelocity'yi sıfırlayarak yerçekimi etkisini ortadan kaldırır.

            // CharacterController'ı zemine hafifçe basılı tutmak için küçük bir negatif hız ayarla.
            if (Ctrl.isGrounded)
            {
                verticalVelocity = -0.5f;
            }
            else
            {
                // Havada kalır ve düşmez.
                verticalVelocity = 0f;
            }

            // Yerçekimini uygula (sadece verticalVelocity'yi kullanır)
            Vector3 gravityMove = new Vector3(0, verticalVelocity, 0) * Time.deltaTime;
            Ctrl.Move(gravityMove);
        }

        //---------------------------------------------------------------------
        // DISSOLVE & DEATH
        //---------------------------------------------------------------------

        void StartDeath()
        {
            Anim.CrossFade(DissolveState, 0.1f);
            DissolveFlg = true;
            Ctrl.enabled = false;
        }

        void UpdateDissolve()
        {
            Dissolve_value -= Time.deltaTime * 0.5f;

            foreach (var mesh in MeshR)
            {
                if (mesh != null)
                    mesh.material.SetFloat("_Dissolve", Dissolve_value);
            }

            if (Dissolve_value <= 0)
            {
                Invoke("RespawnAI", 3f);
            }
        }

        void RespawnAI()
        {
            HP = maxHP;
            Dissolve_value = 1f;

            foreach (var mesh in MeshR)
            {
                if (mesh != null)
                    mesh.material.SetFloat("_Dissolve", Dissolve_value);
            }

            transform.position = spawnPoint;
            transform.rotation = Quaternion.identity;

            Ctrl.enabled = true;
            DissolveFlg = false;
            currentState = AIState.Idle;
            idleTimer = IdleTime;

            GetNewPatrolPoint();
        }

        //---------------------------------------------------------------------
        // PUBLIC METHODS
        //---------------------------------------------------------------------

        public void TakeDamage(int damage)
        {
            if (DissolveFlg) return;

            HP -= damage;

            if (HP > 0)
            {
                Anim.CrossFade(SurprisedState, 0.1f);
            }
        }

        //---------------------------------------------------------------------
        // DEBUG
        //---------------------------------------------------------------------

        void OnDrawGizmos()
        {
            // Spawn ve patrol alanı
            Vector3 spawn = Application.isPlaying ? spawnPoint : transform.position;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(spawn, PatrolRadius);

            // Detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, DetectionRange);

            // Attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AttackRange);

            // Hedef nokta
            if (Application.isPlaying)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(targetPosition, 0.5f);
                Gizmos.DrawLine(transform.position, targetPosition);
            }

            // İleri bakış
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward * ObstacleCheckDistance);
        }

        void OnDrawGizmosSelected()
        {
            // State bilgisi
            if (Application.isPlaying)
            {
                // UnityEditor.Handles.Label fonksiyonu Editor dışı derlemelerde hata verecektir.
                // Bu yüzden bunu sadece Editor ortamı için kullanmak en iyisidir.
                // Oyun içinde Debug amacıyla ekranda gösterim yapmıyoruz.
                /*
                UnityEditor.Handles.Label(transform.position + Vector3.up * 3,
                    "State: " + currentState.ToString() + "\nHP: " + HP);
                */
            }
        }
    }
}
