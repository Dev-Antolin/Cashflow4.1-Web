Imports INI_DLL
Imports DB_DLL
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports System.IO
Imports System.Web.Configuration
Imports System.Data.SqlClient

Public Class GenerateReport
    Dim serverR, dbR, unameR, passR As String
    Private Sub IniReportSetup()
        Dim ini_Path As String = AppDomain.CurrentDomain.BaseDirectory + "cashflowv3report.ini"
        Dim line As String = Nothing

        Dim rdr As New ReadWriteINI
        Dim strConfR As String

        serverR = rdr.readINI("SERVER INI", "SERVER", False, ini_Path)
        dbR = rdr.readINI("SERVER INI", "DBNAME", False, ini_Path)
        unameR = rdr.readINI("SERVER INI", "USERNAME", False, ini_Path)
        passR = rdr.readINI("SERVER INI", "PASSWORD", False, ini_Path)
        strConfR = "user id=" & unameR & ";password=" & passR & ";data source=" & serverR & ";persist security info=False;initial catalog=" & dbR
    End Sub
    Private Sub CrystalReportViewer1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CrystalReportViewer1.Load
        Dim con As New clsDBConnection
        Dim rpt As New ReportDocument
        'Dim strCon As String

        Dim rGen_date As String = gs_transdate
        Dim rReport As String = "CF_Vismin_Wide"
        Dim rWide As String = "VISMIN"
        Dim rRegion As String = "0"
        Dim rArea As String = "0"
        Dim rBranch As String = "0"
        'exec(SP_CashFlowVer3Report) '5/30/2010','VISMIN','0','0','0','CF_Vismin_Wide'
        Dim sp As String = "SP_CashFlowVer3Report"
        'Dim Filename As String = rBranch + rGen_date

        Dim reportPath As String = Application.StartupPath

        Dim crLogin As New TableLogOnInfo
        Dim crConnectionInfo1 As New ConnectionInfo
        Dim crTables1 As Tables = Nothing

        rpt = New ReportDocument
        rpt.Load(reportPath & "\CFRVW1.rpt")
        rpt.SetParameterValue("@gen_date", rGen_date)
        rpt.SetParameterValue("@wide", rWide)
        rpt.SetParameterValue("@region", rRegion)
        rpt.SetParameterValue("@area", rArea)
        rpt.SetParameterValue("@branch", rBranch)
        rpt.SetParameterValue("@report", rReport)
        'rpt.ParameterFields(0) = "rptTitle;" & rBranch + rGen_date & ";true"

        IniReportSetup()

        For Each crTable In rpt.Database.Tables
            crLogin = crTable.LogOnInfo
            crLogin = New TableLogOnInfo
            crLogin.ConnectionInfo.ServerName = serverR
            crLogin.ConnectionInfo.DatabaseName = dbR
            crLogin.ConnectionInfo.Password = passR
            crLogin.ConnectionInfo.UserID = unameR
            crTable.ApplyLogOnInfo(crLogin)
            'rReport value is the stored procedure name
            crTable.Location = dbR + ".dbo." + sp
        Next
        CrystalReportViewer1.ReportSource = rpt

        Dim rptDocument As New ReportDocument
        Dim objDiskOpt As New DiskFileDestinationOptions
        Dim objExOpt As ExportOptions
        Dim errorExport As Boolean = True
        Dim layout As Integer = 2
        Dim objOrientation As PaperOrientation = CInt(layout)

        '/--------to generate report
        rpt.PrintOptions.PaperSize = PaperSize.PaperLegal
        rptDocument.PrintOptions.PaperOrientation = objOrientation
        MakeFolder()
        objDiskOpt.DiskFileName = Application.StartupPath & "\reports\" + gs_HO_Email_Info + ".pdf"
        objExOpt = rpt.ExportOptions
        objExOpt.ExportDestinationType = ExportDestinationType.DiskFile
        objExOpt.ExportFormatType = ExportFormatType.PortableDocFormat
        objExOpt.DestinationOptions = objDiskOpt
        rpt.Export()
        rpt.Close()
        rpt.Dispose()
        'Process.Start("explorer.exe", Application.StartupPath) --> open folder for pdf report
        Me.Close()
        ''\--------to generate report

        '\-----------this code is for automatic report generation
    End Sub
    Private Sub MakeFolder()
        If Not IO.File.Exists(Application.StartupPath & "\reports\") Then
            IO.Directory.CreateDirectory(Application.StartupPath & "\reports\")
        End If
    End Sub

    Private Sub GenerateReport_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
End Class