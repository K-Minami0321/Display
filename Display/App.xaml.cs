using System.Threading;
using System.Windows;

namespace Display
{
    public partial class App : Application
    {
        Mutex mutex = new Mutex(false, "ApplicationName");

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Mutexの所有権を要求
            if (!mutex.WaitOne(0, false))
            {
                mutex.Close();
                this.Shutdown();        // 既に起動しているため、終了する
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (mutex != null)
            {
                mutex.ReleaseMutex();
                mutex.Close();
            }
        }
    }
}
