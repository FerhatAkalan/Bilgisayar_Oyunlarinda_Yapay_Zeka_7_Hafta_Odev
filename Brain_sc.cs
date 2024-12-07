using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Brain_sc : MonoBehaviour
{
    public GameObject paddle;// Paddle nesnesi
    public GameObject ball;// Top nesnesi
    Rigidbody2D brb;// Topun Rigidbody2D bileşeni
    float yvel;// Paddle'ın yukarı/aşağı hareket hızı
    // Paddle hareket sınırları
    float paddleMinY = 8.8f;
    float paddleMaxY = 17.4f;
    float paddleMaxSpeed = 15;
    // Paddle'ın başarı ve başarısızlık durumları için sayaçlar
    public float numSaved = 0;
    public float numMissed = 0;
    ANN ann;// Yapay sinir ağı nesnesi
     // Sinir ağına veri gönderip çıktıları alır
    List<double> Run(double bx, double by, double bvx, double bvy, double px, double py, double pv, bool train)
    {
        // Girdi ve çıktı değerlerini tanımla
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();
        // Girdi değerlerini ekle
        inputs.Add(bx);// Topun x pozisyonu
        inputs.Add(by);// Topun y pozisyonu
        inputs.Add(bvx);// Topun x eksenindeki hızı
        inputs.Add(bvy);// Topun y eksenindeki hızı
        inputs.Add(px);// Paddle'ın x pozisyonu
        inputs.Add(py);// Paddle'ın y pozisyonu
        // Çıktı değeri ekle
        outputs.Add(pv);// Paddle'ın hızının tahmini
        // Eğer eğitim modu aktifse ağı eğit, değilse tahmin yap
        if(train)
            return (ann.Train(inputs, outputs));// Ağı eğit
        else
            return (ann.CalcOutput(inputs, outputs));// Çıkış değerlerini hesapla
    }
    void Start()
    {
        // Yapay sinir ağını başlat (6 giriş, 1 çıkış, 1 gizli katman, 4 nöron, 0.11 öğrenme oranı)
        ann = new ANN(6, 1, 1, 4, 0.11);
        // Topun Rigidbody2D bileşenini al
        brb = ball.GetComponent<Rigidbody2D>();
    }
    [System.Obsolete]
    void Update () 
    {
         // Paddle'ı yukarı ve aşağı hareket ettirirken sınırları kontrol et
        float posy = Mathf.Clamp(paddle.transform.position.y + (yvel * Time.deltaTime * paddleMaxSpeed),
            paddleMinY,
            paddleMaxY);
        // Paddle'ın pozisyonunu güncelle
        paddle.transform.position = new Vector3(paddle.transform.position.x,
            posy,
            paddle.transform.position.z);

        List<double> output = new List<double>();
        // "ball" katmanındaki nesneleri kontrol etmek için bir Raycast gönder
        int layerMask = 1 << 9;
        RaycastHit2D hit = Physics2D.Raycast(ball.transform.position, brb.velocity, 1000, layerMask);

        if (hit.collider != null)
        {
            // Eğer çarpılan nesne "ball" etiketi taşırsa, vektörü yansıt
            if(hit.collider.gameObject.tag == "ball")
            {
                Vector3 reflection = Vector3.Reflect(brb.velocity, hit.normal);
                hit = Physics2D.Raycast(hit.point, reflection, 1000, layerMask);
            }
            // Eğer çarpılan nesne "backwall" etiketi taşıyorsa
            if(hit.collider != null && hit.collider.gameObject.tag == "backwall")
            {
                // Paddle'ın topa göre pozisyon farkını hesapla
                float dy = (hit.point.y - paddle.transform.position.y);
                // Yapay sinir ağına girişleri ver ve çıktı al
                output = Run(ball.transform.position.x,
                            ball.transform.position.y,
                            brb.velocity.x, brb.velocity.y,
                            paddle.transform.position.x,
                            paddle.transform.position.y,
                            dy, true);
                // Çıktıya göre paddle'ın hareket hızını ayarla
                yvel = (float) output[0];
            }
            else
                // Eğer paddle hareket etmeye gerek yoksa hızı sıfırla
                yvel = 0;
        }
    }
}