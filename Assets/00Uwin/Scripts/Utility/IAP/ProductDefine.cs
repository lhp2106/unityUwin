using UnityEngine.Purchasing;

public class ProductDefine
{
    public static readonly ProductIAP PACK1 = new ProductIAP("sanh.vip.pack1", ProductType.Consumable);
    public static readonly ProductIAP PACK2 = new ProductIAP("sanh.vip.pack2", ProductType.Consumable);
    public static readonly ProductIAP PACK5 = new ProductIAP("sanh.vip.pack5", ProductType.Consumable);
    public static readonly ProductIAP PACK9 = new ProductIAP("sanh.vip.pack9", ProductType.Consumable);
    public static readonly ProductIAP PACK19 = new ProductIAP("sanh.vip.pack19", ProductType.Consumable);
    public static readonly ProductIAP PACK49 = new ProductIAP("sanh.vip.pack49", ProductType.Consumable);
    public static readonly ProductIAP PACK99 = new ProductIAP("sanh.vip.pack99", ProductType.Consumable);
    public static readonly ProductIAP PACK199 = new ProductIAP("sanh.vip.pack199", ProductType.Consumable);

    private static ProductIAP[] arr = {
        PACK1,
        PACK2,
        PACK5,
        PACK9,
        PACK19,
        PACK49,
        PACK99,
        PACK199,
    };

    public static ProductIAP[] GetListProducts()
    {
        return arr;
    }
}
