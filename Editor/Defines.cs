#if UNITY_EDITOR

namespace Nekobox.ImportSearcher
{
    public static class Defines
    {
        public const string PACKAGE_PATH = "Packages/com.github.nekobox-dev.import-searcher";
        public const string PACKAGE_NAME = "Import Searcher";
        public const string MENU_PATH = "Tools/Import Searcher";
        public const string SAVE_FOLDER_PATH = PACKAGE_PATH + "/Editor/SaveData";
        public const string STYLESHEET_FOLDER_PATH = PACKAGE_PATH + "/Editor/StyleSheets";
        public const string LOG_PREFIX = "[" + PACKAGE_NAME + "] ";
    }
}

#endif // UNITY_EDITOR
