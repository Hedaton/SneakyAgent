using System.Collections;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Camera fpsCamera;
    public WeaponData weaponData;
    public InventorySystem inventorySystem;
    public UIManager uiManager;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AnimatorOverrideController overrideController;


    public int currentAmmo;
    public int totalAmmo;

    private Animator animator;
    private AudioSource audioSource;
    private float _range;
    private float nextTimeToFire = 0f;
    private bool isReloading;
    private bool wasEquippedLastFrame = false;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
        if (overrideController != null)
            animator.runtimeAnimatorController = overrideController;
        currentAmmo = weaponData.maxAmmo;
        _range = weaponData.range;

    }

    private void Update()
    {
        bool isEquipped = inventorySystem.equippedItem == gameObject;

        if (isEquipped != wasEquippedLastFrame)
        {
            uiManager.AmmoDisplay(isEquipped);
            if (isEquipped) UpdateAmmo();
            wasEquippedLastFrame = isEquipped;
        }

        if (!isEquipped || isReloading) return;

        if (Input.GetMouseButton(0) && currentAmmo > 0 && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + weaponData.fireRate;
            Shoot();
        }

        if ((Input.GetKeyDown(KeyCode.R) && totalAmmo > 0 && currentAmmo < weaponData.maxAmmo) ||
            (currentAmmo == 0 && totalAmmo > 0))
        {
            StartCoroutine(Reload());
        }
    }

    private void Shoot()
    {
        currentAmmo--;
        UpdateAmmo();
        animator.SetTrigger("Shoot");
        audioSource.PlayOneShot(shootSound);
        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, _range))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(weaponData.damage);
                }
            }

        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        animator.SetTrigger("Reload");
        audioSource.PlayOneShot(reloadSound);
        yield return new WaitForSeconds(weaponData.reloadDuration);

        if (weaponData.maxAmmo - currentAmmo <= totalAmmo)
        {
            totalAmmo -= weaponData.maxAmmo - currentAmmo;
            currentAmmo += weaponData.maxAmmo - currentAmmo;
        }
        else
        {
            currentAmmo += totalAmmo;
            totalAmmo = 0;
        }
        UpdateAmmo();

        isReloading = false;
    }


    public void AddAmmo(int amount)
    {
        totalAmmo += amount;
        UpdateAmmo();
    }

    private void UpdateAmmo()
    {
        if (uiManager != null)
        {
            uiManager.UpdateAmmoDislay(currentAmmo, totalAmmo);
        }
    }


}
