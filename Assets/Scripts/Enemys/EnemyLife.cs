using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    public int life;
    public Rigidbody2D rb;

    public bool canBeSmashed;

    public int damage; //para causar dano ao jogador em caso de contato

    public void TakeDamage(int damage){
        life-=damage;

        if(life <= 0){
            Destroy(this.gameObject);
        }
    }

    void Start(){
        rb = GetComponent<Rigidbody2D>();
    }

    public void PushBack(Vector2 hitDirection)
    {
        if(rb == null){
            return;
        }
        // Normaliza a direção do golpe e aplica a força
        Vector2 pushDirection = hitDirection.normalized * 5f;
        //Animação do pushback
        rb.AddForce(pushDirection * 2f, ForceMode2D.Impulse);
    }
}
