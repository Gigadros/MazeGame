using UnityEngine;

public class Enemy : MonoBehaviour {

    public float health = 50f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TakeDamage (float amount) {
        health -= amount;
        if (health <= 0f) {
            Die();
        }
    }

    void Die () {
        Destroy(gameObject);
    }
}
