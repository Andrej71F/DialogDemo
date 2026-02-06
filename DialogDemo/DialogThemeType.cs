namespace DialogDemo
{
    public enum DialogThemeType
    {
        Sand,

        Hell,

        Blau,

        Dunkel
    }

    public static class DialogThemeTypeExtensions
    {
        #region Public Properties

        public static Array Values => Enum.GetValues(typeof(DialogThemeType));

        #endregion Public Properties

        #region Public Methods

        public static DialogTheme Resolve(this DialogThemeType t)
        {
            return t switch
            {
                DialogThemeType.Sand => DialogTheme.Sand(),
                DialogThemeType.Hell => DialogTheme.Hell(),
                DialogThemeType.Blau => DialogTheme.Blau(),
                DialogThemeType.Dunkel => DialogTheme.Dunkel(),
                _ => DialogTheme.Sand()
            };
        }

        #endregion Public Methods
    }
}