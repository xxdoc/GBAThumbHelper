Imports System.IO
Imports System.Net

Public Class MnFrm

    Public AppPath As String = System.AppDomain.CurrentDomain.BaseDirectory()
    Public LoadedASM As String

    Private Sub MnFrm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        fileOpenDialog.FileName = ""
        fileOpenDialog.CheckFileExists = True

        ' Check to ensure that the selected path exists.  Dialog box displays 
        ' a warning otherwise.
        fileOpenDialog.CheckPathExists = True

        ' Get or set default extension. Doesn't include the leading ".".
        fileOpenDialog.DefaultExt = "ASM"

        ' Return the file referenced by a link? If False, simply returns the selected link
        ' file. If True, returns the file linked to the LNK file.
        fileOpenDialog.DereferenceLinks = True

        ' Just as in VB6, use a set of pairs of filters, separated with "|". Each 
        ' pair consists of a description|file spec. Use a "|" between pairs. No need to put a
        ' trailing "|". You can set the FilterIndex property as well, to select the default
        ' filter. The first filter is numbered 1 (not 0). The default is 1. 
        fileOpenDialog.Filter =
            "(*.asm)|*.asm*"

        fileOpenDialog.Multiselect = False

        ' Restore the original directory when done selecting
        ' a file? If False, the current directory changes
        ' to the directory in which you selected the file.
        ' Set this to True to put the current folder back
        ' where it was when you started.
        ' The default is False.
        '.RestoreDirectory = False

        ' Show the Help button and Read-Only checkbox?
        fileOpenDialog.ShowHelp = False
        fileOpenDialog.ShowReadOnly = False

        ' Start out with the read-only check box checked?
        ' This only make sense if ShowReadOnly is True.
        fileOpenDialog.ReadOnlyChecked = False

        fileOpenDialog.Title = "Select File to open:"

        ' Only accept valid Win32 file names?
        fileOpenDialog.ValidateNames = True


        If fileOpenDialog.ShowDialog = DialogResult.OK Then

            LoadedASM = fileOpenDialog.FileName

            EditorTextBox.Text = File.ReadAllText(LoadedASM)

        End If

    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        OpenToolStripMenuItem.PerformClick()
    End Sub

    Private Sub CloseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CloseToolStripMenuItem.Click
        LoadedASM = ""
        EditorTextBox.Text = ""
    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        SaveFileDialog.FileName = LoadedASM

        ' Check to ensure that the selected path exists.  Dialog box displays 
        ' a warning otherwise.
        SaveFileDialog.CheckPathExists = True

        ' Get or set default extension. Doesn't include the leading ".".
        SaveFileDialog.DefaultExt = "ASM"

        ' Return the file referenced by a link? If False, simply returns the selected link
        ' file. If True, returns the file linked to the LNK file.
        SaveFileDialog.DereferenceLinks = True

        ' Just as in VB6, use a set of pairs of filters, separated with "|". Each 
        ' pair consists of a description|file spec. Use a "|" between pairs. No need to put a
        ' trailing "|". You can set the FilterIndex property as well, to select the default
        ' filter. The first filter is numbered 1 (not 0). The default is 1. 
        SaveFileDialog.Filter =
            "(*.asm)|*.asm*"


        ' Restore the original directory when done selecting
        ' a file? If False, the current directory changes
        ' to the directory in which you selected the file.
        ' Set this to True to put the current folder back
        ' where it was when you started.
        ' The default is False.
        '.RestoreDirectory = False

        ' Show the Help button and Read-Only checkbox?
        SaveFileDialog.ShowHelp = False

        SaveFileDialog.Title = "Select File to open:"

        ' Only accept valid Win32 file names?
        SaveFileDialog.ValidateNames = True


        If SaveFileDialog.ShowDialog = DialogResult.OK Then

            File.WriteAllText(SaveFileDialog.FileName, EditorTextBox.Text)

        End If
    End Sub

    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        SaveToolStripMenuItem.PerformClick()
    End Sub

    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        CloseToolStripMenuItem.PerformClick()
    End Sub

    Private Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click
        AssembleToolStripMenuItem.PerformClick()
    End Sub

    Private Sub AssembleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AssembleToolStripMenuItem.Click
        If (System.IO.File.Exists(AppPath & "thumb\thumb.bat") And System.IO.File.Exists(AppPath & "thumb\objcopy.exe") And System.IO.File.Exists(AppPath & "thumb\as.exe")) = True Then

            If System.IO.File.Exists(AppPath & "thumb\temp.asm") = True Then
                System.IO.File.Delete(AppPath & "\\temp.asm")
            End If

            If System.IO.File.Exists(AppPath & "thumb\temp.bin") = True Then
                System.IO.File.Delete(AppPath & "thumb\temp.bin")
            End If

            File.WriteAllText(AppPath & "thumb\temp.asm", EditorTextBox.Text)

            Dim proc As New Process
            proc.StartInfo.FileName = AppPath & "thumb\thumb.bat" 'Use the the full Pathname of the program
            proc.StartInfo.Arguments = AppPath & "thumb\temp.asm" 'This is the argument that is used
            proc.StartInfo.UseShellExecute = False
            proc.StartInfo.RedirectStandardOutput = True
            proc.StartInfo.CreateNoWindow = True 'Dont show the cmd window when the program is running
            proc.StartInfo.WorkingDirectory = AppPath & "thumb\"
            proc.Start()

            'Read the text from the cmd window
            Dim output As String = proc.StandardOutput.ReadToEnd()

            'Check to see if the text contains whatever the program shows in the cmd window to show it was successful
            If output.Contains("Assembled successfully.") Then 'Replace (Success Text) with the text that shows it was successful
                'MessageBox.Show("Success")

                Dim bytebuff As Byte()

                bytebuff = File.ReadAllBytes(AppPath & "thumb\temp.bin")

                HexOutputTextBox.Text = ""

                For Each b As Byte In bytebuff
                    HexOutputTextBox.Text = HexOutputTextBox.Text & MakeProperByte(b)
                Next

            Else

                MsgBox(output)
                'MessageBox.Show("Code was not assembled...")

            End If
            proc.Dispose()

        Else
            MsgBox("Thumb.bat or it's files are missing...")
        End If
    End Sub

    Public Function MakeProperByte(DaByte)
        Dim OutputByte As String


        If Len(Hex(DaByte)) = 1 Then
            OutputByte = "0" & Hex(DaByte)
        Else
            OutputByte = Hex(DaByte)
        End If

        MakeProperByte = OutputByte

    End Function
End Class
