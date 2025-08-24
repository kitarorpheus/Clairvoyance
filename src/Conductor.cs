namespace Clairvoyance
{
    internal class Conductor
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                WinOperator win = new();
                ApplicationConfiguration.Initialize();
                Application.Run(new Displayer(args[0], win));
            }
            catch (Exception)
            {
                Environment.Exit(1);
            }
        }
    }
}
