using UnityEngine;

public class MovingSound : MonoBehaviour
{

    public AudioSource audioSource;
    public Vector2 earPoint;
    public bool onlyVerticalSoundChange;
    public Transform movingSound;

    float distance2EarPoint;
    float cameraHeight;
    float volumeMax;

    private void Start()
    {
        volumeMax = audioSource.volume;

        cameraHeight = Camera.main.orthographicSize;
        if (!onlyVerticalSoundChange)
        {
            //added 0.7f so that the transform can still be heard if a lil of screen
            distance2EarPoint = Vector2.Distance(new Vector2(Camera.main.aspect * cameraHeight, cameraHeight), earPoint) + 0.7f;
        }
        else
        {
            distance2EarPoint = Vector2.Distance(new Vector2(earPoint.x, cameraHeight), earPoint) + 0.7f;
        }
    }

    public void PlayAudioClip()
    {
        audioSource.Play();
    }

    void Update()
    {
        if (!onlyVerticalSoundChange)
        {
            audioSource.volume = volumeMax - volumeMax * Mathf.Clamp((Vector2.Distance(transform.position, earPoint) / distance2EarPoint), 0, 1);
        }
        else
        {
            audioSource.volume = volumeMax - volumeMax * Mathf.Clamp((Mathf.Abs(transform.position.y - earPoint.y) / distance2EarPoint), 0, 1);
        }
    }
}
