using System.Collections;
using System.Linq.Expressions;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class WeaponController : MonoBehaviour, IEquippable
{
    public Camera fpsCamera;
    public InventorySystem inventorySystem;
    public UIManager uiManager;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AnimatorOverrideController overrideController;


    public int currentAmmo;
    public int totalAmmo;

    private WeaponData weaponData;
    private Animator animator;
    private AudioSource audioSource;
    private Coroutine reloadCoroutine;
    private float _range;
    private float nextTimeToFire = 0f;
    private bool isReloading;
    private bool isEquipped;



    private void Start()
    {
        Item item = GetComponent<Item>();
        weaponData = item.itemData as WeaponData;
        if (weaponData == null)
        {
            Debug.LogError("WeaponData is not set on the item.");
            this.enabled = false;
            return;
        }

        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
        if (overrideController != null)
            animator.runtimeAnimatorController = overrideController;
        currentAmmo = weaponData.maxAmmo;
        _range = weaponData.range;

    }

    private void Update()
    { 
        if (!isEquipped || isReloading) return;

        if (Input.GetMouseButton(0) && currentAmmo > 0 && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + weaponData.fireRate;
            Shoot();
        }

        if ((Input.GetKeyDown(KeyCode.R) && totalAmmo > 0 && currentAmmo < weaponData.maxAmmo) ||
            (currentAmmo == 0 && totalAmmo > 0))
        {
            if (reloadCoroutine == null)
            {
                reloadCoroutine = StartCoroutine(Reload());
            }
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
        reloadCoroutine = null;
    }

    public void CancelReload()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
            isReloading = false;

            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();
            if(animator != null)
            {
                animator.Rebind();
            }
        }
    }

    public void OnEquip()
    {
        isEquipped = true;
        uiManager.AmmoDisplay(true);
        UpdateAmmo();
    }

    public void OnUnequip()
    {
        isEquipped = false;
        if(uiManager != null)
        {
            uiManager.AmmoDisplay(false);
        }
        CancelReload();
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
