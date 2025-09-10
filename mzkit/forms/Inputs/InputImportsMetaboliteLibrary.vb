Imports Microsoft.VisualBasic.Text

Public Class InputImportsMetaboliteLibrary

    Shared ReadOnly rtf As String =
        <rtf>
{\rtf1\ansi\ansicpg936\deff0\nouicompat\deflang1033\deflangfe2052{\fonttbl{\f0\fnil\fcharset0 Calibri;}{\f1\fnil Calibri;}}
{\*\generator Riched20 10.0.20348}\viewkind4\uc1 
\pard\sl240\slmult1\f0\fs22\lang9 The data source in table format for database import should be a CSV or XLSX file with the following structure:  \par
\par
\b Mandatory fields:  \par
\b0\par
\f1\bullet  \b id\b0 : The unique identifier of the metabolite.  \par
\bullet  \b name\b0 : The name of the metabolite.  \par
\bullet  \b formula\b0 : The chemical formula of the metabolite.  \par
\par
\b Optional fields \b0 (for additional import flexibility):  \par
\par
\bullet  \i kegg\i0 : The metabolite ID in the KEGG database.  \par
\bullet  \i hmdb\i0 : The metabolite ID in the HMDB database.  \par
\bullet  \i cas\i0 : The CAS registry number of the metabolite.  \par
\par
This structured format ensures seamless integration into the database while accommodating essential and supplementary metadata.\f0\par
} 
        </rtf>

    Private Sub InputImportsMetaboliteLibrary_Load(sender As Object, e As EventArgs) Handles Me.Load
        RichTextBox1.Rtf = rtf.Trim(" "c, ASCII.CR, ASCII.LF, ASCII.TAB)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub
End Class