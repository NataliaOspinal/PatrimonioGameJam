using UnityEngine;
using System; 
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [Header("Conexión con el Mixer")]
    public AudioMixer mainMixer;


    [Header("Reproductores")]
    public AudioSource bgmSource; 
    public AudioSource sfxSource;

    [Header("Listas de Audios")]
    public Sonido[] pistasBGM;
    public Sonido[] efectosSFX;

    void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    public void CambiarVolumenBGM(float valorSlider)
    {
        if (valorSlider <= 0.0001f) 
        {
            mainMixer.SetFloat("volBGM", -80f);
        }
        else
        {
            mainMixer.SetFloat("volBGM", Mathf.Log10(valorSlider) * 20f);
        }
    }

    public void CambiarVolumenSFX(float valorSlider)
    {
        if (valorSlider <= 0.0001f) 
        {
            mainMixer.SetFloat("volSFX", -80f);
        }
        else
        {
            mainMixer.SetFloat("volSFX", Mathf.Log10(valorSlider) * 20f);
        }
    }

    public void ReproducirBGM(string nombre)
    {
        Sonido s = Array.Find(pistasBGM, x => x.nombre == nombre);
        if (s != null)
        {
            bgmSource.clip = s.clip;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning("No se encontró la música: " + nombre);
        }
    }

    public void ReproducirSFX(string nombre)
    {
        Sonido s = Array.Find(efectosSFX, x => x.nombre == nombre);
        if (s != null)
        {
            sfxSource.PlayOneShot(s.clip);
        }
        else
        {
            Debug.LogWarning("No se encontró el efecto: " + nombre);
        }
    }
}

[Serializable]
public class Sonido
{
    public string nombre;
    public AudioClip clip;
}