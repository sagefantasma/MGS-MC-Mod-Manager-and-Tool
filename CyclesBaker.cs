using System;
using System.Runtime.InteropServices;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public static class CyclesBaker
    {
        [DllImport("CyclesBake.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int CycleBake(string blendFilePath, string outputImagePath);

        public static bool Bake(string blendFilePath, string outputImagePath)
        {
            int result = CycleBake(blendFilePath, outputImagePath);
            return result == 0;
        }
    }
}
