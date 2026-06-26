using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Menu Sound")]
    public AudioClip menuClick;

    [Header("Letter Sound")]
    public AudioClip letterClick;

    [Header("Result Sound")]
    public AudioClip winSound;
    public AudioClip loseSound;

    // tombol menu
    public void PlayMenuSound()
    {
        audioSource.PlayOneShot(menuClick);
    }

    // tombol huruf
    public void PlayLetterSound()
    {
        audioSource.PlayOneShot(letterClick);
    }

    // menang
    public void PlayWinSound()
    {
        audioSource.PlayOneShot(winSound);
    }

    // kalah
    public void PlayLoseSound()
    {
        audioSource.PlayOneShot(loseSound);
    }
}