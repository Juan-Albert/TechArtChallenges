using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public GameObject proyectile;
    public ParticleSystem hitVFX;
    
    
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

        if (Input.GetKeyDown(KeyCode.V))
        {
            CastProyectile();
        }

    }

    private void HitShield(Vector3 hitPos)
    {
        _renderer.material.SetVector("_HitPos", hitPos);
        StopAllCoroutines();
        StartCoroutine(Coroutine_HitDisplacement());
    }

    private void OpenCloseShield()
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

    private void DestroyShield()
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

    private void CastProyectile()
    {
        Transform camera = Camera.main.transform;
        Vector3 spawnPos = camera.position - camera.forward * 3 + camera.right * Random.Range(-2, 2) +
                           camera.up * Random.Range(-2, 2);

        GameObject newProyectile = Instantiate(proyectile, spawnPos, Quaternion.identity);

        newProyectile.transform.DOJump(transform.position, Random.Range(0.5f,2), 1, 1f);
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

    IEnumerator DelayDestroy(GameObject gameObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Proyectile"))
        {

            Vector3 hitPos = other.ClosestPoint(other.transform.position);
            
            other.GetComponent<ParticleSystem>().Stop();

            
            StartCoroutine(DelayDestroy(other.gameObject, 1f));

            hitVFX.transform.position = hitPos;
            hitVFX.Play();
            
            HitShield(hitPos);
        }
    }
}
