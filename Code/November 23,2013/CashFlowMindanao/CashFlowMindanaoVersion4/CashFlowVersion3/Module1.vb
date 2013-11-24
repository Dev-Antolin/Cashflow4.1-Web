Imports System.IO
Module Module1
    Public ls_timestart As String = Nothing ' added on 9-1-2010
    Public ls_timeend As String = Nothing ' added on 9-1-2010
    Public gs_transdate As String = Nothing 'added for email function
    Public gs_transdatereport_title As String = Nothing 'added for email function
    Public gs_HO_Email_Info As String = Nothing 'added for email function



    Public Sub AutoDirectory(ByVal dr As String)
        Try
            'Dim dir As Directory
            If Directory.Exists(dr) = False Then
                Directory.CreateDirectory(dr)
            Else
            End If
        Catch ex As Exception

        End Try

    End Sub
    Public Sub Log_Ex(ByVal as_log As String, ByVal as_filename As String)
        Try

            Dim objFile As FileStream
            Dim objWriter As System.IO.StreamWriter


            If System.IO.File.Exists(as_filename) Then
                objFile = New FileStream(as_filename, FileMode.Append, FileAccess.Write, FileShare.Write)
            Else
                objFile = New FileStream(as_filename, FileMode.Create, FileAccess.Write, FileShare.Write)
            End If

            objWriter = New System.IO.StreamWriter(objFile)

            objWriter.BaseStream.Seek(0, SeekOrigin.End)
            objWriter.WriteLine(as_log)
            objWriter.Close()
        Catch ex As Exception

        End Try


    End Sub
    Public Sub LogError(ByVal as_log As String)
        Dim strPath As String = Application.StartupPath & "\Logs\Error"
        AutoDirectory(strPath)
        Call Log_Ex(as_log, strPath & "\" & Replace(CStr(Now.ToShortDateString), "/", "-") + ".log")
    End Sub

    Public Sub Log(ByVal as_log As String)
        Dim strPath As String = Application.StartupPath & "\Logs\System"
        AutoDirectory(strPath)
        Call Log_Ex(as_log, strPath & "\" & Replace(CStr(Now.ToShortDateString), "/", "-") + ".log")
    End Sub

End Module
