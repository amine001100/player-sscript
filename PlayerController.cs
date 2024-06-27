using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]float speed = 0.15f;
    [SerializeField]float JForce = 10f;
    [SerializeField]float KBForce = 5f;
    [SerializeField] Transform respawnPoint;
    SpriteRenderer vec;
    Rigidbody2D rb;
    Animator anim;
    bool isJump;
    int score;
    int MaxHealth;
    int PlayerHealth;
    public Transform holder;
    Text ScoreT;
    Text HaelfT;
    Vector3[] initialEnemyPositions;
    
    Vector3[] initialItemPositions;
    
    
    [SerializeField]Transform ItemSound;
    [SerializeField] Transform JumpSound;
    [SerializeField] Transform DamageSound;
    [SerializeField] Transform EnemySound;
    
    

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    vec = GetComponent<SpriteRenderer>();
    rb = GetComponent<Rigidbody2D>();
    anim = GetComponent<Animator>();
    MaxHealth = 5;
    isJump = false;
    score = 0;
    PlayerHealth = MaxHealth;
    HaelfT = holder.Find("HaelfText").GetComponent<Text>();
    ScoreT = holder.Find("ScoreText").GetComponent<Text>();

    HaelfT.text = PlayerHealth + "/" + MaxHealth;//5/5 
    ScoreT.text =  "score: " + score;//score: 0
    
    
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            vec.flipX = false;
        }
        else if(Input.GetAxisRaw("Horizontal") < 0)
        {
            vec.flipX = true;
        }
        if (Input.GetButtonDown("Jump") && isJump == false)
        {
            rb.velocity = new Vector2(rb.velocity.x,JForce);
            isJump = true;
        //JumpSound parameter
            {
                Transform obj = Instantiate(JumpSound, transform.position, Quaternion.identity);
                obj.gameObject.SetActive(true);
                Destroy(obj.gameObject, obj.GetComponent<AudioSource>().clip.length);
            }
        }

        if (Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            isJump =  false;
        }
        if(Input.GetAxisRaw("Horizontal") != 0)
        {
            anim.SetBool("isRun",true);
        }
        else
        {
            anim.SetBool("isRun",false);
        }
        //anim.SetFloat("Speed",Mathf.Abs(rb.velocity.x));
        anim.SetBool("isJump",isJump);
       
        if(transform.position.y <= -10)
        {
            transform.position = respawnPoint.position;
            PlayerHealth = MaxHealth;
            HaelfT.text = PlayerHealth + "/" + MaxHealth;
            score = 0;
            ScoreT.text =  "score: " + score;
        }
    }
    
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(speed*Input.GetAxisRaw("Horizontal"),rb.velocity.y);
        /*if(Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(-speed,0));
            vec.flipX = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(speed,0));
            vec.flipX = false;
        }*/
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //enemy parameter
        if (collision.CompareTag("enemy"))
        {
            if(isJump && rb.velocity.y < 0)
            
            {
                Destroy(collision.gameObject);
                EnemyDamageSound(collision.transform.position);
            }
            
            else
            {
                PlayerHealth-=2;
                PlayerDamageSound(collision.transform.position);
                CheckHealth();            
            }
            HaelfT.text = PlayerHealth + "/" + MaxHealth;
            ApplyKnockback(collision.transform);
        }
        //items parameter
        //item 1
        else if(collision.CompareTag("item"))
        {
            score += 2;
            ScoreT.text =  "score: " + score;
            Destroy(collision.gameObject);
            PlayerItemSound(collision.transform.position);
        }
        //item 2
        else if(collision.CompareTag("item2"))
        {
            score += 10;
            ScoreT.text =  "score: " + score;
            Destroy(collision.gameObject);
            PlayerItemSound(collision.transform.position);
        }
        //item health
        else if(collision.CompareTag("haelt"))
        {
            if(PlayerHealth < MaxHealth)
            {
                PlayerHealth++;
                Destroy(collision.gameObject);
                PlayerItemSound(collision.transform.position);
            }
            HaelfT.text = PlayerHealth + "/" + MaxHealth;
            
        }
        
    }
    //item sound
    void PlayerItemSound(Vector3 itemPos)
    {
        
        Transform obj = Instantiate(ItemSound, itemPos, new Quaternion());
        obj.gameObject.SetActive(true);
        Destroy(obj.gameObject,obj.GetComponent<AudioSource>().clip.length);
    }
    // damage sound
    void PlayerDamageSound(Vector3 damegePos)
    {
        Transform obj = Instantiate(DamageSound, damegePos, new Quaternion());
        obj.gameObject.SetActive(true);
        Destroy(obj.gameObject,obj.GetComponent<AudioSource>().clip.length);
    }
        void EnemyDamageSound(Vector3 enemyPos)
    {
        Transform obj = Instantiate(EnemySound, enemyPos, new Quaternion());
        obj.gameObject.SetActive(true);
        Destroy(obj.gameObject,obj.GetComponent<AudioSource>().clip.length);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //enemy rd parameter
        if (collision.gameObject.CompareTag("enemy"))
        {
            if(isJump && rb.velocity.y < 0)
            {
                Destroy(collision.gameObject);
                EnemyDamageSound(collision.transform.position);
            }
            else
            {
                PlayerHealth--;
                PlayerDamageSound(collision.transform.position);
                CheckHealth();
            }
            HaelfT.text = PlayerHealth + "/" + MaxHealth;
            ApplyKnockback(collision.transform);
        }
                //enemy parameter
        /*else if (collision.gameObject.CompareTag("enemy"))
        {
            if(isJump && rb.velocity.y < 0)
            {
                Destroy(collision.gameObject);
            }
            else
            {
                PlayerHealth-=2;
                PlayerDamage(collision.transform.position);
            }
            HaelfT.text = PlayerHealth + "/" + MaxHealth;
            ApplyKnockback(collision.transform);
        }*/
    //knockbackDirection parameter
    }
    void ApplyKnockback(Transform enemy)
    {
        
        Vector2 knockbackDirection = (transform.position - enemy.position).normalized;
        rb.AddForce(knockbackDirection * KBForce, ForceMode2D.Impulse);
    }
    //Respawn parameter
    /*void CheckHealth()
    {
        if (PlayerHealth <= 0)
        {
            Debug.Log("Game Over");
            transform.position = respawnPoint.position;
            RespawnEnemiesAndItems();
            PlayerHealth = MaxHealth;
            HaelfT.text = PlayerHealth + "/" + MaxHealth;
            score = 0;
            ScoreT.text =  "score: " + score;
            

        }
    }*/
    void CheckHealth()
    {
        if (PlayerHealth <= 0)
        {
            Debug.Log("Game Over");
            RespawnPlayer();
        }
    }

    void RespawnPlayer()
    {
        transform.position = respawnPoint.position;
        PlayerHealth = MaxHealth;
        HaelfT.text = PlayerHealth + "/" + MaxHealth;
        score = 0;
        ScoreT.text = "score: " + score;
        //RespawnEnemiesAndItems();
    }
    void RespawnEnemiesAndItems()
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("enemy");
        for (int i = 0; i < enemys.Length; i++)
        {
            enemys[i].transform.position = initialEnemyPositions[i];
            if (!enemys[i].activeSelf)
            {
                enemys[i].SetActive(true);
            }
        }

        GameObject[] items = GameObject.FindGameObjectsWithTag("item");
        for (int i = 0; i < items.Length; i++)
        {
            items[i].transform.position = initialItemPositions[i];
            if (!items[i].activeSelf)
            {
                items[i].SetActive(true);
            }
        }
    }

    
}
