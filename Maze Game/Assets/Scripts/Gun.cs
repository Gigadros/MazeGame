using UnityEngine;
using EZCameraShake;
using System.Collections;

public class Gun : MonoBehaviour {

    public float damage = 10f;
    public float range = 100f; // 100m
    public float fireRate = 0.5f;
    public float impactForce = 60f;
    public float explosionRadius = 2f; // 2m

    public Camera fpsCam;
    public GameObject impactEffect;
    public AudioSource gunAudioSource;

    private float nextTimeToFire = 0f;
    private float projectileSpeed = 250f; // 250m/s based on RPG-7 avg velocity
    private float soundSpeed = 343f; // 343m/s speed of sound in air normal conditions

    void Update () {
		if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire) {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
	}

    void Shoot () {
        gunAudioSource.volume = Random.Range(0.45f, 0.55f);
        gunAudioSource.pitch = Random.Range(0.75f, 0.85f);
        gunAudioSource.Play();
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range)) {
        } else {
            // create a false impact point to explode in mid-air
            hit = new RaycastHit
            {
                point = fpsCam.transform.position + fpsCam.transform.forward * range,
                distance = range,
                normal = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized
            };
        }
        StartCoroutine(Impact(hit)); // projectile travel delay
    }

    IEnumerator Impact (RaycastHit hit) {
        yield return new WaitForSeconds(hit.distance / projectileSpeed);
        Explosion(hit.point);
        // TODO: impact effect object pooling
        GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(impactGO, 1.95f);
        AudioSource explosionAudioSource = impactGO.GetComponent<AudioSource>();
        StartCoroutine(Shockwave(explosionAudioSource, hit, hit.distance / soundSpeed)); // shockwave delay
    }

    void Explosion(Vector3 center)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, explosionRadius); // AoE
        for (int i = 0; i < hitColliders.Length; i++)
        {
            Enemy enemy = hitColliders[i].transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Rigidbody rb = hitColliders[i].transform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 forceDir = (center - rb.position).normalized;
                rb.AddForce(forceDir * impactForce);
            }
        }
    }

    IEnumerator Shockwave (AudioSource explosionAudioSource, RaycastHit hit, float delay) {
        yield return new WaitForSeconds(delay);
        explosionAudioSource.pitch = Random.Range(0.7f, 0.9f);
        explosionAudioSource.Play();
        float shakeStrength = range / hit.distance * 1.25f;
        shakeStrength = Mathf.Clamp(shakeStrength, 1f, 6f);
        CameraShaker.Instance.ShakeOnce(shakeStrength, shakeStrength / 1.5f, 0.1f, shakeStrength / 4f);
    }
}
