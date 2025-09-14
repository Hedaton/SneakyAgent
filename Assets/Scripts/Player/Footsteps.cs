using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Footsteps : MonoBehaviour
{
    // Ses dosyaları için dizi.
    public AudioClip[] clips;
    // Adım sesleri arasındaki temel zaman aralığı (saniyeler içinde).
    public float baseStepInterval = 0.4f;
    // Hız faktörü, adım sesinin hızla orantısını ayarlar.
    public float speedFactor = 7f;

    // Ses bileşenine referans.
    private AudioSource src;
    // Movement bileşenine referans.
    private Movement move;
    // Son adım sesinin ne zaman çalındığını takip eder.
    private float lastStepTime;

    void Start()
    {
        src = GetComponent<AudioSource>();
        // Sadece tek bir Movement script'ine ihtiyacımız var.
        move = GetComponent<Movement>();
    }

    void Update()
    {
        // Eğer hareket script'i yoksa, fonksiyonu sonlandır.
        if (!move) return;

        // Güncellenmiş Movement script'inden gerekli bilgileri alıyoruz.
        float speed = move.CurrentSpeed;
        bool grounded = move.IsGrounded();
        bool moving = move.isMoving;

        // Yerdeyse ve hareket ediyorsa adım seslerini kontrol et.
        if (grounded && moving)
        {
            // Hıza bağlı olarak adım sesi aralığını hesapla.
            float interval = baseStepInterval * (speedFactor / speed);

            // Eğer yeterli zaman geçtiyse, yeni bir adım sesi çal.
            if (Time.time - lastStepTime > interval)
            {
                PlayStep();
                lastStepTime = Time.time;
            }
        }
    }

    void PlayStep()
    {
        // Ses dosyası yoksa, fonksiyondan çık.
        if (clips.Length == 0) return;
        // Diziden rastgele bir adım sesi seç ve çal.
        src.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }
}