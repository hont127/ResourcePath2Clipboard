using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;

namespace Hont
{
    public static class QuickFetchResourcePath
    {
        #region ---Win32 Interface---
        [DllImport("user32.dll")]
        public static extern bool EmptyClipboard();
        [DllImport("user32.dll", SetLastError = true)]
        private extern static IntPtr SetClipboardData(uint format, IntPtr handle);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool OpenClipboard(IntPtr hWndNewOwner);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool CloseClipboard();
        [DllImport("kernel32.dll")]
        static extern IntPtr GlobalLock(IntPtr hMem);
        [DllImport("kernel32.dll")]
        static extern bool GlobalUnlock(IntPtr hMem);

        public const uint CF_UNICODETEXT = 13;
        #endregion

        public static bool CopyToClipboard(uint id, string content)
        {
            if (OpenClipboard(IntPtr.Zero))
            {
                EmptyClipboard();
                var hmem = Marshal.StringToHGlobalUni(content);
                var ptr = GlobalLock(hmem);
                GlobalUnlock(ptr);
                SetClipboardData(id, ptr);
                CloseClipboard();
                return true;
            }
            return false;
        }

        [MenuItem("Tools/Hont/Hot Keys/Quick Fetch ResourcePath #&_r")]
        public static void QuickFetchResourcePathFunc()
        {
            var targetObj = Selection.activeObject;

            if (targetObj != null)
            {
                var resourcePath = AssetDatabase.GetAssetPath(targetObj.GetInstanceID());
                if (resourcePath.Contains("Resources/"))
                {
                    var resourceIndex = resourcePath.IndexOf("Resources/") + "Resources/".Length;
                    resourcePath = resourcePath.Substring(resourceIndex);
                    resourcePath = resourcePath.Substring(0, resourcePath.LastIndexOf('.'));
                }

                CopyToClipboard(CF_UNICODETEXT, resourcePath);
                Debug.Log("Copy Suceess! " + resourcePath);
            }
        }

    }
}
