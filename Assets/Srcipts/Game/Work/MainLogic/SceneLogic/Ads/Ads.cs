using System;
using System.Collections.Generic;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnitData.Const;
using UnityEngine;

public class Ads : MonoBehaviour, IInteractive
{
    [SerializeField]
    private string _adId;
    private MeshRenderer _outLineRenderer;
    [SerializeField]
    private List<GameObject> _adsNotices;
    [SerializeField]
    private List<AdsInteract> _adsInteract;
    private Camera _mainCamera;
    private MaterialPropertyBlock _materialPropertyBlock;

    private readonly int OpaqueID = Shader.PropertyToID("_Opaque");

    public void Init()
    {
        _adsInteract = new List<AdsInteract>();
        _adsNotices = new List<GameObject>();
        //用名字判断
        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject child = this.transform.GetChild(i).gameObject;
            if (child.name.Contains("Collider"))
            {
                var adsInteractCollider = this.transform.GetChild(i).gameObject.AddComponent<AdsInteract>();
                adsInteractCollider.SetOwner(this);
                _adsInteract.Add(adsInteractCollider);
            }
            else
            {
                _adsNotices.Add(child);
            }
        }
    }

    private void Awake()
    {
        this._mainCamera = Camera.main;
    }

    private void Start()
    {
        _outLineRenderer = this.transform.GetComponent<MeshRenderer>();

        this._materialPropertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (this._adsInteract != null && _adsNotices != null)
        {
            Vector3 direction = this.transform.position - this._mainCamera.transform.position;
            float distance = direction.magnitude;

            this._materialPropertyBlock.SetFloat(OpaqueID, distance < 5.0f ? 1f : 0f);
            this._outLineRenderer.SetPropertyBlock(this._materialPropertyBlock);
            foreach (var go in this._adsNotices)
            {
                go.SetActive(distance < 5.0f);
            }
        }
    }

    public void Interactive(InterActiveArgs data)
    {
        if (data.interactiveColliderType == InteractiveColliderType.MouseClick)
        {
            IWebService webService = Context.GetApplicationContext().GetService<IWebService>();
            string id = data.str;

            XLog.I($"发送点击广告位{id}事件");
            webService.UnityCallWeb(Constant.UNITY_CALL_CLICK_ADS, id);
        }
    }


#if UNITY_EDITOR

    public string GetAdsData()
    {
        return this._adId;
    }

#endif
}
