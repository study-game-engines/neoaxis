#if !NO_UI_WEB_BROWSER
namespace Internal.Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;
    using Internal.Xilium.CefGlue.Interop;

    public static unsafe class CefRuntime
    {
        private static readonly CefRuntimePlatform _platform;

        private static bool _loaded;
        private static bool _initialized;

        static CefRuntime()
        {
            _platform = DetectPlatform();
        }

        #region Platform Detection
        private static CefRuntimePlatform DetectPlatform()
        {
            var platformId = Environment.OSVersion.Platform;

            if (platformId == PlatformID.MacOSX)
                return CefRuntimePlatform.MacOSX;

            int p = (int)platformId;
            if ((p == 4) || (p == 128))
                return IsRunningOnMac() ? CefRuntimePlatform.MacOSX : CefRuntimePlatform.Linux;

            return CefRuntimePlatform.Windows;
        }

        //From Managed.Windows.Forms/XplatUI
        private static bool IsRunningOnMac()
        {
            IntPtr buf = IntPtr.Zero;
            try
            {
                buf = Marshal.AllocHGlobal(8192);
                // This is a hacktastic way of getting sysname from uname ()
                if (uname(buf) == 0)
                {
                    string os = Marshal.PtrToStringAnsi(buf);
                    if (os == "Darwin")
                        return true;
                }
            }
            catch { }
            finally
            {
                if (buf != IntPtr.Zero)
                    Marshal.FreeHGlobal(buf);
            }

            return false;
        }

        [DllImport("libc")]
        private static extern int uname(IntPtr buf);

        public static CefRuntimePlatform Platform
        {
            get { return _platform; }
        }
        #endregion

        /// <summary>
        /// Loads CEF runtime.
        /// </summary>
        /// <exception cref="DllNotFoundException"></exception>
        /// <exception cref="CefVersionMismatchException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static void Load()
        {
            Load(null);
        }

        /// <summary>
        /// Loads CEF runtime from specified path.
        /// </summary>
        /// <exception cref="DllNotFoundException"></exception>
        /// <exception cref="CefVersionMismatchException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static void Load(string path)
        {
            if (_loaded) return;

            if (!string.IsNullOrEmpty(path))
            {
                if (Platform == CefRuntimePlatform.Windows)
                    LoadLibraryWindows(path);
                else
                    throw new PlatformNotSupportedException("CEF Runtime can't be initialized on altered path on this platform. Use CefRuntime.Load() instead.");
            }

			//!!!!betauser
            //CheckVersion();

            _loaded = true;
        }

        private static void LoadLibraryWindows(string path)
        {
            Internal.Xilium.CefGlue.Platform.Windows.NativeMethods.LoadLibraryEx(
                System.IO.Path.Combine(path, "libcef.dll"),
                IntPtr.Zero,
                Internal.Xilium.CefGlue.Platform.Windows.LoadLibraryFlags.LOAD_WITH_ALTERED_SEARCH_PATH
                );
        }

        #region cef_version

        private static void CheckVersion()
        {
            CheckVersionByApiHash();
        }

        private static void CheckVersionByApiHash()
        {
            // get CEF_API_HASH_PLATFORM
            string actual;
            try
            {
                var n_actual = libcef.api_hash(0);
                actual = n_actual != null ? new string(n_actual) : null;
            }
            catch (EntryPointNotFoundException ex)
            {
                throw new NotSupportedException("cef_api_hash call is not supported.", ex);
            }
            if (string.IsNullOrEmpty(actual)) throw new NotSupportedException();

            string expected;
            switch (CefRuntime.Platform)
            {
                case CefRuntimePlatform.Windows: expected = libcef.CEF_API_HASH_PLATFORM_WIN; break;
                case CefRuntimePlatform.MacOSX: expected = libcef.CEF_API_HASH_PLATFORM_MACOSX; break;
                case CefRuntimePlatform.Linux: expected = libcef.CEF_API_HASH_PLATFORM_LINUX; break;
                default: throw new PlatformNotSupportedException();
            }

            if (string.Compare(actual, expected, StringComparison.OrdinalIgnoreCase) != 0)
            {
                var expectedVersion = libcef.CEF_VERSION;
                throw ExceptionBuilder.RuntimeVersionApiHashMismatch(actual, expected, expectedVersion);
            }
        }

        #endregion

        #region cef_app

        /// <summary>
        /// This function should be called from the application entry point function to
        /// execute a secondary process. It can be used to run secondary processes from
        /// the browser client executable (default behavior) or from a separate
        /// executable specified by the CefSettings.browser_subprocess_path value. If
        /// called for the browser process (identified by no "type" command-line value)
        /// it will return immediately with a value of -1. If called for a recognized
        /// secondary process it will block until the process should exit and then return
        /// the process exit code. The |application| parameter may be empty. The
        /// |windows_sandbox_info| parameter is only used on Windows and may be NULL (see
        /// cef_sandbox_win.h for details).
        /// </summary>
        public static int ExecuteProcess(CefMainArgs args, CefApp application, IntPtr windowsSandboxInfo)
        {
            LoadIfNeed();

            var n_args = args.ToNative();
            var n_app = application != null ? application.ToNative() : null;

            try
            {
                return libcef.execute_process(n_args, n_app, (void*)windowsSandboxInfo);
            }
            finally
            {
                CefMainArgs.Free(n_args);
            }
        }

        [Obsolete]
        public static int ExecuteProcess(CefMainArgs args, CefApp application)
        {
            return ExecuteProcess(args, application, IntPtr.Zero);
        }


        /// <summary>
        /// This function should be called on the main application thread to initialize
        /// the CEF browser process. The |application| parameter may be empty. A return
        /// value of true indicates that it succeeded and false indicates that it failed.
        /// The |windows_sandbox_info| parameter is only used on Windows and may be NULL
        /// (see cef_sandbox_win.h for details).
        /// </summary>
        public static void Initialize(CefMainArgs args, CefSettings settings, CefApp application, IntPtr windowsSandboxInfo)
        {
            LoadIfNeed();

            if (args == null) throw new ArgumentNullException("args");
            if (settings == null) throw new ArgumentNullException("settings");

            if (_initialized) throw ExceptionBuilder.CefRuntimeAlreadyInitialized();

            var n_main_args = args.ToNative();
            var n_settings = settings.ToNative();
            var n_app = application != null ? application.ToNative() : null;

            try
            {
                if (libcef.initialize(n_main_args, n_settings, n_app, (void*)windowsSandboxInfo) != 0)
                {
                    _initialized = true;
                }
                else
                {
                    throw ExceptionBuilder.CefRuntimeFailedToInitialize();
                }
            }
            finally
            {
                CefMainArgs.Free(n_main_args);
                CefSettings.Free(n_settings);
            }
        }

        [Obsolete]
        public static void Initialize(CefMainArgs args, CefSettings settings, CefApp application)
        {
            Initialize(args, settings, application, IntPtr.Zero);
        }


        /// <summary>
        /// This function should be called on the main application thread to shut down
        /// the CEF browser process before the application exits.
        /// </summary>
        public static void Shutdown()
        {
            if (!_initialized) return;

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

            libcef.shutdown();
        }

        /// <summary>
        /// Perform a single iteration of CEF message loop processing. This function is
        /// provided for cases where the CEF message loop must be integrated into an
        /// existing application message loop. Use of this function is not recommended
        /// for most users; use either the CefRunMessageLoop() function or
        /// CefSettings.multi_threaded_message_loop if possible. When using this function
        /// care must be taken to balance performance against excessive CPU usage. It is
        /// recommended to enable the CefSettings.external_message_pump option when using
        /// this function so that CefBrowserProcessHandler::OnScheduleMessagePumpWork()
        /// callbacks can facilitate the scheduling process. This function should only be
        /// called on the main application thread and only if CefInitialize() is called
        /// with a CefSettings.multi_threaded_message_loop value of false. This function
        /// will not block.
        /// </summary>
        public static void DoMessageLoopWork()
        {
            libcef.do_message_loop_work();
        }

        /// <summary>
        /// Run the CEF message loop. Use this function instead of an application-
        /// provided message loop to get the best balance between performance and CPU
        /// usage. This function should only be called on the main application thread and
        /// only if CefInitialize() is called with a
        /// CefSettings.multi_threaded_message_loop value of false. This function will
        /// block until a quit message is received by the system.
        /// </summary>
        public static void RunMessageLoop()
        {
            libcef.run_message_loop();
        }

        /// <summary>
        /// Quit the CEF message loop that was started by calling CefRunMessageLoop().
        /// This function should only be called on the main application thread and only
        /// if CefRunMessageLoop() was used.
        /// </summary>
        public static void QuitMessageLoop()
        {
            libcef.quit_message_loop();
        }

        /// <summary>
        /// Set to true before calling Windows APIs like TrackPopupMenu that enter a
        /// modal message loop. Set to false after exiting the modal message loop.
        /// </summary>
        public static void SetOSModalLoop(bool osModalLoop)
        {
            libcef.set_osmodal_loop(osModalLoop ? 1 : 0);
        }

        /// <summary>
        /// Call during process startup to enable High-DPI support on Windows 7 or newer.
        /// Older versions of Windows should be left DPI-unaware because they do not
        /// support DirectWrite and GDI fonts are kerned very badly.
        /// </summary>
        public static void EnableHighDpiSupport()
        {
            libcef.enable_highdpi_support();
        }

        #endregion

        #region cef_task

        /// <summary>
        /// CEF maintains multiple internal threads that are used for handling different
        /// types of tasks in different processes. See the cef_thread_id_t definitions in
        /// cef_types.h for more information. This function will return true if called on
        /// the specified thread. It is an error to request a thread from the wrong
        /// process.
        /// </summary>
        public static bool CurrentlyOn(CefThreadId threadId)
        {
            return libcef.currently_on(threadId) != 0;
        }

        /// <summary>
        /// Post a task for execution on the specified thread. This function may be
        /// called on any thread. It is an error to request a thread from the wrong
        /// process.
        /// </summary>
        public static bool PostTask(CefThreadId threadId, CefTask task)
        {
            if (task == null) throw new ArgumentNullException("task");

            return libcef.post_task(threadId, task.ToNative()) != 0;
        }

        /// <summary>
        /// Post a task for delayed execution on the specified thread. This function may
        /// be called on any thread. It is an error to request a thread from the wrong
        /// process.
        /// </summary>
        public static bool PostTask(CefThreadId threadId, CefTask task, long delay)
        {
            if (task == null) throw new ArgumentNullException("task");

            return libcef.post_delayed_task(threadId, task.ToNative(), delay) != 0;
        }
        #endregion

        #region cef_geolocation

        /// <summary>
        /// Request a one-time geolocation update. This function bypasses any user
        /// permission checks so should only be used by code that is allowed to access
        /// location information.
        /// </summary>
        public static bool GetGeolocation(CefGetGeolocationCallback callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");

            return libcef.get_geolocation(callback.ToNative()) != 0;
        }

        #endregion

        #region cef_origin_whitelist

        /// <summary>
        /// Add an entry to the cross-origin access whitelist.
        ///
        /// The same-origin policy restricts how scripts hosted from different origins
        /// (scheme + domain + port) can communicate. By default, scripts can only access
        /// resources with the same origin. Scripts hosted on the HTTP and HTTPS schemes
        /// (but no other schemes) can use the "Access-Control-Allow-Origin" header to
        /// allow cross-origin requests. For example, https://source.example.com can make
        /// XMLHttpRequest requests on http://target.example.com if the
        /// http://target.example.com request returns an "Access-Control-Allow-Origin:
        /// https://source.example.com" response header.
        ///
        /// Scripts in separate frames or iframes and hosted from the same protocol and
        /// domain suffix can execute cross-origin JavaScript if both pages set the
        /// document.domain value to the same domain suffix. For example,
        /// scheme://foo.example.com and scheme://bar.example.com can communicate using
        /// JavaScript if both domains set document.domain="example.com".
        ///
        /// This method is used to allow access to origins that would otherwise violate
        /// the same-origin policy. Scripts hosted underneath the fully qualified
        /// |source_origin| URL (like http://www.example.com) will be allowed access to
        /// all resources hosted on the specified |target_protocol| and |target_domain|.
        /// If |target_domain| is non-empty and |allow_target_subdomains| if false only
        /// exact domain matches will be allowed. If |target_domain| contains a top-
        /// level domain component (like "example.com") and |allow_target_subdomains| is
        /// true sub-domain matches will be allowed. If |target_domain| is empty and
        /// |allow_target_subdomains| if true all domains and IP addresses will be
        /// allowed.
        ///
        /// This method cannot be used to bypass the restrictions on local or display
        /// isolated schemes. See the comments on CefRegisterCustomScheme for more
        /// information.
        ///
        /// This function may be called on any thread. Returns false if |source_origin|
        /// is invalid or the whitelist cannot be accessed.
        /// </summary>
        public static bool AddCrossOriginWhitelistEntry(string sourceOrigin, string targetProtocol, string targetDomain, bool allowTargetSubdomains)
        {
            if (string.IsNullOrEmpty("sourceOrigin")) throw new ArgumentNullException("sourceOrigin");
            if (string.IsNullOrEmpty("targetProtocol")) throw new ArgumentNullException("targetProtocol");

            fixed (char* sourceOrigin_ptr = sourceOrigin)
            fixed (char* targetProtocol_ptr = targetProtocol)
            fixed (char* targetDomain_ptr = targetDomain)
            {
                var n_sourceOrigin = new cef_string_t(sourceOrigin_ptr, sourceOrigin.Length);
                var n_targetProtocol = new cef_string_t(targetProtocol_ptr, targetProtocol.Length);
                var n_targetDomain = new cef_string_t(targetDomain_ptr, targetDomain != null ? targetDomain.Length : 0);

                return libcef.add_cross_origin_whitelist_entry(
                    &n_sourceOrigin,
                    &n_targetProtocol,
                    &n_targetDomain,
                    allowTargetSubdomains ? 1 : 0
                    ) != 0;
            }
        }

        /// <summary>
        /// Remove an entry from the cross-origin access whitelist. Returns false if
        /// |source_origin| is invalid or the whitelist cannot be accessed.
        /// </summary>
        public static bool RemoveCrossOriginWhitelistEntry(string sourceOrigin, string targetProtocol, string targetDomain, bool allowTargetSubdomains)
        {
            if (string.IsNullOrEmpty("sourceOrigin")) throw new ArgumentNullException("sourceOrigin");
            if (string.IsNullOrEmpty("targetProtocol")) throw new ArgumentNullException("targetProtocol");

            fixed (char* sourceOrigin_ptr = sourceOrigin)
            fixed (char* targetProtocol_ptr = targetProtocol)
            fixed (char* targetDomain_ptr = targetDomain)
            {
                var n_sourceOrigin = new cef_string_t(sourceOrigin_ptr, sourceOrigin.Length);
                var n_targetProtocol = new cef_string_t(targetProtocol_ptr, targetProtocol.Length);
                var n_targetDomain = new cef_string_t(targetDomain_ptr, targetDomain != null ? targetDomain.Length : 0);

                return libcef.remove_cross_origin_whitelist_entry(
                    &n_sourceOrigin,
                    &n_targetProtocol,
                    &n_targetDomain,
                    allowTargetSubdomains ? 1 : 0
                    ) != 0;
            }
        }

        /// <summary>
        /// Remove all entries from the cross-origin access whitelist. Returns false if
        /// the whitelist cannot be accessed.
        /// </summary>
        public static bool ClearCrossOriginWhitelist()
        {
            return libcef.clear_cross_origin_whitelist() != 0;
        }

        #endregion

        #region cef_scheme

        /// <summary>
        /// Register a scheme handler factory for the specified |scheme_name| and
        /// optional |domain_name|. An empty |domain_name| value for a standard scheme
        /// will cause the factory to match all domain names. The |domain_name| value
        /// will be ignored for non-standard schemes. If |scheme_name| is a built-in
        /// scheme and no handler is returned by |factory| then the built-in scheme
        /// handler factory will be called. If |scheme_name| is a custom scheme then
        /// also implement the CefApp::OnRegisterCustomSchemes() method in all processes.
        /// This function may be called multiple times to change or remove the factory
        /// that matches the specified |scheme_name| and optional |domain_name|.
        /// Returns false if an error occurs. This function may be called on any thread
        /// in the browser process.
        /// </summary>
        public static bool RegisterSchemeHandlerFactory(string schemeName, string domainName, CefSchemeHandlerFactory factory)
        {
            if (string.IsNullOrEmpty(schemeName)) throw new ArgumentNullException("schemeName");
            if (factory == null) throw new ArgumentNullException("factory");

            fixed (char* schemeName_str = schemeName)
            fixed (char* domainName_str = domainName)
            {
                var n_schemeName = new cef_string_t(schemeName_str, schemeName.Length);
                var n_domainName = new cef_string_t(domainName_str, domainName != null ? domainName.Length : 0);

                return libcef.register_scheme_handler_factory(&n_schemeName, &n_domainName, factory.ToNative()) != 0;
            }
        }

        /// <summary>
        /// Clear all registered scheme handler factories. Returns false on error. This
        /// function may be called on any thread in the browser process.
        /// </summary>
        public static bool ClearSchemeHandlerFactories()
        {
            return libcef.clear_scheme_handler_factories() != 0;
        }

        #endregion

        #region cef_trace

        // TODO: CefBeginTracing
        //[DllImport(libcef.DllName, EntryPoint = "cef_begin_tracing", CallingConvention = libcef.CEF_CALL)]
        //public static extern int begin_tracing(cef_trace_client_t* client, cef_string_t* categories);

        // TODO: CefGetTraceBufferPercentFullAsync
        //[DllImport(libcef.DllName, EntryPoint = "cef_get_trace_buffer_percent_full_async", CallingConvention = libcef.CEF_CALL)]
        //public static extern int get_trace_buffer_percent_full_async();

        // TODO: CefEndTracingAsync
        //[DllImport(libcef.DllName, EntryPoint = "cef_end_tracing_async", CallingConvention = libcef.CEF_CALL)]
        //public static extern int end_tracing_async();

        // TODO: functions from cef_trace_event.h (not generated automatically)

        #endregion

        #region cef_parser

        // Methods from cef_parser.h.

        /// <summary>
        /// Parse the specified |url| into its component parts.
        /// Returns false if the URL is empty or invalid.
        /// </summary>
        public static bool ParseUrl(string url, out CefUrlParts parts)
        {
            fixed (char* url_str = url)
            {
                var n_url = new cef_string_t(url_str, url != null ? url.Length : 0);
                var n_parts = new cef_urlparts_t();

                var result = libcef.parse_url(&n_url, &n_parts) != 0;

                parts = result ? CefUrlParts.FromNative(&n_parts) : null;
                cef_urlparts_t.Clear(&n_parts);
                return result;
            }
        }

        public static bool CreateUrl(CefUrlParts parts, out string url)
        {
            if (parts == null) throw new ArgumentNullException("parts");

            var n_parts = parts.ToNative();
            var n_url = new cef_string_t();

            var result = libcef.create_url(&n_parts, &n_url) != 0;

            url = result ? cef_string_t.ToString(&n_url) : null;

            cef_urlparts_t.Clear(&n_parts);
            libcef.string_clear(&n_url);

            return result;
        }

        /// <summary>
        /// Returns the mime type for the specified file extension or an empty string if
        /// unknown.
        /// </summary>
        public static string GetMimeType(string extension)
        {
            fixed (char* extension_str = extension)
            {
                var n_extension = new cef_string_t(extension_str, extension != null ? extension.Length : 0);

                var n_result = libcef.get_mime_type(&n_extension);
                return cef_string_userfree.ToString(n_result);
            }
        }

        /// <summary>
        /// Get the extensions associated with the given mime type. This should be passed
        /// in lower case. There could be multiple extensions for a given mime type, like
        /// "html,htm" for "text/html", or "txt,text,html,..." for "text/*". Any existing
        /// elements in the provided vector will not be erased.
        /// </summary>
        public static string[] GetExtensionsForMimeType(string mimeType)
        {
            fixed (char* mimeType_str = mimeType)
            {
                var n_mimeType = new cef_string_t(mimeType_str, mimeType != null ? mimeType.Length : 0);

                var n_list = libcef.string_list_alloc();
                libcef.get_extensions_for_mime_type(&n_mimeType, n_list);
                var result = cef_string_list.ToArray(n_list);
                libcef.string_list_free(n_list);
                return result;
            }
        }

        /// <summary>
        /// Encodes |data| as a base64 string.
        /// </summary>
        public static unsafe string Base64Encode(void* data, int size)
        {
            var n_result = libcef.base64encode(data, (UIntPtr)size);
            return cef_string_userfree.ToString(n_result);
        }

        public static string Base64Encode(byte[] bytes, int offset, int length)
        {
            // TODO: check bounds

            fixed (byte* bytes_ptr = &bytes[offset])
            {
                return Base64Encode(bytes_ptr, length);
            }
        }

        public static string Base64Encode(byte[] bytes)
        {
            return Base64Encode(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Decodes the base64 encoded string |data|. The returned value will be NULL if
        /// the decoding fails.
        /// </summary>
        public static CefBinaryValue Base64Decode(string data)
        {
            fixed (char* data_str = data)
            {
                var n_data = new cef_string_t(data_str, data != null ? data.Length : 0);
                return CefBinaryValue.FromNative(libcef.base64decode(&n_data));
            }
        }


        /// <summary>
        /// Escapes characters in |text| which are unsuitable for use as a query
        /// parameter value. Everything except alphanumerics and -_.!~*'() will be
        /// converted to "%XX". If |use_plus| is true spaces will change to "+". The
        /// result is basically the same as encodeURIComponent in Javacript.
        /// </summary>
        public static string UriEncode(string text, bool usePlus)
        {
            fixed (char* text_str = text)
            {
                var n_text = new cef_string_t(text_str, text != null ? text.Length : 0);

                var n_result = libcef.uriencode(&n_text, usePlus ? 1 : 0);
                return cef_string_userfree.ToString(n_result);
            }
        }

        /// <summary>
        /// Unescapes |text| and returns the result. Unescaping consists of looking for
        /// the exact pattern "%XX" where each X is a hex digit and converting to the
        /// character with the numerical value of those digits (e.g. "i%20=%203%3b"
        /// unescapes to "i = 3;"). If |convert_to_utf8| is true this function will
        /// attempt to interpret the initial decoded result as UTF-8. If the result is
        /// convertable into UTF-8 it will be returned as converted. Otherwise the
        /// initial decoded result will be returned.  The |unescape_rule| parameter
        /// supports further customization the decoding process.
        /// </summary>
        public static string UriDecode(string text, bool convertToUtf8, CefUriUnescapeRules unescapeRule)
        {
            fixed (char* text_str = text)
            {
                var n_text = new cef_string_t(text_str, text != null ? text.Length : 0);

                var n_result = libcef.uridecode(&n_text, convertToUtf8 ? 1 : 0, unescapeRule);
                return cef_string_userfree.ToString(n_result);
            }
        }

        /// <summary>
        /// Parses the specified |json_string| and returns a dictionary or list
        /// representation. If JSON parsing fails this method returns NULL.
        /// </summary>
        public static CefValue ParseJson(string value, CefJsonParserOptions options)
        {
            fixed (char* value_str = value)
            {
                var n_value = new cef_string_t(value_str, value != null ? value.Length : 0);
                var n_result = libcef.parse_json(&n_value, options);
                return CefValue.FromNativeOrNull(n_result);
            }
        }

        /// <summary>
        /// Parses the specified |json_string| and returns a dictionary or list
        /// representation. If JSON parsing fails this method returns NULL and populates
        /// |error_code_out| and |error_msg_out| with an error code and a formatted error
        /// message respectively.
        /// </summary>
        public static CefValue ParseJsonAndReturnError(string value, CefJsonParserOptions options, out CefJsonParserError errorCode, out string errorMessage)
        {
            fixed (char* value_str = value)
            {
                var n_value = new cef_string_t(value_str, value != null ? value.Length : 0);

                CefJsonParserError n_error_code;
                cef_string_t n_error_msg;
                var n_result = libcef.parse_jsonand_return_error(&n_value, options, & n_error_code, &n_error_msg);

                var result = CefValue.FromNativeOrNull(n_result);
                errorCode = n_error_code;
                errorMessage = cef_string_userfree.ToString((cef_string_userfree*)&n_error_msg);
                return result;
            }
        }

        /// <summary>
        /// Generates a JSON string from the specified root |node| which should be a
        /// dictionary or list value. Returns an empty string on failure. This method
        /// requires exclusive access to |node| including any underlying data.
        /// </summary>
        public static string WriteJson(CefValue value, CefJsonWriterOptions options)
        {
            if (value == null) throw new ArgumentNullException("value");
            var n_result = libcef.write_json(value.ToNative(), options);
            return cef_string_userfree.ToString(n_result);
        }

        #endregion

        #region cef_v8

        /// <summary>
        /// Register a new V8 extension with the specified JavaScript extension code and
        /// handler. Functions implemented by the handler are prototyped using the
        /// keyword 'native'. The calling of a native function is restricted to the scope
        /// in which the prototype of the native function is defined. This function may
        /// only be called on the render process main thread.
        ///
        /// Example JavaScript extension code:
        /// <code>
        ///   // create the 'example' global object if it doesn't already exist.
        ///   if (!example)
        ///     example = {};
        ///   // create the 'example.test' global object if it doesn't already exist.
        ///   if (!example.test)
        ///     example.test = {};
        ///   (function() {
        ///     // Define the function 'example.test.myfunction'.
        ///     example.test.myfunction = function() {
        ///       // Call CefV8Handler::Execute() with the function name 'MyFunction'
        ///       // and no arguments.
        ///       native function MyFunction();
        ///       return MyFunction();
        ///     };
        ///     // Define the getter function for parameter 'example.test.myparam'.
        ///     example.test.__defineGetter__('myparam', function() {
        ///       // Call CefV8Handler::Execute() with the function name 'GetMyParam'
        ///       // and no arguments.
        ///       native function GetMyParam();
        ///       return GetMyParam();
        ///     });
        ///     // Define the setter function for parameter 'example.test.myparam'.
        ///     example.test.__defineSetter__('myparam', function(b) {
        ///       // Call CefV8Handler::Execute() with the function name 'SetMyParam'
        ///       // and a single argument.
        ///       native function SetMyParam();
        ///       if(b) SetMyParam(b);
        ///     });
        ///
        ///     // Extension definitions can also contain normal JavaScript variables
        ///     // and functions.
        ///     var myint = 0;
        ///     example.test.increment = function() {
        ///       myint += 1;
        ///       return myint;
        ///     };
        ///   })();
        /// </code>
        /// Example usage in the page:
        /// <code>
        ///   // Call the function.
        ///   example.test.myfunction();
        ///   // Set the parameter.
        ///   example.test.myparam = value;
        ///   // Get the parameter.
        ///   value = example.test.myparam;
        ///   // Call another function.
        ///   example.test.increment();
        /// </code>
        /// </summary>
        public static bool RegisterExtension(string extensionName, string javascriptCode, CefV8Handler handler)
        {
            if (string.IsNullOrEmpty(extensionName)) throw new ArgumentNullException("extensionName");
            if (string.IsNullOrEmpty(javascriptCode)) throw new ArgumentNullException("javascriptCode");

            fixed (char* extensionName_str = extensionName)
            fixed (char* javascriptCode_str = javascriptCode)
            {
                var n_extensionName = new cef_string_t(extensionName_str, extensionName.Length);
                var n_javascriptCode = new cef_string_t(javascriptCode_str, javascriptCode.Length);

                return libcef.register_extension(&n_extensionName, &n_javascriptCode, handler != null ? handler.ToNative() : null) != 0;
            }
        }

        #endregion

        #region cef_web_plugin

        // TODO: move web plugins methods to CefRuntime.WebPlugin.Xxx

        /// <summary>
        /// Visit web plugin information. Can be called on any thread in the browser
        /// process.
        /// </summary>
        public static void VisitWebPluginInfo(CefWebPluginInfoVisitor visitor)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");

            libcef.visit_web_plugin_info(visitor.ToNative());
        }

        /// <summary>
        /// Cause the plugin list to refresh the next time it is accessed regardless
        /// of whether it has already been loaded. Can be called on any thread in the
        /// browser process.
        /// </summary>
        public static void RefreshWebPlugins()
        {
            libcef.refresh_web_plugins();
        }

        /// <summary>
        /// Unregister an internal plugin. This may be undone the next time
        /// CefRefreshWebPlugins() is called. Can be called on any thread in the browser
        /// process.
        /// </summary>
        public static void UnregisterInternalWebPlugin(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");

            fixed (char* path_str = path)
            {
                var n_path = new cef_string_t(path_str, path.Length);
                libcef.unregister_internal_web_plugin(&n_path);
            }
        }

        /// <summary>
        /// Register a plugin crash. Can be called on any thread in the browser process
        /// but will be executed on the IO thread.
        /// </summary>
        public static void RegisterWebPluginCrash(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");

            fixed (char* path_str = path)
            {
                var n_path = new cef_string_t(path_str, path.Length);
                libcef.register_web_plugin_crash(&n_path);
            }
        }

        /// <summary>
        /// Query if a plugin is unstable. Can be called on any thread in the browser
        /// process.
        /// </summary>
        public static void IsWebPluginUnstable(string path, CefWebPluginUnstableCallback callback)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            if (callback == null) throw new ArgumentNullException("callback");

            fixed (char* path_str = path)
            {
                var n_path = new cef_string_t(path_str, path.Length);
                libcef.is_web_plugin_unstable(&n_path, callback.ToNative());
            }
        }

        /// <summary>
        /// Register the Widevine CDM plugin.
        ///
        /// The client application is responsible for downloading an appropriate
        /// platform-specific CDM binary distribution from Google, extracting the
        /// contents, and building the required directory structure on the local machine.
        /// The CefBrowserHost::StartDownload method and CefZipArchive class can be used
        /// to implement this functionality in CEF. Contact Google via
        /// https://www.widevine.com/contact.html for details on CDM download.
        ///
        /// |path| is a directory that must contain the following files:
        ///   1. manifest.json file from the CDM binary distribution (see below).
        ///   2. widevinecdm file from the CDM binary distribution (e.g.
        ///      widevinecdm.dll on on Windows, libwidevinecdm.dylib on OS X,
        ///      libwidevinecdm.so on Linux).
        ///   3. widevidecdmadapter file from the CEF binary distribution (e.g.
        ///      widevinecdmadapter.dll on Windows, widevinecdmadapter.plugin on OS X,
        ///      libwidevinecdmadapter.so on Linux).
        ///
        /// If any of these files are missing or if the manifest file has incorrect
        /// contents the registration will fail and |callback| will receive a |result|
        /// value of CEF_CDM_REGISTRATION_ERROR_INCORRECT_CONTENTS.
        ///
        /// The manifest.json file must contain the following keys:
        ///   A. "os": Supported OS (e.g. "mac", "win" or "linux").
        ///   B. "arch": Supported architecture (e.g. "ia32" or "x64").
        ///   C. "x-cdm-module-versions": Module API version (e.g. "4").
        ///   D. "x-cdm-interface-versions": Interface API version (e.g. "8").
        ///   E. "x-cdm-host-versions": Host API version (e.g. "8").
        ///   F. "version": CDM version (e.g. "1.4.8.903").
        ///   G. "x-cdm-codecs": List of supported codecs (e.g. "vp8,vp9.0,avc1").
        ///
        /// A through E are used to verify compatibility with the current Chromium
        /// version. If the CDM is not compatible the registration will fail and
        /// |callback| will receive a |result| value of
        /// CEF_CDM_REGISTRATION_ERROR_INCOMPATIBLE.
        ///
        /// |callback| will be executed asynchronously once registration is complete.
        ///
        /// On Linux this function must be called before CefInitialize() and the
        /// registration cannot be changed during runtime. If registration is not
        /// supported at the time that CefRegisterWidevineCdm() is called then |callback|
        /// will receive a |result| value of CEF_CDM_REGISTRATION_ERROR_NOT_SUPPORTED.
        /// </summary>
        public static void CefRegisterWidevineCdm(string path, CefRegisterCdmCallback callback = null)
        {
            fixed (char* path_str = path)
            {
                var n_path = new cef_string_t(path_str, path.Length);
                libcef.register_widevine_cdm(&n_path,
                    callback != null ? callback.ToNative() : null);
            }
        }

        #endregion

        #region cef_path_util

        /// <summary>
        /// Retrieve the path associated with the specified |key|. Returns true on
        /// success. Can be called on any thread in the browser process.
        /// </summary>
        public static string GetPath(CefPathKey pathKey)
        {
            var n_value = new cef_string_t();
            var success = libcef.get_path(pathKey, &n_value) != 0;
            var value = cef_string_t.ToString(&n_value);
            libcef.string_clear(&n_value);
            if (!success)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "Failed to get path for key {0}.", pathKey)
                    );
            }
            return value;
        }

        #endregion

        #region cef_process_util

        /// <summary>
        /// Launches the process specified via |command_line|. Returns true upon
        /// success. Must be called on the browser process TID_PROCESS_LAUNCHER thread.
        ///
        /// Unix-specific notes:
        /// - All file descriptors open in the parent process will be closed in the
        ///   child process except for stdin, stdout, and stderr.
        /// - If the first argument on the command line does not contain a slash,
        ///   PATH will be searched. (See man execvp.)
        /// </summary>
        public static bool LaunchProcess(CefCommandLine commandLine)
        {
            if (commandLine == null) throw new ArgumentNullException("commandLine");

            return libcef.launch_process(commandLine.ToNative()) != 0;
        }

        #endregion

        #region cef_sandbox_win
        // TODO: investigate using of sandbox on windows and .net
        #endregion

        private static void LoadIfNeed()
        {
            if (!_loaded) Load();
        }

        #region linux

        /////
        //// Return the singleton X11 display shared with Chromium. The display is not
        //// thread-safe and must only be accessed on the browser process UI thread.
        /////
        //CEF_EXPORT XDisplay* cef_get_xdisplay();

        #endregion
    }
}

#endif