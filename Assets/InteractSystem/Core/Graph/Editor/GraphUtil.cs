using UnityEditor;

namespace InteractSystem.Graph
{
    public class GraphUtil
    {
        private static TextureContent _backgroundContents;
        private static TextureContent _portContents;
        private static TextureContent _iconContents;
        private const string backgroundTexturesGUID = "bd4d13d14a284ac40996cc8a3741a565";
        private const string portTexturesGUID = "9d43325a83bf7404ba11517173f153c4";
        private const string iconTexturesGUID = "07147659d96bff741b3c2b89f89dfece";

        public static TextureContent BackgroundContent
        {
            get
            {
                if (!_backgroundContents)
                {
                    var path = AssetDatabase.GUIDToAssetPath(backgroundTexturesGUID);
                    _backgroundContents = AssetDatabase.LoadAssetAtPath<TextureContent>(path);
                }
                return _backgroundContents;
            }
        }
        public static TextureContent PortContent
        {
            get
            {
                if (!_portContents)
                {
                    var path = AssetDatabase.GUIDToAssetPath(portTexturesGUID);
                    _portContents = AssetDatabase.LoadAssetAtPath<TextureContent>(path);
                }
                return _portContents;
            }
        }
        public static TextureContent IconContents
        {
            get
            {
                if (!_iconContents)
                {
                    var path = AssetDatabase.GUIDToAssetPath(iconTexturesGUID);
                    _iconContents = AssetDatabase.LoadAssetAtPath<TextureContent>(path);
                }
                return _iconContents;
            }
        }
    }
}
