// Themes.cs
using System;
using System.Windows.Media;

namespace DialogDemo
{
    public enum DialogButtonResult
    {
        None = 0,

        Option1 = 1,

        Option2 = 2,

        Option3 = 3,

        Option4 = 4,

        Timeout = 100,

        DialogFailed = 101
    }

    public enum DialogIcon
    {
        None = 0,

        Info,

        Warning,

        Error,

        Question
    }

    public enum DialogModalMode
    {
        None = 0,

        BlockClientWindow
    }

    // ---------------------------------------------------------
    // NEW: DialogSize with CustomDef
    // ---------------------------------------------------------
    public enum DialogSize
    {
        Small = 0,

        Medium = 1,

        Large = 2,

        CustomDef = 100
    }

    public static class DialogLayoutDefinition
    {
        #region Public Fields

        // Row proportions
        public const double TitleStar = 0.10;

        public const double MainStar = 0.40;

        public const double SubStar = 0.25;

        public const double ProgressStar = 0.05;

        public const double ButtonsStar = 0.20;

        // When SubMessage is empty
        public const double MainStarWhenNoSub = MainStar + SubStar; // 0.65

        // TitleBar columns
        public const double TitleIconStar = 0.20;

        public const double TitleTextStar = 0.80;

        // Buttons row columns
        public const double ButtonsLeftStar = 0.60;

        public const double ButtonsRightStar = 0.40;

        #endregion Public Fields
    }

    public sealed class DialogClientTarget
    {
        #region Public Fields

        public static readonly string[] DefaultVb6WindowClasses =
        {
            "ThunderRT6FormDC",
            "ThunderRT6Main",
            "ThunderRT6MDIForm"
        };

        #endregion Public Fields

        #region Public Properties

        public IntPtr WindowHandle { get; set; }

        // Always an array. Never null.
        public string[] WindowClassNames { get; set; } = Array.Empty<string>();

        public string ProcessName { get; set; }

        #endregion Public Properties

        #region Public Methods

        public static DialogClientTarget CreateDefaultVb6Target()
        {
            return new DialogClientTarget
            {
                WindowClassNames = DefaultVb6WindowClasses,
                ProcessName = null,
                WindowHandle = IntPtr.Zero
            };
        }

        #endregion Public Methods
    }

    public sealed class DialogButtonConfig
    {
        #region Public Properties

        public string Text { get; set; }
        public string AutomationId { get; set; }

        public System.Windows.Input.Key? HotKey { get; set; }

        public bool IsDefault { get; set; }
        public bool IsCancel { get; set; }

        #endregion Public Properties
    }

    public sealed class DialogTheme
    {
        #region Public Properties

        public Brush TitleForeground { get; set; }
        public Brush TitleBackground { get; set; }
        public double TitleFontSize { get; set; }
        public string TitleFontFamily { get; set; }

        public Brush MainForeground { get; set; }
        public Brush MainBackground { get; set; }
        public double MainFontSize { get; set; }
        public string MainFontFamily { get; set; }

        public Brush SubForeground { get; set; }
        public Brush SubBackground { get; set; }
        public double SubFontSize { get; set; }
        public string SubFontFamily { get; set; }

        public Brush ButtonBackground { get; set; }
        public Brush ButtonForeground { get; set; }
        public double ButtonFontSize { get; set; }
        public string ButtonFontFamily { get; set; }

        #endregion Public Properties

        // ---------------------------------------------------------
        // THEMES
        // ---------------------------------------------------------

        #region Public Methods

        public static DialogTheme Hell()
        {
            return new DialogTheme
            {
                TitleForeground = Brushes.White,
                TitleBackground = Brushes.SteelBlue,
                TitleFontSize = 16,
                TitleFontFamily = "Segoe UI",

                MainForeground = Brushes.Black,
                MainBackground = Brushes.White,
                MainFontSize = 14,
                MainFontFamily = "Segoe UI",

                SubForeground = Brushes.Gray,
                SubBackground = Brushes.White,
                SubFontSize = 12,
                SubFontFamily = "Segoe UI",

                ButtonBackground = Brushes.LightGray,
                ButtonForeground = Brushes.Black,
                ButtonFontSize = 13,
                ButtonFontFamily = "Segoe UI"
            };
        }

