using System;
using UnityEngine;
using UnityEngine.UI;

public class VKPageController : MonoBehaviour
{
    public Text[] txtPages;

    public VKButton btBackAll;
    public VKButton btBack;
    public VKButton btPage1;
    public VKButton btPage2;
    public VKButton btPage3;
    public VKButton btNext;
    public VKButton btNextAll;

    public int currentPage;
    public int maxPage;

    public Color cNormalPage;
    public Color cCurrentPage;

    public Action<int> OnSelectPage;

    private int[] numberPages;

    public void ButtonBackAllClickListener()
    {
        currentPage = 1;
        SetupButton();
        SetupNumberPage();

        if (OnSelectPage != null)
            OnSelectPage(currentPage);
    }

    public void ButtonBackClickListener()
    {
        currentPage--;
        SetupButton();
        SetupNumberPage();

        if (OnSelectPage != null)
            OnSelectPage(currentPage);
    }

    public void ButtonNextAllClickListener()
    {
        currentPage = maxPage;
        SetupButton();
        SetupNumberPage();

        if (OnSelectPage != null)
            OnSelectPage(currentPage);
    }

    public void ButtonNextClickListener()
    {
        currentPage++;
        SetupButton();
        SetupNumberPage();

        if (OnSelectPage != null)
            OnSelectPage(currentPage);
    }

    public void ButtonPageClickListener(int index)
    {
        if (currentPage == numberPages[index])
            return;

        currentPage = numberPages[index];
        SetupButton();
        SetupNumberPage();

        if (OnSelectPage != null)
            OnSelectPage(currentPage);
    }

    public void InitPage(int maxPage, Action<int> onSelectPage)
    {
        numberPages = new[] { 1, 2, 3 };
        if (maxPage > 0)
            currentPage = 1;
        else
            currentPage = 0;
        this.maxPage = maxPage;
        this.OnSelectPage = onSelectPage;

        SetupButton();
        SetupTextPage();
    }

    private void SetupButton()
    {
        btBack.VKInteractable = currentPage > 1;
        btBackAll.VKInteractable = currentPage > 2;
        btNext.VKInteractable = currentPage < maxPage;
        btNextAll.VKInteractable = maxPage > 2 && currentPage < maxPage - 1;

        btPage1.VKInteractable = maxPage >= numberPages[0];
        btPage2.VKInteractable = maxPage >= numberPages[1];
        btPage3.VKInteractable = maxPage >= numberPages[2];
    }

    private void SetupNumberPage()
    {
        int index = Array.IndexOf(numberPages, currentPage);
        if (index == 2 && maxPage > currentPage || index == 0 && currentPage > 1)
        {
            numberPages = new[] { currentPage - 1, currentPage, currentPage + 1 };
        }
        else if (currentPage == 1 || (currentPage < 3 && maxPage <= 3))
        {
            numberPages = new[] { 1, 2, 3 };
        }
        else if (currentPage == maxPage)
        {
            numberPages = new[] { maxPage - 2, maxPage - 1, maxPage };
        }
        SetupTextPage();
    }

    private void SetupTextPage()
    {
        for (int i = 0; i < 3; i++)
        {
            txtPages[i].text = numberPages[i].ToString();
            txtPages[i].color = currentPage == numberPages[i] ? cCurrentPage : cNormalPage;
        }

        if (!btPage1.VKInteractable)
            btPage1.SetupAll(btPage1.VKInteractable);
        if (!btPage2.VKInteractable)
            btPage2.SetupAll(btPage2.VKInteractable);
        if (!btPage3.VKInteractable)
            btPage3.SetupAll(btPage3.VKInteractable);
    }
}
