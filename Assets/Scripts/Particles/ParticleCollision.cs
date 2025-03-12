using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ParticleCollision : MonoBehaviour
{
    [Header("Settings")]
    public bool isPlayerParticle; // Indica se a partícula pertence ao jogador
    public GameObject explosionPrefab; // Prefab da explosão ao colidir
    public int damage = 1;

    [Header("References")]
    private ParticleSystem particleSystem;
    private List<ParticleCollisionEvent> collisionEvents;
    private CinemachineImpulseSource cameraShake;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();

        // Encontra a câmera virtual Cinemachine para o efeito de shake
        CinemachineVirtualCamera virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCamera != null)
        {
            cameraShake = virtualCamera.GetComponent<CinemachineImpulseSource>();
        }
    }

    void OnParticleCollision(GameObject other)
    {
        // Verifica se a colisão deve ser ignorada
        if (ShouldIgnoreCollision(other))
        {
            Debug.Log("ESCUDOOO");
            particleSystem.Stop();
            return;
        }else{
            // Processa a colisão com base no tipo de objeto
            ProcessCollision(other);

            // Cria uma explosão no ponto de colisão
            CreateExplosion(other);

            // Aplica um efeito de shake na câmera
            if (cameraShake != null)
            {
                cameraShake.GenerateImpulse();
            }
        }
    }

    bool ShouldIgnoreCollision(GameObject other)
    {
        // Ignora colisão com o próprio jogador se a partícula for do jogador
        if (isPlayerParticle && other.CompareTag("Player"))
        {
            return true;
        }else if(other.CompareTag("Shield")){
            Debug.Log("ESCUDO");
            return true;
        }else{
            return false;
        }

        // Ignora colisão com inimigos se a partícula não for do jogador
        //if (!isPlayerParticle && other.CompareTag("Player"))
        //{
           // PlayerCombat player = other.GetComponent<PlayerCombat>();
           // if (player != null) //&& player.Intangible)
            //{
             //   return true;
            //}
        //}

        //return false;
    }

    void ProcessCollision(GameObject other)
    {
        if (isPlayerParticle && other.CompareTag("Enemy"))
        {
            // Causa dano ao inimigo
            EnemyLife enemyLife = other.GetComponent<EnemyLife>();
            if (enemyLife != null)
            {
               enemyLife.TakeDamage(damage);
            }
        }
        else if (!isPlayerParticle && other.CompareTag("Player"))
        {
            // Causa dano ao jogador
            PlayerCombat player = other.GetComponent<PlayerCombat>();
            if (player != null)
            {
                //player.TakeDamage(2);
            }
        }
    }

    void CreateExplosion(GameObject other)
    {
        // Obtém os eventos de colisão das partículas
        int numCollisionEvents = particleSystem.GetCollisionEvents(other, collisionEvents);

        if (numCollisionEvents > 0 && explosionPrefab != null)
        {
            // Instancia a explosão no ponto de colisão
            Vector3 collisionPoint = collisionEvents[0].intersection;
            GameObject explosion = Instantiate(explosionPrefab, collisionPoint, Quaternion.identity);

            // Reproduz o sistema de partículas da explosão
            ParticleSystem explosionParticles = explosion.GetComponent<ParticleSystem>();
            if (explosionParticles != null)
            {
                explosionParticles.Play();
            }
        }
    }
}