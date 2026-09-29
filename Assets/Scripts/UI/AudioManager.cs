using UnityEngine;
using System; 

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

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