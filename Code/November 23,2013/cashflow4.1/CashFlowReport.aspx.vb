Imports System
Imports System.IO
Imports INI_DLL
Imports DB_DLL
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports System.Web.Configuration
Imports System.Data.SqlClient
Imports System.Data
Imports System.Web.UI.WebControls

Partial Class CashFlowReport
    Inherits System.Web.UI.Page
    Dim serverR, dbR, unameR, passR As String
    Public branch_Update As Boolean
    Dim strCon As String = Nothing
    Dim db As New clsDBConnection
    'Dim rdr As SqlClient.SqlDataReader
    Dim bcode As String = Nothing
    Dim begbal As Double = Nothing
    Dim endingbal As Double = Nothing
    Dim foodproducts As Double = Nothing
    Dim insurance As Double = Nothing
    Dim outrightsales As Double = Nothing
    Dim layaway As Double = Nothing
    Dim salesreturn As Double = Nothing
    Dim layawaycancel As Double = Nothing
    Dim interest As Double = Nothing
    Dim kpsendout As Double = Nothing
    Dim kpPayout As Double = Nothing
    Dim kpsendoutcomm As Double = Nothing
    Dim lukat As Double = Nothing
    Dim rematado As Double = Nothing
    Dim otherincome As Double = Nothing
    Dim prenda As Double = Nothing
    Dim telecomms As Double = Nothing
    Dim souvenirs As Double = Nothing
    Dim corpsendout As Double = Nothing
    Dim corppayout As Double = Nothing
    Dim corpcomm As Double = Nothing
    Dim travelandtours As Double = Nothing
    Dim healthandcare As Double = Nothing
    Dim WesternUnionSendout As Double = Nothing
    Dim fundtransferdepositfrombank As Double = Nothing
    Dim FundTransferDebit As Double = Nothing
    Dim fundtransferWithDrawalfrombankCredit As Double = Nothing
    Dim FundTransferCredit As Double = Nothing
    Dim BranchExpense As Double = Nothing
    Dim OtherExpenseNotRMBase As Double = Nothing
    Dim OtherExpense As Double = Nothing
    Dim nso As Double = Nothing
    Dim mccr As Double = Nothing
    Dim mccd As Double = Nothing
    Dim racr As Double = Nothing
    Dim racd As Double = Nothing
    Dim dfb As Double = Nothing
    Dim wfb As Double = Nothing
    Dim rts As Double = Nothing
    Dim cashover As Double = Nothing
    Dim cashshort As Double = Nothing
    Dim ld_lukat As Double = Nothing
    Dim totalreceipts As Double = Nothing
    Dim totaldisbursements As Double = Nothing
    Dim strCon1 As String
    'Public Sub RunSP()
    '    With CurrentDb.CreateQueryDef("")
    '        .Connect = "<valid ODBC connect string>"
    '        .SQL = "EXEC StoredProcedure1"
    '        .ODBCTimeout = 300 'Seconds
    '        .ReturnsRecords = False
    '        .Execute()
    '    End With

    'End Sub
    Private Sub IniReportSetup()
        Dim ini_Path As String = AppDomain.CurrentDomain.BaseDirectory + "cashflowv3report.ini"
        Dim line As String = Nothing

        Dim rdr As New ReadWriteINI
        Dim strConfR As String

        serverR = rdr.readINI("SERVER INI", "SERVER", False, ini_Path)
        dbR = rdr.readINI("SERVER INI", "DBNAME", False, ini_Path)
        unameR = rdr.readINI("SERVER INI", "USERNAME", False, ini_Path)
        passR = rdr.readINI("SERVER INI", "PASSWORD", False, ini_Path)
        strConfR = "user id=" & unameR & ";password=" & passR & ";data source=" & serverR & ";persist security info=False;initial catalog=" & dbR & "; Connection Timeout = 3600;"
        Me.Session.Add("strConfR", strConfR)
    End Sub

  
    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        Response.Redirect("MainMenu.aspx")
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'strCon = Me.Session("strConfRVismin")

        CheckLogin()

        Dim con As New clsDBConnection
        Dim rpt As New ReportDocument


        Dim rGen_date As String = Me.Session("gen_date")
        Dim rReport As String = Me.Session("reportR")
        Dim rWide As String = Me.Session("wideR")
        Dim rRegion As String = Me.Session("regionR")
        Dim rArea As String = Me.Session("areaR")
        Dim rBranch As String = Me.Session("branchR")
        Dim condi As String = Me.Session("_SUMMARY")

        'If rArea = "0" And rBranch = "0" Then 
        '    rRegion = Me.Session("Regionname")
        'End If
        

        Dim sp As String = "SP_CashFlowVer3Report"
        'Dim Filename As String = rBranch + rGen_date

        Dim reportPath As String = AppDomain.CurrentDomain.BaseDirectory

        Dim crLogin As New TableLogOnInfo
        Dim crConnectionInfo1 As New ConnectionInfo
        Dim crTables1 As Tables = Nothing

        rpt = New ReportDocument

        If condi = "SUMMARY" Then

            rpt.Load(reportPath & "CFRVW1.rpt")
        Else

            rpt.Load(reportPath & "CFRVW.rpt")
        End If
        rpt.SetParameterValue("@gen_date", rGen_date)
        rpt.SetParameterValue("@wide", rWide)

        rpt.SetParameterValue("@region", rRegion)

        'If rRegion = "" Then 'JENIENA 

        '    rRegion = Me.Session("Regionname")
        'Else
        '    rpt.SetParameterValue("@region", "0")

        'End If
        rpt.SetParameterValue("@area", rArea)
        rpt.SetParameterValue("@branch", rBranch)
        rpt.SetParameterValue("@report", rReport)
        'rpt.ParameterFields(0) = "rptTitle;" & rBranch + rGen_date & ";true"

        IniReportSetup()
        strCon1 = Me.Session("strConfR")

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
    End Sub

    Private Sub CheckLogin()
        If Me.Session("uname") = "" Then
            Response.Redirect("Login.aspx")
        End If
    End Sub

    Protected Sub UpdateBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles UpdateBtn.Click
        '.Timeout = System.Threading.Timeout.Infinite

        Me.Page.Form.Disabled = True

        'System.Threading.Thread.Sleep(1000)
        Dim str As String = Trim(Me.Session("WideN"))

        If Trim(Me.Session("WideN")) = "VISAYAS" Or Trim(Me.Session("Management")) = "VISAYAS" Then
            If Trim(Me.Session("BranchN")) <> "" Then
                generatebranch()
                Bqueries(bcode)
                Response.Redirect("cashflowreport.aspx")
                Me.Page.Form.Disabled = False
                Exit Sub
            ElseIf Trim(Me.Session("AreaN")) <> "" Then
                generateArea(Me.Session("AreaN"))
                Aqueries(Me.Session("AreaN"))
                Response.Redirect("cashflowreport.aspx")
                Me.Page.Form.Disabled = False
                Exit Sub
            ElseIf Trim(Me.Session("RegionN")) <> "" Then
                generateRegion(Me.Session("RegionN"))
                Rqueries(Me.Session("RegionN"))
                Response.Redirect("cashflowreport.aspx")
                Me.Page.Form.Disabled = False
                Exit Sub
            ElseIf Trim(Me.Session("WideN")) <> "" Or Trim(Me.Session("Management")) <> "" Then
                'generateWideVismin()
                Vismin_Wqueries()
                Response.Redirect("cashflowreport.aspx")
                Me.Page.Form.Disabled = False
                Exit Sub
            End If
            '------------------------------------------------------
        ElseIf Trim(Me.Session("WideN")) = "MINDANAO" Or Trim(Me.Session("Management")) = "MINDANAO" Then
            If Trim(Me.Session("BranchN")) <> "" Then
                generatebranch()
                Bqueries(bcode)
                'MdalForm.Hide()
                Response.Redirect("cashflowreport.aspx")
                Me.Page.Form.Disabled = False
                Exit Sub
            ElseIf Trim(Me.Session("AreaN")) <> "" Then
                generateArea(Me.Session("AreaN"))
                Aqueries(Me.Session("AreaN"))
                'MdalForm.Hide()
                Response.Redirect("cashflowreport.aspx")
                Me.Page.Form.Disabled = False
                Exit Sub
            ElseIf Trim(Me.Session("RegionN")) <> "" Then
                generateRegion(Me.Session("RegionN"))
                Rqueries(Me.Session("RegionN"))
                'MdalForm.Hide()
                Response.Redirect("cashflowreport.aspx")
                Me.Page.Form.Disabled = False
                Exit Sub
            ElseIf Trim(Me.Session("WideN")) <> "" Or Trim(Me.Session("Management")) <> "" Then
                ' generateWideVismin()
                Vismin_Wqueries()
                'MdalForm.Hide()
                Response.Redirect("cashflowreport.aspx")
                Me.Page.Form.Disabled = False
                Exit Sub
            End If
            '----------------------------------------------------

        ElseIf Trim(Me.Session("WideN")) = "LUZON" Or Trim(Me.Session("Management")) = "LUZON" Then
            If Trim(Me.Session("BranchN")) <> "" Then
                generatebranch()
                Bqueries(bcode)
                'MdalForm.Hide()
                Response.Redirect("cashflowreport.aspx")
                Me.Page.Form.Disabled = False
                Exit Sub
            ElseIf Trim(Me.Session("AreaN")) <> "" Then
                generateArea(Me.Session("AreaN"))
                Aqueries(Me.Session("AreaN"))
                'MdalForm.Hide()
                Response.Redirect("cashflowreport.aspx")
                Me.Page.Form.Disabled = False
                Exit Sub
            ElseIf Trim(Me.Session("RegionN")) <> "" Then
                generateRegion(Me.Session("RegionN"))
                Rqueries(Me.Session("RegionN"))
                'MdalForm.Hide()
                Response.Redirect("cashflowreport.aspx")
                Me.Page.Form.Disabled = False
                Exit Sub
            ElseIf Trim(Me.Session("WideN")) <> "" Or Trim(Me.Session("Management")) <> "" Then
                ' generateWideVismin()
                Vismin_Wqueries()
                'MdalForm.Hide()
                Response.Redirect("cashflowreport.aspx")
                Me.Page.Form.Disabled = False
                Exit Sub
            End If
            '----------------------------------------------------

        ElseIf Trim(Me.Session("WideN")) = "SHOWROOM" Or Trim(Me.Session("Management")) = "SHOWROOM" Then
            If Trim(Me.Session("BranchN")) <> "" Then
                generatebranch()
                Bqueries(bcode)
                'MdalForm.Hide()
                Response.Redirect("cashflowreport.aspx")
                Me.Page.Form.Disabled = False
                Exit Sub
            ElseIf Trim(Me.Session("AreaN")) <> "" Then
                generateArea(Me.Session("AreaN"))
                Aqueries(Me.Session("AreaN"))
                'MdalForm.Hide()
                Response.Redirect("cashflowreport.aspx")
                Me.Page.Form.Disabled = False
                Exit Sub
            ElseIf Trim(Me.Session("RegionN")) <> "" Then
                generateRegion(Me.Session("RegionN"))
                Rqueries(Me.Session("RegionN"))
                'MdalForm.Hide()
                Response.Redirect("cashflowreport.aspx")
                Me.Page.Form.Disabled = False
                Exit Sub
            ElseIf Trim(Me.Session("WideN")) <> "" Or Trim(Me.Session("Management")) <> "" Then
                ' generateWideVismin()
                Vismin_Wqueries()
                'MdalForm.Hide()
                Response.Redirect("cashflowreport.aspx")
                Me.Page.Form.Disabled = False
                Exit Sub
            End If
        End If
        'MsgBox("Not Available")
    End Sub
    Private Sub generatebranch()

        Dim sql As String = "select bedrnr from bedryf where bedrnm = '" & Trim(Me.Session("BranchN")) & "' and class_04 = '" & Trim(Me.Session("AreaN")) & "' and class_03 = '" & Trim(Me.Session("RegionN")) & "'"

        If Me.Session("WideN") = "VISAYAS" Then
            strCon = Me.Session("strConfRVisayas")

        ElseIf Me.Session("WideN") = "MINDANAO" Then
        strCon = Me.Session("strConfRMindanao")

        ElseIf Me.Session("WideN") = "LUZON" Then
            strCon = Me.Session("strConfRLuzon")

        ElseIf Me.Session("WideN") = "SHOWROOM" Then
            strCon = Me.Session("strConfRShowroom")
        End If
        db.ConnectDB(strCon)
        Dim rdr As SqlClient.SqlDataReader = db.Execute_SQL_DataReader(sql)
        If rdr.Read Then
            bcode = rdr("bedrnr").ToString
        End If
        rdr.Close()
        db.CloseConnection()
    End Sub

    Private Sub Bqueries(ByVal branchcode As String)
        Dim rdr As SqlClient.SqlDataReader
        Dim begbal1 As Double
        Dim begbal2 As Double
       
        ' beggining balance 10/22/2012
        Dim ls_begbal As String = "select dbo.EXEC_SF_1000006CASHBALANCEPESO_CF_V3 ('" + Me.Session("dategen") + "','" + branchcode + "')"
        db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_begbal)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_begbal)
        End While

        If rdr.Read Then
            Dim str_begbal As String = Trim(rdr(0).ToString)
            If str_begbal <> "" Then
                begbal1 = CDbl(rdr(0).ToString)
            Else
                begbal1 = 0
            End If
        End If
       
        rdr.Close()
        db.CloseConnection()

        Dim ls_begbal2 As String = "select dbo.EXEC_SF_1000004CASHBALANCEPESO_CF_V3 ('" + Me.Session("dategen") + "','" + branchcode + "')"
        db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_begbal2)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_begbal2)
        End While

        If rdr.Read Then
            Dim str_begbal2 As String = Trim(rdr(0).ToString)
            If str_begbal2 <> "" Then
                begbal2 = CDbl(rdr(0).ToString)
            Else
                begbal2 = 0
            End If
        End If
        rdr.Close()

        begbal = begbal1 + begbal2
        db.CloseConnection()
        'foodproduct 10/22/2012
        Dim ls_foodproducts As String = "select -1 * sum(totalfoodproducts) as totalfoodproducts from vwCashFlowFoodProducts_CF_ver3 " & _
            "where transdate = '" + Me.Session("dategen") + "' and branchcode = '" + branchcode + "' group by branchcode "
        db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_foodproducts)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_foodproducts)
        End While
        If rdr.Read Then
            Dim fp As String = Trim(rdr(0).ToString)
            If fp <> "" Then
                foodproducts = CDbl(rdr(0).ToString)
            Else
                foodproducts = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        'insurance 10/22/2012
        Dim ls_insurance As String = "select -1 * sum(InsuranceAmt) from " & _
          " vwCashFlowInsurance_CF_ver3 where branchcode = '" + branchcode + "' and transdate = '" + Me.Session("dategen") + "'"
        db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_insurance)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_insurance)
        End While
        If rdr.Read Then
            Dim Str_insurance As String = Trim(rdr(0).ToString)
            If Str_insurance <> "" Then
                insurance = CDbl(rdr(0).ToString)
            Else
                insurance = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()


        Dim outrightsales As String = "select dbo.SF_OUTRIGHTSALES_SHOWROOM ('" + Me.Session("dategen") + "','" + Me.Session("areaR") + "','" + branchcode + "','" + Me.Session("Regionname") + "')"
        db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(outrightsales)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(outrightsales)
        End While

        If rdr.Read Then
            Dim str_outrightsales As String = Trim(rdr(0).ToString)
            If str_outrightsales <> "" Then
                outrightsales = CDbl(rdr(0).ToString)
                If outrightsales.Contains("-") Then

                    outrightsales = outrightsales * -1
                Else
                    outrightsales = 0
                End If

            End If
        End If
        rdr.Close()
        db.CloseConnection()

        Dim layaway As String = "select dbo.SF_LAYAWAY_SHOWROOM ('" + Me.Session("dategen") + "','" + Me.Session("areaR") + "','" + branchcode + "','" + Me.Session("Regionname") + "')" ' change branchcode instead of branchname
        db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(layaway)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(layaway)
        End While

        If rdr.Read Then
            Dim str_layaway As String = Trim(rdr(0).ToString)
            If str_layaway <> "" Then
                layaway = CDbl(rdr(0).ToString)
                If layaway.Contains("-") Then

                    layaway = layaway * -1
                Else
                    layaway = 0
                End If

            End If
        End If
        rdr.Close()
        db.CloseConnection()


        Dim salesreturn As String = "select dbo.SF_SALESRETURN_SHOWROOM ('" + Me.Session("dategen") + "','" + Me.Session("areaR") + "','" + branchcode + "','" + Me.Session("Regionname") + "')"
        db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(salesreturn)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(salesreturn)
        End While

        If rdr.Read Then
            Dim str_salesreturn As String = Trim(rdr(0).ToString)
            If str_salesreturn <> "" Then
                salesreturn = CDbl(rdr(0).ToString)

                If salesreturn.Contains("-") Then

                    salesreturn = salesreturn * -1
                Else
                    salesreturn = 0
                End If
          
            End If
        End If
        rdr.Close()
        db.CloseConnection()


        Dim layawaycancel As String = "select dbo.SF_LAYAWAYCANCEL_SHOWROOM ('" + Me.Session("dategen") + "','" + Me.Session("areaR") + "','" + branchcode + "','" + Me.Session("Regionname") + "')"
        db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(layawaycancel)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(layawaycancel)
        End While

        If rdr.Read Then
            Dim str_layawaycancel As String = Trim(rdr(0).ToString)
            If str_layawaycancel <> "" Then
                layawaycancel = CDbl(rdr(0).ToString)

                If layawaycancel.Contains("-") Then

                    layawaycancel = layawaycancel * -1
                    'Else
                    '    layawaycancel = 0
                End If

            End If
        End If
        rdr.Close()
        db.CloseConnection()
                
                Dim ls_interest As String = "select -1 *  sum(interestamt) from" & _
                  " vwcashflowinterest_CF_ver3 where branchcode = '" + branchcode + "' and transdate = '" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_interest)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_interest)
        End While
                If rdr.Read Then
                    Dim str_insurance As String = Trim(rdr(0).ToString)
                    If str_insurance <> "" Then
                        interest = CDbl(rdr(0).ToString)
                    Else
                        interest = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()
                Dim ls_kppayout As String = "select sum(kppayoutamt) from " & _
                   " vwCashFlowKPPayout_CF_ver3 where branchcode = '" + branchcode + "' and transdate = '" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_kppayout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_kppayout)
        End While
                If rdr.Read Then
                    Dim str_kpPayOut As String = Trim(rdr(0).ToString)
                    If str_kpPayOut <> "" Then
                        kpPayout = CDbl(rdr(0).ToString)
                    Else
                        kpPayout = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()
                Dim ls_kpsendout As String = "select -1 * sum(kpsendoutamt) " & _
                           " from vwCashFlowKPSendout_CF_ver3 where branchcode = '" + branchcode + "' and transdate = '" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_kpsendout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_kpsendout)
        End While
                If rdr.Read Then
                    Dim str_kpSendOut As String = Trim(rdr(0).ToString)
                    If str_kpSendOut <> "" Then
                        kpsendout = CDbl(rdr(0).ToString)
                    Else
                        kpsendout = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

                Dim ls_kpsendoutcomm As String = "select -1 * sum(KPCOMMISSIONAMT) " & _
                   " from vwCashFlowKPCommission_CF_ver3 where branchcode ='" + branchcode + "' and transdate = '" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_kpsendoutcomm)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_kpsendoutcomm)
        End While
                If rdr.Read Then
                    Dim str_kpSendOutComm As String = Trim(rdr(0).ToString)
                    If str_kpSendOutComm <> "" Then
                        kpsendoutcomm = CDbl(rdr(0).ToString)
                    Else
                        kpsendoutcomm = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

                Dim ls_lukat As String = "select -1 * sum(lukatamt) " & _
                   " from vwCashFlowLukat_CF_Ver3 where branchcode = '" + branchcode + "' and transdate = '" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_lukat)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_lukat)
        End While
                If rdr.Read Then
                    Dim Str_lukat As String = Trim(rdr(0).ToString)
                    If Str_lukat <> "" Then
                        lukat = CDbl(rdr(0).ToString)
                    Else
                        lukat = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

                Dim ls_rematado As String = "select sum(totrematado) from" & _
                   " vwCashFlowRematado_CF_ver3 where branchcode = '" + branchcode + "' and transdate = '" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_rematado)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_rematado)
        End While
                If rdr.Read Then
                    Dim str_rematado As String = Trim(rdr(0).ToString)
                    If str_rematado <> "" Then
                        rematado = CDbl(rdr(0).ToString)
                    Else
                        rematado = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

                Dim ls_otherincome As String = "select -1 * sum(OtherIncomeAmt) " & _
                  "from vwCashFlowOtherIncome_CF_V3 where branchcode = '" + branchcode + "' and transdate = '" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_otherincome)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_otherincome)
        End While
                If rdr.Read Then
                    Dim str_otherIncome As String = Trim(rdr(0).ToString)
                    If str_otherIncome <> "" Then
                        otherincome = CDbl(rdr(0).ToString)
                    Else
                        otherincome = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

                Dim ls_Prenda As String = "select sum(PrendaAmt) from " & _
                   " vwCashFlowPrenda_CF_ver3 where branchcode = '" + branchcode + "' and transdate = '" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_Prenda)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_Prenda)
        End While
                If rdr.Read Then
                    Dim str_prenda As String = Trim(rdr(0).ToString)
                    If str_prenda <> "" Then
                        prenda = CDbl(rdr(0).ToString)
                    Else
                        prenda = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()


                Dim ls_telecomms As String = "select -1 * sum (totaltelecomms) " & _
                  " from vwCashFlowTelecomms_CF_ver3 where branchcode = '" + branchcode + "' and transdate = '" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_telecomms)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_telecomms)
        End While
                If rdr.Read Then
                    Dim str_telecomms As String = Trim(rdr(0).ToString)
                    If str_telecomms <> "" Then
                        telecomms = CDbl(rdr(0).ToString)
                    Else
                        telecomms = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

                Dim ls_souvenirs As String = "select -1 * sum(totalSouvenirs) from " & _
                   " vwCashFlowSouvenirs_CF_ver3 where branchcode = '" + branchcode + "' and transdate = '" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_souvenirs)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_souvenirs)
        End While
                If rdr.Read Then
                    Dim str_souvenirs As String = Trim(rdr(0).ToString)
                    If str_souvenirs <> "" Then
                        souvenirs = CDbl(rdr(0).ToString)
                    Else
                        souvenirs = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

                Dim ls_Corp_Sendout As String = "SELECT -1 * sum(amountcentral) " & _
                   " FROM vwCashFlowCorpPartnersSendout_CF_VER3 WHERE branchcode = '" + branchcode + "' and TRANSDATE = '" + Me.Session("dategen") + "' "
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_Corp_Sendout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_Corp_Sendout)
        End While
                If rdr.Read Then
                    Dim str_corpsendout As String = Trim(rdr(0).ToString)
                    If str_corpsendout <> "" Then
                        corpsendout = CDbl(rdr(0).ToString)
                    Else
                        corpsendout = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

                Dim ls_Corp_Payout As String = "SELECT sum(amountcentral) FROM " & _
                    " vwCashFlowCorpPartnersPayout_CF_Ver3 WHERE branchcode = '" + branchcode + "' and TRANSDATE = '" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_Corp_Payout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_Corp_Payout)
        End While
                If rdr.Read Then
                    Dim str_corppayout As String = Trim(rdr(0).ToString)
                    If str_corppayout <> "" Then
                        corppayout = CDbl(rdr(0).ToString)
                    Else
                        corppayout = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

        Dim ls_Corp_Comm As String = "select isnull (-1 *sum(amountcentral),0) from " & _
                    " vwCashFlowCorpPartnersCommision_CF_Ver3 where branchcode = '" + branchcode + "' and transdate = '" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_Corp_Comm)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_Corp_Comm)
        End While
                If rdr.Read Then
                    Dim str_corpcomm As String = Trim(rdr(0).ToString)
                    If str_corpcomm <> "" Then
                        corpcomm = CDbl(rdr(0).ToString)
                    Else
                        corpcomm = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

        Dim ls_WesternUnionComm As String = "select isnull(-1 * sum(TravelAndToursAmt),0) " & _
                      " from vwCashFlowTravelandTours_CF_ver3 where branchcode = '" + branchcode + "' and transdate = '" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_WesternUnionComm)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_WesternUnionComm)
        End While
                If rdr.Read Then
                    Dim str_travelandtours As String = Trim(rdr(0).ToString)
                    If str_travelandtours <> "" Then
                        travelandtours = CDbl(rdr(0).ToString)
                    Else
                        travelandtours = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

        Dim ls_WesternUnionPayout As String = "select isnull(-1 * sum(HealthCareAmt),0) " & _
                     " from vwCashFlowHealthCare_CF_ver3 where branchcode = '" + branchcode + "' and transdate = '" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_WesternUnionPayout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_WesternUnionPayout)
        End While
                If rdr.Read Then
                    Dim str_healthandcare As String = Trim(rdr(0).ToString)
                    If str_healthandcare <> "" Then
                        healthandcare = CDbl(rdr(0).ToString)
                    Else
                        healthandcare = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

                Dim ls_WesternUnionSendout As String = "select dbo.EXEC_SF_1020001CASHBALANCEPESO_CF_V3 ('" + Me.Session("dategen") + "','" + branchcode + "')"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_WesternUnionSendout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_WesternUnionSendout)
        End While
                If rdr.Read Then
                    Dim str_westernUnionSendout As String = Trim(rdr(0).ToString)
                    If str_westernUnionSendout <> "" Then
                        WesternUnionSendout = CDbl(rdr(0).ToString)
                    Else
                        WesternUnionSendout = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

        Dim ls_fundtransferdepositfrombank As String = "select isnull( sum(TotalWithDrawFromBankFundTransferDebit),0) " & _
                     " from vwCashFlowDepositFromBankFundTransferDebit_CF_ver3 where branchcode = '" + branchcode + "' and transdate = '" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_fundtransferdepositfrombank)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_fundtransferdepositfrombank)
        End While
                If rdr.Read Then
                    Dim str_fundtransferdepositfrombank As String = Trim(rdr(0).ToString)
                    If str_fundtransferdepositfrombank <> "" Then
                        fundtransferdepositfrombank = CDbl(rdr(0).ToString)
                    Else
                        fundtransferdepositfrombank = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

                Dim ls_FundTransferDebit As String = "select sum(totalfundtransferdebit) from vwCashFlowFundTransferDebit_CF_Ver3" & _
                   " where branchcode = '" + branchcode + "' and transdate = '" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_FundTransferDebit)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_FundTransferDebit)
        End While
                If rdr.Read Then
                    Dim str_FundTransferDebit As String = Trim(rdr(0).ToString)
                    If str_FundTransferDebit <> "" Then
                        FundTransferDebit = CDbl(rdr(0).ToString)
                    Else
                        FundTransferDebit = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

        Dim ls_fundtransferWithDrawalfrombankCredit As String = "select isnull(-1 * sum(TotalWithDrawFromBankFundTransferCredit),0) " & _
                " from vwCashFlowWithdrawalFromBankFundTransferCredit_CF_ver3 where branchcode = '" + branchcode + "' and transdate = '" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_fundtransferWithDrawalfrombankCredit)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_fundtransferWithDrawalfrombankCredit)
        End While
                If rdr.Read Then
                    Dim str_fundtransferWithDrawalfrombankCredit As String = Trim(rdr(0).ToString)
                    If str_fundtransferWithDrawalfrombankCredit <> "" Then
                        fundtransferWithDrawalfrombankCredit = CDbl(rdr(0).ToString)
                    Else
                        fundtransferWithDrawalfrombankCredit = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

                Dim ls_FundTransferCredit As String = "select isnull(sum(totalfundtransfercredit) * -1,0) " & _
                   " from vwCashFlowFundTransferCredit_CF_ver3 where branchcode = '" + branchcode + "' and transdate = '" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_FundTransferCredit)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_FundTransferCredit)
        End While
                If rdr.Read Then
                    Dim str_FundTransferCredit As String = Trim(rdr(0).ToString)
                    If str_FundTransferCredit <> "" Then
                        FundTransferCredit = CDbl(rdr(0).ToString)
                    Else
                        FundTransferCredit = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

                Dim ls_BranchExpense As String = "select sum(totalbranchexpenses) " & _
                    " from vwCashFlowBranchExpenses_CF_ver3" & _
                    " where branchcode = '" + branchcode + "' and transdate = '" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_BranchExpense)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_BranchExpense)
        End While
                If rdr.Read Then
                    Dim str_BranchExpense As String = Trim(rdr(0).ToString)
                    If str_BranchExpense <> "" Then
                        BranchExpense = CDbl(rdr(0).ToString)
                    Else
                        BranchExpense = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

        Dim ls_OtherExpenseNotRMBase As String = "select  sum(totalotherexpensenotRMBase) from vwCashFlowOtherExpenseNotRMBase_CF_ver3  " & _
                   " where branchcode = '" + branchcode + "' and transdate ='" + Me.Session("dategen") + "' and rmbase_CostCenter = '0" + bcode + "-" + bcode + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_OtherExpenseNotRMBase)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_OtherExpenseNotRMBase)
        End While
                If rdr.Read Then
                    Dim str_OtherExpenseNotRMBase As String = Trim(rdr(0).ToString)
                    If str_OtherExpenseNotRMBase <> "" Then
                        OtherExpenseNotRMBase = CDbl(rdr(0).ToString)
                    Else
                        OtherExpenseNotRMBase = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

                Dim ls_OtherExpense As String = "select sum(totalotherexpense) " & _
                    " from vwCashFlowOtherExpense_CF_ver3" & _
                    " where branchcode = '" + branchcode + "' and transdate ='" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_OtherExpense)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_OtherExpense)
        End While
                If rdr.Read Then
                    Dim str_OtherExpense As String = Trim(rdr(0).ToString)
                    If str_OtherExpense <> "" Then
                        OtherExpense = CDbl(rdr(0).ToString)
                    Else
                        OtherExpense = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

                Dim ls_NSO As String = "select -1 * sum(totalNSo) " & _
                    " from vwCashFlowNSO_CF_ver3" & _
                    " where branchcode = '" + branchcode + "' and transdate ='" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_NSO)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_NSO)
        End While
                If rdr.Read Then
                    Dim str_nso As String = Trim(rdr(0).ToString)
                    If str_nso <> "" Then
                        nso = CDbl(rdr(0).ToString)
                    Else
                        nso = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

                Dim ls_mccr As String = "select sum(TotalMCCashReceipt) " & _
                    " from vwCashFlowMCCashReceipt_CF_ver3" & _
                    " where branchcode = '" + branchcode + "' and transdate ='" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_mccr)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_mccr)
        End While
                If rdr.Read Then
                    Dim str_mccr As String = Trim(rdr(0).ToString)
                    If str_mccr <> "" Then
                        mccr = CDbl(rdr(0).ToString)
                    Else
                        mccr = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

        'Dim ls_mccd As String = "select sum(totalcashdisbursements * -1) " & _
        ' " from (select * from vwCashFlowMCCashDisbursement_CF_ver3 " & _
        '" where transdate = '" + Me.Session("dategen") + "' and branchcode = '" + branchcode + "')x where totalcashdisbursements > 0"
        Dim ls_mccd As String = "select sum(totalcashdisbursements) " & _
                  " from (select * from vwCashFlowMCCashDisbursement_CF_ver3 " & _
                  " where transdate = '" + Me.Session("dategen") + "' and branchcode = '" + branchcode + "')x where totalcashdisbursements > 0"
        db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_mccd)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_mccd)
        End While
                If rdr.Read Then
                    Dim str_mccd As String = Trim(rdr(0).ToString)
                    If str_mccd <> "" Then
                        mccd = CDbl(rdr(0).ToString)
                    Else
                        mccd = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

                Dim ls_racr As String = "select -1 * sum(TotalRACashReceipts) " & _
                  " from vwCashflowRACashReceipts_CF_ver3" & _
                  " where branchcode = '" + branchcode + "' and transdate ='" + Me.Session("dategen") + "' and TotalRACashReceipts < 0 "
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_racr)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_racr)
        End While
                If rdr.Read Then
                    Dim str_racr As String = Trim(rdr(0).ToString)
            'If str_racr <> "" Then
            '    racr = CDbl(rdr(0).ToString)
            'Else
            '    racr = 0
            'End If
            If IsDBNull(rdr(0)) Then
                If str_racr <> "" Then
                    racr = 0.0
                Else
                    racr = CDbl(rdr(0).ToString)
                End If
            End If
                End If
                rdr.Close()
                db.CloseConnection()

                Dim ls_racd As String = "select sum(TotalRACashDisbursements) " & _
                    " from vwCashFlowRACashDisbursements_CF_ver3" & _
                    " where branchcode = '" + branchcode + "' and transdate ='" + Me.Session("dategen") + "' and TotalRACashDisbursements > 0"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_racd)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_racd)
        End While
                If rdr.Read Then
                    Dim str_racd As String = Trim(rdr(0).ToString)
                    If str_racd <> "" Then
                        racd = CDbl(rdr(0).ToString)
                    Else
                        racd = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()

                Dim ls_dfb As String = "select -1 * sum(TotalDeposit) " & _
                   " from vwCashFlowDeposit_CF_ver3" & _
                   " where branchcode = '" + branchcode + "' and transdate ='" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_dfb)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_dfb)
        End While
                If rdr.Read Then
                    Dim str_dfb As String = Trim(rdr(0).ToString)
                    If str_dfb <> "" Then
                        dfb = CDbl(rdr(0).ToString)
                    Else
                        dfb = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()
                Dim ls_wfb As String = "select sum(TotalWithDrawal) " & _
                  " from vwCashFlowWithDrawal_CF_ver3" & _
                  " where branchcode = '" + branchcode + "' and transdate ='" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_wfb)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_wfb)
        End While
                If rdr.Read Then
                    Dim str_wfb As String = Trim(rdr(0).ToString)
                    If str_wfb <> "" Then
                        wfb = CDbl(rdr(0).ToString)
                    Else
                        wfb = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()
                Dim ls_rts As String = "select sum(TotalReturnToSender) " & _
                    " from vwCashFlowReturnToSender_CF_ver3" & _
                    " where branchcode = '" + branchcode + "' and transdate ='" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_rts)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_rts)
        End While
                If rdr.Read Then
                    Dim str_rts As String = Trim(rdr(0).ToString)
                    If str_rts <> "" Then
                        rts = CDbl(rdr(0).ToString)
                    Else
                        rts = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()
                Dim ls_cashover As String = "select isnull(sum(CASHOVERAmt)* -1,0) from vwCashFlowCashOver_CF_Ver3 " & _
                " where branchcode = '" + branchcode + "' and transdate ='" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_cashover)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_cashover)
        End While
                If rdr.Read Then
                    Dim str_cashover As String = Trim(rdr(0).ToString)
                    If str_cashover <> "" Then
                        cashover = CDbl(rdr(0).ToString)
                    Else
                        cashover = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()
                Dim ls_cashshort As String = "select isnull(sum(CASHSHORTAmt),0) from vwCashFlowCashShort_CF_Ver3  " & _
                   " where branchcode = '" + branchcode + "' and transdate ='" + Me.Session("dategen") + "'"
                db.ConnectDB(strCon)
        rdr = db.Execute_SQL_DataReader(ls_cashshort)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_cashshort)
        End While
                If rdr.Read Then
                    Dim str_cashshort As String = Trim(rdr(0).ToString)
                    If str_cashshort <> "" Then
                        cashshort = CDbl(rdr(0).ToString)
                    Else
                        cashshort = 0
                    End If
                End If
                rdr.Close()
                db.CloseConnection()
        totalreceipts = begbal + lukat + interest + otherincome + kpsendout + kpsendoutcomm + corpsendout + corpcomm + insurance + outrightsales + layaway + foodproducts + telecomms + souvenirs + FundTransferCredit
                'Not pure lukat amount------------/

                totaldisbursements = prenda + kpPayout + corppayout + FundTransferDebit + BranchExpense + OtherExpense
                'ld_totaldisbursements = ld_prenda + ld_kppayout + ld_corpcomm + ld_fundtransferdebit + ld_branchexpense + ld_otherexpense ' commented  last 9/20/2010 because instead of corp payout t'was corp commission being inputted
                endingbal = totalreceipts - totaldisbursements
                '\----------pure lukat
                'ld_lukat = ld_NP_lukat ' adjusted last 8/24/2011 findings by ms judith
        ld_lukat = lukat
        Dim ls_update As String = Nothing

        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Management") = "VISAYAS" Then
            ls_update = "UPDATE [CF_PB_Vismin]" & _
            " SET [BeginningBalance]=" & begbal & ", [EndingBalance]=" & endingbal & ", [FoodProducts]=" & foodproducts & ",[Insurance]=" & insurance & ", [outrightsales]=" & outrightsales & ",[layaway]=" & layaway & ",[salesreturn]=" & salesreturn & ",[layawaycancel]=" & layawaycancel & ",[Interest]=" & interest & ",  " & _
            " [KP_Payout]= " & kpPayout & ", [KP_Sendout]=" & kpsendout & ", [KP_Sendout_Comm]=" & kpsendoutcomm & " , [Lukat]= " & lukat & ", [OtherIncome]= " & otherincome & "  , [Prenda]=" & prenda & " , " & _
            " [Telecomms] = " & telecomms & ", [Souvenirs]=" & souvenirs & ", [Corp_Sendout]=" & corpsendout & ", [Corp_Payout]=" & corppayout & ",[Corp_Comm]=" & corpcomm & " ,[WesternUnionComm]=" & travelandtours & ", " & _
            " [WesternUnionPayout]=" & healthandcare & ", [WesternUnionSendout]=" & WesternUnionSendout & ", [FundTransferDebit]=" & FundTransferDebit & ", [FundTransferCredit]=" & FundTransferCredit & ", " & _
            " [BranchExpense]=" & BranchExpense & ",[OtherExpense] = " & OtherExpense & " ,[NSO] = " & nso & " ,[MCCashReceipts] = " & mccr & ",[MCCashDisbursements] = " & mccd & ",[RACashReceipts] = " & racr & ",[RACashDisbursements] = " & racd & ",[DepositFromBank] = " & dfb & " ,[WithdrawalFromBank] = " & wfb & " ,[ReturnToSender] = " & rts & " ,[cashover] = " & cashover & " ,[cashshort] = " & cashshort & "  ,[dategenerated] = '" + Now.Date + "'         WHERE class_02 = 'Visayas' and  transdate = '" + Me.Session("dategen") + "' and branchcode = '" + branchcode + "' "
            'Log("Update Vismin Wide-- " & " " & transdate & " " & Now.TimeOfDay.ToString)
            '--------cani cashflow102312 need to be change in live

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Management") = "MINDANAO" Then
            ls_update = "UPDATE [CF_PB_Vismin]" & _
            " SET [BeginningBalance]=" & begbal & ", [EndingBalance]=" & endingbal & ", [FoodProducts]=" & foodproducts & ",[Insurance]=" & insurance & ", [outrightsales]=" & outrightsales & ",[layaway]=" & layaway & ",[salesreturn]=" & salesreturn & ",[layawaycancel]=" & layawaycancel & ",[Interest]=" & interest & ",  " & _
            " [KP_Payout]= " & kpPayout & ", [KP_Sendout]=" & kpsendout & ", [KP_Sendout_Comm]=" & kpsendoutcomm & " , [Lukat]= " & lukat & ", [OtherIncome]= " & otherincome & "  , [Prenda]=" & prenda & " , " & _
            " [Telecomms] = " & telecomms & ", [Souvenirs]=" & souvenirs & ", [Corp_Sendout]=" & corpsendout & ", [Corp_Payout]=" & corppayout & ",[Corp_Comm]=" & corpcomm & " ,[WesternUnionComm]=" & travelandtours & ", " & _
            " [WesternUnionPayout]=" & healthandcare & ", [WesternUnionSendout]=" & WesternUnionSendout & ", [FundTransferDebit]=" & FundTransferDebit & ", [FundTransferCredit]=" & FundTransferCredit & ", " & _
            " [BranchExpense]=" & BranchExpense & ",[OtherExpense] = " & OtherExpense & " ,[NSO] = " & nso & " ,[MCCashReceipts] = " & mccr & ",[MCCashDisbursements] = " & mccd & ",[RACashReceipts] = " & racr & ",[RACashDisbursements] = " & racd & ",[DepositFromBank] = " & dfb & " ,[WithdrawalFromBank] = " & wfb & " ,[ReturnToSender] = " & rts & " ,[cashover] = " & cashover & " ,[cashshort] = " & cashshort & "  ,[dategenerated] = '" + Now.Date + "'         WHERE class_02 = 'Mindanao' and  transdate = '" + Me.Session("dategen") + "' and branchcode = '" + branchcode + "' "
            'Log("Update Vismin Wide-- " & " " & transdate & " " & Now.TimeOfDay.ToString)
            '--------cani cashflow102312 need to be change in live

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Management") = "LUZON" Then
            ls_update = "UPDATE [CF_PB_Luzon]" & _
            " SET [BeginningBalance]=" & begbal & ", [EndingBalance]=" & endingbal & ", [FoodProducts]=" & foodproducts & ",[Insurance]=" & insurance & ",[outrightsales]=" & outrightsales & ",[layaway]=" & layaway & ",[salesreturn]=" & salesreturn & ",[layawaycancel]=" & layawaycancel & ",[Interest]=" & interest & ",  " & _
            " [KP_Payout]= " & kpPayout & ", [KP_Sendout]=" & kpsendout & ", [KP_Sendout_Comm]=" & kpsendoutcomm & " , [Lukat]= " & lukat & ", [OtherIncome]= " & otherincome & "  , [Prenda]=" & prenda & " , " & _
            " [Telecomms] = " & telecomms & ", [Souvenirs]=" & souvenirs & ", [Corp_Sendout]=" & corpsendout & ", [Corp_Payout]=" & corppayout & ",[Corp_Comm]=" & corpcomm & " ,[WesternUnionComm]=" & travelandtours & ", " & _
            " [WesternUnionPayout]=" & healthandcare & ", [WesternUnionSendout]=" & WesternUnionSendout & ", [FundTransferDebit]=" & FundTransferDebit & ", [FundTransferCredit]=" & FundTransferCredit & ", " & _
            " [BranchExpense]=" & BranchExpense & ",[OtherExpense] = " & OtherExpense & " ,[NSO] = " & nso & " ,[MCCashReceipts] = " & mccr & ",[MCCashDisbursements] = " & mccd & ",[RACashReceipts] = " & racr & ",[RACashDisbursements] = " & racd & ",[DepositFromBank] = " & dfb & " ,[WithdrawalFromBank] = " & wfb & " ,[ReturnToSender] = " & rts & " ,[cashover] = " & cashover & " ,[cashshort] = " & cashshort & "  ,[dategenerated] = '" + Now.Date + "'         WHERE  transdate = '" + Me.Session("dategen") + "' and branchcode = '" + branchcode + "' "
            'Log("Update Vismin Wide-- " & " " & transdate & " " & Now.TimeOfDay.ToString)
            '--------cani cashflow102312 need to be change in live

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Management") = "SHOWROOM" Then
            ls_update = "UPDATE [CF_PB_Showroom]" & _
            " SET [BeginningBalance]=" & begbal & ", [EndingBalance]=" & endingbal & ", [FoodProducts]=" & foodproducts & ",[Insurance]=" & insurance & ", [outrightsales]=" & outrightsales & ",[layaway]=" & layaway & ",[salesreturn]=" & salesreturn & ",[layawaycancel]=" & layawaycancel & ",[Interest]=" & interest & ",  " & _
            " [KP_Payout]= " & kpPayout & ", [KP_Sendout]=" & kpsendout & ", [KP_Sendout_Comm]=" & kpsendoutcomm & " , [Lukat]= " & lukat & ", [OtherIncome]= " & otherincome & "  , [Prenda]=" & prenda & " , " & _
            " [Telecomms] = " & telecomms & ", [Souvenirs]=" & souvenirs & ", [Corp_Sendout]=" & corpsendout & ", [Corp_Payout]=" & corppayout & ",[Corp_Comm]=" & corpcomm & " ,[WesternUnionComm]=" & travelandtours & ", " & _
            " [WesternUnionPayout]=" & healthandcare & ", [WesternUnionSendout]=" & WesternUnionSendout & ", [FundTransferDebit]=" & FundTransferDebit & ", [FundTransferCredit]=" & FundTransferCredit & ", " & _
            " [BranchExpense]=" & BranchExpense & ",[OtherExpense] = " & OtherExpense & " ,[NSO] = " & nso & " ,[MCCashReceipts] = " & mccr & ",[MCCashDisbursements] = " & mccd & ",[RACashReceipts] = " & racr & ",[RACashDisbursements] = " & racd & ",[DepositFromBank] = " & dfb & " ,[WithdrawalFromBank] = " & wfb & " ,[ReturnToSender] = " & rts & " ,[cashover] = " & cashover & " ,[cashshort] = " & cashshort & "  ,[dategenerated] = '" + Now.Date + "'         WHERE class_02 = 'Showrooms' and  transdate = '" + Me.Session("dategen") + "' and branchcode = '" + branchcode + "' "

        End If
        db.ConnectDB(strCon1)

        If db.Execute_SQLQuery(ls_update) = -1 Then
            db.RollbackTransaction()
        End If
        db.CloseConnection()
    End Sub
    Private Sub generateArea(ByVal areaName As String)

        Try
            Dim sql As String = "select bedrnr from bedryf where class_04 = '" & Trim(areaName) & "'"

            If Me.Session("WideN") = "VISAYAS" Or Me.Session("Management") = "VISAYAS" Then
                strCon = Me.Session("strConfRVisayas")

            ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Management") = "MINDANAO" Then
            strCon = Me.Session("strConfRMindanao")

            ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Management") = "LUZON" Then
                strCon = Me.Session("strConfRLuzon")

            ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Management") = "SHOWROOM" Then
                strCon = Me.Session("strConfRShowroom")
            End If

            db.ConnectDB(strCon)

            Dim ds As DataSet = db.Execute_SQL_DataSet(sql, "bedrnr")
            Dim dt As DataTable = ds.Tables(0)

            For Each dr As DataRow In dt.Rows
                bcode = dr.Item(0).ToString
                Bqueries(bcode)
            Next dr
            db.CloseConnection()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
     

    End Sub


    Private Sub Aqueries(ByVal areaName As String)
        Dim rdr As SqlClient.SqlDataReader
        Dim ld_begbal As Double = Nothing 'beginning balance
        Dim ld_endingbal As Double = Nothing 'ending balance
        Dim ld_foodproducts As Double = Nothing 'food products
        Dim ld_insurance As Double = Nothing 'insurance
        Dim ld_outrightsales As Double = Nothing 'outright sales
        Dim ld_layaway As Double = Nothing 'layaway'
        Dim ld_salesreturn As Double = Nothing 'sales return
        Dim ld_layawaycancel As Double = Nothing 'layawaycancel
        Dim ld_interest As Double = Nothing 'interest
        Dim ld_kppayout As Double = Nothing ' kppayout
        Dim ld_kpsendout As Double = Nothing 'kp sendout
        Dim ld_kpsendoutcomm As Double = Nothing 'kp sendout comm
        Dim ld_lukat As Double = Nothing 'lukat
        Dim ld_otherincome As Double = Nothing 'otheincome
        Dim ld_prenda As Double = Nothing 'prenda
        Dim ld_telecomms As Double = Nothing ' telecomms
        Dim ld_souvenirs As Double = Nothing 'souvenirs
        Dim ld_corpsendout As Double = Nothing 'corpsendout
        Dim ld_corppayout As Double = Nothing 'corppayout
        Dim ld_corpcomm As Double = Nothing 'corpcomm
        Dim ld_westernUnionComm As Double = Nothing 'western Union Comm
        Dim ld_westernunionPayout As Double = Nothing ' western union Payout
        Dim ld_westernunionsendout As Double = Nothing 'western union sendout
        Dim ld_fundtransferdebit As Double = Nothing 'fundtransferdebit
        Dim ld_fundtransfercredit As Double = Nothing 'fund transfer Credit
        Dim ld_branchexpense As Double = Nothing 'branch expense
        Dim ld_otherexpense As Double = Nothing 'otherexpense

        Dim ld_nso As Double = Nothing 'NSO
        Dim ld_MCCR As Double = Nothing 'Money Changer Cash Receipts
        Dim ld_MCCD As Double = Nothing 'Money Changer Cash Disbursements
        Dim ld_RACR As Double = Nothing 'Renewal anywhere Cash Receipts
        Dim ld_RACD As Double = Nothing ' Renewal Anywhere Cash Disbursements
        Dim ld_Depositfrombank As Double = Nothing ' 'Deposit From Bank
        Dim ld_withdrawalfrombank As Double = Nothing ' Withdrawal from Bank
        Dim ld_returntosender As Double = Nothing ' Returntosender 2/10/2011
        Dim ld_cashover As Double = Nothing 'Cash OVer
        Dim ld_cashshort As Double = Nothing 'Cash Short


        Dim ls_begbal As String = Nothing

        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_begbal = "select sum(beginningbalance) from CF_pb_vismin where class_02 = 'Visayas' and " & _
                "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_begbal = "select sum(beginningbalance) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
                "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_begbal = "select sum(beginningbalance) from CF_pb_luzon where " & _
                "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_begbal = "select sum(beginningbalance) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
                "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If

        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_begbal)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_begbal)
        End While
        If rdr.Read Then
            Dim str_begbal As String = Trim(rdr(0).ToString)
            If str_begbal <> "" Then
                ld_begbal = CDbl(rdr(0).ToString)
            Else
                ld_begbal = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------

        Dim ls_endingbal As String = Nothing

        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_endingbal = "select sum(EndingBalance) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_endingbal = "select sum(EndingBalance) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_endingbal = "select sum(EndingBalance) from CF_pb_luzon where " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_endingbal = "select sum(EndingBalance) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_endingbal)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_endingbal)
        End While
        If rdr.Read Then
            Dim str_endingbal As String = Trim(rdr(0).ToString)
            If str_endingbal <> "" Then
                ld_endingbal = CDbl(rdr(0).ToString)
            Else
                ld_endingbal = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '-------------------------------------------------

        Dim ls_foodproducts As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_foodproducts = "select sum(Foodproducts) from CF_pb_vismin where class_02 = 'Visayas' and " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_foodproducts = "select sum(Foodproducts) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_foodproducts = "select sum(Foodproducts) from CF_pb_luzon where " & _
         "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_foodproducts = "select sum(Foodproducts) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
         "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_foodproducts)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_foodproducts)
        End While
        If rdr.Read Then
            Dim str_foodproducts As String = Trim(rdr(0).ToString)
            If str_foodproducts <> "" Then
                ld_foodproducts = CDbl(rdr(0).ToString)
            Else
                ld_foodproducts = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------
        Dim ls_insurance As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_insurance = "select sum(Insurance) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_insurance = "select sum(Insurance) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_insurance = "select sum(Insurance) from CF_pb_luzon where " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_insurance = "select sum(Insurance) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_insurance)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_insurance)
        End While
        If rdr.Read Then
            Dim str_insurance As String = Trim(rdr(0).ToString)
            If str_insurance <> "" Then
                ld_insurance = CDbl(rdr(0).ToString)
            Else
                ld_insurance = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------jeniena
        Dim ls_outrightsales As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_outrightsales = "select sum(outrightsales) from CF_pb_vismin where class_02 = 'Visayas' and " & _
         "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_outrightsales = "select sum(outrightsales) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
         "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_outrightsales = "select sum(outrightsales) from CF_pb_luzon where " & _
        "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_outrightsales = "select sum(outrightsales) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
        "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_outrightsales)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_outrightsales)
        End While
        If rdr.Read Then
            Dim str_outrightsales As String = Trim(rdr(0).ToString)
            If str_outrightsales <> "" Then
                ld_outrightsales = CDbl(rdr(0).ToString)
            Else
                ld_outrightsales = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '-------------------------------- 
        Dim ls_layaway As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_layaway = "select sum(layaway) from CF_pb_vismin where class_02 = 'Visayas' and " & _
         "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_layaway = "select sum(layaway) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
         "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_layaway = "select sum(layaway) from CF_pb_luzon where " & _
        "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_layaway = "select sum(layaway) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
        "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_layaway)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_layaway)
        End While
        If rdr.Read Then
            Dim str_layaway As String = Trim(rdr(0).ToString)
            If str_layaway <> "" Then
                ld_layaway = CDbl(rdr(0).ToString)
            Else
                ld_layaway = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        '--------------------------------
        Dim ls_salesreturn As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_salesreturn = "select sum(salesreturn) from CF_pb_vismin where class_02 = 'Visayas' and " & _
         "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_salesreturn = "select sum(salesreturn) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
         "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_salesreturn = "select sum(salesreturn) from CF_pb_luzon where " & _
        "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_salesreturn = "select sum(salesreturn) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
        "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_salesreturn)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_salesreturn)
        End While
        If rdr.Read Then
            Dim str_salesreturn As String = Trim(rdr(0).ToString)
            If str_salesreturn <> "" Then
                ld_salesreturn = CDbl(rdr(0).ToString)
            Else
                ld_salesreturn = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()



        '----------------------------------------------------
        Dim ls_layawaycancel As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_layawaycancel = "select sum(layawaycancel) from CF_pb_vismin where class_02 = 'Visayas' and " & _
         "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_layawaycancel = "select sum(layawaycancel) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
         "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_layawaycancel = "select sum(layawaycancel) from CF_pb_luzon where " & _
        "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_layawaycancel = "select sum(layawaycancel) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
        "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_layawaycancel)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_layawaycancel)
        End While
        If rdr.Read Then
            Dim str_layawaycancel As String = Trim(rdr(0).ToString)
            If str_layawaycancel <> "" Then
                ld_layawaycancel = CDbl(rdr(0).ToString)
            Else
                ld_layawaycancel = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()



        '---------------------------------------------------------------------------------------------
        Dim ls_interest As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_interest = "select sum(interest) from CF_pb_vismin where class_02 = 'Visayas' and " & _
         "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_interest = "select sum(interest) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
         "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_interest = "select sum(interest) from CF_pb_luzon where " & _
        "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_interest = "select sum(interest) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
        "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_interest)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_interest)
        End While
        If rdr.Read Then
            Dim str_interest As String = Trim(rdr(0).ToString)
            If str_interest <> "" Then
                ld_interest = CDbl(rdr(0).ToString)
            Else
                ld_interest = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '---------------------------------
        Dim ls_kppayout As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_kppayout = "select sum(kp_payout) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_kppayout = "select sum(kp_payout) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_kppayout = "select sum(kp_payout) from CF_pb_luzon where " & _
          "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_kppayout = "select sum(kp_payout) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
          "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_kppayout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_kppayout)
        End While
        If rdr.Read Then
            Dim str_kppayout As String = Trim(rdr(0).ToString)
            If str_kppayout <> "" Then
                ld_kppayout = CDbl(rdr(0).ToString)
            Else
                ld_kppayout = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '-------------------------------
        Dim ls_kpsendout As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_kpsendout = "select sum(kp_sendout) from CF_pb_vismin where class_02 = 'Visayas' and " & _
              "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_kpsendout = "select sum(kp_sendout) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
              "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_kpsendout = "select sum(kp_sendout) from CF_pb_luzon where " & _
             "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_kpsendout = "select sum(kp_sendout) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
             "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_kpsendout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_kpsendout)
        End While
        If rdr.Read Then
            Dim str_kpsendout As String = Trim(rdr(0).ToString)
            If str_kpsendout <> "" Then
                ld_kpsendout = CDbl(rdr(0).ToString)
            Else
                ld_kpsendout = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '-------------------------------
        Dim ls_kpsendoutcomm As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_kpsendoutcomm = "select sum(KP_Sendout_Comm) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_kpsendoutcomm = "select sum(KP_Sendout_Comm) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_kpsendoutcomm = "select sum(KP_Sendout_Comm) from CF_pb_luzon where " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_kpsendoutcomm = "select sum(KP_Sendout_Comm) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_kpsendoutcomm)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_kpsendoutcomm)
        End While
        If rdr.Read Then
            Dim str_kpsendoutcomm As String = Trim(rdr(0).ToString)
            If str_kpsendoutcomm <> "" Then
                ld_kpsendoutcomm = CDbl(rdr(0).ToString)
            Else
                ld_kpsendoutcomm = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '------------------------------

        Dim ls_lukat As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_lukat = "select sum(lukat) from CF_pb_vismin where class_02 = 'Visayas' and " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_lukat = "select sum(lukat) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_lukat = "select sum(lukat) from CF_pb_luzon where " & _
          "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_lukat = "select sum(lukat) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
          "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_lukat)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_lukat)
        End While
        If rdr.Read Then
            Dim str_lukat As String = Trim(rdr(0).ToString)
            If str_lukat <> "" Then
                ld_lukat = CDbl(rdr(0).ToString)
            Else
                ld_lukat = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '------------------------------

        Dim ls_otherincome As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_otherincome = "select sum(OtherIncome) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_otherincome = "select sum(OtherIncome) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_otherincome = "select sum(OtherIncome) from CF_pb_luzon where " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_otherincome = "select sum(OtherIncome) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_otherincome)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_otherincome)
        End While
        If rdr.Read Then
            Dim str_otherincome As String = Trim(rdr(0).ToString)
            If str_otherincome <> "" Then
                ld_otherincome = CDbl(rdr(0).ToString)
            Else
                ld_otherincome = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '-------------------------------
        Dim ls_Prenda As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_Prenda = "select sum(prenda) from CF_pb_vismin where class_02 = 'Visayas' and " & _
         "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_Prenda = "select sum(prenda) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
         "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_Prenda = "select sum(prenda) from CF_pb_luzon where " & _
         "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_Prenda = "select sum(prenda) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
         "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_Prenda)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_Prenda)
        End While
        If rdr.Read Then
            Dim str_prenda As String = Trim(rdr(0).ToString)
            If str_prenda <> "" Then
                ld_prenda = CDbl(rdr(0).ToString)
            Else
                ld_prenda = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------

        Dim ls_telecomms As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_telecomms = "select sum(telecomms) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_telecomms = "select sum(telecomms) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_telecomms = "select sum(telecomms) from CF_pb_luzon where " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_telecomms = "select sum(telecomms) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_telecomms)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_telecomms)
        End While
        If rdr.Read Then
            Dim str_telecomms As String = Trim(rdr(0).ToString)
            If str_telecomms <> "" Then
                ld_telecomms = CDbl(rdr(0).ToString)
            Else
                ld_telecomms = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------

        Dim ls_souvenirs As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_souvenirs = "select sum(Souvenirs) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_souvenirs = "select sum(Souvenirs) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_souvenirs = "select sum(Souvenirs) from CF_pb_luzon where " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_souvenirs = "select sum(Souvenirs) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_souvenirs)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_souvenirs)
        End While
        If rdr.Read Then
            Dim str_souvenirs As String = Trim(rdr(0).ToString)
            If str_souvenirs <> "" Then
                ld_souvenirs = CDbl(rdr(0).ToString)
            Else
                ld_souvenirs = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '------------------------------------
        Dim ls_Corp_Sendout As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_Corp_Sendout = "select sum(Corp_Sendout) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_Corp_Sendout = "select sum(Corp_Sendout) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_Corp_Sendout = "select sum(Corp_Sendout) from CF_pb_luzon where " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_Corp_Sendout = "select sum(Corp_Sendout) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_Corp_Sendout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_Corp_Sendout)
        End While
        If rdr.Read Then
            Dim str_corpsendout As String = Trim(rdr(0).ToString)
            If str_corpsendout <> "" Then
                ld_corpsendout = CDbl(rdr(0).ToString)
            Else
                ld_corpsendout = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '------------------------------------

        Dim ls_Corp_Payout As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_Corp_Payout = "select sum(Corp_Payout) from CF_pb_vismin where class_02 = 'Visayas' and " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_Corp_Payout = "select sum(Corp_Payout) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_Corp_Payout = "select sum(Corp_Payout) from CF_pb_luzon where " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_Corp_Payout = "select sum(Corp_Payout) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_Corp_Payout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_Corp_Payout)
        End While
        If rdr.Read Then
            Dim str_corppayout As String = Trim(rdr(0).ToString)
            If str_corppayout <> "" Then
                ld_corppayout = CDbl(rdr(0).ToString)
            Else
                ld_corppayout = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '-------------------------------------
        Dim ls_Corp_Comm As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_Corp_Comm = "select sum(Corp_Comm) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_Corp_Comm = "select sum(Corp_Comm) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_Corp_Comm = "select sum(Corp_Comm) from CF_pb_luzon where " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_Corp_Comm = "select sum(Corp_Comm) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_Corp_Comm)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_Corp_Comm)
        End While
        If rdr.Read Then
            Dim str_corpcomm As String = Trim(rdr(0).ToString)
            If str_corpcomm <> "" Then
                ld_corpcomm = CDbl(rdr(0).ToString)
            Else
                ld_corpcomm = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '------------------------------------

        Dim ls_WesternUnionComm As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_WesternUnionComm = "select sum(WesternUnionComm) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_WesternUnionComm = "select sum(WesternUnionComm) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_WesternUnionComm = "select sum(WesternUnionComm) from CF_pb_luzon where " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_WesternUnionComm = "select sum(WesternUnionComm) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_WesternUnionComm)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_WesternUnionComm)
        End While
        If rdr.Read Then
            Dim str_WesternUnionComm As String = Trim(rdr(0).ToString)
            If str_WesternUnionComm <> "" Then
                ld_westernUnionComm = CDbl(rdr(0).ToString)
            Else
                ld_westernUnionComm = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '-----------------------------------
        Dim ls_WesternUnionPayout As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_WesternUnionPayout = "select sum(WesternUnionPayout) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_WesternUnionPayout = "select sum(WesternUnionPayout) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_WesternUnionPayout = "select sum(WesternUnionPayout) from CF_pb_luzon where " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_WesternUnionPayout = "select sum(WesternUnionPayout) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_WesternUnionPayout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_WesternUnionPayout)
        End While
        If rdr.Read Then
            Dim str_WesternUnionpayout As String = Trim(rdr(0).ToString)
            If str_WesternUnionpayout <> "" Then
                ld_westernunionPayout = CDbl(rdr(0).ToString)
            Else
                ld_westernunionPayout = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '-----------------------------------
        Dim ls_WesternUnionSendout As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_WesternUnionSendout = "select sum(WesternUnionSendout) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_WesternUnionSendout = "select sum(WesternUnionSendout) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_WesternUnionSendout = "select sum(WesternUnionSendout) from CF_pb_luzon where " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_WesternUnionSendout = "select sum(WesternUnionSendout) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_WesternUnionSendout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_WesternUnionSendout)
        End While
        If rdr.Read Then
            Dim str_WesternUnionSendout As String = Trim(rdr(0).ToString)
            If str_WesternUnionSendout <> "" Then
                ld_westernunionsendout = CDbl(rdr(0).ToString)
            Else
                ld_westernunionsendout = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------
        Dim ls_FundTransferDebit As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_FundTransferDebit = "select sum(FundTransferDebit) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_FundTransferDebit = "select sum(FundTransferDebit) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_FundTransferDebit = "select sum(FundTransferDebit) from CF_pb_luzon where " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_FundTransferDebit = "select sum(FundTransferDebit) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_FundTransferDebit)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_FundTransferDebit)
        End While
        If rdr.Read Then
            Dim str_FundTransferDebit As String = Trim(rdr(0).ToString)
            If str_FundTransferDebit <> "" Then
                ld_fundtransferdebit = CDbl(rdr(0).ToString)
            Else
                ld_fundtransferdebit = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        Dim ls_FundTransferCredit As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_FundTransferCredit = "select sum(FundTransferCredit) from CF_pb_vismin where class_02 = 'Visayas' and " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_FundTransferCredit = "select sum(FundTransferCredit) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_FundTransferCredit = "select sum(FundTransferCredit) from CF_pb_luzon where " & _
          "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_FundTransferCredit = "select sum(FundTransferCredit) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
          "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_FundTransferCredit)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_FundTransferCredit)
        End While
        If rdr.Read Then
            Dim str_FundTransferCredit As String = Trim(rdr(0).ToString)
            If str_FundTransferCredit <> "" Then
                ld_fundtransfercredit = CDbl(rdr(0).ToString)
            Else
                ld_fundtransfercredit = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        Dim ls_BranchExpense As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_BranchExpense = "select sum(BranchExpense) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_BranchExpense = "select sum(BranchExpense) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_BranchExpense = "select sum(BranchExpense) from CF_pb_luzon where " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_BranchExpense = "select sum(BranchExpense) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_BranchExpense)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_BranchExpense)
        End While
        If rdr.Read Then
            Dim str_BranchExpense As String = Trim(rdr(0).ToString)
            If str_BranchExpense <> "" Then
                ld_branchexpense = CDbl(rdr(0).ToString)
            Else
                ld_branchexpense = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        Dim ls_OtherExpense As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_OtherExpense = "select sum(OtherExpense) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_OtherExpense = "select sum(OtherExpense) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_OtherExpense = "select sum(OtherExpense) from CF_pb_luzon where " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_OtherExpense = "select sum(OtherExpense) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_OtherExpense)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_OtherExpense)
        End While
        If rdr.Read Then
            Dim str_otherexpense As String = Trim(rdr(0).ToString)
            If str_otherexpense <> "" Then
                ld_otherexpense = CDbl(rdr(0).ToString)
            Else
                ld_otherexpense = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        Dim ls_nso As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_nso = "select sum(NSO) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_nso = "select sum(NSO) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_nso = "select sum(NSO) from CF_pb_luzon where " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_nso = "select sum(NSO) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_nso)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_nso)
        End While
        If rdr.Read Then
            Dim str_nso As String = Trim(rdr(0).ToString)
            If str_nso <> "" Then
                ld_nso = CDbl(rdr(0).ToString)
            Else
                ld_nso = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        Dim ls_mccr As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_mccr = "select sum(MCCashReceipts) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_mccr = "select sum(MCCashReceipts) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_mccr = "select sum(MCCashReceipts) from CF_pb_luzon where " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_mccr = "select sum(MCCashReceipts) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_mccr)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_mccr)
        End While
        If rdr.Read Then
            Dim str_mccr As String = Trim(rdr(0).ToString)
            If str_mccr <> "" Then
                ld_MCCR = CDbl(rdr(0).ToString)
            Else
                ld_MCCR = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        Dim ls_mccd As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_mccd = "select sum(MCCashDisbursements) from CF_pb_vismin where class_02 = 'Visayas' and " & _
        "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_mccd = "select sum(MCCashDisbursements) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
        "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_mccd = "select sum(MCCashDisbursements) from CF_pb_luzon where " & _
      "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_mccd = "select sum(MCCashDisbursements) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
      "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_mccd)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_mccd)
        End While
        If rdr.Read Then
            Dim str_mccd As String = Trim(rdr(0).ToString)
            If str_mccd <> "" Then
                ld_MCCD = CDbl(rdr(0).ToString)
            Else
                ld_MCCD = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        Dim ls_racr As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_racr = "select sum(RACashReceipts) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_racr = "select sum(RACashReceipts) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_racr = "select sum(RACashReceipts) from CF_pb_luzon where " & _
          "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_racr = "select sum(RACashReceipts) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
          "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_racr)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_racr)
        End While
        If rdr.Read Then
            Dim str_racr As String = Trim(rdr(0).ToString)
            If str_racr <> "" Then
                ld_RACR = CDbl(rdr(0).ToString)
            Else
                ld_RACR = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        Dim ls_racd As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_racd = "select sum(RACashDisbursements) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_racd = "select sum(RACashDisbursements) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_racd = "select sum(RACashDisbursements) from CF_pb_luzon where " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_racd = "select sum(RACashDisbursements) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_racd)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_racd)
        End While
        If rdr.Read Then
            Dim str_racd As String = Trim(rdr(0).ToString)
            If str_racd <> "" Then
                ld_RACD = CDbl(rdr(0).ToString)
            Else
                ld_RACD = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        Dim ls_dfb As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_dfb = "select sum(DepositFromBank) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_dfb = "select sum(DepositFromBank) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_dfb = "select sum(DepositFromBank) from CF_pb_luzon where " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_dfb = "select sum(DepositFromBank) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_dfb)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_dfb)
        End While
        If rdr.Read Then
            Dim str_dfb As String = Trim(rdr(0).ToString)
            If str_dfb <> "" Then
                ld_Depositfrombank = CDbl(rdr(0).ToString)
            Else
                ld_Depositfrombank = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        Dim ls_wfb As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_wfb = "select sum(WithdrawalFromBank) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_wfb = "select sum(WithdrawalFromBank) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_wfb = "select sum(WithdrawalFromBank) from CF_pb_luzon where " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_wfb = "select sum(WithdrawalFromBank) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_wfb)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_wfb)
        End While
        If rdr.Read Then
            Dim str_wfb As String = Trim(rdr(0).ToString)
            If str_wfb <> "" Then
                ld_withdrawalfrombank = CDbl(rdr(0).ToString)
            Else
                ld_withdrawalfrombank = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        Dim ls_rts As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_rts = "select sum(ReturnToSender) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_rts = "select sum(ReturnToSender) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_rts = "select sum(ReturnToSender) from CF_pb_luzon where " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_rts = "select sum(ReturnToSender) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_rts)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_rts)
        End While
        If rdr.Read Then
            Dim str_rts As String = Trim(rdr(0).ToString)
            If str_rts <> "" Then
                ld_returntosender = CDbl(rdr(0).ToString)
            Else
                ld_returntosender = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        Dim ls_cashover As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_cashover = "select sum(cashover) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_cashover = "select sum(cashover) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_cashover = "select sum(cashover) from CF_pb_luzon where " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_cashover = "select sum(cashover) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_cashover)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_cashover)
        End While
        If rdr.Read Then
            Dim str_cashover As String = Trim(rdr(0).ToString)
            If str_cashover <> "" Then
                ld_cashover = CDbl(rdr(0).ToString)
            Else
                ld_cashover = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        Dim ls_cashshort As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_cashshort = "select sum(cashshort) from CF_pb_vismin where class_02 = 'Visayas' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_cashshort = "select sum(cashshort) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_cashshort = "select sum(cashshort) from CF_pb_luzon where " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_cashshort = "select sum(cashshort) from CF_pb_showroom where class_02 = 'Showrooms' and " & _
           "class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_cashshort)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_cashshort)
        End While
        If rdr.Read Then
            Dim str_cashshort As String = Trim(rdr(0).ToString)
            If str_cashshort <> "" Then
                ld_cashshort = CDbl(rdr(0).ToString)
            Else
                ld_cashshort = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        Dim ls_update As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_update = "UPDATE [CF_area_VisMin]" & _
                    " SET [BeginningBalance]=" & ld_begbal & ", [EndingBalance]=" & ld_endingbal & ", [FoodProducts]=" & ld_foodproducts & ",[Insurance]=" & ld_insurance & ",[outrightsales]=" & ld_outrightsales & ",[layaway]=" & ld_layaway & ",[salesreturn]=" & ld_salesreturn & ",[layawaycancel]=" & ld_layawaycancel & ",[Interest]=" & ld_interest & ",  " & _
                    " [KP_Payout]= " & ld_kppayout & ", [KP_Sendout]=" & ld_kpsendout & ", [KP_Sendout_Comm]=" & ld_kpsendoutcomm & " , [Lukat]= " & ld_lukat & ", [OtherIncome]= " & ld_otherincome & "  , [Prenda]=" & ld_prenda & " , " & _
                    " [Telecomms] = " & ld_telecomms & ", [Souvenirs]=" & ld_souvenirs & ", [Corp_Sendout]=" & ld_corpsendout & ", [Corp_Payout]=" & ld_corppayout & ",[Corp_Comm]=" & ld_corpcomm & " ,[WesternUnionComm]=" & ld_westernUnionComm & ", " & _
                    " [WesternUnionPayout]=" & ld_westernunionPayout & ", [WesternUnionSendout]=" & ld_westernunionsendout & ", [FundTransferDebit]=" & ld_fundtransferdebit & ", [FundTransferCredit]=" & ld_fundtransfercredit & ", " & _
                    " [BranchExpense]=" & ld_branchexpense & ",[OtherExpense] = " & ld_otherexpense & " ,[NSO] = " & ld_nso & " ,[MCCashReceipts] = " & ld_MCCR & ",[MCCashDisbursements] = " & ld_MCCD & ",[RACashReceipts] = " & ld_RACR & "," & _
                    " [RACashDisbursements] = " & ld_RACD & ",[DepositFromBank] = " & ld_Depositfrombank & " ,[WithdrawalFromBank] = " & ld_withdrawalfrombank & ",[ReturnToSender] = " & ld_returntosender & " ,[cashover] = " & ld_cashover & "  ," & _
                    " [cashshort] = " & ld_cashshort & "  ,[dategenerated] = '" + Now.Date + "' " & _
                    " WHERE class_02 = 'Visayas' and class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "' "

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_update = "UPDATE [CF_area_VisMin]" & _
                    " SET [BeginningBalance]=" & ld_begbal & ", [EndingBalance]=" & ld_endingbal & ", [FoodProducts]=" & ld_foodproducts & ",[Insurance]=" & ld_insurance & ",[outrightsales]=" & ld_outrightsales & ",[layaway]=" & ld_layaway & ",[salesreturn]=" & ld_salesreturn & ",[layawaycancel]=" & ld_layawaycancel & ",[Interest]=" & ld_interest & ",  " & _
                    " [KP_Payout]= " & ld_kppayout & ", [KP_Sendout]=" & ld_kpsendout & ", [KP_Sendout_Comm]=" & ld_kpsendoutcomm & " , [Lukat]= " & ld_lukat & ", [OtherIncome]= " & ld_otherincome & "  , [Prenda]=" & ld_prenda & " , " & _
                    " [Telecomms] = " & ld_telecomms & ", [Souvenirs]=" & ld_souvenirs & ", [Corp_Sendout]=" & ld_corpsendout & ", [Corp_Payout]=" & ld_corppayout & ",[Corp_Comm]=" & ld_corpcomm & " ,[WesternUnionComm]=" & ld_westernUnionComm & ", " & _
                    " [WesternUnionPayout]=" & ld_westernunionPayout & ", [WesternUnionSendout]=" & ld_westernunionsendout & ", [FundTransferDebit]=" & ld_fundtransferdebit & ", [FundTransferCredit]=" & ld_fundtransfercredit & ", " & _
                    " [BranchExpense]=" & ld_branchexpense & ",[OtherExpense] = " & ld_otherexpense & " ,[NSO] = " & ld_nso & " ,[MCCashReceipts] = " & ld_MCCR & ",[MCCashDisbursements] = " & ld_MCCD & ",[RACashReceipts] = " & ld_RACR & "," & _
                    " [RACashDisbursements] = " & ld_RACD & ",[DepositFromBank] = " & ld_Depositfrombank & " ,[WithdrawalFromBank] = " & ld_withdrawalfrombank & ",[ReturnToSender] = " & ld_returntosender & " ,[cashover] = " & ld_cashover & "  ," & _
                    " [cashshort] = " & ld_cashshort & "  ,[dategenerated] = '" + Now.Date + "' " & _
                    " WHERE class_02 = 'Mindanao' and class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "' "

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_update = "UPDATE [CF_area_Luzon]" & _
                    " SET [BeginningBalance]=" & ld_begbal & ", [EndingBalance]=" & ld_endingbal & ", [FoodProducts]=" & ld_foodproducts & ",[Insurance]=" & ld_insurance & ", [outrightsales]=" & ld_outrightsales & ",[layaway]=" & ld_layaway & ",[salesreturn]=" & ld_salesreturn & ",[layawaycancel]=" & ld_layawaycancel & ",[Interest]=" & ld_interest & ",  " & _
                    " [KP_Payout]= " & ld_kppayout & ", [KP_Sendout]=" & ld_kpsendout & ", [KP_Sendout_Comm]=" & ld_kpsendoutcomm & " , [Lukat]= " & ld_lukat & ", [OtherIncome]= " & ld_otherincome & "  , [Prenda]=" & ld_prenda & " , " & _
                    " [Telecomms] = " & ld_telecomms & ", [Souvenirs]=" & ld_souvenirs & ", [Corp_Sendout]=" & ld_corpsendout & ", [Corp_Payout]=" & ld_corppayout & ",[Corp_Comm]=" & ld_corpcomm & " ,[WesternUnionComm]=" & ld_westernUnionComm & ", " & _
                    " [WesternUnionPayout]=" & ld_westernunionPayout & ", [WesternUnionSendout]=" & ld_westernunionsendout & ", [FundTransferDebit]=" & ld_fundtransferdebit & ", [FundTransferCredit]=" & ld_fundtransfercredit & ", " & _
                    " [BranchExpense]=" & ld_branchexpense & ",[OtherExpense] = " & ld_otherexpense & " ,[NSO] = " & ld_nso & " ,[MCCashReceipts] = " & ld_MCCR & ",[MCCashDisbursements] = " & ld_MCCD & ",[RACashReceipts] = " & ld_RACR & "," & _
                    " [RACashDisbursements] = " & ld_RACD & ",[DepositFromBank] = " & ld_Depositfrombank & " ,[WithdrawalFromBank] = " & ld_withdrawalfrombank & ",[ReturnToSender] = " & ld_returntosender & " ,[cashover] = " & ld_cashover & "  ," & _
                    " [cashshort] = " & ld_cashshort & "  ,[dategenerated] = '" + Now.Date + "' " & _
                    " WHERE class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "' "

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_update = "UPDATE [CF_area_Showroom]" & _
                    " SET [BeginningBalance]=" & ld_begbal & ", [EndingBalance]=" & ld_endingbal & ", [FoodProducts]=" & ld_foodproducts & ",[Insurance]=" & ld_insurance & ", [outrightsales]=" & ld_outrightsales & ",[layaway]=" & ld_layaway & ",[salesreturn]=" & ld_salesreturn & ",[layawaycancel]=" & ld_layawaycancel & ",[Interest]=" & ld_interest & ",  " & _
                    " [KP_Payout]= " & ld_kppayout & ", [KP_Sendout]=" & ld_kpsendout & ", [KP_Sendout_Comm]=" & ld_kpsendoutcomm & " , [Lukat]= " & ld_lukat & ", [OtherIncome]= " & ld_otherincome & "  , [Prenda]=" & ld_prenda & " , " & _
                    " [Telecomms] = " & ld_telecomms & ", [Souvenirs]=" & ld_souvenirs & ", [Corp_Sendout]=" & ld_corpsendout & ", [Corp_Payout]=" & ld_corppayout & ",[Corp_Comm]=" & ld_corpcomm & " ,[WesternUnionComm]=" & ld_westernUnionComm & ", " & _
                    " [WesternUnionPayout]=" & ld_westernunionPayout & ", [WesternUnionSendout]=" & ld_westernunionsendout & ", [FundTransferDebit]=" & ld_fundtransferdebit & ", [FundTransferCredit]=" & ld_fundtransfercredit & ", " & _
                    " [BranchExpense]=" & ld_branchexpense & ",[OtherExpense] = " & ld_otherexpense & " ,[NSO] = " & ld_nso & " ,[MCCashReceipts] = " & ld_MCCR & ",[MCCashDisbursements] = " & ld_MCCD & ",[RACashReceipts] = " & ld_RACR & "," & _
                    " [RACashDisbursements] = " & ld_RACD & ",[DepositFromBank] = " & ld_Depositfrombank & " ,[WithdrawalFromBank] = " & ld_withdrawalfrombank & ",[ReturnToSender] = " & ld_returntosender & " ,[cashover] = " & ld_cashover & "  ," & _
                    " [cashshort] = " & ld_cashshort & "  ,[dategenerated] = '" + Now.Date + "' " & _
                    " WHERE class_02 = 'Showrooms' and class_04 = '" + Trim(areaName) + "' and transdate = '" + Me.Session("dategen") + "' "

        End If
        db.ConnectDB(strCon1)
        If db.Execute_SQLQuery(ls_update) = -1 Then
            db.RollbackTransaction()
        End If

        db.CloseConnection()
        '-------cani cashflow102312 need to be change in live
    End Sub
    Private Sub generateRegion(ByVal Region As String)
        Dim sql As String = "select distinct class_04 from bedryf where class_03 = '" & Trim(Region) & "'"

        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Management") = "VISAYAS" Then
            strCon = Me.Session("strConfRVisayas")

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Management") = "MINDANAO" Then
            strCon = Me.Session("strConfRMindanao")

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Management") = "LUZON" Then
            strCon = Me.Session("strConfRLuzon")

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Management") = "SHOWROOM" Then
            strCon = Me.Session("strConfRShowroom")
        End If

        'If db.isConnected = False Then
        db.ConnectDB(strCon)
        ' End If
        Dim ds As DataSet = db.Execute_SQL_DataSet(sql, "class_04")
        Dim dt As DataTable = ds.Tables(0)

        For Each dr As DataRow In dt.Rows
            Dim areaN As String = dr.Item(0).ToString
            generateArea(areaN)
            Aqueries(areaN)
        Next dr
        db.CloseConnection()

    End Sub

    Private Sub Rqueries(ByVal Rname As String)


        'Me.Session("Regionname") = dplRegion.SelectedItem.Text
        Dim rdr As SqlClient.SqlDataReader
        Dim ld_begbal_lv As Double = Nothing 'beginning balance
        Dim ld_endingbal_lv As Double = Nothing 'ending balance
        Dim ld_foodproducts_lv As Double = Nothing 'food products
        Dim ld_insurance_lv As Double = Nothing 'insurance
        Dim ld_outrightsales_lv As Double = Nothing 'outright sales
        Dim ld_layaway_lv As Double = Nothing 'layaway
        Dim ld_salesreturn_lv As Double = Nothing 'sales return 
        Dim ld_layawaycancel_lv As Double = Nothing 'layaway cancel
        Dim ld_interest_lv As Double = Nothing 'interest
        Dim ld_kppayout_lv As Double = Nothing ' kppayout
        Dim ld_kpsendout_lv As Double = Nothing 'kp sendout
        Dim ld_kpsendoutcomm_lv As Double = Nothing 'kp sendout comm
        Dim ld_lukat_lv As Double = Nothing 'lukat
        Dim ld_otherincome_lv As Double = Nothing 'otheincome
        Dim ld_prenda_lv As Double = Nothing 'prenda
        Dim ld_telecomms_lv As Double = Nothing ' telecomms
        Dim ld_souvenirs_lv As Double = Nothing 'souvenirs
        Dim ld_corpsendout_lv As Double = Nothing 'corpsendout
        Dim ld_corppayout_lv As Double = Nothing 'corppayout
        Dim ld_corpcomm_lv As Double = Nothing 'corpcomm
        Dim ld_westernUnionComm_lv As Double = Nothing 'western Union Comm
        Dim ld_westernunionPayout_lv As Double = Nothing ' western union Payout
        Dim ld_westernunionsendout_lv As Double = Nothing 'western union sendout
        Dim ld_fundtransferdebit_lv As Double = Nothing 'fundtransferdebit
        Dim ld_fundtransfercredit_lv As Double = Nothing 'fund transfer Credit
        Dim ld_branchexpense_lv As Double = Nothing 'branch expense
        Dim ld_otherexpense_lv As Double = Nothing 'otherexpense

        Dim ld_nso_lv As Double = Nothing 'NSO
        Dim ld_MCCR_lv As Double = Nothing 'Money Changer Cash Receipts
        Dim ld_MCCD_lv As Double = Nothing 'Money Changer Cash Disbursements
        Dim ld_RACR_lv As Double = Nothing 'Renewal anywhere Cash Receipts
        Dim ld_RACD_lv As Double = Nothing ' Renewal Anywhere Cash Disbursements
        Dim ld_Depositfrombank_lv As Double = Nothing ' 'Deposit From Bank
        Dim ld_withdrawalfrombank_lv As Double = Nothing ' Withdrawal from Bank
        Dim ld_returntosender_lv As Double = Nothing ' Returntosender 2/10/2011
        Dim ld_cashover_lv As Double = Nothing 'Cash OVer
        Dim ld_cashshort_lv As Double = Nothing 'Cash Short

        Dim ls_begbal As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_begbal = "select sum(beginningbalance) from CF_area_vismin where class_02 = 'Visayas' and " & _
               "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_begbal = "select sum(beginningbalance) from CF_area_vismin where class_02 = 'Mindanao' and  " & _
               "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_begbal = "select sum(beginningbalance) from CF_area_luzon where " & _
             "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_begbal = "select sum(beginningbalance) from CF_area_showrooms where class_02 = 'Showrooms' and " & _
             "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_begbal)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_begbal)
        End While
        If rdr.Read Then
            Dim str_begbal As String = Trim(rdr(0).ToString)
            If str_begbal <> "" Then
                ld_begbal_lv = CDbl(rdr(0).ToString)
            Else
                ld_begbal_lv = 0
            End If
        End If
        rdr.Close()

        '--------------------------------------------------------------
        Dim ls_endingbal As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_endingbal = "select sum(EndingBalance) from CF_area_vismin where class_02 = 'Visayas' and " & _
              "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_endingbal = "select sum(EndingBalance) from CF_area_vismin where class_02 = 'Mindanao' and " & _
              "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_endingbal = "select sum(EndingBalance) from CF_area_luzon where " & _
             "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_endingbal = "select sum(EndingBalance) from CF_area_showroom where class_02 = 'Showrooms' and " & _
             "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_endingbal)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_endingbal)
        End While
        If rdr.Read Then
            Dim str_endingbal As String = Trim(rdr(0).ToString)
            If str_endingbal <> "" Then
                ld_endingbal_lv = CDbl(rdr(0).ToString)
            Else
                ld_endingbal_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        Dim ls_foodproducts As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_foodproducts = "select sum(Foodproducts) from CF_area_vismin where class_02 = 'Visayas' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_foodproducts = "select sum(Foodproducts) from CF_area_vismin where class_02 = 'Mindanao' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_foodproducts = "select sum(Foodproducts) from CF_area_luzon where " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_foodproducts = "select sum(Foodproducts) from CF_area_showroom where class_02 = 'Showrooms' and " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_foodproducts)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_foodproducts)
        End While
        If rdr.Read Then
            Dim str_foodproducts As String = Trim(rdr(0).ToString)
            If str_foodproducts <> "" Then
                ld_foodproducts_lv = CDbl(rdr(0).ToString)
            Else
                ld_foodproducts_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------------------------
        Dim ls_insurance As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_insurance = "select sum(Insurance) from CF_area_vismin where class_02 = 'Visayas' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_insurance = "select sum(Insurance) from CF_area_vismin where class_02 = 'Mindanao' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_insurance = "select sum(Insurance) from CF_area_luzon where " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_insurance = "select sum(Insurance) from CF_area_showroom where class_02 = 'Showrooms' and " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_insurance)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_insurance)
        End While
        If rdr.Read Then
            Dim str_insurance As String = Trim(rdr(0).ToString)
            If str_insurance <> "" Then
                ld_insurance_lv = CDbl(rdr(0).ToString)
            Else
                ld_insurance_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        '--------------------------------------------

        Dim ls_outrightsales As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_outrightsales = "select sum(outrightsales) from CF_area_vismin where class_02 = 'Visayas' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_outrightsales = "select sum(outrightsales) from CF_area_vismin where class_02 = 'Mindanao' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_outrightsales = "select sum(outrightsales) from CF_area_luzon where " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_outrightsales = "select sum(outrightsales) from CF_area_showroom where class_02 = 'Showrooms' and " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_outrightsales)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_outrightsales)
        End While
        If rdr.Read Then
            Dim str_outrightsales As String = Trim(rdr(0).ToString)
            If str_outrightsales <> "" Then
                ld_outrightsales_lv = CDbl(rdr(0).ToString)
            Else
                ld_outrightsales_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        '----------------------------------------------------------
        Dim ls_layaway As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_layaway = "select sum(layaway) from CF_area_vismin where class_02 = 'Visayas' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_layaway = "select sum(layaway) from CF_area_vismin where class_02 = 'Mindanao' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_layaway = "select sum(layaway) from CF_area_luzon where " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_layaway = "select sum(layaway) from CF_area_showroom where class_02 = 'Showrooms' and " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_layaway)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_layaway)
        End While
        If rdr.Read Then
            Dim str_layaway As String = Trim(rdr(0).ToString)
            If str_layaway <> "" Then
                ld_layaway_lv = CDbl(rdr(0).ToString)
            Else
                ld_layaway_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()



        '----------------------------------------------------------

        Dim ls_salesreturn As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_salesreturn = "select sum(salesreturn) from CF_area_vismin where class_02 = 'Visayas' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_salesreturn = "select sum(salesreturn) from CF_area_vismin where class_02 = 'Mindanao' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_salesreturn = "select sum(salesreturn) from CF_area_luzon where " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_salesreturn = "select sum(salesreturn) from CF_area_showroom where class_02 = 'Showrooms' and " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_salesreturn)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_salesreturn)
        End While
        If rdr.Read Then
            Dim str_salesreturn As String = Trim(rdr(0).ToString)
            If str_salesreturn <> "" Then
                ld_salesreturn_lv = CDbl(rdr(0).ToString)
            Else
                ld_salesreturn_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()


        '----------------------------------------------------------------------------------------------------
        Dim ls_layawaycancel As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_layawaycancel = "select sum(layawaycancel) from CF_area_vismin where class_02 = 'Visayas' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_layawaycancel = "select sum(layawaycancel) from CF_area_vismin where class_02 = 'Mindanao' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_layawaycancel = "select sum(layawaycancel) from CF_area_luzon where " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_layawaycancel = "select sum(layawaycancel) from CF_area_showroom where class_02 = 'Showrooms' and " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_layawaycancel)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_layawaycancel)
        End While
        If rdr.Read Then
            Dim str_layawaycancel As String = Trim(rdr(0).ToString)
            If str_layawaycancel <> "" Then
                ld_layawaycancel_lv = CDbl(rdr(0).ToString)
            Else
                ld_layawaycancel_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()



        '----------------------------------------------------------

        Dim ls_interest As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_interest = "select sum(interest) from CF_area_vismin where class_02 = 'Visayas' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_interest = "select sum(interest) from CF_area_vismin where class_02 = 'Mindanao' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_interest = "select sum(interest) from CF_area_luzon where " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_interest = "select sum(interest) from CF_area_showroom where class_02 = 'Showrooms' and " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_interest)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_interest)
        End While
        If rdr.Read Then
            Dim str_interest As String = Trim(rdr(0).ToString)
            If str_interest <> "" Then
                ld_interest_lv = CDbl(rdr(0).ToString)
            Else
                ld_interest_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------------------------
        Dim ls_kppayout As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_kppayout = "select sum(kp_payout) from CF_area_vismin where class_02 = 'Visayas' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_kppayout = "select sum(kp_payout) from CF_area_vismin where class_02 = 'Mindanao' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_kppayout = "select sum(kp_payout) from CF_area_luzon where " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_kppayout = "select sum(kp_payout) from CF_area_showroom where class_02 = 'Showrooms' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_kppayout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_kppayout)
        End While
        If rdr.Read Then
            Dim str_kppayout As String = Trim(rdr(0).ToString)
            If str_kppayout <> "" Then
                ld_kppayout_lv = CDbl(rdr(0).ToString)
            Else
                ld_kppayout_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        Dim ls_kpsendout As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_kpsendout = "select sum(kp_sendout) from CF_area_vismin where class_02 = 'Visayas' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_kpsendout = "select sum(kp_sendout) from CF_area_vismin where class_02 = 'Mindanao' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_kpsendout = "select sum(kp_sendout) from CF_area_luzon where " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_kpsendout = "select sum(kp_sendout) from CF_area_showroom where class_02 = 'Showrooms' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_kpsendout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_kpsendout)
        End While
        If rdr.Read Then
            Dim str_kpsendout As String = Trim(rdr(0).ToString)
            If str_kpsendout <> "" Then
                ld_kpsendout_lv = CDbl(rdr(0).ToString)
            Else
                ld_kpsendout_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------------------------
        Dim ls_kpsendoutcomm As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_kpsendoutcomm = "select sum(KP_Sendout_Comm) from CF_area_vismin where class_02 = 'Visayas' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_kpsendoutcomm = "select sum(KP_Sendout_Comm) from CF_area_vismin where class_02 = 'Mindanao' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_kpsendoutcomm = "select sum(KP_Sendout_Comm) from CF_area_luzon where " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_kpsendoutcomm = "select sum(KP_Sendout_Comm) from CF_area_showroom where class_02 = 'Showrooms' and " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_kpsendoutcomm)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_kpsendoutcomm)
        End While
        If rdr.Read Then
            Dim str_kpsendoutComm As String = Trim(rdr(0).ToString)
            If str_kpsendoutComm <> "" Then
                ld_kpsendoutcomm_lv = CDbl(rdr(0).ToString)
            Else
                ld_kpsendoutcomm_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        '----------------------------------------------------------
        Dim ls_lukat As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_lukat = "select sum(Lukat) from CF_area_vismin where class_02 = 'Visayas' and " & _
                        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_lukat = "select sum(Lukat) from CF_area_vismin where class_02 = 'Mindanao' and " & _
                        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_lukat = "select sum(Lukat) from CF_area_luzon where " & _
                        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_lukat = "select sum(Lukat) from CF_area_showroom where class_02 = 'Showrooms' and " & _
                        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_lukat)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_lukat)
        End While
        If rdr.Read Then
            Dim str_lukat As String = Trim(rdr(0).ToString)
            If str_lukat <> "" Then
                ld_lukat_lv = CDbl(rdr(0).ToString)
            Else
                ld_lukat_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------------------------
        Dim ls_otherincome As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_otherincome = "select sum(OtherIncome) from CF_area_vismin where class_02 = 'Visayas' and " & _
                            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_otherincome = "select sum(OtherIncome) from CF_area_vismin where class_02 = 'Mindanao' and " & _
                            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_otherincome = "select sum(OtherIncome) from CF_area_luzon where " & _
                            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_otherincome = "select sum(OtherIncome) from CF_area_showroom where class_02 = 'Showrooms' and " & _
                            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_otherincome)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_otherincome)
        End While
        If rdr.Read Then
            Dim str_otherincome As String = Trim(rdr(0).ToString)
            If str_otherincome <> "" Then
                ld_otherincome_lv = CDbl(rdr(0).ToString)
            Else
                ld_otherincome_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        Dim ls_Prenda As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_Prenda = "select sum(prenda) from CF_area_vismin where class_02 = 'Visayas' and " & _
                            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_Prenda = "select sum(prenda) from CF_area_vismin where class_02 = 'Mindanao' and " & _
                            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_Prenda = "select sum(prenda) from CF_area_luzon where " & _
                            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_Prenda = "select sum(prenda) from CF_area_showroom where class_02 = 'Showrooms' and " & _
                            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If

        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_Prenda)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_Prenda)
        End While
        If rdr.Read Then
            Dim str_prenda As String = Trim(rdr(0).ToString)
            If str_prenda <> "" Then
                ld_prenda_lv = CDbl(rdr(0).ToString)
            Else
                ld_prenda_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------------------------
        Dim ls_telecomms As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_telecomms = "select sum(telecomms) from CF_area_vismin where class_02 = 'Visayas' and " & _
         "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_telecomms = "select sum(telecomms) from CF_area_vismin where class_02 = 'Mindanao' and " & _
         "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_telecomms = "select sum(telecomms) from CF_area_luzon where " & _
        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_telecomms = "select sum(telecomms) from CF_area_showroom where class_02 = 'Showrooms' and " & _
        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_telecomms)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_telecomms)
        End While
        If rdr.Read Then
            Dim str_telecomms As String = Trim(rdr(0).ToString)
            If str_telecomms <> "" Then
                ld_telecomms_lv = CDbl(rdr(0).ToString)
            Else
                ld_telecomms_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        Dim ls_souvenirs As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_souvenirs = "select sum(Souvenirs) from CF_area_vismin where class_02 = 'Visayas' and " & _
        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_souvenirs = "select sum(Souvenirs) from CF_area_vismin where class_02 = 'Mindanao' and " & _
        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_souvenirs = "select sum(Souvenirs) from CF_area_luzon where " & _
       "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_souvenirs = "select sum(Souvenirs) from CF_area_showroom where class_02 = 'Showrooms' and " & _
       "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_souvenirs)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_souvenirs)
        End While
        If rdr.Read Then
            Dim str_souvenirs As String = Trim(rdr(0).ToString)
            If str_souvenirs <> "" Then
                ld_souvenirs_lv = CDbl(rdr(0).ToString)
            Else
                ld_souvenirs_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------------------------
        Dim ls_Corp_Sendout As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_Corp_Sendout = "select sum(Corp_Sendout) from CF_area_vismin where class_02 = 'Visayas' and " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_Corp_Sendout = "select sum(Corp_Sendout) from CF_area_vismin where class_02 = 'Mindanao' and " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_Corp_Sendout = "select sum(Corp_Sendout) from CF_area_luzon where " & _
          "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_Corp_Sendout = "select sum(Corp_Sendout) from CF_area_showroom where class_02 = 'Showrooms' and " & _
          "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_Corp_Sendout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_Corp_Sendout)
        End While
        If rdr.Read Then
            Dim str_corp_sendout As String = Trim(rdr(0).ToString)
            If str_corp_sendout <> "" Then
                ld_corpsendout_lv = CDbl(rdr(0).ToString)
            Else
                ld_corpsendout_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        Dim ls_Corp_Payout As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_Corp_Payout = "select sum(Corp_Payout) from CF_area_vismin where class_02 = 'Visayas' and " & _
     "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_Corp_Payout = "select sum(Corp_Payout) from CF_area_vismin where class_02 = 'Mindanao' and " & _
     "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_Corp_Payout = "select sum(Corp_Payout) from CF_area_luzon where " & _
    "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_Corp_Payout = "select sum(Corp_Payout) from CF_area_showroom where class_02 = 'Showrooms' and " & _
    "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_Corp_Payout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_Corp_Payout)
        End While
        If rdr.Read Then
            Dim str_corp_payout As String = Trim(rdr(0).ToString)
            If str_corp_payout <> "" Then
                ld_corppayout_lv = CDbl(rdr(0).ToString)
            Else
                ld_corppayout_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------------------------
        Dim ls_Corp_Comm As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_Corp_Comm = "select sum(Corp_Comm) from CF_area_vismin where class_02 = 'Visayas' and " & _
      "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_Corp_Comm = "select sum(Corp_Comm) from CF_area_vismin where class_02 = 'Mindanao' and " & _
      "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_Corp_Comm = "select sum(Corp_Comm) from CF_area_luzon where " & _
     "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_Corp_Comm = "select sum(Corp_Comm) from CF_area_showroom where class_02 = 'Showrooms' and " & _
     "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_Corp_Comm)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_Corp_Comm)
        End While
        If rdr.Read Then
            Dim str_corp_comm As String = Trim(rdr(0).ToString)
            If str_corp_comm <> "" Then
                ld_corpcomm_lv = CDbl(rdr(0).ToString)
            Else
                ld_corpcomm_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        Dim ls_WesternUnionComm As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_WesternUnionComm = "select sum(WesternUnionComm) from CF_area_vismin where class_02 = 'Visayas' and " & _
     "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_WesternUnionComm = "select sum(WesternUnionComm) from CF_area_vismin where class_02 = 'Mindanao' and " & _
     "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_WesternUnionComm = "select sum(WesternUnionComm) from CF_area_luzon where " & _
     "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_WesternUnionComm = "select sum(WesternUnionComm) from CF_area_showroom where class_02 = 'Showrooms' and " & _
     "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_WesternUnionComm)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_WesternUnionComm)
        End While
        If rdr.Read Then
            Dim str_westernunioncomm As String = Trim(rdr(0).ToString)
            If str_westernunioncomm <> "" Then
                ld_westernUnionComm_lv = CDbl(rdr(0).ToString)
            Else
                ld_westernUnionComm_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------------------------
        Dim ls_WesternUnionPayout As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_WesternUnionPayout = "select sum(WesternUnionPayout) from CF_area_vismin where class_02 = 'Visayas' and " & _
     "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_WesternUnionPayout = "select sum(WesternUnionPayout) from CF_area_vismin where class_02 = 'Mindanao' and " & _
     "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_WesternUnionPayout = "select sum(WesternUnionPayout) from CF_area_luzon where " & _
     "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_WesternUnionPayout = "select sum(WesternUnionPayout) from CF_area_showroom where class_02 = 'Showrooms' and " & _
     "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_WesternUnionPayout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_WesternUnionPayout)
        End While
        If rdr.Read Then
            Dim str_westernunion_payout As String = Trim(rdr(0).ToString)
            If str_westernunion_payout <> "" Then
                ld_westernunionPayout_lv = CDbl(rdr(0).ToString)
            Else
                ld_westernunionPayout_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        Dim ls_WesternUnionSendout As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_WesternUnionSendout = "select sum(WesternUnionSendout) from CF_area_vismin where class_02 = 'Visayas' and " & _
     "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_WesternUnionSendout = "select sum(WesternUnionSendout) from CF_area_vismin where class_02 = 'Mindanao' and " & _
     "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_WesternUnionSendout = "select sum(WesternUnionSendout) from CF_area_luzon where " & _
    "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_WesternUnionSendout = "select sum(WesternUnionSendout) from CF_area_showroom where class_02 = 'Showrooms' and " & _
    "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_WesternUnionSendout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_WesternUnionSendout)
        End While
        If rdr.Read Then
            Dim str_westernunionsendout As String = Trim(rdr(0).ToString)
            If str_westernunionsendout <> "" Then
                ld_westernunionsendout_lv = CDbl(rdr(0).ToString)
            Else
                ld_westernunionsendout_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------------------------
        Dim ls_FundTransferDebit As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_FundTransferDebit = "select sum(FundTransferDebit) from CF_area_vismin where class_02 = 'Visayas' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_FundTransferDebit = "select sum(FundTransferDebit) from CF_area_vismin where class_02 = 'Mindanao' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_FundTransferDebit = "select sum(FundTransferDebit) from CF_area_luzon where " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_FundTransferDebit = "select sum(FundTransferDebit) from CF_area_showroom where class_02 = 'Showrooms' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_FundTransferDebit)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_FundTransferDebit)
        End While
        If rdr.Read Then
            Dim str_fundtransferdebit As String = Trim(rdr(0).ToString)
            If str_fundtransferdebit <> "" Then
                ld_fundtransferdebit_lv = CDbl(rdr(0).ToString)
            Else
                ld_fundtransferdebit_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        Dim ls_FundTransferCredit As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_FundTransferCredit = "select sum(FundTransferCredit) from CF_area_vismin where class_02 = 'Visayas' and " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_FundTransferCredit = "select sum(FundTransferCredit) from CF_area_vismin where class_02 = 'Mindanao' and " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_FundTransferCredit = "select sum(FundTransferCredit) from CF_area_luzon where " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_FundTransferCredit = "select sum(FundTransferCredit) from CF_area_showroom where class_02 = 'Showrooms' and " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_FundTransferCredit)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_FundTransferCredit)
        End While
        If rdr.Read Then
            Dim str_fundtransfercredit As String = Trim(rdr(0).ToString)
            If str_fundtransfercredit <> "" Then
                ld_fundtransfercredit_lv = CDbl(rdr(0).ToString)
            Else
                ld_fundtransfercredit_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------------------------
        Dim ls_BranchExpense As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_BranchExpense = "select sum(BranchExpense) from CF_area_vismin where class_02 = 'Visayas' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_BranchExpense = "select sum(BranchExpense) from CF_area_vismin where class_02 = 'Mindanao' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_BranchExpense = "select sum(BranchExpense) from CF_area_luzon where " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_BranchExpense = "select sum(BranchExpense) from CF_area_showroom where class_02 = 'Showrooms' and " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_BranchExpense)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_BranchExpense)
        End While
        If rdr.Read Then
            Dim str_branchexpense As String = Trim(rdr(0).ToString)
            If str_branchexpense <> "" Then
                ld_branchexpense_lv = CDbl(rdr(0).ToString)
            Else
                ld_branchexpense_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        Dim ls_OtherExpense As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_OtherExpense = "select sum(OtherExpense) from CF_area_vismin where class_02 = 'Visayas' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_OtherExpense = "select sum(OtherExpense) from CF_area_vismin where class_02 = 'Mindanao' and  " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_OtherExpense = "select sum(OtherExpense) from CF_area_luzon where " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_OtherExpense = "select sum(OtherExpense) from CF_area_showroom where class_02 = 'Showrooms' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_OtherExpense)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_OtherExpense)
        End While
        If rdr.Read Then
            Dim str_otherexpense As String = Trim(rdr(0).ToString)
            If str_otherexpense <> "" Then
                ld_otherexpense_lv = CDbl(rdr(0).ToString)
            Else
                ld_otherexpense_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_NSO As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_NSO = "select sum(NSO) from CF_area_vismin where class_02 = 'Visayas' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_NSO = "select sum(NSO) from CF_area_vismin where class_02 = 'Mindanao' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_NSO = "select sum(NSO) from CF_area_luzon where " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_NSO = "select sum(NSO) from CF_area_showroom where class_02 = 'Showrooms' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_NSO)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_NSO)
        End While
        If rdr.Read Then
            Dim str_NSO As String = Trim(rdr(0).ToString)
            If str_NSO <> "" Then
                ld_nso_lv = CDbl(rdr(0).ToString)
            Else
                ld_nso_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_MCCR As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_MCCR = "select sum(MCCashReceipts) from CF_area_vismin where class_02 = 'Visayas' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_MCCR = "select sum(MCCashReceipts) from CF_area_vismin where class_02 = 'Mindanao' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_MCCR = "select sum(MCCashReceipts) from CF_area_luzon where " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_MCCR = "select sum(MCCashReceipts) from CF_area_showroom where class_02 = 'Showrooms' and " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_MCCR)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_MCCR)
        End While
        If rdr.Read Then
            Dim str_mccr As String = Trim(rdr(0).ToString)
            If str_mccr <> "" Then
                ld_MCCR_lv = CDbl(rdr(0).ToString)
            Else
                ld_MCCR_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_MCCD As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_MCCD = "select sum(MCCashDisbursements) from CF_area_vismin where class_02 = 'Visayas' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_MCCD = "select sum(MCCashDisbursements) from CF_area_vismin where class_02 = 'Mindanao' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_MCCD = "select sum(MCCashDisbursements) from CF_area_luzon where " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_MCCD = "select sum(MCCashDisbursements) from CF_area_showroom where class_02 = 'Showrooms' and " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_MCCD)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_MCCD)
        End While
        If rdr.Read Then
            Dim str_mccd As String = Trim(rdr(0).ToString)
            If str_mccd <> "" Then
                ld_MCCD_lv = CDbl(rdr(0).ToString)
            Else
                ld_MCCD_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_RACR As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_RACR = "select sum(RACashReceipts) from CF_area_vismin where class_02 = 'Visayas' and " & _
        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_RACR = "select sum(RACashReceipts) from CF_area_vismin where class_02 = 'Mindanao' and " & _
        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_RACR = "select sum(RACashReceipts) from CF_area_luzon where " & _
       "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_RACR = "select sum(RACashReceipts) from CF_area_showroom where class_02 = 'Showrooms' and " & _
       "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_RACR)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_RACR)
        End While
        If rdr.Read Then
            Dim str_racr As String = Trim(rdr(0).ToString)
            If str_racr <> "" Then
                ld_RACR_lv = CDbl(rdr(0).ToString)
            Else
                ld_RACR_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        '----------------------------------------------------------

        Dim ls_RACD As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_RACD = "select sum(RACashDisbursements) from CF_area_vismin where class_02 = 'Visayas' and " & _
        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_RACD = "select sum(RACashDisbursements) from CF_area_vismin where class_02 = 'Mindanao' and " & _
        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_RACD = "select sum(RACashDisbursements) from CF_area_luzon where " & _
        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_RACD = "select sum(RACashDisbursements) from CF_area_showroom where class_02 = 'Showrooms' and " & _
        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_RACD)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_RACD)
        End While
        If rdr.Read Then
            Dim str_racd As String = Trim(rdr(0).ToString)
            If str_racd <> "" Then
                ld_RACD_lv = CDbl(rdr(0).ToString)
            Else
                ld_RACD_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_DFB As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_DFB = "select sum(DepositFromBank) from CF_area_vismin where class_02 = 'Visayas' and " & _
        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_DFB = "select sum(DepositFromBank) from CF_area_vismin where class_02 = 'Mindanao' and " & _
        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_DFB = "select sum(DepositFromBank) from CF_area_luzon where " & _
       "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_DFB = "select sum(DepositFromBank) from CF_area_showroom where class_02 = 'Showrooms' and " & _
       "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_DFB)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_DFB)
        End While
        If rdr.Read Then
            Dim str_DFB As String = Trim(rdr(0).ToString)
            If str_DFB <> "" Then
                ld_Depositfrombank_lv = CDbl(rdr(0).ToString)
            Else
                ld_Depositfrombank_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_WFB As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_WFB = "select sum(WithdrawalFromBank) from CF_area_vismin where class_02 = 'Visayas' and " & _
        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_WFB = "select sum(WithdrawalFromBank) from CF_area_vismin where class_02 = 'Mindanao' and " & _
        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_WFB = "select sum(WithdrawalFromBank) from CF_area_luzon where " & _
        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_WFB = "select sum(WithdrawalFromBank) from CF_area_showroom where class_02 = 'Showrooms' and " & _
        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_WFB)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_WFB)
        End While
        If rdr.Read Then
            Dim str_WFB As String = Trim(rdr(0).ToString)
            If str_WFB <> "" Then
                ld_withdrawalfrombank_lv = CDbl(rdr(0).ToString)
            Else
                ld_withdrawalfrombank_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_rts As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_rts = "select sum(returntosender) from CF_area_vismin where class_02 = 'Visayas' and " & _
        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_rts = "select sum(returntosender) from CF_area_vismin where class_02 = 'Mindanao' and " & _
        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_rts = "select sum(returntosender) from CF_area_luzon where " & _
        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_rts = "select sum(returntosender) from CF_area_showroom where class_02 = 'Showrooms' and " & _
        "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_rts)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_rts)
        End While
        If rdr.Read Then
            Dim str_RTS As String = Trim(rdr(0).ToString)
            If str_RTS <> "" Then
                ld_returntosender_lv = CDbl(rdr(0).ToString)
            Else
                ld_returntosender_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        Dim ls_cashover As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_cashover = "select sum(cashover) from CF_area_vismin where class_02 = 'Visayas' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_cashover = "select sum(cashover) from CF_area_vismin where class_02 = 'Mindanao' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_cashover = "select sum(cashover) from CF_area_luzon where " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_cashover = "select sum(cashover) from CF_area_showroom where class_02 = 'Showrooms' and " & _
           "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_cashover)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_cashover)
        End While
        If rdr.Read Then
            Dim str_cashover As String = Trim(rdr(0).ToString)
            If str_cashover <> "" Then
                ld_cashover_lv = CDbl(rdr(0).ToString)
            Else
                ld_cashover_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        Dim ls_cashshort As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_cashshort = "select sum(cashshort) from CF_area_vismin where class_02 = 'Visayas' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_cashshort = "select sum(cashshort) from CF_area_vismin where class_02 = 'Mindanao' and  " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_cashshort = "select sum(cashshort) from CF_area_luzon where " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_cashshort = "select sum(cashshort) from CF_area_showroom where class_02 = 'Showrooms' and " & _
            "class_03 = '" + Rname + "' and transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_cashshort)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_cashshort)
        End While
        If rdr.Read Then
            Dim str_cashshort As String = Trim(rdr(0).ToString)
            If str_cashshort <> "" Then
                ld_cashshort_lv = CDbl(rdr(0).ToString)
            Else
                ld_cashshort_lv = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        Dim ls_update As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_update = "UPDATE [CF_region_VisMin]" & _
                  " SET [BeginningBalance]=" & ld_begbal_lv & ", [EndingBalance]=" & ld_endingbal_lv & ", [FoodProducts]=" & ld_foodproducts_lv & ",[Insurance]=" & ld_insurance_lv & ",[outrightsales]=" & ld_outrightsales_lv & ",[layaway]=" & ld_layaway_lv & ",[salesreturn]=" & ld_salesreturn_lv & ",[layawaycancel]=" & ld_layawaycancel_lv & ", [Interest]=" & ld_interest_lv & ",  " & _
                  " [KP_Payout]= " & ld_kppayout_lv & ", [KP_Sendout]=" & ld_kpsendout_lv & ", [KP_Sendout_Comm]=" & ld_kpsendoutcomm_lv & " , [Lukat]= " & ld_lukat_lv & ", [OtherIncome]= " & ld_otherincome_lv & "  , [Prenda]=" & ld_prenda_lv & " , " & _
                  " [Telecomms] = " & ld_telecomms_lv & ", [Souvenirs]=" & ld_souvenirs_lv & ", [Corp_Sendout]=" & ld_corpsendout_lv & ", [Corp_Payout]=" & ld_corppayout_lv & ",[Corp_Comm]=" & ld_corpcomm_lv & " ,[WesternUnionComm]=" & ld_westernUnionComm_lv & ", " & _
                  " [WesternUnionPayout]=" & ld_westernunionPayout_lv & ", [WesternUnionSendout]=" & ld_westernunionsendout_lv & ", [FundTransferDebit]=" & ld_fundtransferdebit_lv & ", [FundTransferCredit]=" & ld_fundtransfercredit_lv & ", " & _
                  " [BranchExpense]=" & ld_branchexpense_lv & ",[OtherExpense] = " & ld_otherexpense_lv & " ,[NSO] = " & ld_nso_lv & " ,[MCCashReceipts] = " & ld_MCCR_lv & ",[MCCashDisbursements] = " & ld_MCCD_lv & ",[RACashReceipts] = " & ld_RACR_lv & ",[RACashDisbursements] = " & ld_RACD_lv & ",[DepositFromBank] = " & ld_Depositfrombank_lv & " ,[WithdrawalFromBank] = " & ld_withdrawalfrombank_lv & ",[returntosender] = " & ld_returntosender_lv & "  ,[cashover] = " & ld_cashover_lv & "  ,[cashshort] = " & ld_cashshort_lv & "  ,[dategenerated] = '" + Now.Date + "' WHERE class_02 = 'Visayas' and  transdate = '" + Me.Session("dategen") + "' and class_03 = '" + Rname + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_update = "UPDATE [CF_region_VisMin]" & _
                  " SET [BeginningBalance]=" & ld_begbal_lv & ", [EndingBalance]=" & ld_endingbal_lv & ", [FoodProducts]=" & ld_foodproducts_lv & ",[Insurance]=" & ld_insurance_lv & ",[outrightsales]=" & ld_outrightsales_lv & ",[layaway]=" & ld_layaway_lv & ",[salesreturn]=" & ld_salesreturn_lv & ",[layawaycancel]=" & ld_layawaycancel_lv & ", [Interest]=" & ld_interest_lv & ",  " & _
                  " [KP_Payout]= " & ld_kppayout_lv & ", [KP_Sendout]=" & ld_kpsendout_lv & ", [KP_Sendout_Comm]=" & ld_kpsendoutcomm_lv & " , [Lukat]= " & ld_lukat_lv & ", [OtherIncome]= " & ld_otherincome_lv & "  , [Prenda]=" & ld_prenda_lv & " , " & _
                  " [Telecomms] = " & ld_telecomms_lv & ", [Souvenirs]=" & ld_souvenirs_lv & ", [Corp_Sendout]=" & ld_corpsendout_lv & ", [Corp_Payout]=" & ld_corppayout_lv & ",[Corp_Comm]=" & ld_corpcomm_lv & " ,[WesternUnionComm]=" & ld_westernUnionComm_lv & ", " & _
                  " [WesternUnionPayout]=" & ld_westernunionPayout_lv & ", [WesternUnionSendout]=" & ld_westernunionsendout_lv & ", [FundTransferDebit]=" & ld_fundtransferdebit_lv & ", [FundTransferCredit]=" & ld_fundtransfercredit_lv & ", " & _
                  " [BranchExpense]=" & ld_branchexpense_lv & ",[OtherExpense] = " & ld_otherexpense_lv & " ,[NSO] = " & ld_nso_lv & " ,[MCCashReceipts] = " & ld_MCCR_lv & ",[MCCashDisbursements] = " & ld_MCCD_lv & ",[RACashReceipts] = " & ld_RACR_lv & ",[RACashDisbursements] = " & ld_RACD_lv & ",[DepositFromBank] = " & ld_Depositfrombank_lv & " ,[WithdrawalFromBank] = " & ld_withdrawalfrombank_lv & ",[returntosender] = " & ld_returntosender_lv & "  ,[cashover] = " & ld_cashover_lv & "  ,[cashshort] = " & ld_cashshort_lv & "  ,[dategenerated] = '" + Now.Date + "' WHERE class_02 = 'Mindanao' and  transdate = '" + Me.Session("dategen") + "' and class_03 = '" + Rname + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_update = "UPDATE [CF_region_Luzon]" & _
                 " SET [BeginningBalance]=" & ld_begbal_lv & ", [EndingBalance]=" & ld_endingbal_lv & ", [FoodProducts]=" & ld_foodproducts_lv & ",[Insurance]=" & ld_insurance_lv & ", [outrightsales]=" & ld_outrightsales_lv & ",[layaway]=" & ld_layaway_lv & ",[salesreturn]=" & ld_salesreturn_lv & ",[layawaycancel]=" & ld_layawaycancel_lv & ", [Interest]=" & ld_interest_lv & ",  " & _
                 " [KP_Payout]= " & ld_kppayout_lv & ", [KP_Sendout]=" & ld_kpsendout_lv & ", [KP_Sendout_Comm]=" & ld_kpsendoutcomm_lv & " , [Lukat]= " & ld_lukat_lv & ", [OtherIncome]= " & ld_otherincome_lv & "  , [Prenda]=" & ld_prenda_lv & " , " & _
                 " [Telecomms] = " & ld_telecomms_lv & ", [Souvenirs]=" & ld_souvenirs_lv & ", [Corp_Sendout]=" & ld_corpsendout_lv & ", [Corp_Payout]=" & ld_corppayout_lv & ",[Corp_Comm]=" & ld_corpcomm_lv & " ,[WesternUnionComm]=" & ld_westernUnionComm_lv & ", " & _
                 " [WesternUnionPayout]=" & ld_westernunionPayout_lv & ", [WesternUnionSendout]=" & ld_westernunionsendout_lv & ", [FundTransferDebit]=" & ld_fundtransferdebit_lv & ", [FundTransferCredit]=" & ld_fundtransfercredit_lv & ", " & _
                 " [BranchExpense]=" & ld_branchexpense_lv & ",[OtherExpense] = " & ld_otherexpense_lv & " ,[NSO] = " & ld_nso_lv & " ,[MCCashReceipts] = " & ld_MCCR_lv & ",[MCCashDisbursements] = " & ld_MCCD_lv & ",[RACashReceipts] = " & ld_RACR_lv & ",[RACashDisbursements] = " & ld_RACD_lv & ",[DepositFromBank] = " & ld_Depositfrombank_lv & " ,[WithdrawalFromBank] = " & ld_withdrawalfrombank_lv & ",[returntosender] = " & ld_returntosender_lv & "  ,[cashover] = " & ld_cashover_lv & "  ,[cashshort] = " & ld_cashshort_lv & "  ,[dategenerated] = '" + Now.Date + "' WHERE  transdate = '" + Me.Session("dategen") + "' and class_03 = '" + Rname + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_update = "UPDATE [CF_region_Showroom]" & _
                 " SET [BeginningBalance]=" & ld_begbal_lv & ", [EndingBalance]=" & ld_endingbal_lv & ", [FoodProducts]=" & ld_foodproducts_lv & ",[Insurance]=" & ld_insurance_lv & ", [outrightsales]=" & ld_outrightsales_lv & ",[layaway]=" & ld_layaway_lv & ",[salesreturn]=" & ld_salesreturn_lv & ",[layawaycancel]=" & ld_layawaycancel_lv & ", [Interest]=" & ld_interest_lv & ",  " & _
                 " [KP_Payout]= " & ld_kppayout_lv & ", [KP_Sendout]=" & ld_kpsendout_lv & ", [KP_Sendout_Comm]=" & ld_kpsendoutcomm_lv & " , [Lukat]= " & ld_lukat_lv & ", [OtherIncome]= " & ld_otherincome_lv & "  , [Prenda]=" & ld_prenda_lv & " , " & _
                 " [Telecomms] = " & ld_telecomms_lv & ", [Souvenirs]=" & ld_souvenirs_lv & ", [Corp_Sendout]=" & ld_corpsendout_lv & ", [Corp_Payout]=" & ld_corppayout_lv & ",[Corp_Comm]=" & ld_corpcomm_lv & " ,[WesternUnionComm]=" & ld_westernUnionComm_lv & ", " & _
                 " [WesternUnionPayout]=" & ld_westernunionPayout_lv & ", [WesternUnionSendout]=" & ld_westernunionsendout_lv & ", [FundTransferDebit]=" & ld_fundtransferdebit_lv & ", [FundTransferCredit]=" & ld_fundtransfercredit_lv & ", " & _
                 " [BranchExpense]=" & ld_branchexpense_lv & ",[OtherExpense] = " & ld_otherexpense_lv & " ,[NSO] = " & ld_nso_lv & " ,[MCCashReceipts] = " & ld_MCCR_lv & ",[MCCashDisbursements] = " & ld_MCCD_lv & ",[RACashReceipts] = " & ld_RACR_lv & ",[RACashDisbursements] = " & ld_RACD_lv & ",[DepositFromBank] = " & ld_Depositfrombank_lv & " ,[WithdrawalFromBank] = " & ld_withdrawalfrombank_lv & ",[returntosender] = " & ld_returntosender_lv & "  ,[cashover] = " & ld_cashover_lv & "  ,[cashshort] = " & ld_cashshort_lv & "  ,[dategenerated] = '" + Now.Date + "' WHERE class_02 = 'Showrooms' and  transdate = '" + Me.Session("dategen") + "' and class_03 = '" + Rname + "'"

        End If
        db.ConnectDB(strCon1)
        If db.Execute_SQLQuery(ls_update) = -1 Then
            db.RollbackTransaction()
        End If
        db.CloseConnection()
    End Sub
    Public Sub generateWideVismin()
        Dim sql As String = "select distinct class_03 from bedryf"
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Management") = "VISAYAS" Then
            strCon = Me.Session("strConfRVisayas")

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Management") = "MINDANAO" Then
            strCon = Me.Session("strConfRMindanao")

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Management") = "LUZON" Then
            strCon = Me.Session("strConfRLuzon")

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Management") = "SHOWROOM" Then
            strCon = Me.Session("strConfRShowroom")
        End If

        db.ConnectDB(strCon)
        Dim ds As DataSet = db.Execute_SQL_DataSet(sql, "class_03")
        Dim dt As DataTable = ds.Tables(0)

        For Each dr As DataRow In dt.Rows
            Dim region As String = dr.Item(0).ToString
            generateRegion(region)
        Next dr
        db.CloseConnection()


    End Sub

    Public Sub Vismin_Wqueries()
        Dim rdr As SqlClient.SqlDataReader
        Dim ld_begbal As Double = Nothing 'beginning balance
        Dim ld_endingbal As Double = Nothing 'ending balance
        Dim ld_foodproducts As Double = Nothing 'food products
        Dim ld_insurance As Double = Nothing 'insurance
        Dim ld_outrightsales As Double = Nothing 'outright sales
        Dim ld_layaway As Double = Nothing ' layaway payments
        Dim ld_salesreturn As Double = Nothing 'sales return
        Dim ld_layawaycancel As Double = Nothing ' layaway cancel 
        Dim ld_interest As Double = Nothing 'interest
        Dim ld_kppayout As Double = Nothing ' kppayout
        Dim ld_kpsendout As Double = Nothing 'kp sendout
        Dim ld_kpsendoutcomm As Double = Nothing 'kp sendout comm
        Dim ld_lukat As Double = Nothing 'lukat
        Dim ld_otherincome As Double = Nothing 'otheincome
        Dim ld_prenda As Double = Nothing 'prenda
        Dim ld_telecomms As Double = Nothing ' telecomms
        Dim ld_souvenirs As Double = Nothing 'souvenirs
        Dim ld_corpsendout As Double = Nothing 'corpsendout
        Dim ld_corppayout As Double = Nothing 'corppayout
        Dim ld_corpcomm As Double = Nothing 'corpcomm
        Dim ld_westernUnionComm As Double = Nothing 'western Union Comm
        Dim ld_westernunionPayout As Double = Nothing ' western union Payout
        Dim ld_westernunionsendout As Double = Nothing 'western union sendout
        Dim ld_fundtransferdebit As Double = Nothing 'fundtransferdebit
        Dim ld_fundtransfercredit As Double = Nothing 'fund transfer Credit
        Dim ld_branchexpense As Double = Nothing 'branch expense
        Dim ld_otherexpense As Double = Nothing 'otherexpense
        '/-------------------added last 2/8/2011 due to judith findings
        Dim ld_nso As Double = Nothing 'NSO
        Dim ld_MCCR As Double = Nothing 'Money Changer Cash Receipts
        Dim ld_MCCD As Double = Nothing 'Money Changer Cash Disbursements
        Dim ld_RACR As Double = Nothing 'Renewal anywhere Cash Receipts
        Dim ld_RACD As Double = Nothing ' Renewal Anywhere Cash Disbursements
        Dim ld_Depositfrombank As Double = Nothing ' 'Deposit From Bank
        Dim ld_withdrawalfrombank As Double = Nothing ' Withdrawal from Bank
        Dim ld_returntosender As Double = Nothing ' Returntosender 2/10/2011
        '\-------------------added last 2/8/2011 due to judith findings

        '/-------------------added last 7/26/2011 due to judith findings
        Dim ld_CashShort As Double = Nothing 'Cash Short
        Dim ld_CashOver As Double = Nothing 'Cash Over

        Dim ls_begbal As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_begbal = "select sum(beginningbalance) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_begbal = "select sum(beginningbalance) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_begbal = "select sum(beginningbalance) from CF_region_luzon where " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_begbal = "select sum(beginningbalance) from CF_region_showroom where class_02 = 'Showrooms' and " & _
            " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_begbal)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_begbal)
        End While
        If rdr.Read Then
            Dim str_begbal As String = Trim(rdr(0).ToString)
            If str_begbal <> "" Then
                ld_begbal = CDbl(rdr(0).ToString)
            Else
                ld_begbal = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------------------------
        Dim ls_endingbal As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_endingbal = "select sum(EndingBalance) from CF_region_vismin where class_02 = 'Visayas' and " & _
              " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_endingbal = "select sum(EndingBalance) from CF_region_vismin where class_02 = 'Mindanao' and " & _
              " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_endingbal = "select sum(EndingBalance) from CF_region_luzon where " & _
             " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_endingbal = "select sum(EndingBalance) from CF_region_showroom where class_02 = 'Showrooms' and " & _
             " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_endingbal)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_endingbal)
        End While
        If rdr.Read Then
            Dim str_endingbal As String = Trim(rdr(0).ToString)
            If str_endingbal <> "" Then
                ld_endingbal = CDbl(rdr(0).ToString)
            Else
                ld_endingbal = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        Dim ls_foodproducts As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_foodproducts = "select sum(Foodproducts) from CF_region_vismin where class_02 = 'Visayas' and " & _
              " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_foodproducts = "select sum(Foodproducts) from CF_region_vismin where class_02 = 'Mindanao' and " & _
              " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_foodproducts = "select sum(Foodproducts) from CF_region_luzon where " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_foodproducts = "select sum(Foodproducts) from CF_region_showroom where class_02 = 'Showrooms' and " & _
            " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_foodproducts)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_foodproducts)
        End While
        If rdr.Read Then
            Dim str_foodproducts As String = Trim(rdr(0).ToString)
            If str_foodproducts <> "" Then
                ld_foodproducts = CDbl(rdr(0).ToString)
            Else
                ld_foodproducts = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------------------------
        Dim ls_insurance As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_insurance = "select sum(Insurance) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_insurance = "select sum(Insurance) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_insurance = "select sum(Insurance) from CF_region_luzon where " & _
           " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_insurance = "select sum(Insurance) from CF_region_showroom where class_02 = 'Showrooms' and " & _
           " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_insurance)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_insurance)
        End While
        If rdr.Read Then
            Dim str_insurance As String = Trim(rdr(0).ToString)
            If str_insurance <> "" Then
                ld_insurance = CDbl(rdr(0).ToString)
            Else
                ld_insurance = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------------------------
        Dim ls_outrightsales As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_outrightsales = "select sum(outrightsales) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_outrightsales = "select sum(outrightsales) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_outrightsales = "select sum(outrightsales) from CF_region_luzon where " & _
           " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_outrightsales = "select sum(outrightsales) from CF_region_showroom where class_02 = 'Showrooms' and " & _
           " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_outrightsales)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_outrightsales)
        End While
        If rdr.Read Then
            Dim str_outrightsales As String = Trim(rdr(0).ToString)
            If str_outrightsales <> "" Then
                ld_outrightsales = CDbl(rdr(0).ToString)
            Else
                ld_outrightsales = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()


        '--------------------------------------------------------------
        Dim ls_layaway As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_layaway = "select sum(layaway) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_layaway = "select sum(layaway) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_layaway = "select sum(layaway) from CF_region_luzon where " & _
           " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_layaway = "select sum(layaway) from CF_region_showroom where class_02 = 'Showrooms' and " & _
           " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_layaway)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_layaway)
        End While
        If rdr.Read Then
            Dim str_layaway As String = Trim(rdr(0).ToString)
            If str_layaway <> "" Then
                ld_layaway = CDbl(rdr(0).ToString)
            Else
                ld_layaway = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()


        '--------------------------------------------------------------
        Dim ls_salesreturn As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_salesreturn = "select sum(salesreturn) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_salesreturn = "select sum(salesreturn) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_salesreturn = "select sum(salesreturn) from CF_region_luzon where " & _
           " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_salesreturn = "select sum(salesreturn) from CF_region_showroom where class_02 = 'Showrooms' and " & _
           " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_salesreturn)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_salesreturn)
        End While
        If rdr.Read Then
            Dim str_salesreturn As String = Trim(rdr(0).ToString)
            If str_salesreturn <> "" Then
                ld_salesreturn = CDbl(rdr(0).ToString)
            Else
                ld_salesreturn = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        '--------------------------------------------------------------
        Dim ls_layawaycancel As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_layawaycancel = "select sum(layawaycancel) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_layawaycancel = "select sum(layawaycancel) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_layawaycancel = "select sum(layawaycancel) from CF_region_luzon where " & _
           " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_layawaycancel = "select sum(layawaycancel) from CF_region_showroom where class_02 = 'Showrooms' and " & _
           " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_layawaycancel)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_layawaycancel)
        End While
        If rdr.Read Then
            Dim str_layawaycancel As String = Trim(rdr(0).ToString)
            If str_layawaycancel <> "" Then
                ld_layawaycancel = CDbl(rdr(0).ToString)
            Else
                ld_layawaycancel = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        '----------------------------------------------------------
        Dim ls_interest As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_interest = "select sum(interest) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_interest = "select sum(interest) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_interest = "select sum(interest) from CF_region_luzon where " & _
          " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_interest = "select sum(interest) from CF_region_showroom where class_02 = 'Showrooms' and " & _
          " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_interest)

        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_interest)
        End While
        If rdr.Read Then
            Dim str_interest As String = Trim(rdr(0).ToString)
            If str_interest <> "" Then
                ld_interest = CDbl(rdr(0).ToString)
            Else
                ld_interest = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------------------------
        Dim ls_kppayout As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_kppayout = "select sum(kp_payout) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_kppayout = "select sum(kp_payout) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_kppayout = "select sum(kp_payout) from CF_region_luzon where " & _
           " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_kppayout = "select sum(kp_payout) from CF_region_showroom where class_02 = 'Showrooms' and " & _
           " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_kppayout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_kppayout)
        End While
        If rdr.Read Then
            Dim str_kppayout As String = Trim(rdr(0).ToString)
            If str_kppayout <> "" Then
                ld_kppayout = CDbl(rdr(0).ToString)
            Else
                ld_kppayout = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        Dim ls_kpsendout As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_kpsendout = "select sum(kp_sendout) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_kpsendout = "select sum(kp_sendout) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "VISMIN" Or Me.Session("Manager") = "VISMIN" Then
            ls_kpsendout = "select sum(kp_sendout) from CF_region_vismin where " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_kpsendout = "select sum(kp_sendout) from CF_region_luzon where " & _
          " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_kpsendout = "select sum(kp_sendout) from CF_region_showroom where class_02 = 'Showrooms' and " & _
          " transdate = '" & Me.Session("dategen") & "'"
        End If

        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_kpsendout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_kpsendout)
        End While
        If rdr.Read Then
            Dim str_kpsendout As String = Trim(rdr(0).ToString)
            If str_kpsendout <> "" Then
                ld_kpsendout = CDbl(rdr(0).ToString)
            Else
                ld_kpsendout = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------------------------
        Dim ls_kpsendoutcomm As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_kpsendoutcomm = "select sum(KP_Sendout_Comm) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_kpsendoutcomm = "select sum(KP_Sendout_Comm) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_kpsendoutcomm = "select sum(KP_Sendout_Comm) from CF_region_luzon where " & _
           " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_kpsendoutcomm = "select sum(KP_Sendout_Comm) from CF_region_showroom where class_02 = 'Showrooms' and " & _
           " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_kpsendoutcomm)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_kpsendoutcomm)
        End While
        If rdr.Read Then
            Dim str_kpsendoutcomm As String = Trim(rdr(0).ToString)
            If str_kpsendoutcomm <> "" Then
                ld_kpsendoutcomm = CDbl(rdr(0).ToString)
            Else
                ld_kpsendoutcomm = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        Dim ls_lukat As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_lukat = "select sum(Lukat) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_lukat = "select sum(Lukat) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_lukat = "select sum(lukat) from CF_region_luzon where " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_lukat = "select sum(lukat) from CF_region_showroom where class_02 = 'Showrooms' and " & _
            " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_lukat)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_lukat)
        End While
        If rdr.Read Then
            Dim str_lukat As String = Trim(rdr(0).ToString)
            If str_lukat <> "" Then
                ld_lukat = CDbl(rdr(0).ToString)
            Else
                ld_lukat = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------------------------
        Dim ls_otherincome As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_otherincome = "select sum(OtherIncome) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_otherincome = "select sum(OtherIncome) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_otherincome = "select sum(OtherIncome) from CF_region_luzon where " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_otherincome = "select sum(OtherIncome) from CF_region_showroom where class_02 = 'Showrooms' and " & _
            " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_otherincome)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_otherincome)
        End While
        If rdr.Read Then
            Dim str_otherincome As String = Trim(rdr(0).ToString)
            If str_otherincome <> "" Then
                ld_otherincome = CDbl(rdr(0).ToString)
            Else
                ld_otherincome = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        Dim ls_Prenda As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_Prenda = "select sum(prenda) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_Prenda = "select sum(prenda) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_Prenda = "select sum(prenda) from CF_region_luzon where " & _
           " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_Prenda = "select sum(prenda) from CF_region_showroom where class_02 = 'Showrooms' and " & _
           " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_Prenda)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_Prenda)
        End While
        If rdr.Read Then
            Dim str_Prenda As String = Trim(rdr(0).ToString)
            If str_Prenda <> "" Then
                ld_prenda = CDbl(rdr(0).ToString)
            Else
                ld_prenda = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------------------------
        Dim ls_telecomms As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_telecomms = "select sum(telecomms) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_telecomms = "select sum(telecomms) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_telecomms = "select sum(telecomms) from CF_region_luzon where " & _
           " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_telecomms = "select sum(telecomms) from CF_region_showroom where class_02 = 'Showrooms' and " & _
           " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_telecomms)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_telecomms)
        End While
        If rdr.Read Then
            Dim str_telecomms As String = Trim(rdr(0).ToString)
            If str_telecomms <> "" Then
                ld_telecomms = CDbl(rdr(0).ToString)
            Else
                ld_telecomms = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        Dim ls_souvenirs As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_souvenirs = "select sum(Souvenirs) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_souvenirs = "select sum(Souvenirs) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_souvenirs = "select sum(Souvenirs) from CF_region_luzon where " & _
          " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_souvenirs = "select sum(Souvenirs) from CF_region_showroom where class_02 = 'Showrooms' and " & _
          " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_souvenirs)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_souvenirs)
        End While
        If rdr.Read Then
            Dim str_souvenirs As String = Trim(rdr(0).ToString)
            If str_souvenirs <> "" Then
                ld_souvenirs = CDbl(rdr(0).ToString)
            Else
                ld_souvenirs = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------------------------
        Dim ls_Corp_Sendout As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_Corp_Sendout = "select sum(Corp_Sendout) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_Corp_Sendout = "select sum(Corp_Sendout) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_Corp_Sendout = "select sum(Corp_Sendout) from CF_region_luzon where " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_Corp_Sendout = "select sum(Corp_Sendout) from CF_region_showroom where class_02 = 'Showrooms' and " & _
            " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_Corp_Sendout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_Corp_Sendout)
        End While
        If rdr.Read Then
            Dim str_corpsendout As String = Trim(rdr(0).ToString)
            If str_corpsendout <> "" Then
                ld_corpsendout = CDbl(rdr(0).ToString)
            Else
                ld_corpsendout = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        Dim ls_Corp_Payout As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_Corp_Payout = "select sum(Corp_Payout) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_Corp_Payout = "select sum(Corp_Payout) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_Corp_Payout = "select sum(Corp_Payout) from CF_region_luzon where " & _
           " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_Corp_Payout = "select sum(Corp_Payout) from CF_region_showroom where class_02 = 'Showrooms' and " & _
           " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_Corp_Payout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_Corp_Payout)
        End While
        If rdr.Read Then
            Dim str_corpPayout As String = Trim(rdr(0).ToString)
            If str_corpPayout <> "" Then
                ld_corppayout = CDbl(rdr(0).ToString)
            Else
                ld_corppayout = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------------------------
        Dim ls_Corp_Comm As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_Corp_Comm = "select sum(Corp_Comm) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_Corp_Comm = "select sum(Corp_Comm) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_Corp_Comm = "select sum(Corp_Comm) from CF_region_luzon where " & _
           " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_Corp_Comm = "select sum(Corp_Comm) from CF_region_showroom where class_02 = 'Showrooms' and " & _
           " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_Corp_Comm)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_Corp_Comm)
        End While
        If rdr.Read Then
            Dim str_corpcomm As String = Trim(rdr(0).ToString)
            If str_corpcomm <> "" Then
                ld_corpcomm = CDbl(rdr(0).ToString)
            Else
                ld_corpcomm = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        Dim ls_WesternUnionComm As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_WesternUnionComm = "select sum(WesternUnionComm) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_WesternUnionComm = "select sum(WesternUnionComm) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_WesternUnionComm = "select sum(WesternUnionComm) from CF_region_luzon where " & _
           " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_WesternUnionComm = "select sum(WesternUnionComm) from CF_region_showroom where class_02 = 'Showrooms' and " & _
           " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_WesternUnionComm)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_WesternUnionComm)
        End While
        If rdr.Read Then
            Dim str_WesternUnionComm As String = Trim(rdr(0).ToString)
            If str_WesternUnionComm <> "" Then
                ld_westernUnionComm = CDbl(rdr(0).ToString)
            Else
                ld_westernUnionComm = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------------------------
        Dim ls_WesternUnionPayout As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_WesternUnionPayout = "select sum(WesternUnionPayout) from CF_region_vismin where class_02 = 'Visayas' and " & _
             " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_WesternUnionPayout = "select sum(WesternUnionPayout) from CF_region_vismin where class_02 = 'Mindanao' and " & _
             " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_WesternUnionPayout = "select sum(WesternUnionPayout) from CF_region_luzon where " & _
           " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_WesternUnionPayout = "select sum(WesternUnionPayout) from CF_region_showroom where class_02 = 'Showrooms' and " & _
           " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_WesternUnionPayout)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_WesternUnionPayout)
        End While
        If rdr.Read Then
            Dim str_WesternUnionPayout As String = Trim(rdr(0).ToString)
            If str_WesternUnionPayout <> "" Then
                ld_westernunionPayout = CDbl(rdr(0).ToString)
            Else
                ld_westernunionPayout = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        '     Dim ls_WesternUnionSendout As String = "select sum(WesternUnionSendout) from CF_region_vismin where " & _
        '" transdate = '" + transdate + "'"
        'Dim ls_WesternUnionSendout As String = "SELECT SUM(bal) FROM (SELECT (SUM(CASE WHEN ((g.FinYear<2012) OR (g.FinYear=2012 AND g.TransType<>'P'))  AND a.bal_vw='B' THEN AmountDebitAC-AmountCreditAC ELSE 0 END)/1) bal FROM Balance g INNER JOIN grtbk ca ON g.CompanyCode=ca.CompanyCode AND g.CompanyAccountCode=ca.reknr INNER JOIN grtbk a ON a.CompanyCode IS NULL AND ca.AccountCode=a.reknr  INNER JOIN bedryf c ON c.bedrnr = g.CompanyCode  INNER JOIN bedryf b ON g.CompanyCode=b.bedrnr  AND b.Class_01='Visayas/Mindanao' WHERE a.reknr='  1020001' AND g.transtype IN ('N','C','I','P','F') AND ((g.FinYear<2012) OR (g.FinYear=2012 AND g.FinPeriod<  7))  AND a.bal_vw='B' group by b.valcode         UNION ALL SELECT (SUM(bdr_hfl)/1) bal FROM gbkmut g INNER JOIN grtbk ca ON g.CompanyCode=ca.CompanyCode AND g.reknr=ca.reknr INNER JOIN grtbk a ON a.CompanyCode IS NULL AND ca.AccountCode=a.reknr  INNER JOIN bedryf c ON c.bedrnr = g.CompanyCode  INNER JOIN bedryf b ON g.CompanyCode=b.bedrnr  AND b.Class_01='Visayas/Mindanao' WHERE a.reknr='  1020001' AND g.transtype IN ('N','C','I','P','F') AND g.verwerknrl<>0 AND  g.bkjrcode=2012 AND g.periode='  7' AND g.datum<{d '2012-07-02'}) x"
        Dim date1 As DateTime = (Me.Session("dategen"))
        Dim year As String = date1.Year
        Dim month As String = date1.Month
        Dim char_month As String = Nothing
        If Len(month) = 1 Then
            char_month = "  " & month
        ElseIf Len(month) = 2 Then
            char_month = " " & month
        End If '
        Dim ls_WesternUnionSendout As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            strCon = Me.Session("strConfRVisayas")
            ls_WesternUnionSendout = "exec spCashRRBeginning_CF_Ver3 '" & year & "'," & month & ",'" & (Me.Session("dategen")) & "'," & char_month & ""
            db.ConnectDB(strCon)

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            strCon = Me.Session("strConfRMindanao")
            ls_WesternUnionSendout = "exec spCashRRBeginning_CF_Ver3 '" & year & "'," & month & ",'" & (Me.Session("dategen")) & "'," & char_month & ""
            db.ConnectDB(strCon)

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_WesternUnionSendout = "select sum(WesternUnionSendout) from CF_region_Luzon where " & _
            " transdate = '" & Me.Session("dategen") & "'"
            db.ConnectDB(strCon1)

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_WesternUnionSendout = "select sum(WesternUnionSendout) from CF_region_Showroom where class_02 = 'Showrooms' and " & _
            " transdate = '" & Me.Session("dategen") & "'"
            db.ConnectDB(strCon1)
        End If

        rdr = db.Execute_SQL_DataReader(ls_WesternUnionSendout)

        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_WesternUnionSendout)
        End While

        If rdr.Read Then
            Dim str_WesternUnionSendout As String = Trim(rdr(0).ToString)
            If str_WesternUnionSendout <> "" Then
                ld_westernunionsendout = CDbl(rdr(0).ToString)
            Else
                ld_westernunionsendout = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------------------------
        Dim ls_FundTransferDebit As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_FundTransferDebit = "select sum(FundTransferDebit) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_FundTransferDebit = "select sum(FundTransferDebit) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_FundTransferDebit = "select sum(FundTransferDebit) from CF_region_luzon where " & _
          " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_FundTransferDebit = "select sum(FundTransferDebit) from CF_region_showroom where class_02 = 'Showrooms' and " & _
          " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_FundTransferDebit)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_FundTransferDebit)
        End While
        If rdr.Read Then
            Dim str_FundTransferDebit As String = Trim(rdr(0).ToString)
            If str_FundTransferDebit <> "" Then
                ld_fundtransferdebit = CDbl(rdr(0).ToString)
            Else
                ld_fundtransferdebit = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        Dim ls_FundTransferCredit As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_FundTransferCredit = "select sum(FundTransferCredit) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_FundTransferCredit = "select sum(FundTransferCredit) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_FundTransferCredit = "select sum(FundTransferCredit) from CF_region_luzon where " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_FundTransferCredit = "select sum(FundTransferCredit) from CF_region_showroom where class_02 = 'Showrooms' and " & _
            " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_FundTransferCredit)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_FundTransferCredit)
        End While
        If rdr.Read Then
            Dim str_FundTransferCredit As String = Trim(rdr(0).ToString)
            If str_FundTransferCredit <> "" Then
                ld_fundtransfercredit = CDbl(rdr(0).ToString)
            Else
                ld_fundtransfercredit = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '--------------------------------------------------------------
        Dim ls_BranchExpense As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_BranchExpense = "select sum(BranchExpense) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_BranchExpense = "select sum(BranchExpense) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_BranchExpense = "select sum(BranchExpense) from CF_region_luzon where " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_BranchExpense = "select sum(BranchExpense) from CF_region_showroom where class_02 = 'Showrooms' and " & _
            " transdate = '" & Me.Session("dategen") & "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_BranchExpense)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_BranchExpense)
        End While
        If rdr.Read Then
            Dim str_BranchExpense As String = Trim(rdr(0).ToString)
            If str_BranchExpense <> "" Then
                ld_branchexpense = CDbl(rdr(0).ToString)
            Else
                ld_branchexpense = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        '----------------------------------------------------------
        Dim ls_OtherExpense As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_OtherExpense = "select sum(OtherExpense) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_OtherExpense = "select sum(OtherExpense) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_OtherExpense = "select sum(OtherExpense) from CF_region_luzon where " & _
           " transdate = '" & Me.Session("dategen") & "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_OtherExpense = "select sum(OtherExpense) from CF_region_showroom where class_02 = 'Showrooms' and " & _
           " transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_OtherExpense)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_OtherExpense)
        End While
        If rdr.Read Then
            Dim str_OtherExpense As String = Trim(rdr(0).ToString)
            If str_OtherExpense <> "" Then
                ld_otherexpense = CDbl(rdr(0).ToString)
            Else
                ld_otherexpense = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------

        '/-added last 2/9/2011
        '----------------------------------------------------------
        Dim ls_nso As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_nso = "select sum(NSO) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_nso = "select sum(NSO) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_nso = "select sum(NSO) from CF_region_luzon where " & _
           " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_nso = "select sum(NSO) from CF_region_showroom where class_02 = 'Showrooms' and " & _
           " transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_nso)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_nso)
        End While
        If rdr.Read Then
            Dim str_nso As String = Trim(rdr(0).ToString)
            If str_nso <> "" Then
                ld_nso = CDbl(rdr(0).ToString)
            Else
                ld_nso = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_mccr As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_mccr = "select sum(MCCashReceipts) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_mccr = "select sum(MCCashReceipts) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_mccr = "select sum(MCCashReceipts) from CF_region_luzon where " & _
            " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_mccr = "select sum(MCCashReceipts) from CF_region_showroom where class_02 = 'Showrooms' and " & _
            " transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_mccr)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_mccr)
        End While
        If rdr.Read Then
            Dim str_mccr As String = Trim(rdr(0).ToString)
            If str_mccr <> "" Then
                ld_MCCR = CDbl(rdr(0).ToString)
            Else
                ld_MCCR = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_mccd As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_mccd = "select sum(MCCashDisbursements) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_mccd = "select sum(MCCashDisbursements) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_mccd = "select sum(MCCashDisbursements) from CF_region_luzon where " & _
           " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_mccd = "select sum(MCCashDisbursements) from CF_region_showroom where class_02 = 'Showrooms' and " & _
           " transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_mccd)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_mccd)
        End While
        If rdr.Read Then
            Dim str_mccd As String = Trim(rdr(0).ToString)
            If str_mccd <> "" Then
                ld_MCCD = CDbl(rdr(0).ToString)
            Else
                ld_MCCD = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_racr As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_racr = "select sum(RACashReceipts) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_racr = "select sum(RACashReceipts) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_racr = "select sum(RACashReceipts) from CF_region_luzon where " & _
           " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_racr = "select sum(RACashReceipts) from CF_region_showroom where class_02 = 'Showrooms' and " & _
           " transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_racr)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_racr)
        End While
        If rdr.Read Then
            Dim str_racr As String = Trim(rdr(0).ToString)
            If str_racr <> "" Then
                ld_RACR = CDbl(rdr(0).ToString)
            Else
                ld_RACR = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_racd As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_racd = "select sum(RACashDisbursements) from CF_region_vismin where class_02 = 'Visayas' and " & _
             " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_racd = "select sum(RACashDisbursements) from CF_region_vismin where class_02 = 'Mindanao' and " & _
             " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_racd = "select sum(RACashDisbursements) from CF_region_luzon where " & _
            " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_racd = "select sum(RACashDisbursements) from CF_region_showroom where class_02 = 'Showrooms' and " & _
            " transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_racd)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_racd)
        End While
        If rdr.Read Then
            Dim str_racd As String = Trim(rdr(0).ToString)
            If str_racd <> "" Then
                ld_RACD = CDbl(rdr(0).ToString)
            Else
                ld_RACD = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()

        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_dfb As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_dfb = "select sum(DepositFromBank) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_dfb = "select sum(DepositFromBank) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_dfb = "select sum(DepositFromBank) from CF_region_luzon where " & _
           " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_dfb = "select sum(DepositFromBank) from CF_region_showroom where class_02 = 'Showrooms' and " & _
           " transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_dfb)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_dfb)
        End While
        If rdr.Read Then
            Dim str_dfb As String = Trim(rdr(0).ToString)
            If str_dfb <> "" Then
                ld_Depositfrombank = CDbl(rdr(0).ToString)
            Else
                ld_Depositfrombank = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_wfb As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_wfb = "select sum(WithdrawalFromBank) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_wfb = "select sum(WithdrawalFromBank) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_wfb = "select sum(WithdrawalFromBank) from CF_region_luzon where " & _
           " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_wfb = "select sum(WithdrawalFromBank) from CF_region_showroom where class_02 = 'Showrooms' and " & _
           " transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_wfb)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_wfb)
        End While
        If rdr.Read Then
            Dim str_wfb As String = Trim(rdr(0).ToString)
            If str_wfb <> "" Then
                ld_withdrawalfrombank = CDbl(rdr(0).ToString)
            Else
                ld_withdrawalfrombank = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_rts As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_rts = "select sum(returntosender) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_rts = "select sum(returntosender) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_rts = "select sum(returntosender) from CF_region_luzon where " & _
          " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_rts = "select sum(returntosender) from CF_region_showroom where class_02 = 'Showrooms' and " & _
          " transdate = '" + Me.Session("dategen") + "'"

        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_rts)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_rts)
        End While
        If rdr.Read Then
            Dim str_rts As String = Trim(rdr(0).ToString)
            If str_rts <> "" Then
                ld_returntosender = CDbl(rdr(0).ToString)
            Else
                ld_returntosender = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        '\-added last 2/9/2011

        '/-added last 7/26/2011
        '----------------------------------------------------------
        Dim ls_cashover As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_cashover = "select isnull(sum(cashover),0) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_cashover = "select isnull(sum(cashover),0) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_cashover = "select isnull(sum(cashover),0) from CF_region_luzon where " & _
           " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_cashover = "select isnull(sum(cashover),0) from CF_region_showroom where class_02 = 'Showrooms' and " & _
           " transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_cashover)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_cashover)
        End While
        If rdr.Read Then
            Dim str_cashover As String = Trim(rdr(0).ToString)
            If str_cashover <> "" Then
                ld_CashOver = CDbl(rdr(0).ToString)
            Else
                ld_CashOver = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_cashshort As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_cashshort = "select isnull(sum(cashshort),0) from CF_region_vismin where class_02 = 'Visayas' and " & _
            " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_cashshort = "select isnull(sum(cashshort),0) from CF_region_vismin where class_02 = 'Mindanao' and " & _
            " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_cashshort = "select isnull(sum(cashshort),0) from CF_region_luzon where " & _
           " transdate = '" + Me.Session("dategen") + "'"

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_cashshort = "select isnull(sum(cashshort),0) from CF_region_showroom where class_02 = 'Showrooms' and " & _
           " transdate = '" + Me.Session("dategen") + "'"
        End If
        db.ConnectDB(strCon1)
        rdr = db.Execute_SQL_DataReader(ls_cashshort)
        While rdr Is Nothing 'jeniena
            rdr = db.Execute_SQL_DataReader(ls_cashshort)
        End While
        If rdr.Read Then
            Dim str_cashshort As String = Trim(rdr(0).ToString)
            If str_cashshort <> "" Then
                ld_CashShort = CDbl(rdr(0).ToString)
            Else
                ld_CashShort = 0
            End If
        End If
        rdr.Close()
        db.CloseConnection()
        '----------------------------------------------------------
        Dim ls_update As String = Nothing
        If Me.Session("WideN") = "VISAYAS" Or Me.Session("Manager") = "VISAYAS" Then
            ls_update = "UPDATE [CF_VisMin_wide]" & _
               " SET [BeginningBalance]=" & ld_begbal & ", [EndingBalance]=" & ld_endingbal & ", [FoodProducts]=" & ld_foodproducts & ",[Insurance]=" & ld_insurance & ", [outrightsales]=" & ld_outrightsales & ", [layaway]=" & ld_layaway & ", [salesreturn]=" & ld_salesreturn & ", [layawaycancel]=" & ld_layawaycancel & ",[Interest]=" & ld_interest & ",  " & _
               " [KP_Payout]= " & ld_kppayout & ", [KP_Sendout]=" & ld_kpsendout & ", [KP_Sendout_Comm]=" & ld_kpsendoutcomm & " , [Lukat]= " & ld_lukat & ", [OtherIncome]= " & ld_otherincome & "  , [Prenda]=" & ld_prenda & " , " & _
               " [Telecomms] = " & ld_telecomms & ", [Souvenirs]=" & ld_souvenirs & ", [Corp_Sendout]=" & ld_corpsendout & ", [Corp_Payout]=" & ld_corppayout & ",[Corp_Comm]=" & ld_corpcomm & " ,[WesternUnionComm]=" & ld_westernUnionComm & ", " & _
               " [WesternUnionPayout]=" & ld_westernunionPayout & ", [WesternUnionSendout]=" & ld_westernunionsendout & ", [FundTransferDebit]=" & ld_fundtransferdebit & ", [FundTransferCredit]=" & ld_fundtransfercredit & ", " & _
               " [BranchExpense]=" & ld_branchexpense & ",[OtherExpense] = " & ld_otherexpense & " ,[NSO] = " & ld_nso & " ,[MCCashReceipts] = " & ld_MCCR & ",[MCCashDisbursements] = " & ld_MCCD & ",[RACashReceipts] = " & ld_RACR & ",[RACashDisbursements] = " & ld_RACD & ",[DepositFromBank] = " & ld_Depositfrombank & " ,[WithdrawalFromBank] = " & ld_withdrawalfrombank & " ,[ReturnToSender] = " & ld_returntosender & " ,[cashover] = " & ld_CashOver & " ,[cashshort] = " & ld_CashShort & "  ,[dategenerated] = '" + Now.Date + "'         WHERE class_02 = 'Visayas' and  transdate = '" + Me.Session("dategen") + "' "

        ElseIf Me.Session("WideN") = "MINDANAO" Or Me.Session("Manager") = "MINDANAO" Then
            ls_update = "UPDATE [CF_VisMin_wide]" & _
               " SET [BeginningBalance]=" & ld_begbal & ", [EndingBalance]=" & ld_endingbal & ", [FoodProducts]=" & ld_foodproducts & ",[Insurance]=" & ld_insurance & ", [outrightsales]=" & ld_outrightsales & ", [layaway]=" & ld_layaway & ", [salesreturn]=" & ld_salesreturn & ", [layawaycancel]=" & ld_layawaycancel & ",[Interest]=" & ld_interest & ",  " & _
               " [KP_Payout]= " & ld_kppayout & ", [KP_Sendout]=" & ld_kpsendout & ", [KP_Sendout_Comm]=" & ld_kpsendoutcomm & " , [Lukat]= " & ld_lukat & ", [OtherIncome]= " & ld_otherincome & "  , [Prenda]=" & ld_prenda & " , " & _
               " [Telecomms] = " & ld_telecomms & ", [Souvenirs]=" & ld_souvenirs & ", [Corp_Sendout]=" & ld_corpsendout & ", [Corp_Payout]=" & ld_corppayout & ",[Corp_Comm]=" & ld_corpcomm & " ,[WesternUnionComm]=" & ld_westernUnionComm & ", " & _
               " [WesternUnionPayout]=" & ld_westernunionPayout & ", [WesternUnionSendout]=" & ld_westernunionsendout & ", [FundTransferDebit]=" & ld_fundtransferdebit & ", [FundTransferCredit]=" & ld_fundtransfercredit & ", " & _
               " [BranchExpense]=" & ld_branchexpense & ",[OtherExpense] = " & ld_otherexpense & " ,[NSO] = " & ld_nso & " ,[MCCashReceipts] = " & ld_MCCR & ",[MCCashDisbursements] = " & ld_MCCD & ",[RACashReceipts] = " & ld_RACR & ",[RACashDisbursements] = " & ld_RACD & ",[DepositFromBank] = " & ld_Depositfrombank & " ,[WithdrawalFromBank] = " & ld_withdrawalfrombank & " ,[ReturnToSender] = " & ld_returntosender & " ,[cashover] = " & ld_CashOver & " ,[cashshort] = " & ld_CashShort & "  ,[dategenerated] = '" + Now.Date + "'         WHERE class_02 = 'Mindanao' and  transdate = '" + Me.Session("dategen") + "' "

        ElseIf Me.Session("WideN") = "LUZON" Or Me.Session("Manager") = "LUZON" Then
            ls_update = "UPDATE [CF_Luzon_wide]" & _
              " SET [BeginningBalance]=" & ld_begbal & ", [EndingBalance]=" & ld_endingbal & ", [FoodProducts]=" & ld_foodproducts & ",[Insurance]=" & ld_insurance & ", [outrightsales]=" & ld_outrightsales & ", [layaway]=" & ld_layaway & ", [salesreturn]=" & ld_salesreturn & ", [layawaycancel]=" & ld_layawaycancel & ",[Interest]=" & ld_interest & ",  " & _
              " [KP_Payout]= " & ld_kppayout & ", [KP_Sendout]=" & ld_kpsendout & ", [KP_Sendout_Comm]=" & ld_kpsendoutcomm & " , [Lukat]= " & ld_lukat & ", [OtherIncome]= " & ld_otherincome & "  , [Prenda]=" & ld_prenda & " , " & _
              " [Telecomms] = " & ld_telecomms & ", [Souvenirs]=" & ld_souvenirs & ", [Corp_Sendout]=" & ld_corpsendout & ", [Corp_Payout]=" & ld_corppayout & ",[Corp_Comm]=" & ld_corpcomm & " ,[WesternUnionComm]=" & ld_westernUnionComm & ", " & _
              " [WesternUnionPayout]=" & ld_westernunionPayout & ", [WesternUnionSendout]=" & ld_westernunionsendout & ", [FundTransferDebit]=" & ld_fundtransferdebit & ", [FundTransferCredit]=" & ld_fundtransfercredit & ", " & _
              " [BranchExpense]=" & ld_branchexpense & ",[OtherExpense] = " & ld_otherexpense & " ,[NSO] = " & ld_nso & " ,[MCCashReceipts] = " & ld_MCCR & ",[MCCashDisbursements] = " & ld_MCCD & ",[RACashReceipts] = " & ld_RACR & ",[RACashDisbursements] = " & ld_RACD & ",[DepositFromBank] = " & ld_Depositfrombank & " ,[WithdrawalFromBank] = " & ld_withdrawalfrombank & " ,[ReturnToSender] = " & ld_returntosender & " ,[cashover] = " & ld_CashOver & " ,[cashshort] = " & ld_CashShort & "  ,[dategenerated] = '" + Now.Date + "'         WHERE  transdate = '" + Me.Session("dategen") + "' "

        ElseIf Me.Session("WideN") = "SHOWROOM" Or Me.Session("Manager") = "SHOWROOM" Then
            ls_update = "UPDATE [CF_Showroom_wide]" & _
              " SET [BeginningBalance]=" & ld_begbal & ", [EndingBalance]=" & ld_endingbal & ", [FoodProducts]=" & ld_foodproducts & ",[Insurance]=" & ld_insurance & ", [outrightsales]=" & ld_outrightsales & ", [layaway]=" & ld_layaway & ", [salesreturn]=" & ld_salesreturn & ", [layawaycancel]=" & ld_layawaycancel & ",[Interest]=" & ld_interest & ",  " & _
              " [KP_Payout]= " & ld_kppayout & ", [KP_Sendout]=" & ld_kpsendout & ", [KP_Sendout_Comm]=" & ld_kpsendoutcomm & " , [Lukat]= " & ld_lukat & ", [OtherIncome]= " & ld_otherincome & "  , [Prenda]=" & ld_prenda & " , " & _
              " [Telecomms] = " & ld_telecomms & ", [Souvenirs]=" & ld_souvenirs & ", [Corp_Sendout]=" & ld_corpsendout & ", [Corp_Payout]=" & ld_corppayout & ",[Corp_Comm]=" & ld_corpcomm & " ,[WesternUnionComm]=" & ld_westernUnionComm & ", " & _
              " [WesternUnionPayout]=" & ld_westernunionPayout & ", [WesternUnionSendout]=" & ld_westernunionsendout & ", [FundTransferDebit]=" & ld_fundtransferdebit & ", [FundTransferCredit]=" & ld_fundtransfercredit & ", " & _
              " [BranchExpense]=" & ld_branchexpense & ",[OtherExpense] = " & ld_otherexpense & " ,[NSO] = " & ld_nso & " ,[MCCashReceipts] = " & ld_MCCR & ",[MCCashDisbursements] = " & ld_MCCD & ",[RACashReceipts] = " & ld_RACR & ",[RACashDisbursements] = " & ld_RACD & ",[DepositFromBank] = " & ld_Depositfrombank & " ,[WithdrawalFromBank] = " & ld_withdrawalfrombank & " ,[ReturnToSender] = " & ld_returntosender & " ,[cashover] = " & ld_CashOver & " ,[cashshort] = " & ld_CashShort & "  ,[dategenerated] = '" + Now.Date + "'         WHERE class_02 = 'Showrooms' and  transdate = '" + Me.Session("dategen") + "' "

        End If
        db.ConnectDB(strCon1)
        If db.Execute_SQLQuery(ls_update) = -1 Then
            db.RollbackTransaction()
        End If
        db.CloseConnection()

    End Sub

    Protected Sub CrystalReportViewer1_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles CrystalReportViewer1.Init

    End Sub
End Class
