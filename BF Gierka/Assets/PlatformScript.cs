﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlatformScript : MonoBehaviour
{
    [Header("Random")]
    public bool isSelected;
    public Renderer rend;
    public LineRenderer line;
    [Header("Growth")]
    public bool isGrowing = false;
    public Vector3 growDir;
    public float Power = 1.0f;
    public float GrowSpeed=0.1f;
    [Header("Become Background")]
    public Vector3 BackgroundScale;
    public Vector3 BackgroundPostion;
    public AnimationCurve ExplodeSpeed;
    public float ExplodeTime;
    [Header("Visuals")]
    public SpriteRenderer Image;
    private float growStart;
    private Player player;
    void Start()
    {
        rend = GetComponent<Renderer>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public float GetBorder(int direction)
    {
        float ret = 0.0f;
        Vector2 place = transform.localPosition;
        Vector2 scale = transform.localScale;
        switch (direction)
        {
            case 0:
                ret = place.x + scale.x;
                break;
            case 1:
                ret = place.y + scale.y;
                break;
            case 2:
                ret = place.x - scale.x;
                break;
            case 3:
                ret = place.y - scale.y;
                break;
        }
        return ret;
    }



    public void Grow(Vector3 direction)
    {
        var colls = Physics2D.BoxCastAll(transform.localPosition, transform.localScale, 0.0f, direction, 0.08f);
        foreach(RaycastHit2D xd in colls){
            //
            if (xd.collider.gameObject == gameObject)
                continue;
            if (xd.collider.GetComponent<GrowBlocker>() != null)
            {
                //Debug.Log("zderzonko");
                return;
            }
        }
        transform.localScale += new Vector3(Mathf.Abs(direction.x), Mathf.Abs(direction.y), Mathf.Abs(direction.z) );
        transform.localPosition += 0.5f * direction;
    }

    public void Grow(float amount)
    {
        Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"),0);
        dir = dir * amount * Time.deltaTime;
        growDir += dir;
        player.futureDir = (growDir)  * Power;
        line.SetPosition(1, growDir);
        if (dir != Vector3.zero)
            Grow(dir);
    }
    

    void OnMouseEnter()
    {
        isGrowing = true;
        growStart = Time.time;
        growDir = Vector3.zero;
        
    }

    void OnMouseExit()
    {
        isGrowing = false;
        Collider2D coll = GetComponent<Collider2D>();
        List<Collider2D> result= new List<Collider2D>();
        coll.OverlapCollider(new ContactFilter2D(), result);
        foreach (Collider2D collider2D in result)
        {
            Rigidbody2D rb = collider2D.GetComponent<Rigidbody2D>();
            if (rb == null)
                continue;
            rb.AddForce( (growDir)  * Power, ForceMode2D.Impulse);
        }
    }

    public void BecomeBackground()
    {
        Rigidbody2D rigidbody2 = gameObject.GetComponent<Rigidbody2D>();
        rigidbody2.simulated = false;

        StartCoroutine(Dying());
        
        
    }
    IEnumerator Dying()
    {
        float startTime = Time.time;
        Vector3 startScale = transform.localScale;
        Vector3 startPostion = transform.localPosition;
        Vector3 withoutZ = new Vector3(1, 1, 0);
        while (startTime+ExplodeTime>Time.time)
        {

            transform.localScale = Vector3.Lerp(startScale,BackgroundScale, ExplodeSpeed.Evaluate((Time.time - startTime) / ExplodeTime));
            transform.localPosition = Vector3.Lerp(startPostion, Camera.main.transform.localPosition+new Vector3(0,0,50), ExplodeSpeed.Evaluate((Time.time - startTime) / ExplodeTime));
            yield return new WaitForEndOfFrame();
        }
        gameObject.GetComponentInChildren<leafesScaler>().enabled = false;
        Image.drawMode = SpriteDrawMode.Simple;
        gameObject.GetComponentInChildren<leafesScaler>().transform.localScale = Vector3.one/100;
        transform.SetParent(Camera.main.transform);
        
        
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("HI");
            BecomeBackground();
        }
    }

    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (isGrowing)
        {
            Grow(GrowSpeed);
        }


    }
}
