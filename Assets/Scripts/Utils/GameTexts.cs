using static Enums;

public static class GameTexts {
    public static string GetMoreStarsToUnlockText(int stars) => $"Get {stars} more stars to unlock";

    public static string GetShopName(ShopItemType shopItemType) {
        switch (shopItemType) {
            case ShopItemType.Nozzle:
                return "Nozzles";
            case ShopItemType.HeatBed:
                return "Heatbeds";
            case ShopItemType.Frame:
                return "Frames";
            default:
                return "";
        }
    }
}
