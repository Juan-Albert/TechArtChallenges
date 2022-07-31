using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] AnimationCurve _DisplacementCurve;
    [SerializeField] float _DisplacementMagnitude;
    [SerializeField] float _DestroyDissolveSpeed;
    [SerializeField] float _DisolveSpeed;
    [SerializeField] float _HitDisolveSpeed;

    [Header("Components")]
    public Animator shieldCasterAnimator;
    public ParticleSystem castShieldVFX, breakShieldVFX;

    
    bool _shieldOn;
    bool _shieldDestroyed;
    Coroutine _disolveCoroutine;
    Coroutine _destroyCoroutine;

    Renderer _renderer;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                HitShield(hit.point);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            OpenCloseShield();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            DestroyShield();
        }

    }

    public void HitShield(Vector3 hitPos)
    {
        _renderer.material.SetVector("_HitPos", hitPos);
        StopAllCoroutines();
        StartCoroutine(Coroutine_HitDisplacement());
    }

    public void OpenCloseShield()
    {
        float target = _shieldOn ? 1 : 0;

        if (!_shieldOn)
        {
            shieldCasterAnimator.SetTrigger("DoCastShield");
            castShieldVFX.Play();
        }
        
        _shieldOn = !_shieldOn;
        if (_disolveCoroutine != null)
        {
            StopCoroutine(_disolveCoroutine);
        }
        _disolveCoroutine = StartCoroutine(Coroutine_DisolveShield(target));
    }

    public void DestroyShield()
    {
        float target = _shieldDestroyed ? 0 : 1;

        if (!_shieldDestroyed)
        {
            breakShieldVFX.Play();
        }
        
        _shieldDestroyed = !_shieldDestroyed;

        if (_destroyCoroutine != null)
        {
            StopCoroutine(_destroyCoroutine);
        }
        _destroyCoroutine = StartCoroutine(Coroutine_DestroyShield(target));
    }

    IEnumerator Coroutine_HitDisplacement()
    {
        _renderer.material.SetFloat("_HitDissolve", 0f);
        float lerp = 0;
        while (lerp < 1)
        {
            _renderer.material.SetFloat("_HitDissolve", _DisplacementCurve.Evaluate(lerp) * _DisplacementMagnitude);
            lerp += Time.deltaTime * _HitDisolveSpeed;
            yield return null;
        }


    }

    IEnumerator Coroutine_DisolveShield(float target)
    {
        float start = _renderer.material.GetFloat("_Disolve");
        float lerp = 0;
        while (lerp < 1)
        {
            _renderer.material.SetFloat("_Disolve", Mathf.Lerp(start, target, lerp));
            lerp += Time.deltaTime * _DisolveSpeed;
            yield return null;
        }
    }

    IEnumerator Coroutine_DestroyShield(float target)
    {
        float start = _renderer.material.GetFloat("_GlobalDissolve");
        float lerp = 0;
        while (lerp < 1)
        {
            _renderer.material.SetFloat("_GlobalDissolve", Mathf.Lerp(start, target, lerp));
            lerp += Time.deltaTime * _DestroyDissolveSpeed;
            yield return null;
        }
    }
}
