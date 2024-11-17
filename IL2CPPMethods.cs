using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AmongUsFishing {
    internal static class IL2CPPMethods {

        static LoadImageDelegate LoadImageFunc;
        delegate bool LoadImageDelegate(IntPtr texture, IntPtr data);
        public static bool LoadImage(this Texture texture, byte[] data) {
            return LoadImageFunc(texture.Pointer, ((Il2CppStructArray<byte>)data).Pointer);
        }
        public static void Init() {
            LoadImageFunc = Il2CppInterop.Runtime.IL2CPP.ResolveICall<LoadImageDelegate>("UnityEngine.ImageConversion::LoadImage");
        }


    }
}
