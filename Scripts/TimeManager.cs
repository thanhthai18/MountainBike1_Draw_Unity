using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float slowdownFactor = 0.05f;
    public float slowdownLength = 2f;
    public bool isSlow;

    private void Start()
    {
        isSlow = false;
    }

    public void DoSlowmotion()
    {
        isSlow = true;
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;    
    }

    private void Update()
    {
        if (!isSlow)
        {
            Time.timeScale += (1f / 0.5f) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
        }
    }


}
