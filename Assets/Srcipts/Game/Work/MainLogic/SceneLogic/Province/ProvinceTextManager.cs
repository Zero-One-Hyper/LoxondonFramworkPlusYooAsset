using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProvinceTextManager
{
    private static readonly int MainColorID = Shader.PropertyToID("_BaseColor");
    private const string DEFAULTCOLOR = "#FFFFFF";
    private const string ACTIVECOLOR = "#1D4683";
    private Color _defaultColor;
    private Color _activeColor;
    public List<MeshRenderer> _allProvinceTextMeshes = new List<MeshRenderer>();
    public List<MaterialPropertyBlock> _allProvinceBlocks = new List<MaterialPropertyBlock>();

    public void Init(GameObject provinceRoot)
    {
        _allProvinceTextMeshes = provinceRoot.GetComponentsInChildren<MeshRenderer>().ToList();
        for (int i = 0; i < _allProvinceTextMeshes.Count; i++)
        {
            this._allProvinceBlocks.Add(new MaterialPropertyBlock());
        }
        ColorUtility.TryParseHtmlString(DEFAULTCOLOR, out _defaultColor);
        ColorUtility.TryParseHtmlString(ACTIVECOLOR, out _activeColor);
    }

    public void AcitveProvince(int index)
    {
        if (index >= _allProvinceBlocks.Count)
        {
            return;
        }

        _allProvinceBlocks[index].SetColor(MainColorID, _activeColor);
        _allProvinceTextMeshes[index].SetPropertyBlock(_allProvinceBlocks[index]);
    }

    public void DisActiveProvince(int index)
    {
        if (index >= _allProvinceBlocks.Count)
        {
            return;
        }

        _allProvinceBlocks[index].SetColor(MainColorID, _defaultColor);
        _allProvinceTextMeshes[index].SetPropertyBlock(_allProvinceBlocks[index]);
    }

}
