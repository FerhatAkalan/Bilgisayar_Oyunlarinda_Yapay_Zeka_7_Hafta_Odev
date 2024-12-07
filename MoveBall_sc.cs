using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MoveBall_sc : MonoBehaviour {
	// Topun başlangıç pozisyonunu saklar
	Vector3 ballStartPosition;
	// Topa fiziksel kuvvet uygulamak için Rigidbody2D referansı
	Rigidbody2D rb;
	// Topun hareket hızı
	float speed = 400;
	// Ses efektleri
	public AudioSource blip;
	public AudioSource blop;
	// Use this for initialization
	void Start () {
		rb = this.GetComponent<Rigidbody2D>();
		ballStartPosition = this.transform.position;
		ResetBall();
	}
	// Top bir nesneyle çarpıştığında çalışır
	void OnCollisionEnter2D(Collision2D col)
	{
		if(col.gameObject.tag == "backwall")
			blop.Play();// "Blop" sesini çal
		else
			blip.Play();// Diğer durumlarda "Blip" sesini çal
	}
	// Topun başlangıç pozisyonuna sıfırlanmasını sağlar
	public void ResetBall()
	{
		this.transform.position = ballStartPosition;
		rb.linearVelocity = Vector3.zero;
		// Rastgele bir yön belirle
		Vector3 dir = new Vector3(Random.Range(100,300),Random.Range(-100,100),0).normalized;
		rb.AddForce(dir*speed);
	}
	void Update () {

		if(Input.GetKeyDown("space"))
		{
			ResetBall();
		}
	}
}