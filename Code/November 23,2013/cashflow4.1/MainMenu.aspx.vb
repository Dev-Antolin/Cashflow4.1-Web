Imports System
Imports System.IO
Imports INI_DLL
Imports DB_DLL
Imports System.Web.UI
Imports System.Data
Imports CrystalDecisions.Shared
Imports CrystalDecisions.CrystalReports.Engine
Imports System.Web.Configuration
Imports System.Data.SqlClient
Imports AjaxControlToolkit

Partial Class MainMenu
    Inherits System.Web.UI.Page
    Public WithEvents orpt1 As ReportDocument
    Dim reportR, wideR, regionR, areaR, branchR As String
    Dim VRAB As String
    Dim gen_date As DateTime
    Dim date_flag As Boolean
    Dim yy As YearFormat 'jeniena
    Dim mm As MonthFormat
    Dim dd As DateFormat
    'Dim strConSR As String


    Protected CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer
    Dim rpt As New ReportDocument

    Private Sub IniSetup()
        Dim ini_Path As String = AppDomain.CurrentDomain.BaseDirectory + "cashflowv3.ini"
        Dim line As String = Nothing

        Dim serverRLuzon, dbRLuzon, unameRLuzon, passRLuzon As String
        Dim serverRVisayas, dbRVisayas, unameRVisayas, passRVisayas As String
        Dim serverRMindanao, dbRMindanao, unameRMindanao, passRMindanao As String
        Dim serverRShowroom, dbRShowroom, unameRShowroom, passRShowroom As String
        Dim rdr As New ReadWriteINI
        Dim strConfRVisayas, strConfRLuzon, strConfRMindanao, strConfRShowroom As String

        serverRLuzon = rdr.readINI("SERVER Luzon INI", "SERVER", False, ini_Path)
        dbRLuzon = rdr.readINI("SERVER Luzon INI", "DBNAME", False, ini_Path)
        unameRLuzon = rdr.readINI("SERVER Luzon INI", "USERNAME", False, ini_Path)
        passRLuzon = rdr.readINI("SERVER Luzon INI", "PASSWORD", False, ini_Path)
        strConfRLuzon = "user id=" & unameRLuzon & ";password=" & passRLuzon & ";data source=" & serverRLuzon & ";persist security info=False;initial catalog=" & dbRLuzon & "; Connection Timeout = 3600;"
        Me.Session.Add("strConfRLuzon", strConfRLuzon)

        serverRVisayas = rdr.readINI("SERVER Visayas INI", "SERVER", False, ini_Path)
        dbRVisayas = rdr.readINI("SERVER Visayas INI", "DBNAME", False, ini_Path)
        unameRVisayas = rdr.readINI("SERVER Visayas INI", "USERNAME", False, ini_Path)
        passRVisayas = rdr.readINI("SERVER Visayas INI", "PASSWORD", False, ini_Path)
        strConfRVisayas = "user id=" & unameRVisayas & ";password=" & passRVisayas & ";data source=" & serverRVisayas & ";persist security info=False;initial catalog=" & dbRVisayas & "; Connection Timeout = 3600;"
        Me.Session.Add("strConfRVisayas", strConfRVisayas)

        serverRMindanao = rdr.readINI("SERVER Mindanao INI", "SERVER", False, ini_Path)
        dbRMindanao = rdr.readINI("SERVER Mindanao INI", "DBNAME", False, ini_Path)
        unameRMindanao = rdr.readINI("SERVER Mindanao INI", "USERNAME", False, ini_Path)
        passRMindanao = rdr.readINI("SERVER Mindanao INI", "PASSWORD", False, ini_Path)
        strConfRMindanao = "user id=" & unameRMindanao & ";password=" & passRMindanao & ";data source=" & serverRMindanao & ";persist security info=False;initial catalog=" & dbRMindanao & "; Connection Timeout = 3600;"
        Me.Session.Add("strConfRMindanao", strConfRMindanao)

        serverRShowroom = rdr.readINI("SERVER Showroom INI", "SERVER", False, ini_Path)
        dbRShowroom = rdr.readINI("SERVER Showroom INI", "DBNAME", False, ini_Path)
        unameRShowroom = rdr.readINI("SERVER Showroom INI", "USERNAME", False, ini_Path)
        passRShowroom = rdr.readINI("SERVER Showroom INI", "PASSWORD", False, ini_Path)
        strConfRShowroom = "user id=" & unameRShowroom & ";password=" & passRShowroom & ";data source=" & serverRShowroom & ";persist security info=False;initial catalog=" & dbRShowroom & "; Connection Timeout = 3600;"
        'strConSR = strConfRShowroom
        Me.Session.Add("strConfRShowroom", strConfRShowroom)


    End Sub

    Private Sub IniReport()
        Dim ini_Path As String = AppDomain.CurrentDomain.BaseDirectory + "cashflowv3report.ini"
        Dim line As String = Nothing

        Dim server, db, uname, pass As String
        Dim rdr As New ReadWriteINI
        Dim strConfReport As String

        server = rdr.readINI("SERVER INI", "SERVER", False, ini_Path)
        db = rdr.readINI("SERVER INI", "DBNAME", False, ini_Path)
        uname = rdr.readINI("SERVER INI", "USERNAME", False, ini_Path)
        pass = rdr.readINI("SERVER INI", "PASSWORD", False, ini_Path)
        strConfReport = "user id=" & uname & ";password=" & pass & ";data source=" & server & ";persist security info=False;initial catalog=" & db & "; Connection Timeout = 3600;"
        Me.Session.Add("strConfReport", strConfReport)
    End Sub

    Protected Sub dplRegion_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dplRegion.SelectedIndexChanged

        dplArea.Items.Clear()
        dplBranch.Items.Clear()
        area()
        'dplArea.Enabled = True
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        IniReport()
        txtStartDate.Enabled = False
        Response.Buffer = True
        Response.ExpiresAbsolute = DateTime.Now.AddDays(-1D)
        Response.Expires = -1500
        Response.CacheControl = "no-cache"
        'If Me.Session("strConfR") = "" Then
        '    Response.Redirect("login.aspx")
        'End If
        CheckLogin()
        Call IniSetup()
        If Not IsPostBack Then
            If date_flag = Me.Session("date_flag") Then
                wide()
                summary()
                dplRegion.Enabled = False
                dplArea.Enabled = False
                dplBranch.Enabled = False
                txtStartDate.Text = Date.Today
            Else
                wide()
                summary()
                dplRegion.Enabled = False
                dplArea.Enabled = False
                dplBranch.Enabled = False
                txtStartDate.Text = Me.Session("dt")
                'dplWide.SelectedValue = " "
            End If
            'wide()
            'dplRegion.Enabled = False
            'dplArea.Enabled = False
            'dplBranch.Enabled = False
            'txtStartDate.Text = " "
        End If
    End Sub

    Public Sub area()
        Dim item As String = Nothing
        Dim rdr As SqlClient.SqlDataReader = Nothing

        If dplWide.Text = "LUZON" And dplRegion.SelectedValue <> "" Then
            Dim strCon As String = Me.Session("strConfRLuzon")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String
            'Dim area As String

            sql = "select distinct class_04 from bedryf where class_03 = '" + dplRegion.SelectedValue + "'"

            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            dplArea.Enabled = True
            dplArea.Items.Clear()
            dplBranch.Items.Clear()
            dplArea.Items.Add(" ")
            While rdr.Read
                item = Trim(rdr(0))
                dplArea.Items.Add(item.ToUpper)
            End While

            rdr.Close()
            db.CloseConnection()

        ElseIf dplWide.Text = "VISAYAS" And dplRegion.SelectedValue <> "" Then
            Dim strCon As String = Me.Session("strConfRVisayas")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String
            'Dim area As String

            sql = "select distinct class_04 from bedryf where class_02 = 'Visayas' and class_03 = '" + dplRegion.SelectedValue + "'"

            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            dplArea.Enabled = True
            dplArea.Items.Clear()
            dplBranch.Items.Clear()
            dplArea.Items.Add(" ")
            While rdr.Read
                item = Trim(rdr(0))
                dplArea.Items.Add(item.ToUpper)
            End While

            rdr.Close()
            db.CloseConnection()

        ElseIf dplWide.Text = "MINDANAO" And dplRegion.SelectedValue <> "" Then
            Dim strCon As String = Me.Session("strConfRMindanao")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String
            'Dim area As String

            sql = "select distinct class_04 from bedryf where class_02 = 'Mindanao' and class_03 = '" + dplRegion.SelectedValue + "'"

            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            dplArea.Enabled = True
            dplArea.Items.Clear()
            dplBranch.Items.Clear()
            dplArea.Items.Add(" ")
            While rdr.Read
                item = Trim(rdr(0))
                dplArea.Items.Add(item.ToUpper)
            End While

            rdr.Close()
            db.CloseConnection()

        ElseIf dplWide.Text = "SHOWROOM" And dplRegion.SelectedValue <> "" Then
            Dim strCon As String = Me.Session("strConfRShowroom")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String
            'Dim area As String

            sql = "select distinct class_04 from bedryf where class_02 = 'Showrooms' and class_03 = '" + dplRegion.SelectedValue + "'"

            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            dplArea.Enabled = True
            dplArea.Items.Clear()
            dplBranch.Items.Clear()
            dplArea.Items.Add(" ")
            While rdr.Read
                item = Trim(rdr(0))
                dplArea.Items.Add(item.ToUpper)
            End While

            rdr.Close()
            db.CloseConnection()

        End If
    End Sub
    Public Sub areaSummary()
        Dim item As String = Nothing
        Dim rdr As SqlClient.SqlDataReader = Nothing

        If dplSummary.Text = "LUZON" And dplRegion.SelectedValue <> "" Then
            Dim strCon As String = Me.Session("strConfRLuzon")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String
            'Dim area As String

            sql = "select distinct class_04 from bedryf where class_03 = '" + dplRegion.SelectedValue + "'"

            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            dplArea.Enabled = False
            dplArea.Items.Clear()
            dplBranch.Items.Clear()
            dplArea.Items.Add(" ")
            While rdr.Read
                item = Trim(rdr(0))
                dplArea.Items.Add(item.ToUpper)
            End While

            rdr.Close()
            db.CloseConnection()

        ElseIf dplSummary.Text = "VISAYAS" And dplRegion.SelectedValue <> "" Then
            Dim strCon As String = Me.Session("strConfRVisayas")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String
            'Dim area As String

            sql = "select distinct class_04 from bedryf where class_02 = 'Visayas' and class_03 = '" + dplRegion.SelectedValue + "'"

            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            dplArea.Enabled = False
            dplArea.Items.Clear()
            dplBranch.Items.Clear()
            dplArea.Items.Add(" ")
            While rdr.Read
                item = Trim(rdr(0))
                dplArea.Items.Add(item.ToUpper)
            End While

            rdr.Close()
            db.CloseConnection()

        ElseIf dplSummary.Text = "MINDANAO" And dplRegion.SelectedValue <> "" Then
            Dim strCon As String = Me.Session("strConfRMindanao")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String
            'Dim area As String

            sql = "select distinct class_04 from bedryf where class_02 = 'Mindanao' and class_03 = '" + dplRegion.SelectedValue + "'"

            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            dplArea.Enabled = False
            dplArea.Items.Clear()
            dplBranch.Items.Clear()
            dplArea.Items.Add(" ")
            While rdr.Read
                item = Trim(rdr(0))
                dplArea.Items.Add(item.ToUpper)
            End While

            rdr.Close()
            db.CloseConnection()

        ElseIf dplSummary.Text = "SHOWROOM" And dplRegion.SelectedValue <> "" Then
            Dim strCon As String = Me.Session("strConfRShowroom")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String
            'Dim area As String

            sql = "select distinct class_04 from bedryf where class_02 = 'Showrooms' and class_03 = '" + dplRegion.SelectedValue + "'"

            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            dplArea.Enabled = False
            dplArea.Items.Clear()
            dplBranch.Items.Clear()
            dplArea.Items.Add(" ")
            While rdr.Read
                item = Trim(rdr(0))
                dplArea.Items.Add(item.ToUpper)
            End While

            rdr.Close()
            db.CloseConnection()
        End If
    End Sub

    Public Sub regionSummary()
        Dim item As String = Nothing
        Dim rdr As SqlClient.SqlDataReader = Nothing

        If dplSummary.Text = "LUZON" Then
            Dim strCon As String = Me.Session("strConfRLuzon")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String

            dplRegion.Enabled = False
            dplRegion.Items.Clear()
            dplArea.Items.Clear()
            dplBranch.Items.Clear()
            dplRegion.Items.Add(" ")
            'sql = "select distinct class_03 from bedryf" '----> ching code
            sql = "select distinct class_03 from bedryf WHERE CLASS_03 <> '<none>'"   '-------->
            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            If rdr.HasRows Then
                While rdr.Read
                    item = Trim(rdr(0))
                    dplRegion.Items.Add(item.ToUpper)
                End While
            End If


            rdr.Close()
            db.CloseConnection()

        ElseIf dplSummary.Text = "VISAYAS" Then
            Dim strCon As String = Me.Session("strConfRVisayas")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String

            dplRegion.Enabled = False
            dplRegion.Items.Clear()
            dplArea.Items.Clear()
            dplBranch.Items.Clear()
            dplRegion.Items.Add(" ")
            'sql = "select distinct class_03 from bedryf"
            sql = "select distinct class_03 from bedryf WHERE class_02 = 'Visayas' and CLASS_03 <> '<none>'"   '--------> 
            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            If rdr.HasRows Then
                While rdr.Read
                    item = Trim(rdr(0))
                    dplRegion.Items.Add(item.ToUpper)
                End While
            End If

            rdr.Close()
            db.CloseConnection()

        ElseIf dplSummary.Text = "MINDANAO" Then
            Dim strCon As String = Me.Session("strConfRMindanao")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String

            dplRegion.Enabled = False
            dplRegion.Items.Clear()
            dplArea.Items.Clear()
            dplBranch.Items.Clear()
            dplRegion.Items.Add(" ")
            'sql = "select distinct class_03 from bedryf"
            sql = "select distinct class_03 from bedryf WHERE class_02 = 'Mindanao' and CLASS_03 <> '<none>'"   '--------> 
            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            If rdr.HasRows Then
                While rdr.Read
                    item = Trim(rdr(0))
                    dplRegion.Items.Add(item.ToUpper)
                End While
            End If

            rdr.Close()
            db.CloseConnection()

        ElseIf dplSummary.Text = "SHOWROOM" Then
            Dim strCon As String = Me.Session("strConfRShowroom")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String

            dplRegion.Enabled = False
            dplRegion.Items.Clear()
            dplArea.Items.Clear()
            dplBranch.Items.Clear()
            dplRegion.Items.Add(" ")
            'sql = "select distinct class_03 from bedryf"
            sql = "select distinct class_03 from bedryf WHERE class_02 = 'Showrooms' and CLASS_03 <> '<none>'"   '--------> new ely code
            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            If rdr.HasRows Then
                While rdr.Read
                    item = Trim(rdr(0))
                    dplRegion.Items.Add(item.ToUpper)
                End While
            End If

            rdr.Close()
            db.CloseConnection()

        ElseIf dplSummary.SelectedValue = " " Then
            dplRegion.Enabled = False
            dplArea.Enabled = False
            dplBranch.Enabled = False
        End If
    End Sub
    Public Sub region()
        Dim item As String = Nothing
        Dim rdr As SqlClient.SqlDataReader = Nothing

        If dplWide.Text = "LUZON" Then
            Dim strCon As String = Me.Session("strConfRLuzon")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String

            dplRegion.Enabled = True
            dplRegion.Items.Clear()
            dplArea.Items.Clear()
            dplBranch.Items.Clear()
            dplRegion.Items.Add(" ")
            'sql = "select distinct class_03 from bedryf" '----> ching code
            sql = "select distinct class_03 from bedryf WHERE CLASS_03 <> '<none> '"   '--------> new ely code

            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            If rdr.HasRows Then
                While rdr.Read
                    item = Trim(rdr(0))
                    dplRegion.Items.Add(item.ToUpper)
                End While
            End If


            rdr.Close()
            db.CloseConnection()

        ElseIf dplWide.Text = "VISAYAS" Then
            Dim strCon As String = Me.Session("strConfRVisayas")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String

            dplRegion.Enabled = True
            dplRegion.Items.Clear()
            dplArea.Items.Clear()
            dplBranch.Items.Clear()
            dplRegion.Items.Add(" ")
            'sql = "select distinct class_03 from bedryf"
            sql = "select distinct class_03 from bedryf WHERE class_02 = 'Visayas' and CLASS_03 <> '<none>'"   '--------> new ely code
            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            While rdr Is Nothing
                rdr = db.Execute_SQL_DataReader(sql)
            End While
            If rdr.HasRows Then
                While rdr.Read
                    item = Trim(rdr(0))
                    dplRegion.Items.Add(item.ToUpper)
                End While
            End If

            rdr.Close()
            db.CloseConnection()

        ElseIf dplWide.Text = "MINDANAO" Then
            Dim strCon As String = Me.Session("strConfRMindanao")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String

            dplRegion.Enabled = True
            dplRegion.Items.Clear()
            dplArea.Items.Clear()
            dplBranch.Items.Clear()
            dplRegion.Items.Add(" ")
            'sql = "select distinct class_03 from bedryf"
            sql = "select distinct class_03 from bedryf WHERE class_02 = 'Mindanao' and CLASS_03 <> '<none>'"   '--------> new ely code
            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            While rdr Is Nothing
                rdr = db.Execute_SQL_DataReader(sql)
            End While
            If rdr.HasRows Then
                While rdr.Read
                    item = Trim(rdr(0))
                    dplRegion.Items.Add(item.ToUpper)
                End While
            End If

            rdr.Close()
            db.CloseConnection()

        ElseIf dplWide.Text = "SHOWROOM" Then
            Dim strCon As String = Me.Session("strConfRShowroom")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String

            dplRegion.Enabled = True
            dplRegion.Items.Clear()
            dplArea.Items.Clear()
            dplBranch.Items.Clear()
            dplRegion.Items.Add(" ")
            'sql = "select distinct class_03 from bedryf"
            sql = "select distinct class_03 from bedryf WHERE class_02 = 'Showrooms' and CLASS_03 <> '<none>'"   '--------> new ely code
            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            If rdr.HasRows Then
                While rdr.Read
                    item = Trim(rdr(0))
                    dplRegion.Items.Add(item.ToUpper)
                End While
            End If

            rdr.Close()
            db.CloseConnection()


        ElseIf dplWide.SelectedValue = " " Then
            dplRegion.Enabled = False
            dplArea.Enabled = False
            dplBranch.Enabled = False
        End If
    End Sub
    Public Sub summary()
        dplSummary.Items.Add(" ")
        dplSummary.Items.Add("LUZON")
        dplSummary.Items.Add("VISAYAS")
        dplSummary.Items.Add("MINDANAO")
        dplSummary.Items.Add("SHOWROOM")
    End Sub

    Public Sub wide()
        dplWide.Items.Add(" ")
        dplWide.Items.Add("LUZON")
        dplWide.Items.Add("VISAYAS")
        dplWide.Items.Add("MINDANAO")
        dplWide.Items.Add("SHOWROOM")
    End Sub

    Protected Sub dplArea_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dplArea.SelectedIndexChanged

        dplBranch.Items.Clear()
        branch()
        'dplBranch.Enabled = True

    End Sub

    Public Sub branch()
        Dim item As String = Nothing
        Dim rdr As SqlClient.SqlDataReader = Nothing

        If dplWide.Text = "LUZON" And dplArea.SelectedValue <> " " Then
            Dim strCon As String = Me.Session("strConfRLuzon")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String

            'Dim area As String


            sql = "select distinct bedrnm from bedryf where class_04 = '" + dplArea.SelectedValue + "'"

            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            dplBranch.Enabled = True
            dplBranch.Items.Clear()
            dplBranch.Items.Add(" ")

            While rdr.Read
                item = Trim(rdr(0))
                dplBranch.Items.Add(item.ToUpper)
            End While

            rdr.Close()
            db.CloseConnection()

        ElseIf dplWide.Text = "VISAYAS" And dplArea.SelectedValue <> "" Then
            Dim strCon As String = Me.Session("strConfRVisayas")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String
            'Dim area As String

            sql = "select distinct bedrnm from bedryf where class_02 = 'Visayas' and class_04 = '" + dplArea.SelectedValue + "'"

            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            dplBranch.Enabled = True
            dplBranch.Items.Clear()
            dplBranch.Items.Add(" ")

            While rdr.Read
                item = Trim(rdr(0))
                dplBranch.Items.Add(item.ToUpper)
            End While

            rdr.Close()
            db.CloseConnection()

        ElseIf dplWide.Text = "MINDANAO" And dplArea.SelectedValue <> "" Then
            Dim strCon As String = Me.Session("strConfRMindanao")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String
            'Dim area As String

            sql = "select distinct bedrnm from bedryf where class_02 = 'Mindanao' and class_04 = '" + dplArea.SelectedValue + "'"

            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            dplBranch.Enabled = True
            dplBranch.Items.Clear()
            dplBranch.Items.Add(" ")

            While rdr.Read
                item = Trim(rdr(0))
                dplBranch.Items.Add(item.ToUpper)
            End While

            rdr.Close()
            db.CloseConnection()

        ElseIf dplWide.Text = "SHOWROOM" And dplArea.SelectedValue <> "" Then
            Dim strCon As String = Me.Session("strConfRShowroom")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String
            'Dim area As String

            sql = "select distinct bedrnm from bedryf where class_02 = 'Showrooms' and class_04 = '" + dplArea.SelectedValue + "'"

            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            dplBranch.Enabled = True
            dplBranch.Items.Clear()
            dplBranch.Items.Add(" ")

            While rdr.Read
                item = Trim(rdr(0))
                dplBranch.Items.Add(item.ToUpper)
            End While

            rdr.Close()
            db.CloseConnection()


        End If
    End Sub
    Public Sub branchSummary()
        Dim item As String = Nothing
        Dim rdr As SqlClient.SqlDataReader = Nothing

        If dplSummary.Text = "LUZON" And dplArea.SelectedValue <> " " Then
            Dim strCon As String = Me.Session("strConfRLuzon")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String

            'Dim area As String

            sql = "select distinct bedrnm from bedryf where class_04 = '" + dplArea.SelectedValue + "'"

            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            dplBranch.Enabled = False
            dplBranch.Items.Clear()
            dplBranch.Items.Add(" ")

            While rdr.Read
                item = Trim(rdr(0))
                dplBranch.Items.Add(item.ToUpper)
            End While

            rdr.Close()
            db.CloseConnection()

        ElseIf dplSummary.Text = "VISAYAS" And dplArea.SelectedValue <> "" Then
            Dim strCon As String = Me.Session("strConfRVisayas")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String
            'Dim area As String

            sql = "select distinct bedrnm from bedryf where class_02 = 'Visayas' and class_04 = '" + dplArea.SelectedValue + "'"

            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            dplBranch.Enabled = False
            dplBranch.Items.Clear()
            dplBranch.Items.Add(" ")

            While rdr.Read
                item = Trim(rdr(0))
                dplBranch.Items.Add(item.ToUpper)
            End While

            rdr.Close()
            db.CloseConnection()

        ElseIf dplSummary.Text = "MINDANAO" And dplArea.SelectedValue <> "" Then
            Dim strCon As String = Me.Session("strConfRMindanao")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String
            'Dim area As String

            sql = "select distinct bedrnm from bedryf where class_02 = 'Mindanao' and  class_04 = '" + dplArea.SelectedValue + "'"

            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            dplBranch.Enabled = False
            dplBranch.Items.Clear()
            dplBranch.Items.Add(" ")

            While rdr.Read
                item = Trim(rdr(0))
                dplBranch.Items.Add(item.ToUpper)
            End While

            rdr.Close()
            db.CloseConnection()

        ElseIf dplSummary.Text = "SHOWROOM" And dplArea.SelectedValue <> "" Then
            Dim strCon As String = Me.Session("strConfRShowroom")
            'Dim dr As SqlClient.SqlDataReader = Nothing
            Dim db As New clsDBConnection
            Dim sql As String
            'Dim area As String

            sql = "select distinct bedrnm from bedryf where class_02 = 'Showrooms' and class_04 = '" + dplArea.SelectedValue + "'"

            If db.isConnected Then
                db.CloseConnection()
            End If

            db.ConnectDB(strCon)
            rdr = db.Execute_SQL_DataReader(sql)

            dplBranch.Enabled = False
            dplBranch.Items.Clear()
            dplBranch.Items.Add(" ")

            While rdr.Read
                item = Trim(rdr(0))
                dplBranch.Items.Add(item.ToUpper)
            End While

            rdr.Close()
            db.CloseConnection()


        End If
    End Sub
    Protected Sub btnGenerate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGenerate.Click

        Me.Session("Regionname") = dplRegion.SelectedItem.Text


        'If dplWide.Text = "SHOWROOM" Or dplSummary.Text = "SHOWROOM" Then
        '    MsgBox("Not Available")
        '    Response.Redirect("MainMenu.aspx")
        'End If

        Me.Session.Add("dategen", txtStartDate.Text)
        Me.Session.Add("AreaN", Trim(dplArea.Text))
        Me.Session.Add("RegionN", Trim(dplRegion.Text))
        Me.Session.Add("BranchN", Trim(dplBranch.Text))
        Me.Session.Add("WideN", Trim(dplWide.Text))
        Me.Session.Add("Management", Trim(dplSummary.Text))
        'MsgBox(dplRegion.Text)
        If Me.IsPostBack Then

            If Not Date.TryParse(txtStartDate.Text, gen_date) Then
                'Response.Write("Please Input a valid date")
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "Please input a valid date." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
                'MsgBox("Please input valid date:", 64, "Cash Flow Version 3.0")
                Exit Sub
            End If
            gen_date = txtStartDate.Text
            If dplWide.Enabled = True Then
                Me.Session.Add("_SUMMARY", "Wide")
                If dplWide.SelectedValue <> " " And dplRegion.SelectedIndex <= 0 And dplArea.SelectedIndex <= 0 And dplBranch.SelectedIndex <= 0 Then

                    date_flag = False
                    wideR = dplWide.Text
                    'regionR = dplRegion.Text
                    regionR = "0"
                    areaR = "0"
                    branchR = "0"
                    Me.Session.Add("wideR", wideR)
                    Me.Session.Add("regionR", regionR)
                    Me.Session.Add("areaR", areaR)
                    Me.Session.Add("branchR", branchR)
                    Me.Session.Add("gen_date", gen_date)
                    Me.Session.Add("date_flag", date_flag)
                    genwide()

                ElseIf dplWide.SelectedValue <> " " And dplRegion.SelectedIndex > 0 And dplArea.SelectedIndex <= 0 And dplBranch.SelectedIndex <= 0 Then
                    date_flag = False
                    wideR = dplWide.Text
                    regionR = dplRegion.Text
                    'regionR = "0"
                    areaR = "0"
                    branchR = "0"
                    Me.Session.Add("wideR", wideR)
                    Me.Session.Add("regionR", regionR)
                    Me.Session.Add("areaR", areaR)
                    Me.Session.Add("branchR", branchR)
                    Me.Session.Add("gen_date", gen_date)
                    Me.Session.Add("date_flag", date_flag)
                    genRegion()

                ElseIf dplWide.SelectedValue <> " " And dplRegion.SelectedIndex > 0 And dplArea.SelectedIndex > 0 And dplBranch.SelectedIndex <= 0 Then
                    date_flag = False
                    wideR = dplWide.Text
                    areaR = dplArea.Text
                    regionR = "0"
                    'regionR = dplRegion.Text
                    branchR = "0"
                    Me.Session.Add("wideR", wideR)
                    Me.Session.Add("regionR", regionR)
                    Me.Session.Add("areaR", areaR)
                    Me.Session.Add("branchR", branchR)
                    Me.Session.Add("gen_date", gen_date)
                    Me.Session.Add("date_flag", date_flag)
                    genArea()

                ElseIf dplWide.SelectedValue <> " " And dplRegion.SelectedIndex > 0 And dplArea.SelectedIndex > 0 And dplBranch.SelectedIndex > 0 Then
                    date_flag = False
                    branchR = dplBranch.Text
                    areaR = dplArea.Text
                    wideR = dplWide.Text
                    regionR = "0"
                    'regionR = dplRegion.Text
                    Me.Session.Add("wideR", wideR)
                    Me.Session.Add("regionR", regionR)
                    Me.Session.Add("areaR", areaR)
                    Me.Session.Add("branchR", branchR)
                    Me.Session.Add("gen_dateR", gen_date)
                    Me.Session.Add("date_flag", date_flag)
                    genbranch()

                ElseIf dplWide.Text = " " And txtStartDate.Text <> " " Then
                    date_flag = True
                    Dim dt As String = txtStartDate.Text
                    'Dim ho As String = dplWide.Text
                    Me.Session.Add("dt", dt)
                    Me.Session.Add("date_flag", date_flag)
                    'Me.Session.Add("ho", ho)
                    Response.Write("<script language=javascript>")
                    Response.Write("alert('" & "Choose H.O.!" & "')")
                    Response.Write("</script>")
                    Response.Write("<script language=javascript>")
                    Response.Write("window.location = 'MainMenu.aspx'")
                    Response.Write("</script>")
                End If
            ElseIf dplSummary.Enabled = True Then
                SummaryCon()
            End If
        End If
    End Sub
    Public Sub SummaryCon()
        Me.Session.Add("_SUMMARY", "SUMMARY")
        If dplSummary.SelectedValue <> " " And dplRegion.SelectedIndex <= 0 And dplArea.SelectedIndex <= 0 And dplBranch.SelectedIndex <= 0 Then

            date_flag = False
            wideR = dplSummary.Text
            regionR = "0"
            areaR = "0"
            branchR = "0"
            Me.Session.Add("wideR", wideR)
            Me.Session.Add("regionR", regionR)
            Me.Session.Add("areaR", areaR)
            Me.Session.Add("branchR", branchR)
            Me.Session.Add("gen_date", gen_date)
            Me.Session.Add("date_flag", date_flag)
            genSummary()

        ElseIf dplSummary.SelectedValue <> " " And dplRegion.SelectedIndex > 0 And dplArea.SelectedIndex <= 0 And dplBranch.SelectedIndex <= 0 Then
            date_flag = False
            wideR = dplSummary.Text
            regionR = dplRegion.Text
            areaR = "0"
            branchR = "0"
            Me.Session.Add("wideR", wideR)
            Me.Session.Add("regionR", regionR)
            Me.Session.Add("areaR", areaR)
            Me.Session.Add("branchR", branchR)
            Me.Session.Add("gen_date", gen_date)
            Me.Session.Add("date_flag", date_flag)
            genRegion()

        ElseIf dplSummary.SelectedValue <> " " And dplRegion.SelectedIndex > 0 And dplArea.SelectedIndex > 0 And dplBranch.SelectedIndex <= 0 Then
            date_flag = False
            wideR = dplSummary.Text
            areaR = dplArea.Text
            regionR = "0"
            branchR = "0"
            Me.Session.Add("wideR", wideR)
            Me.Session.Add("regionR", regionR)
            Me.Session.Add("areaR", areaR)
            Me.Session.Add("branchR", branchR)
            Me.Session.Add("gen_date", gen_date)
            Me.Session.Add("date_flag", date_flag)
            genArea()

        ElseIf dplSummary.SelectedValue <> " " And dplRegion.SelectedIndex > 0 And dplArea.SelectedIndex > 0 And dplBranch.SelectedIndex > 0 Then
            date_flag = False
            branchR = dplBranch.Text
            areaR = dplArea.Text
            wideR = dplWide.Text
            regionR = "0"
            Me.Session.Add("wideR", wideR)
            Me.Session.Add("regionR", regionR)
            Me.Session.Add("areaR", areaR)
            Me.Session.Add("branchR", branchR)
            Me.Session.Add("gen_dateR", gen_date)
            Me.Session.Add("date_flag", date_flag)
            genbranch()

        ElseIf dplSummary.Text = " " And txtStartDate.Text <> " " Then
            date_flag = True
            Dim dt As String = txtStartDate.Text
            'Dim ho As String = dplWide.Text
            Me.Session.Add("dt", dt)
            Me.Session.Add("date_flag", date_flag)
            'Me.Session.Add("ho", ho)
            Response.Write("<script language=javascript>")
            Response.Write("alert('" & "Choose H.O.!" & "')")
            Response.Write("</script>")
            Response.Write("<script language=javascript>")
            Response.Write("window.location = 'MainMenu.aspx'")
            Response.Write("</script>")
        End If
    End Sub

    Public Sub genwide()
        Dim strConR As String = Nothing
        Dim rdr As SqlClient.SqlDataReader = Nothing
        Dim dbr As New clsDBConnection
        Dim sql As String = Nothing
        Dim t_date As String = Convert.ToString(gen_date)
        Dim t2_date As String = t_date.Substring(0, t_date.Length - 12)

        If dplWide.Text = "LUZON" Then
            reportR = "CF_Luzon_Wide"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()

            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)
            While rdr Is Nothing
                rdr = dbr.Execute_SQL_DataReader(sql)
            End While
            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If
            'If rdr.Read Then
            '    Me.Session.Add("gen_date", gen_date)
            '    Me.Session.Add("reportR", reportR)
            '    Me.Session.Add("wideR", wideR)
            '    Me.Session.Add("regionR", regionR)
            '    Me.Session.Add("areaR", areaR)
            '    Me.Session.Add("branchR", branchR)
            '    'Response.Redirect("CashFlowReport.aspx")
            '    CrystalReportViewer1.ReportSource = Server.MapPath("~/CFRVW.rpt")
            '    CrystalReportViewer1.Visible = True

            '    'MemoryStream oStream // using System.IO
            '    'IO.Stream = (MemoryStream)
            '    rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat)
            '    Response.Clear()
            '    Response.Buffer = True
            '    Response.ContentType = "application/pdf"
            '    'Response.BinaryWrite(oStream.ToArray())
            '    Response.End()
            'Else
            '    Response.Write("<script language=javascript>")
            '    Response.Write("alert('" & "No data to view." & "')")
            '    Response.Write("</script>")
            '    Response.Write("<script language=javascript>")
            '    Response.Write("window.location = 'MainMenu.aspx'")
            '    Response.Write("</script>")
            'End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplWide.Text = "VISAYAS" Then

            reportR = "CF_Vismin_Wide"
            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplWide.Text = "MINDANAO" Then

            reportR = "CF_Vismin_Wide"
            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplWide.Text = "SHOWROOM" Then

            reportR = "CF_Showroom_Wide"
            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()
        End If
    End Sub
    Public Sub genSummary()
        Dim strConR As String = Nothing
        Dim rdr As SqlClient.SqlDataReader = Nothing
        Dim dbr As New clsDBConnection
        Dim sql As String = Nothing
        Dim t_date As String = Convert.ToString(gen_date)
        Dim t2_date As String = t_date.Substring(0, t_date.Length - 12)

        If dplSummary.Text = "LUZON" Then
            reportR = "CF_Luzon_Wide"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)
            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)

                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If
            'If rdr.Read Then
            '    Me.Session.Add("gen_date", gen_date)
            '    Me.Session.Add("reportR", reportR)
            '    Me.Session.Add("wideR", wideR)
            '    Me.Session.Add("regionR", regionR)
            '    Me.Session.Add("areaR", areaR)
            '    Me.Session.Add("branchR", branchR)
            '    'Response.Redirect("CashFlowReport.aspx")
            '    CrystalReportViewer1.ReportSource = Server.MapPath("~/CFRVW.rpt")
            '    CrystalReportViewer1.Visible = True

            '    'MemoryStream oStream // using System.IO
            '    'IO.Stream = (MemoryStream)
            '    rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat)
            '    Response.Clear()
            '    Response.Buffer = True
            '    Response.ContentType = "application/pdf"
            '    'Response.BinaryWrite(oStream.ToArray())
            '    Response.End()
            'Else
            '    Response.Write("<script language=javascript>")
            '    Response.Write("alert('" & "No data to view." & "')")
            '    Response.Write("</script>")
            '    Response.Write("<script language=javascript>")
            '    Response.Write("window.location = 'MainMenu.aspx'")
            '    Response.Write("</script>")
            'End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplSummary.Text = "VISAYAS" Then

            reportR = "CF_Vismin_Wide"
            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplSummary.Text = "MINDANAO" Then

            reportR = "CF_Vismin_Wide"
            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplSummary.Text = "SHOWROOM" Then

            reportR = "CF_Showroom_Wide"
            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        End If
    End Sub

    Public Sub genRegion()
        'Dim strConR As String = Nothing JENIENA
        'Dim rdr As SqlClient.SqlDataReader = Nothing
        'Dim dbr As New clsDBConnection
        'Dim sql As String = Nothing
        'Dim t_date As String = Convert.ToString(gen_date)
        'Dim t2_date As String = t_date.Substring(0, t_date.Length - 12)
        'Dim wRegion As String = Me.Session("regionR")

        Dim strConR As String = Nothing
        Dim rdr As SqlClient.SqlDataReader = Nothing
        Dim dbr As New clsDBConnection
        Dim sql As String = Nothing
        Dim timeformat As String = "yyyy-MM-dd"


        Dim t_date As String = gen_date.ToString(timeformat)

        ' Dim t2_date As String = t_date.Substring(0, t_date.Length - 12)


        If dplWide.Text = "LUZON" Then
            reportR = "CF_Region_Luzon"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()

            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplWide.Text = "VISAYAS" Then
            reportR = "CF_Region_Vismin"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplWide.Text = "MINDANAO" Then
            reportR = "CF_Region_Vismin"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplWide.Text = "SHOWROOM" Then
            reportR = "CF_Region_Showroom"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        End If
    End Sub
    Public Sub genRegionSum()
        Dim strConR As String = Nothing
        Dim rdr As SqlClient.SqlDataReader = Nothing
        Dim dbr As New clsDBConnection
        Dim sql As String = Nothing
        Dim t_date As String = Convert.ToString(gen_date)
        Dim t2_date As String = t_date.Substring(0, t_date.Length - 12)
        Dim wRegion As String = Me.Session("regionR")

        If dplSummary.Text = "Luzon" Then
            reportR = "CF_Region_Luzon"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            Me.Session("Regionname") = dplRegion.SelectedItem.Text ' JENIENA

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplSummary.Text = "VISAYAS" Then
            reportR = "CF_Region_Vismin"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplSummary.Text = "MINDANAO" Then
            reportR = "CF_Region_Vismin"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplSummary.Text = "SHOWROOM" Then
            reportR = "CF_Region_Showroom"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        End If
    End Sub

    Protected Sub dplWide_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dplWide.SelectedIndexChanged
        Disablewide()
        dplRegion.Items.Clear()
        dplArea.Items.Clear()
        dplBranch.Items.Clear()
        region()
    End Sub

    Public Sub genArea()
       
        Dim strConR As String = Nothing
        Dim rdr As SqlClient.SqlDataReader = Nothing
        Dim dbr As New clsDBConnection
        Dim sql As String = Nothing
        Dim timeformat As String = "yyyy-MM-dd"


        Dim t_date As String = gen_date.ToString(timeformat)

        ' Dim t2_date As String = t_date.Substring(0, t_date.Length - 12)




        Dim wArea As String = Me.Session("areaR")

        If dplWide.Text = "LUZON" Then
            reportR = "CF_Area_Luzon"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplWide.Text = "VISAYAS" Then
            reportR = "CF_Area_Vismin"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplWide.Text = "MINDANAO" Then
            reportR = "CF_Area_Vismin"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplWide.Text = "SHOWROOM" Then
            reportR = "CF_Area_Showroom"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()


        End If
    End Sub
    Public Sub genAreaSum()
        Dim strConR As String = Nothing
        Dim rdr As SqlClient.SqlDataReader = Nothing
        Dim dbr As New clsDBConnection
        Dim sql As String = Nothing
        Dim t_date As String = Convert.ToString(gen_date)
        Dim t2_date As String = t_date.Substring(yy - mm - dd) '(0, t_date.Length - 12)

        If dplSummary.Text = "LUZON" Then
            reportR = "CF_Area_Luzon"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            Me.Session("Regionname") = dplRegion.SelectedItem.Text 'AJENIENA

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplSummary.Text = "VISAYAS" Then
            reportR = "CF_Area_Vismin"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplSummary.Text = "MINDANAO" Then
            reportR = "CF_Area_Vismin"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplSummary.Text = "SHOWROOM" Then
            reportR = "CF_Area_Showroom"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        End If
    End Sub

    Public Sub genbranch()
        Dim strConR As String = Nothing
        Dim rdr As SqlClient.SqlDataReader = Nothing
        Dim dbr As New clsDBConnection
        Dim sql As String = Nothing
        Dim t_date As String = Convert.ToString(gen_date)
        Dim t2_date As String = t_date.Substring(0, t_date.Length - 12)
        Dim wBranch As String = Me.Session("branchR")

        If dplWide.Text = "LUZON" Then
            reportR = "CF_pb_Luzon"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If
            Me.Session("Regionname") = dplRegion.SelectedItem.Text

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplWide.Text = "VISAYAS" Then
            reportR = "CF_pb_Vismin"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplWide.Text = "MINDANAO" Then
            reportR = "CF_pb_Vismin"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplWide.Text = "SHOWROOM" Then
            reportR = "CF_pb_Showroom"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()
        End If
    End Sub
    Public Sub genbranchSum()
        Dim strConR As String = Nothing
        Dim rdr As SqlClient.SqlDataReader = Nothing
        Dim dbr As New clsDBConnection
        Dim sql As String = Nothing
        Dim t_date As String = Convert.ToString(gen_date)
        Dim t2_date As String = t_date.Substring(0, t_date.Length - 12)
        Dim wBranch As String = Me.Session("branchR")

        If dplSummary.Text = "LUZON" Then
            reportR = "CF_pb_Luzon"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplSummary.Text = "VISAYAS" Then
            reportR = "CF_pb_Vismin"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplSummary.Text = "MINDANAO" Then
            reportR = "CF_pb_Vismin"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()

        ElseIf dplSummary.Text = "SHOWROOM" Then
            reportR = "CF_pb_Showroom"

            sql = "exec SP_CashFlowVer3Report" & " " & "'" + t2_date + "'" & "," & "'" + wideR + "'" & "," & "'" + regionR + "'" & "," & "'" + areaR + "'" & "," & "'" + branchR + "'" & "," & "'" + reportR + "'"

            If dbr.isConnected Then
                dbr.CloseConnection()
            End If

            IniReport()
            strConR = Me.Session("strConfReport")
            dbr.ConnectDB(strConR)
            rdr = dbr.Execute_SQL_DataReader(sql)

            If rdr.Read Then
                Me.Session.Add("gen_date", gen_date)
                Me.Session.Add("reportR", reportR)
                Me.Session.Add("wideR", wideR)
                Me.Session.Add("regionR", regionR)
                Me.Session.Add("areaR", areaR)
                Me.Session.Add("branchR", branchR)
                Response.Redirect("CashFlowReport.aspx")
            Else
                Response.Write("<script language=javascript>")
                Response.Write("alert('" & "No data to view." & "')")
                Response.Write("</script>")
                Response.Write("<script language=javascript>")
                Response.Write("window.location = 'MainMenu.aspx'")
                Response.Write("</script>")
            End If

            rdr.Close()
            dbr.CloseConnection()
        End If
    End Sub
    Private Sub CheckLogin()
        If Me.Session("strConfR") = "" OrElse HttpContext.Current.Session("uname") = "" Then
            Response.Redirect("Login.aspx")
        End If
    End Sub

    Protected Sub dplSummary_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dplSummary.SelectedIndexChanged
        DisableSummary()
        regionSummary()
        areaSummary()
        branchSummary()

    End Sub
    Public Sub DisableSummary()
        If dplSummary.Text = " " Then
            dplWide.Enabled = True

        Else

            dplWide.Enabled = False

        End If
    End Sub
    Public Sub Disablewide()
        If dplWide.Text = " " Then
            dplSummary.Enabled = True

        Else
            dplSummary.Enabled = False

        End If
    End Sub

    
End Class
