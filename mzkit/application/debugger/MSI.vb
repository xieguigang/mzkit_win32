Imports System.Threading
Imports BioNovoGene.mzkit_win32.My
Imports BioNovoGene.mzkit_win32.ServiceHub
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit

Namespace Debugger

    Module MSI

        Public Sub Run()
            If MSIDataService.debugPort Is Nothing Then
                MessageBox.Show("MZKit workbench software should be start with debug mode!", "Invalid Parameter", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                Dim Rscript As String = MSIDataService.GetRscript
                Dim mb As Double = MyApplication.buffer_size / ByteSize.MB

                Global.ServiceHub.Protocols.StartServer(Rscript, 0, debugPort:=MSIDataService.debugPort, buf_size:=mb)

                Call Thread.Sleep(1000)
                Call Run2()
            End If
        End Sub

        Private Sub Run2()
            Dim viewer As frmMsImagingViewer = WindowModules.viewer
            Dim dockPanel = MyApplication.host.DockPanel

            WindowModules.msImageParameters.Show(dockPanel)

            viewer.Show(MyApplication.host.DockPanel)

            If Not viewer.MSIservice Is Nothing Then
                viewer.MSIservice.CloseMSIEngine()
            End If

            viewer.MSIservice = MSIDataService.ConnectCloud(viewer.MSIservice, "127.0.0.1", MSIDataService.debugPort)
            viewer.HookBlender()

            ' load rawdata file
            Using file As OpenFileDialog = RibbonEvents.OpenMSIRawDialog
                If file.ShowDialog = DialogResult.OK Then
                    Call RibbonEvents.OpenMSIRaw(file.FileName, debug:=True)
                End If
            End Using
        End Sub
    End Module
End Namespace