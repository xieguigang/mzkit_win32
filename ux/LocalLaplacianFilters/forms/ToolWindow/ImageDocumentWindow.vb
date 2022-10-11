Imports System
Imports System.Windows.Forms
Imports WeifenLuo.WinFormsUI.Docking
Imports System.IO


Public Partial Class ImageDocumentWindow
        Inherits DockContent

        Public Sub New()
            InitializeComponent()
            AutoScaleMode = AutoScaleMode.Dpi
            DockAreas = DockAreas.Document Or DockAreas.Float
        End Sub

        Private m_fileName As String = String.Empty

        Public Property FileName As String
            Get
                Return m_fileName
            End Get
            Set(ByVal value As String)

                If Not Equals(value, String.Empty) Then
                    Dim s As Stream = New FileStream(value, FileMode.Open)
                    Dim efInfo As FileInfo = New FileInfo(value)
                    Dim fext As String = efInfo.Extension.ToUpper()

                    If fext.Equals(".RTF") Then
                        richTextBox1.LoadFile(s, RichTextBoxStreamType.RichText)
                    Else
                        richTextBox1.LoadFile(s, RichTextBoxStreamType.PlainText)
                    End If

                    s.Close()
                End If

                m_fileName = value
                ToolTipText = value
            End Set
        End Property

        ' workaround of RichTextbox control's bug:
        ' If load file before the control showed, all the text format will be lost
        ' re-load the file after it get showed.
        Private m_resetText As Boolean = True

        Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
            MyBase.OnPaint(e)

            If m_resetText Then
                m_resetText = False
                FileName = FileName
            End If
        End Sub

        Protected Overrides Function GetPersistString() As String
            ' Add extra information into the persist string for this document
            ' so that it is available when deserialized.
            Return [GetType]().ToString() & "," & FileName & "," & Text
        End Function

        Private Sub menuItem2_Click(ByVal sender As Object, ByVal e As EventArgs)
            MessageBox.Show("This is to demostrate menu item has been successfully merged into the main form. Form Text=" & Text)
        End Sub

        Private Sub menuItemCheckTest_Click(ByVal sender As Object, ByVal e As EventArgs)
            menuItemCheckTest.Checked = Not menuItemCheckTest.Checked
        End Sub

        Protected Overrides Sub OnTextChanged(ByVal e As EventArgs)
            MyBase.OnTextChanged(e)
            If Equals(FileName, String.Empty) Then richTextBox1.Text = Text
        End Sub
    End Class

