using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PageGroup : MonoBehaviour
{
    private List<PageBase> _PageList = new List<PageBase>();

    public void ShowPage(string name, bool isMain, params object[] pars)
    {
        if (isMain)
        {
            while (this.transform.childCount > 0)
            {
                Transform page = this.transform.GetChild(0);
                page.GetComponent<PageBase>().ExcuteFree();
                SignalObjectManager.Instance.Despawn(page);
            }
            _PageList.Clear();
        }

        ShowNewPage(name, pars);
    }

    public void ReplacePage(string name, params object[] pars)
    {
        if (_PageList.Count > 0)
        {
            PageBase childPage = _PageList[_PageList.Count - 1];
            childPage.ExcuteFree();
            SignalObjectManager.Instance.Despawn(childPage.transform);
            _PageList.Remove(childPage);
        }
        ShowNewPage(name, pars);
    }

    private void ShowNewPage(string name, params object[] pars)
    {
        for (int i=0;i< this.transform.childCount;i++)
        {
            Transform page = this.transform.GetChild(i);
            page.gameObject.SetActive(false);
        }

        Transform childPage = SignalObjectManager.Instance.Spawn(name);
        if (childPage == null)
            return;
        childPage.GetComponent<RectTransform>().SetParent(this.transform, false);
        childPage.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        childPage.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        childPage.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);

        PageBase pageBase = childPage.GetComponent<PageBase>();
        if (pars != null)
        {
            for (int i = 0; i < pars.Length; i++)
            {
                pageBase.SetPar(i, pars[i]);
            }
        }
        pageBase.ExcuteInit();
        _PageList.Add(pageBase);
    }

    public void ClosePage()
    {
        if (_PageList.Count <= 1)
            return;
        PageBase childPage = _PageList[_PageList.Count - 1];
        childPage.ExcuteFree();
        SignalObjectManager.Instance.Despawn(childPage.transform);
        _PageList.Remove(childPage);

        if (_PageList.Count >= 0)
        {
            PageBase lastPage = _PageList[_PageList.Count - 1];
            Dictionary<int, object> pars = new Dictionary<int, object>(lastPage._Pars);
            lastPage.ExcuteFree();
            lastPage._Pars = pars;
            lastPage.ExcuteInit();
            lastPage.gameObject.SetActive(true);
        }
    }

    public int PageCount()
    {
        return _PageList.Count;
    }
}
