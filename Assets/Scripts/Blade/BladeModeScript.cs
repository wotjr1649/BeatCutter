using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MeshSlice;

public class BladeModeScript : MonoBehaviour
{
    public bool bladeMode;

    private Animator anim;
    public Transform cutPlane;
    

    public Material crossMaterial;

    public LayerMask layerMask;
    ParticleSystem[] particles;

    void Start()
    {
        cutPlane.gameObject.SetActive(false);
        anim = GetComponent<Animator>();
        particles = cutPlane.GetComponentsInChildren<ParticleSystem>();
        Zoom(true);
    }

    void Update()
    {
        anim.SetFloat("x", Mathf.Clamp(Camera.main.transform.GetChild(0).localPosition.x + 0.3f,-1,1));
        anim.SetFloat("y", Mathf.Clamp(Camera.main.transform.GetChild(0).localPosition.y + .18f, -1,1));
        
//        Debug.Log(NoteGenerator.totalNode);
        

        if (bladeMode)
        {
            RotatePlane();

            if (Input.GetMouseButtonDown(0))
            {
                cutPlane.GetChild(0).DOComplete();
                cutPlane.GetChild(0).DOLocalMoveX(cutPlane.GetChild(0).localPosition.x * -1, .05f).SetEase(Ease.OutExpo);
                ShakeCamera();
                Slice();
            }
        }

        Debugs();
    }

    public void Slice()
    {
        Collider[] hits = Physics.OverlapBox(cutPlane.position, new Vector3(5, 0.1f, 5), cutPlane.rotation, layerMask);

        if (hits.Length <= 0)
            return;

        for (int i = 0; i < hits.Length; i++)
        {
            SlicedHull hull = SliceObject(hits[i].gameObject, crossMaterial);
            if (hull != null)
            {
                UIRootController.instance.UpdateScoreText();
                GameObject bottom = hull.CreateLowerHull(hits[i].gameObject, crossMaterial);
                GameObject top = hull.CreateUpperHull(hits[i].gameObject, crossMaterial);
                AddHullComponents(bottom);
                AddHullComponents(top);
                Destroy(hits[i].gameObject);
                Destroy(bottom, 1f);
                Destroy(top, 1f);
            }
        }
    }

    public void AddHullComponents(GameObject go)
    {
        go.layer = 9;
        Rigidbody rb = go.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        MeshCollider collider = go.AddComponent<MeshCollider>();
        collider.convex = true;

        rb.AddExplosionForce(100, go.transform.position, 20);
        collider.convex = false;
    }

    public SlicedHull SliceObject(GameObject obj, Material crossSectionMaterial = null)
    {
        if (obj.GetComponent<MeshFilter>() == null)
            return null;

        return obj.Slice(cutPlane.position, cutPlane.up, crossSectionMaterial);
    }

    public void Zoom(bool state)
    {
        bladeMode = state;
        anim.SetBool("bladeMode", bladeMode);

        cutPlane.localEulerAngles = Vector3.zero;
        cutPlane.gameObject.SetActive(true);

        string x = state ? "Horizontal" : "Mouse X"; string y = state ? "Vertical" : "Mouse Y";


        DOVirtual.Float(Time.timeScale, 1.0f, .02f, SetTimeScale);
        //movement.enabled = !state;

        if (state)
        {
            //GetComponent<Animator>().SetTrigger("draw");
        }
        else
        {
            transform.DORotate(new Vector3(0, transform.eulerAngles.y, 0), .2f);
        }
    }

    public void RotatePlane()
    {
        cutPlane.eulerAngles += new Vector3(0, 0, -Input.GetAxis("Mouse X") * 5);
    }
    void SetTimeScale(float time)
    {
        Time.timeScale = time;
    }


    void Debugs()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    public void ShakeCamera()
    {
        foreach (ParticleSystem p in particles)
        {
            p.Play();
        }
    }
}
