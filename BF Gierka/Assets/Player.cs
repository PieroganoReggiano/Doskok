﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float V = 0.1f;
    public int styki = 0;
    Rigidbody2D rigidbody;
    public float Power = 1.0f;
    public float amount = 3.0f;
    public List<int> kolizje;

    public void ReadResetKey()
    {
        if (Input.GetKeyDown("r"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    
    public GameObject preView;
    private float LastPath=0.0f;
    private bool sitting = false;
    private Collider2D myColl;
    public Vector3 futureDir;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        myColl = gameObject.GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        ReadResetKey();
       
        if (sitting && Time.time > LastPath + 0.1f)
        {
            LastPath = Time.time;
            //GameObject path = Instantiate(preView, transform.position, transform.rotation);
            //Physics2D.IgnoreCollision(path.GetComponent<Collider2D>(), myColl);
            //path.GetComponent<Rigidbody2D>().AddForce(futureDir*10, ForceMode2D.Impulse);
        }
        if (styki >= 1 && Input.GetMouseButtonDown(1))
        {
            rigidbody.velocity = Vector2.zero;
        }
        if (styki >= 1 && Input.GetMouseButton(1))
        {
            // int ruchH = Input.GetAxisRaw("Horizontal");
            // int ruchV = Input.GetAxisRaw("Vertical");
            Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
            dir = dir * amount * Time.deltaTime;
            rigidbody.AddForce((dir) * Power, ForceMode2D.Impulse);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        rigidbody.velocity = Vector2.zero;
        rigidbody.gravityScale = 0;
        styki++;
        kolizje.Add(collision.gameObject.GetInstanceID());
    }
    public void OnCollisionExit2D(Collision2D collision)
    {
        --styki;
        kolizje.Remove(collision.gameObject.GetInstanceID());
        if (styki == 0)
            rigidbody.gravityScale = 1;
    }
}