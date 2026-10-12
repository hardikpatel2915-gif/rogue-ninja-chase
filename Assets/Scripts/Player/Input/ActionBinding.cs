using UnityEngine.InputSystem;

namespace RogueNinjaChase.Player.Input
{
    static class ActionBinding
    {
        public static InputAction Find(InputActionAsset asset, string mapName, string actionName)
        {
            if (asset == null || string.IsNullOrEmpty(mapName) || string.IsNullOrEmpty(actionName))
                return null;

            InputActionMap map = asset.FindActionMap(mapName, false);
            if (map == null)
                return null;

            return map.FindAction(actionName, false);
        }

        public static InputActionMap FindMap(InputActionAsset asset, string mapName)
        {
            if (asset == null || string.IsNullOrEmpty(mapName))
                return null;

            return asset.FindActionMap(mapName, false);
        }
    }
}
