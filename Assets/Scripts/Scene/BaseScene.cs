using System.Collections;
using UnityEngine;

public abstract class BaseScene : MonoBehaviour
{
    public float progress { get; protected set; }
    protected abstract IEnumerator LoadingRoutine();

    public void LoadAsync()
    {
        Clear();
        StartCoroutine(LoadingRoutine());
    }

    protected virtual void Start()
    {

        //Time.timeScale = 1f;
        GameManager.UI.FadeIn(1f);
    }

    protected virtual void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
    }

    private void OnDestroy()
    {
    }

    public abstract void Clear();
}
