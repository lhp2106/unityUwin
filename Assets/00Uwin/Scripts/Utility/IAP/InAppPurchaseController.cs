using UnityEngine;
using System.Collections;
using UnityEngine.Purchasing;
using System;
using UnityEngine.Purchasing.Security;
using UnityEngine.UI;
using UnityEngine.Events;

public class InAppPurchaseController : MonoBehaviour, IStoreListener
{
    public static InAppPurchaseController Instance { get; private set; }

    private IStoreController m_StoreController;
    private IExtensionProvider m_StoreExtensionProvider;
    private CrossPlatformValidator validator;

    private UnityAction<Product> buyProductCallbackDefault;
    private UnityAction<Product> buyProductCallback;
    private UnityAction initializedCallback;

    private bool isRestoringPurchases;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(this);
    }


    #region PUBLIC METHODS

    public void InitializePurchasing(ProductIAP[] listProduct, UnityAction<Product> buyCallback, UnityAction initCallback = null)
    {
        if (IsInitialized())
        {
            if (initCallback != null)
            {
                initCallback();
            }

            return;
        }

        buyProductCallback = buyCallback;
        buyProductCallbackDefault = buyCallback;
        initializedCallback = initCallback;

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        for (int i = 0; i < listProduct.Length; i++)
        {
            builder.AddProduct(listProduct[i].productId, listProduct[i].productType);
        }

#if UNITY_ANDROID || UNITY_IOS
        //@ToDo
        //validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
#endif
        UnityPurchasing.Initialize(this, builder);
    }

    /// <summary>
    /// Buy Product
    /// </summary>
    /// <param name="productId">Id of IAP pack</param>
    /// <param name="callback">Buy callback, if callback is NULL, the defaut buy callback from InitializePurchasing will be use</param>
    public void BuyProductID(string productId, UnityAction<Product> callback = null)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                if (callback != null)
                {
                    // Override purchase callback
                    buyProductCallback = callback;
                }
                else
                {
                    buyProductCallback = buyProductCallbackDefault;
                }

                m_StoreController.InitiatePurchase(product);

                VKDebug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
            }
            else
            {
                VKDebug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            VKDebug.Log("BuyProductID FAIL. Not initialized or Callback is null.");
        }
    }

    public void RestorePurchases(UnityAction<bool> callback = null)
    {
        if (!IsInitialized())
        {
            VKDebug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        if (isRestoringPurchases)
        {
            VKDebug.Log("Restoring Purchases");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            VKDebug.Log("RestorePurchases started ...");

            isRestoringPurchases = true;
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();

            apple.RestoreTransactions((result) =>
            {
                VKDebug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");

                isRestoringPurchases = false;

                if (callback != null)
                {
                    callback(result);
                }
            });
        }
        else
        {
            VKDebug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public Product GetProduct(string productId)
    {
        Product re = null;

        if (IsInitialized())
        {
            for (int i = 0; i < m_StoreController.products.all.Length; i++)
            {
                if (string.Equals(productId, m_StoreController.products.all[i].definition.id, StringComparison.Ordinal))
                {
                    re = m_StoreController.products.all[i];
                    break;
                }
            }
        }

        return re;
    }

    public Product[] GetAllProducts()
    {
        if (IsInitialized())
        {
            return m_StoreController.products.all;
        }
        else
        {
            VKDebug.Log("IAP is not Init, return null");
            return null;
        }
    }

    public bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    #endregion


    #region IMPLEMENTION IStoreListener

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        VKDebug.Log("OnInitializeFailed " + error);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;

        if (initializedCallback != null)
            initializedCallback();

        VKDebug.Log("On Initialized Success");
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        VKDebug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", i.definition.storeSpecificId, p));
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        bool validPurchase = true;

#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
        try
        {
            //@ToDo
            //var result = validator.Validate(args.purchasedProduct.receipt);

            //VKDebug.Log("********** Receipt is valid **********\n");
            //foreach (IPurchaseReceipt productReceipt in result)
            //{
            //    VKDebug.Log(string.Format("Product ID: {0}\nPurchased date: {1}\nReceipt: {2}", productReceipt.productID, productReceipt.purchaseDate, productReceipt));
            //}
            //VKDebug.Log("**************************************\n");
        }
        catch (IAPSecurityException e)
        {
            validPurchase = false;
            VKDebug.Log(string.Format("Invalid receipt: {0}", e.Message));
        }
#endif

        if (validPurchase)
        {
            if (buyProductCallback != null)
                buyProductCallback.Invoke(args.purchasedProduct);
        }

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }

    #endregion
}