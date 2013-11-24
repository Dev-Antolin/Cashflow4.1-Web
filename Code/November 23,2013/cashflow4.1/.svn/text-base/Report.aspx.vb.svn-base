Imports DB_DLL
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports System.IO
Imports System.Web.Configuration
Imports System.Data.SqlClient
Imports INI_DLL

Partial Class WideVismin
    Inherits System.Web.UI.Page

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
        Me.Session.Add("strConfR", strConfR)
    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        Response.Redirect("MainMenu.aspx")
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        CheckLogin()

        Dim con As New clsDBConnection
        Dim rpt As New ReportDocument
        Dim strCon As String


        Dim rGen_date As String = Me.Session("gen_date")
        Dim rReport As String = Me.Session("reportR")
        Dim rWide As String = Me.Session("wideR")
        Dim rRegion As String = Me.Session("regionR")
        Dim rArea As String = Me.Session("areaR")
        Dim rBranch As String = Me.Session("branchR")
        Dim sp As String = "SP_CashFlowVer3Report"

        Dim reportPath As String = AppDomain.CurrentDomain.BaseDirectory

        Dim crLogin As New TableLogOnInfo
        Dim crConnectionInfo1 As New ConnectionInfo
        Dim crTables1 As Tables = Nothing

        rpt = New ReportDocument
        rpt.Load(reportPath & "CFRVW.rpt")
        rpt.SetParameterValue("@gen_date", rGen_date)
        rpt.SetParameterValue("@wide", rWide)
        rpt.SetParameterValue("@region", rRegion)
        rpt.SetParameterValue("@area", rArea)
        rpt.SetParameterValue("@branch", rBranch)
        rpt.SetParameterValue("@report", rReport)

        IniReportSetup()
        strCon = Me.Session("strConfR")

        For Each crTable In rpt.Database.Tables
            crLogin = crTable.LogOnInfo
            crLogin = New TableLogOnInfo
            crLogin.ConnectionInfo.ServerName = serverR
            crLogin.ConnectionInfo.DatabaseName = dbR
            crLogin.ConnectionInfo.Password = passR
            crLogin.ConnectionInfo.UserID = unameR
            crTable.ApplyLogOnInfo(crLogin)
            ' rReport value is the stored procedure name
            crTable.Location = dbR + ".dbo." + sp
        Next

        CrystalReportViewer1.ReportSource = rpt
    End Sub

    Private Sub CheckLogin()
        If Me.Session("uname") = "" Then
            Response.Redirect("Login.aspx")
        End If
    End Sub
End Class
