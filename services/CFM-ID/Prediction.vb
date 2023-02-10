#Region "Microsoft.VisualBasic::185bf79aa62226115454fe1e470f73be, mzkit\src\mzkit\services\CFM-ID\Prediction.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 125
    '    Code Lines: 48
    ' Comment Lines: 53
    '   Blank Lines: 24
    '     File Size: 6.01 KB


    ' Class Prediction
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: PredictMs2, ToString, WriteInputTempfile
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSP
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine

Public Class Prediction

    ' Usage: cfm-predict.exe <input_smiles_or_inchi> <prob_thresh_for_prune> <param_filename> <config_filename> <include_annotations> <output_filename> <apply_post_processing>

    ' input_smiles_or_inchi_or_file:
    ' The smiles or inchi string of the structure whose spectra you want to predict, or a .txt file containing a list of <id smiles> pairs, one per line.

    ' prob_thresh_for_prune (opt):
    ' The probability below which to prune unlikely fragmentations (default 0.001)

    ' param_filename (opt):
    ' The filename where the parameters of a trained cfm model can be found (if not given, assumes param_output.log in current directory)

    ' config_filename (opt):
    ' The filename where the configuration parameters of the cfm model can be found (if not given, assumes param_config.txt in current directory)

    ' include_annotations (opt):
    ' Whether to include fragment information in the output spectra (0 = NO (default), 1 = YES ). Note: ignored for msp/mgf output.

    ' output_filename_or_dir (opt):
    ' The filename of the output spectra file to write to (if not given, prints to stdout), OR directory if multiple smiles inputs are given (else current directory) OR msp or mgf file.

    ' apply_postprocessing (opt):
    ' Whether or not to post-process predicted spectra to take the top 80% of energy (at least 5 peaks), or the highest 30 peaks (whichever comes first) (0 = OFF, 1 = ON (default) ).

    ' suppress_exception (opt):
    ' Suppress exceptions so that the program returns normally even when it fails to produce a result (0 = OFF (default), 1 = ON).

    Dim appPath As String
    Dim current_task As String

    Sub New(app As String)
        Me.appPath = app
    End Sub

    ''' <summary>
    ''' Do MS2 spectrum data prediction
    ''' </summary>
    ''' <param name="input_smiles_or_inchi">
    ''' The smiles or inchi string of the structure whose spectra you want to predict
    ''' </param>
    ''' <param name="prob_thresh_for_prune">
    ''' The probability below which to prune unlikely fragmentations (default 0.001)
    ''' </param>
    ''' <param name="param_filename">
    ''' The filename where the parameters of a trained cfm model can be found 
    ''' (if not given, assumes param_output.log in current directory)
    ''' </param>
    ''' <param name="config_filename">
    ''' The filename where the configuration parameters of the cfm model can be found 
    ''' (if not given, assumes param_config.txt in current directory)
    ''' </param>
    ''' <param name="include_annotations">
    ''' Whether to include fragment information in the output spectra 
    ''' (0 = NO (default), 1 = YES ). Note: ignored for msp/mgf output.
    ''' </param>
    ''' <param name="output_filename">
    ''' The filename of the output spectra file to write to (if not given, prints to stdout),
    ''' OR directory if multiple smiles inputs are given (else current directory) OR msp 
    ''' or mgf file.
    ''' </param>
    ''' <param name="apply_post_processing">
    ''' Whether or not to post-process predicted spectra to take the top 80% of energy
    ''' (at least 5 peaks), or the highest 30 peaks (whichever comes first) (0 = OFF, 
    ''' 1 = ON (default) ).
    ''' </param>
    ''' <param name="suppress_exception">
    ''' Suppress exceptions so that the program returns normally even when it fails to 
    ''' produce a result (0 = OFF (default), 1 = ON).
    ''' </param>
    ''' <returns></returns>
    Public Function PredictMs2(input_smiles_or_inchi As String,
                               Optional prob_thresh_for_prune As Double = 0.001,
                               Optional param_filename As String = Nothing,
                               Optional config_filename As String = Nothing,
                               Optional include_annotations As Boolean = False,
                               Optional apply_post_processing As Boolean = True,
                               Optional suppress_exception As Boolean = False) As MspData()

        Dim argv As New StringBuilder
        Dim input_filename As String = WriteInputTempfile(input_smiles_or_inchi)
        Dim output_filename As String = input_filename.ChangeSuffix("msp")

        Call argv.AppendLine(input_filename.CLIPath)          ' <input_smiles_or_inchi> 
        Call argv.AppendLine(prob_thresh_for_prune)           ' <prob_thresh_for_prune>
        Call argv.AppendLine(param_filename.CLIPath)          ' <param_filename>
        Call argv.AppendLine(config_filename.CLIPath)         ' <config_filename>
        Call argv.AppendLine(If(include_annotations, 1, 0))   ' <include_annotations>
        Call argv.AppendLine(output_filename.CLIPath)         ' <output_filename>
        Call argv.AppendLine(If(apply_post_processing, 1, 0)) ' <apply_post_processing>

        current_task = argv.ToString.TrimNewLine

        Call New IORedirectFile(appPath, current_task, win_os:=True).Run()

        If Not output_filename.FileExists(ZERO_Nonexists:=False) Then
            If suppress_exception Then
                Return Nothing
            Else
                Throw New InvalidProgramException
            End If
        End If

        Return MspData.Load(output_filename).ToArray
    End Function

    Private Shared Function WriteInputTempfile(input_smiles_or_inchi As String) As String
        Dim tempfile As String = TempFileSystem.GetAppSysTempFile(".txt", sessionID:=$"cfm_id-${App.PID.ToHexString}", prefix:="ms2_predictions")
        Dim uid As String = App.GetNextUniqueName("molecule_")

        Call $"{uid} {input_smiles_or_inchi}".SaveTo(tempfile, encoding:=Encoding.ASCII)

        Return tempfile
    End Function

    Public Overrides Function ToString() As String
        Return appPath & " " & current_task
    End Function

End Class
