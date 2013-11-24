Imports DB_DLL
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports System.IO
Imports System.Web.Configuration
Imports System.Data.SqlClient
Imports INI_DLL

Partial Class CashFlowReportVer3
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
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim con As New clsDBConnection
        Dim rpt As New ReportDocument
        Dim strCon As String

        Dim rBranch As String
        Dim rWide As String
        Dim rGen_date As DateTime
        Dim rReport As String
        Dim rRegion As String
        Dim rArea As String

        rGen_date = Me.Session("gen_date")
        rWide = Me.Session("wideR")
        rBranch = Me.Session("branchR")
        rReport = Me.Session("reportR")
        rArea = Me.Session("areaR")
        rRegion = Me.Session("regionR")

        IniReportSetup()
        strCon = Me.Session("strConfR")

        If con.isConnected Then
            con.CloseConnection()
        End If

        con.ConnectDB(strCon)

        Dim crTable As CrystalDecisions.CrystalReports.Engine.Table
        Dim crLogin As CrystalDecisions.Shared.TableLogOnInfo
        Dim reportPath As String = AppDomain.CurrentDomain.BaseDirectory + "CashFlowReporVer3.rpt"
        Dim objOrientation As PaperOrientation = PaperOrientation.Portrait

        rpt.Load(reportPath)

        rpt.SetParameterValue("@gen_date", rGen_date)
        rpt.SetParameterValue("@wide", rWide)
        rpt.SetParameterValue("@branch", rBranch)
        rpt.SetParameterValue("@area", rArea)
        rpt.SetParameterValue("@region", rRegion)
        rpt.SetParameterValue("@report", rReport)

        For Each crTable In rpt.Database.Tables
            crLogin = crTable.LogOnInfo
            crLogin = New TableLogOnInfo

            crLogin.ConnectionInfo.ServerName = serverR
            crLogin.ConnectionInfo.DatabaseName = dbR
            crLogin.ConnectionInfo.Password = passR
            crLogin.ConnectionInfo.UserID = unameR
            crTable.ApplyLogOnInfo(crLogin)
            crTable.Location = dbR + ".dbo." + rReport

        Next

        rpt.PrintOptions.PaperOrientation = objOrientation
        CrystalReportViewer1.ReportSource = rpt
    End Sub
    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        Response.Redirect("MainMenu.aspx")
    End Sub
End Class
