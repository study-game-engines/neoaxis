#if !NO_UI_WEB_BROWSER
//
// This file manually written from cef/include/internal/cef_string_map.h
//
namespace Internal.Xilium.CefGlue.Interop
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    internal static unsafe partial class libcef
    {
        // Allocate a new string map.
        [DllImport(DllName, EntryPoint = "cef_string_map_alloc", CallingConvention = CEF_CALL)]
        public static extern cef_string_map* string_map_alloc();

        // Return the number of elements in the string map.
        [DllImport(DllName, EntryPoint = "cef_string_map_size", CallingConvention = CEF_CALL)]
        public static extern int string_map_size(cef_string_map* map);

        // Return the value assigned to the specified key.
        [DllImport(DllName, EntryPoint = "cef_string_map_find", CallingConvention = CEF_CALL)]
        public static extern int string_map_find(cef_string_map* map, cef_string_t* key, cef_string_t* value);

        // Return the key at the specified zero-based string map index.
        [DllImport(DllName, EntryPoint = "cef_string_map_key", CallingConvention = CEF_CALL)]
        public static extern int string_map_key(cef_string_map* map, int index, cef_string_t* key);

        // Return the value at the specified zero-based string map index.
        [DllImport(DllName, EntryPoint = "cef_string_map_value", CallingConvention = CEF_CALL)]
        public static extern int string_map_value(cef_string_map* map, int index, cef_string_t* value);

        // Append a new key/value pair at the end of the string map.
        [DllImport(DllName, EntryPoint = "cef_string_map_append", CallingConvention = CEF_CALL)]
        public static extern int string_map_append(cef_string_map* map, cef_string_t* key, cef_string_t* value);

        // Clear the string map.
        [DllImport(DllName, EntryPoint = "cef_string_map_clear", CallingConvention = CEF_CALL)]
        public static extern void string_map_clear(cef_string_map* map);

        // Free the string map.
        [DllImport(DllName, EntryPoint = "cef_string_map_free", CallingConvention = CEF_CALL)]
        public static extern void string_map_free(cef_string_map* map);
    }
}

#endif