using ControlzEx.Theming;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using _2pool.Chia;
using ControlzEx.Standard;
using _2pool.NonFunctional;
using Chia.NET.Clients;
using NativeMethods = _2pool.NonFunctional.NativeMethods;


namespace _2pool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    ///
    public partial class App : Application
    {
        static Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");

        [STAThread]
        protected override void OnStartup(StartupEventArgs e)
        {
            if(mutex.WaitOne(TimeSpan.Zero, true)) {
                
                mutex.ReleaseMutex();
            } else {
                // send our Win32 message to make the currently running instance
                // jump on top of all the other windows
                NativeMethods.PostMessage(
                    (IntPtr)NativeMethods.HWND_BROADCAST,
                    NativeMethods.WM_SHOWME,
                    IntPtr.Zero,
                    IntPtr.Zero);
            }
            try
            {
                ThemeManager.Current.ChangeTheme(this, "Dark.Green");
                MainWindow = new MainWindow();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
        mutex.ReleaseMutex();
        mutex.Close();
    }
}

}