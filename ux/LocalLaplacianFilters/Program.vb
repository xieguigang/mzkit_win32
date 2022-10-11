Imports System
Imports System.Globalization
Imports System.Threading
Imports System.Windows.Forms

Namespace LaplacianHDR
    Friend Module Program
        ''' <summary>
        ''' Главная точка входа для приложения.
        ''' </summary>
        <STAThread>
        Private Sub Main()
            Call Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture
            Call Application.Run(New Form1())
        End Sub
    End Module
End Namespace