        public static DialogTheme Dunkel()
        {
            return new DialogTheme
            {
                TitleForeground = Brushes.White,
                TitleBackground = Brushes.Black,
                TitleFontSize = 16,
                TitleFontFamily = "Segoe UI",

                MainForeground = Brushes.White,
                MainBackground = Brushes.Black,
                MainFontSize = 14,
                MainFontFamily = "Segoe UI",

                SubForeground = Brushes.LightGray,
                SubBackground = Brushes.Black,
                SubFontSize = 12,
                SubFontFamily = "Segoe UI",

                ButtonBackground = Brushes.DimGray,
                ButtonForeground = Brushes.White,
                ButtonFontSize = 13,
                ButtonFontFamily = "Segoe UI"
            };
        }

        public static DialogTheme Blau()
        {
            return new DialogTheme
            {
                TitleForeground = Brushes.White,
                TitleBackground = Brushes.DarkBlue,
                TitleFontSize = 16,
                TitleFontFamily = "Segoe UI",

                MainForeground = Brushes.Navy,
                MainBackground = Brushes.AliceBlue,
                MainFontSize = 14,
                MainFontFamily = "Segoe UI",

                SubForeground = Brushes.SlateGray,
                SubBackground = Brushes.AliceBlue,
                SubFontSize = 12,
                SubFontFamily = "Segoe UI",

                ButtonBackground = Brushes.SteelBlue,
                ButtonForeground = Brushes.White,
                ButtonFontSize = 13,
                ButtonFontFamily = "Segoe UI"
            };
        }

        public static DialogTheme Sand()
        {
            return new DialogTheme
            {
                TitleForeground = Brushes.SaddleBrown,
                TitleBackground = Brushes.BurlyWood,
                TitleFontSize = 16,
                TitleFontFamily = "Segoe UI",

                MainForeground = Brushes.Sienna,
                MainBackground = Brushes.Bisque,
                MainFontSize = 14,
                MainFontFamily = "Segoe UI",

                SubForeground = Brushes.Peru,
                SubBackground = Brushes.Bisque,
                SubFontSize = 12,
                SubFontFamily = "Segoe UI",

                ButtonBackground = Brushes.Tan,
                ButtonForeground = Brushes.SaddleBrown,
                ButtonFontSize = 13,
                ButtonFontFamily = "Segoe UI"
            };
        }

        #endregion Public Methods
    }

    public sealed class DialogOptions
    {
        #region Public Properties

        public string Title { get; set; }
        public string MainMessage { get; set; }
        public string SubMessage { get; set; }

        public DialogIcon Icon { get; set; } = DialogIcon.Info;
        public DialogModalMode ModalMode { get; set; } = DialogModalMode.BlockClientWindow;

        public DialogClientTarget ClientTarget { get; set; }

        public DialogButtonConfig Button1 { get; set; }
        public DialogButtonConfig Button2 { get; set; }
        public DialogButtonConfig Button3 { get; set; }
        public DialogButtonConfig Button4 { get; set; }

        public DialogTheme Theme { get; set; } = DialogTheme.Hell();

        public int AutoCloseSeconds { get; set; } = 0;

        // NEW: size selection logic
        public DialogSize DialogSize { get; set; } = DialogSize.Medium;

        // NEW: explicit override for CustomDef
        public double? Width { get; set; }

        public double? Height { get; set; }

        #endregion Public Properties
    }

    public sealed class DialogResultInfo
    {
        #region Public Properties

        public DialogButtonResult Result { get; set; } = DialogButtonResult.None;
        public bool ClosedByExternalRequest { get; set; }

        #endregion Public Properties
    }
}