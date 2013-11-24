Imports DB_DLL
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports System.IO
Imports System.Web.Configuration
Imports System.Data.SqlClient
Imports INI_DLL

Partial Class RegionReport
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
    Private Sub checklogin()
        If Me.Session("strConfR") = "" OrElse HttpContext.Current.Session("uname") = "" Then
            Response.Redirect("Login.aspx")
        End If
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'If IsPostBack Then
        '    Response.Redirect("MainMenu.aspx")
        'Else

        Response.Buffer = True
        Response.ExpiresAbsolute = DateTime.Now.AddDays(-1D)
        Response.Expires = -1500
        Response.CacheControl = "no-cache"
        'If Me.Session("strConfR") = "" Then
        '    Response.Redirect("login.aspx")
        'End If
        checklogin()

        Dim con As New clsDBConnection
        Dim rpt As New ReportDocument
        Dim strCon As String

        Dim rRegion As String
        Dim rWide As String
        Dim rGen_date As DateTime
        Dim rReport As String

        rGen_date = Me.Session("gen_date")
        rWide = Me.Session("wideR")
        rRegion = Me.Session("regionR")
        rReport = Me.Session("reportR")

        IniReportSetup()
        strCon = Me.Session("strConfR")

        If con.isConnected Then
            con.CloseConnection()
        End If

        con.ConnectDB(strCon)

        Dim crTable As CrystalDecisions.CrystalReports.Engine.Table
        Dim crLogin As CrystalDecisions.Shared.TableLogOnInfo
        Dim reportPath As String = AppDomain.CurrentDomain.BaseDirectory + "region.rpt"
        Dim objOrientation As PaperOrientation = PaperOrientation.Portrait

        rpt.Load(reportPath)

        rpt.SetParameterValue("param_genDate", rGen_date)
        rpt.SetParameterValue("param_wide", rWide)
        rpt.SetParameterValue("param_region", rRegion)

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

        'If Me.Session.Item("rbselection") = "Default" Then
        '    'rpt.PrintOptions.PaperOrientation = objOrientation
        '    'rpt.SetParameterValue("@StartDate", Me.Session("StartDate"))
        '    CrystalReportViewer1.ReportSource = rpt
        'Else
        '    Dim oStream As New MemoryStream   ' // using System.IO
        '    ' oStream = (MemoryStream)

        '    oStream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat)
        '    Response.Clear()
        '    Response.ClearContent()
        '    Response.ClearHeaders()
        '    Response.Buffer = True
        '    Response.ContentType = "application/pdf"
        '    Response.BinaryWrite(oStream.ToArray())
        '    Response.End()
        '    Response.Write("Export Done")
        '    rpt.Dispose()
        '    'Response.OutputStream()
        '    oStream = Nothing
        'End If
    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        Response.Redirect("MainMenu.aspx")
    End Sub
End Class
