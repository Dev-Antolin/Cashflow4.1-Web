Imports System
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports INI_DLL
Imports DB_DLL
Imports CrystalDecisions.Shared
Imports CrystalDecisions.CrystalReports.Engine
Imports System.Web.Configuration

Partial Class Login
    Inherits System.Web.UI.Page

    Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLogin.Click
        If txtPassword.Text = "" And txtUsername.Text = "" Then
            lblMsg.Visible = True
            lblMsg.Text = "No Username and Password!"
            txtUsername.Focus()
            Exit Sub
        ElseIf txtPassword.Text = "" And txtUsername.Text = txtUsername.Text Then
            lblMsg.Visible = True
            lblMsg.Text = "Please input password!"
            txtPassword.Focus()
            Exit Sub
        ElseIf txtUsername.Text = "" And txtPassword.Text = txtPassword.Text Then
            lblMsg.Visible = True
            lblMsg.Text = "Please input username!"
            txtPassword.Text = " "
            txtUsername.Focus()
            Exit Sub
        ElseIf txtUsername.Text <> "" And txtPassword.Text <> "" Then
            Dim strConR As String = Nothing
            Dim rdr As SqlClient.SqlDataReader = Nothing
            Dim dbr As New clsDBConnection
            Dim sql As String = Nothing
            login()
            Search()
            Dim user1 As String = Me.Session("user1")
            Dim user2 As String = Me.Session("user2")
            Dim user3 As String = Me.Session("user3")

            sql = "select res_id,fullname from humres where job_title in ('" & user1 & "','" & user2 & "','" & user3 & "') and usr_id = '" & txtUsername.Text & "'" 'and res_id = '" & Trim(ToString(pWordL)) & "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            strConR = Me.Session("strConfR")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)
            'rdr.Read()

            If rdr.Read Then
                Dim resid As Integer = Trim(rdr(0))
                Dim residS As String = Convert.ToString(resid)
                Dim pWordL As String = txtPassword.Text.Trim
                Dim full_name As String = Trim(rdr(1))
                If pWordL = residS Then
                    Dim uname As String = txtUsername.Text
                    Dim ps As String = txtPassword.Text
                    Me.Session.Add("uname", uname)
                    Me.Session.Add("ps", ps)
                    Me.Session.Add("full_name", full_name)
                    Response.Redirect("MainMenu.aspx")
                Else
                    lblMsg.Text = "Invalid User/Password"
                    txtPassword.Text = ""
                    txtUsername.Text = ""
                    txtUsername.Focus()
                End If
            End If
        End If


    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        txtUsername.Focus()
        Me.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Me.Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1))
        Me.Response.Cache.SetValidUntilExpires(False)
        lblMsg.Visible = True
    End Sub

    Public Sub login()
        Dim ini_Path As String = AppDomain.CurrentDomain.BaseDirectory + "cashflowv3.ini"
        Dim line As String = Nothing

        Dim server, db, uname, pass, user1, user2, user3 As String
        Dim rdr As New ReadWriteINI
        Dim strConfR As String

        server = rdr.readINI("SERVER Synergy", "SERVER", False, ini_Path)
        db = rdr.readINI("SERVER Synergy", "DBNAME", False, ini_Path)
        uname = rdr.readINI("SERVER Synergy", "USERNAME", False, ini_Path)
        pass = rdr.readINI("SERVER Synergy", "PASSWORD", False, ini_Path)
        user1 = rdr.readINI("SERVER Synergy", "user1", False, ini_Path)
        user2 = rdr.readINI("SERVER Synergy", "user2", False, ini_Path)
        user3 = rdr.readINI("SERVER Synergy", "user3", False, ini_Path)
        strConfR = "user id=" & uname & ";password=" & pass & ";data source=" & server & ";persist security info=False;initial catalog=" & db
        Me.Session.Add("strConfR", strConfR)
        Me.Session.Add("user1", user1)
        Me.Session.Add("user2", user2)
        Me.Session.Add("user3", user3)
    End Sub

    Public Sub Search()
        Dim strConR As String = Nothing
        Dim rdr As SqlClient.SqlDataReader = Nothing
        Dim dbr As New clsDBConnection
        Dim sql As String = Nothing

        sql = "Select task,comp,costcenter from humres where usr_id = '" & txtUserName.Text & "' "

        strConR = Me.Session("strConfR")
        dbr.ConnectDB(strConR)
        rdr = dbr.Execute_SQL_DataReader(sql)

        If rdr.Read Then
            Me.Session.Add("_task", rdr("task").ToString)
            Me.Session.Add("_costcenter", rdr("costcenter").ToString)
            Me.Session.Add("_comp", rdr("comp").ToString)
        End If

    End Sub
End Class
