using System.Drawing;
using System.Windows.Forms;

namespace ANTIBigBoss_MGS_Mod_Manager
{

    internal class GuiManager
    {
        #region Form Location Saving Functions

        private static Point lastFormLocation;

        public static void LogFormLocation(Form form, string formName)
        {
            Point location = form.Location;
            string message = $"Switching to {formName}. Current form location: X = {location.X}, Y = {location.Y}\n";
            LoggingManager.Instance.Log(message);
        }

        public static void UpdateLastFormLocation(Point location)
        {
            lastFormLocation = location;
        }

        public static Point GetLastFormLocation()
        {
            return lastFormLocation;
        }

        #endregion
    }
}
