Imports INI_DLL
Imports DB_DLL

Public Class Form1
    Dim sqlmsg As String = Nothing
    Dim transdate As DateTime = Nothing
    Dim transmonth As String = Nothing
    Dim transyear As String = Nothing
    Dim gb_branchcode As String = Nothing
    Dim ls_amount As String = Nothing
    Dim gi_idcount_pb As String = Nothing
    Dim gi_idcount_area As String = Nothing
    Dim gi_idcount_region As String = Nothing
    Dim gi_idcount_visminwide As String = Nothing
    Dim gb_pb_update As Boolean = Nothing
    Dim gb_area_update As Boolean = Nothing 'added last 9-3-2010
    Dim gb_region_update As Boolean = Nothing 'added last 9-3-2010
    Dim gb_vismin_update As Boolean = Nothing 'added last 9-3-2010
    Dim strcon As String = Nothing
    Dim gs_bcode As String = Nothing
    Dim gs_class04 As String = Nothing ' added last 9-3-2010
    Dim gs_class03 As String = Nothing ' added last 9-3-2010



    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
  
        DtTransdate.Text = Now
        gs_transdatereport_title = Convert.ToDateTime(DtTransdate.Text).ToString("MM-dd-yyyy")
        'select_per_branch()
        'select_area()
        'select_region()
        'Tmr_CashFlow.Start()
    End Sub
    Private Sub IniReport()
        Dim ini_Path As String = AppDomain.CurrentDomain.BaseDirectory + "maincashflow.ini"
        Dim line As String = Nothing

        Dim server, db, uname, pass As String
        Dim rdr As New ReadWriteINI
        Dim strConfReport As String

        server = rdr.readINI("SERVER INI", "SERVER", False, ini_Path)
        db = rdr.readINI("SERVER INI", "DBNAME", False, ini_Path)
        uname = rdr.readINI("SERVER INI", "USERNAME", False, ini_Path)
        pass = rdr.readINI("SERVER INI", "PASSWORD", False, ini_Path)
        strConfReport = "user id=" & uname & ";password=" & pass & ";data source=" & server & ";persist security info=False;initial catalog=" & db & "; Connection Timeout = 3600;"
    End Sub
    Private Sub calltime()
        Dim sr As New IO.StreamReader(Application.StartupPath & "\MainCashFlow.ini")
        Dim line As String = Nothing
        line = sr.ReadLine
        While Not line Is Nothing
            line = line.Replace(" =", "=").Replace("= ", "=")

            If line.StartsWith("[Time Start]=") Then
                ls_timestart = Replace(line, "[Time Start]=", "")
            End If
            If line.StartsWith("[Time End]=") Then
                ls_timeend = Replace(line, "[Time End]=", "")
            End If
            'If line.StartsWith("[password]=") Then
            '    pass = decryptPass(Replace(line, "[passw`ord]=", ""))
            'End If
            line = sr.ReadLine
        End While
        sr.Close()
    End Sub
    Private Sub calldate()
        Dim ls_transdate As String = Nothing
        Dim ls_periode As String = Nothing

        ls_transdate = DtTransdate.Text
        transdate = Format(CDate(ls_transdate), "yyyy-MM-dd") ' example 2010-08-28
        transmonth = Format(CDate(ls_transdate), "MM") 'example 08
        transyear = Format(CDate(ls_transdate), "yyyy") 'example 2010
    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Tmr_CashFlow.Stop()
        Dim li_i As Integer = Nothing
        Dim li_idcount As Integer = Nothing
        Dim li_count As Integer = Nothing
        Dim li_ii As Integer = 0
        Dim ls_transdate As String = Nothing


        calldate()

        For li_count = 0 To 30
            transdate = DateAdd(DateInterval.Day, -1, transdate)
            insertion_to_cf_pb_vismin()
            insert_to_CF_area_vismin() 'MsgBox("area finished") - per area insertion
            insert_to_CF_region_vismin() 'MsgBox("region finished") - per region insertion
            insert_to_CF_vismin_wide() 'MsgBox("vismin finished") - vismin wide insertion.
        Next

        'For li_i = 0 To ListViewPb.Items.Count - 1
        '    ' insert code here.
        'Next
        '------------------------
        'MsgBox("Insert Finished")
    End Sub
    Private Sub insert_to_CF_vismin_wide()
        Dim li_i As Integer = Nothing
        Dim li_idcount As Integer = Nothing
        Dim li_listviewcount As Integer = Nothing

        li_listviewcount = ListViewRegion.Items.Count - 1
        ' For li_i = 0 To li_listviewcount
        'Dim ls_class03 As String = Nothing
        ' ls_class03 = Trim(ListViewRegion.Items(li_i).Text.ToString)

        ' Dim class03 As String = Replace(ls_class03, ",", " ")
        id_count_vismin_wide()
        li_idcount = gi_idcount_visminwide
        li_idcount = li_idcount + 1
        Dim ld_begbal As Double = Nothing 'beginning balance
        Dim ld_endingbal As Double = Nothing 'ending balance
        Dim ld_foodproducts As Double = Nothing 'food products
        Dim ld_insurance As Double = Nothing 'insurance
        Dim ld_outrightsales As Double = Nothing 'outright sales-------------------jen
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
        '\-------------------added last 7/26/2011 due to judith findings


        Dim ld_begbal_lv As Double = Nothing 'beginning balance
        Dim ld_endingbal_lv As Double = Nothing 'ending balance
        Dim ld_foodproducts_lv As Double = Nothing 'food products
        Dim ld_insurance_lv As Double = Nothing 'insurance
        Dim ld_outrightsales_lv As Double = Nothing 'outright sales--------------------------jen
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

        '/-------------------added last 2/8/2011 due to judith findings
        Dim ld_nso_lv As Double = Nothing 'NSO
        Dim ld_MCCR_lv As Double = Nothing 'Money Changer Cash Receipts
        Dim ld_MCCD_lv As Double = Nothing 'Money Changer Cash Disbursements
        Dim ld_RACR_lv As Double = Nothing 'Renewal anywhere Cash Receipts
        Dim ld_RACD_lv As Double = Nothing ' Renewal Anywhere Cash Disbursements
        Dim ld_Depositfrombank_lv As Double = Nothing ' 'Deposit From Bank
        Dim ld_withdrawalfrombank_lv As Double = Nothing ' Withdrawal from Bank
        Dim ld_returntosender_lv As Double = Nothing ' Returntosender 2/10/2011
        '\-------------------added last 2/8/2011 due to judith findings

        '/-------------------added last 7/26/2011 due to judith findings
        Dim ld_CashShort_lv As Double = Nothing 'Cash Short
        Dim ld_CashOver_lv As Double = Nothing 'Cash Over
        '\-------------------added last 7/26/2011 due to judith findings


        'code starts here-------------------------------------------------------------------
        'updated by Arthur 6/17/2013
        Dim ls_begbal As String = "select sum(beginningbalance) from CF_region_vismin where class_02 = 'Mindanao' and " & _
        " transdate = '" + transdate + "'"
        Dim c As New clsData
        Dim rdr As SqlClient.SqlDataReader = Nothing
        If c.Error_Inititalize_INI Then Exit Sub
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_begbal, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_begbal = 0.0
                Else
                    ld_begbal = Trim(rdr(0)) 'area beginning balance
                End If
            End If
        End If
        c.DisposeR()
        '--------------------------------------------------------------
        'updated by Arthur 6/17/2013
        Dim ls_endingbal As String = "select sum(EndingBalance) from CF_region_vismin where class_02 = 'Mindanao' and " & _
          " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_endingbal, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_endingbal = 0.0
                Else
                    ld_endingbal = Trim(rdr(0)) 'area ending balance
                End If
            End If
        End If
        c.DisposeR()
        '----------------------------------------------------------
        'updated by Arthur 6/17/2013
        Dim ls_foodproducts As String = "select sum(Foodproducts) from CF_region_vismin where class_02 = 'Mindanao' and " & _
          " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_foodproducts, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_foodproducts = 0.0
                Else
                    ld_foodproducts = Trim(rdr(0)) 'area food products
                End If
            End If
        End If
        c.DisposeR()
        '--------------------------------------------------------------
        'updated by Arthur 6/17/2013
        Dim ls_insurance As String = "select sum(Insurance) from CF_region_vismin where class_02 = 'Mindanao' and " & _
    " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_insurance, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_insurance = 0.0
                Else
                    ld_insurance = Trim(rdr(0)) 'area Insurance
                End If
            End If
        End If
        c.DisposeR()
        '--------------------------------------------------------------jen 
        'updated by Arthur 6/17/2013
        'Dim ls_outrightsales As String = "select sum(outrightsales) from CF_region_vismin where class_02 = 'Mindanao' and transdate =  '" & transdate & "','" & "" & "','" & "" & "','" & "" & "'"
        Dim ls_outrightsales As String = "select sum(outrightsales) from CF_region_vismin where class_02 = 'Mindanao' and transdate =  '" & transdate & "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_outrightsales, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_outrightsales = 0.0
                Else
                    ld_outrightsales = Trim(rdr(0))
                End If
            End If
        End If
        'If Not c.Error_SetRdr(ls_outrightsales, rdr, sqlmsg) Then
        '    If rdr.Read Then
        '        ld_outrightsales = Trim(rdr(0))
        '    End If
        'End If
        c.DisposeR()

        '--------------------------------------------------------------
        'updated by Arthur 6/17/2013
        'Dim ls_layaway As String = "select sum(layaway) from CF_region_vismin where class_02 = 'Mindanao' and transdate =  '" & transdate & "','" & "" & "','" & "" & "','" & "" & "'"
        Dim ls_layaway As String = "select sum(layaway) from CF_region_vismin where class_02 = 'Mindanao' and transdate =  '" & transdate & "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_layaway, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_layaway = 0.0
                Else
                    ld_layaway = Trim(rdr(0))
                End If
            End If
        End If
        c.DisposeR()

        '--------------------------------------------------------------
        'updated by Arthur 6/17/2013
        'Dim ls_salesreturn As String = "select sum(salesreturn) from CF_region_vismin where class_02 = 'Mindanao' and transdate =  '" & transdate & "','" & "" & "','" & "" & "','" & "" & "'"
        Dim ls_salesreturn As String = "select sum(salesreturn) from CF_region_vismin where class_02 = 'Mindanao' and transdate =  '" & transdate & "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_salesreturn, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_salesreturn = 0.0
                Else
                    ld_salesreturn = Trim(rdr(0))
                End If
            End If
        End If
        c.DisposeR()

        '--------------------------------------------------------------
        'updated by Arthur 6/17/2013
        'Dim ls_layawaycancel As String = "select sum(layawaycancel) from CF_region_vismin where class_02 = 'Mindanao' and transdate =  '" & transdate & "','" & "" & "','" & "" & "','" & "" & "'"
        Dim ls_layawaycancel As String = "select sum(layawaycancel) from CF_region_vismin where class_02 = 'Mindanao' and transdate =  '" & transdate & "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_layawaycancel, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_layawaycancel = 0.0
                Else
                    ld_layawaycancel = Trim(rdr(0))
                End If
            End If
        End If
        c.DisposeR()


        '----------------------------------------------------------
        'updated by Arthur 6/17/2013
        Dim ls_interest As String = "select sum(interest) from CF_region_vismin where class_02 = 'Mindanao' and " & _
    " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_interest, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_interest = 0.0
                Else
                    ld_interest = Trim(rdr(0)) 'area interest
                End If
            End If
        End If
        c.DisposeR()
        '--------------------------------------------------------------
        'updated by Arthur 6/17/2013
        Dim ls_kppayout As String = "select sum(kp_payout) from CF_region_vismin where class_02 = 'Mindanao' and " & _
    " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_kppayout, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_kppayout = 0.0
                Else
                    ld_kppayout = Trim(rdr(0)) 'area kp payout
                End If
            End If
        End If
        c.DisposeR()
        '----------------------------------------------------------
        'updated by Arthur 6/17/2013
        Dim ls_kpsendout As String = "select sum(kp_sendout) from CF_region_vismin where class_02 = 'Mindanao' and " & _
    " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_kpsendout, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_kpsendout = 0.0
                Else
                    ld_kpsendout = Trim(rdr(0)) 'area kp sendout
                End If
            End If
        End If
        c.DisposeR()
        '--------------------------------------------------------------
        'updated by Arthur 6/17/2013
        Dim ls_kpsendoutcomm As String = "select sum(KP_Sendout_Comm) from CF_region_vismin where class_02 = 'Mindanao' and " & _
    " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_kpsendoutcomm, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_kpsendoutcomm = 0.0
                Else
                    ld_kpsendoutcomm = Trim(rdr(0)) 'area kp sendout comm
                End If
            End If
        End If
        c.DisposeR()
        '----------------------------------------------------------
        'updated by Arthur 6/17/2013
        Dim ls_lukat As String = "select sum(Lukat) from CF_region_vismin where class_02 = 'Mindanao' and " & _
    " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_lukat, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_lukat = 0.0
                Else
                    ld_lukat = Trim(rdr(0)) 'area lukat
                End If
            End If
        End If
        c.DisposeR()
        '--------------------------------------------------------------
        'updated by Arthur 6/17/2013
        Dim ls_otherincome As String = "select sum(OtherIncome) from CF_region_vismin where class_02 = 'Mindanao' and " & _
    " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_otherincome, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_otherincome = 0.0
                Else
                    ld_otherincome = Trim(rdr(0)) 'area otherincome
                End If
            End If
        End If
        c.DisposeR()
        '----------------------------------------------------------
        'updated by Arthur 6/17/2013
        Dim ls_Prenda As String = "select sum(prenda) from CF_region_vismin where class_02 = 'Mindanao' and " & _
      " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_Prenda, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_prenda = 0.0
                Else
                    ld_prenda = Trim(rdr(0)) 'area prenda
                End If
            End If
        End If
        c.DisposeR()
        '--------------------------------------------------------------
        'updated by Arthur 6/17/2013
        Dim ls_telecomms As String = "select sum(telecomms) from CF_region_vismin where class_02 = 'Mindanao' and " & _
       " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_telecomms, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_telecomms = 0.0
                Else
                    ld_telecomms = Trim(rdr(0)) 'area telecomms
                End If
            End If
        End If
        c.DisposeR()
        '----------------------------------------------------------
        'updated by Arthur 6/17/2013
        Dim ls_souvenirs As String = "select sum(Souvenirs) from CF_region_vismin where class_02 = 'Mindanao' and " & _
       " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_souvenirs, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_souvenirs = 0.0
                Else
                    ld_souvenirs = Trim(rdr(0)) 'area souvenirs
                End If
            End If
        End If
        c.DisposeR()
        '--------------------------------------------------------------
        'updated by Arthur 6/17/2013
        Dim ls_Corp_Sendout As String = "select sum(Corp_Sendout) from CF_region_vismin where class_02 = 'Mindanao' and " & _
      " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_Corp_Sendout, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_corpsendout = 0.0
                Else
                    ld_corpsendout = Trim(rdr(0)) 'area ending balance
                End If
            End If
        End If
        c.DisposeR()
        '----------------------------------------------------------
        'updated by Arthur 6/17/2013
        Dim ls_Corp_Payout As String = "select sum(Corp_Payout) from CF_region_vismin where class_02 = 'Mindanao' and " & _
      " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_Corp_Payout, rdr, sqlmsg) Then
            If rdr.Read Then
                ld_corppayout = Trim(rdr(0)) 'area food products
            End If
        End If
        c.DisposeR()
        '--------------------------------------------------------------
        'updated by Arthur 6/17/2013
        Dim ls_Corp_Comm As String = "select sum(Corp_Comm) from CF_region_vismin where class_02 = 'Mindanao' and " & _
   " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_Corp_Comm, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_corpcomm = 0.0
                Else
                    ld_corpcomm = Trim(rdr(0)) 'area Insurance
                End If
            End If
        End If
        c.DisposeR()
        '----------------------------------------------------------
        'updated by Arthur 6/17/2013
        Dim ls_WesternUnionComm As String = "select sum(WesternUnionComm) from CF_region_vismin where class_02 = 'Mindanao' and " & _
   " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_WesternUnionComm, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_westernUnionComm = 0.0
                Else
                    ld_westernUnionComm = Trim(rdr(0)) 'area interest
                End If
            End If
        End If
        c.DisposeR()
        '--------------------------------------------------------------
        'updated by Arthur 6/17/2013
        Dim ls_WesternUnionPayout As String = "select sum(WesternUnionPayout) from CF_region_vismin where class_02 = 'Mindanao' and " & _
   " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_WesternUnionPayout, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_westernunionPayout = 0.0
                Else
                    ld_westernunionPayout = Trim(rdr(0)) 'area kp payout
                End If
            End If
        End If
        c.DisposeR()
        '----------------------------------------------------------
        '     Dim ls_WesternUnionSendout As String = "select sum(WesternUnionSendout) from CF_region_vismin where " & _
        '" transdate = '" + transdate + "'"
        'Dim ls_WesternUnionSendout As String = "SELECT SUM(bal) FROM (SELECT (SUM(CASE WHEN ((g.FinYear<2012) OR (g.FinYear=2012 AND g.TransType<>'P'))  AND a.bal_vw='B' THEN AmountDebitAC-AmountCreditAC ELSE 0 END)/1) bal FROM Balance g INNER JOIN grtbk ca ON g.CompanyCode=ca.CompanyCode AND g.CompanyAccountCode=ca.reknr INNER JOIN grtbk a ON a.CompanyCode IS NULL AND ca.AccountCode=a.reknr  INNER JOIN bedryf c ON c.bedrnr = g.CompanyCode  INNER JOIN bedryf b ON g.CompanyCode=b.bedrnr  AND b.Class_01='Visayas/Mindanao' WHERE a.reknr='  1020001' AND g.transtype IN ('N','C','I','P','F') AND ((g.FinYear<2012) OR (g.FinYear=2012 AND g.FinPeriod<  7))  AND a.bal_vw='B' group by b.valcode         UNION ALL SELECT (SUM(bdr_hfl)/1) bal FROM gbkmut g INNER JOIN grtbk ca ON g.CompanyCode=ca.CompanyCode AND g.reknr=ca.reknr INNER JOIN grtbk a ON a.CompanyCode IS NULL AND ca.AccountCode=a.reknr  INNER JOIN bedryf c ON c.bedrnr = g.CompanyCode  INNER JOIN bedryf b ON g.CompanyCode=b.bedrnr  AND b.Class_01='Visayas/Mindanao' WHERE a.reknr='  1020001' AND g.transtype IN ('N','C','I','P','F') AND g.verwerknrl<>0 AND  g.bkjrcode=2012 AND g.periode='  7' AND g.datum<{d '2012-07-02'}) x"
        Dim year As String = transdate.ToString("yyyy")
        Dim month As String = transdate.Month
        Dim char_month As String = Nothing
        If Len(month) = 1 Then
            char_month = "  " & month
        ElseIf Len(month) = 2 Then
            char_month = " " & month
        End If
        Dim ls_WesternUnionSendout As String = "exec spCashRRBeginning_CF_Ver3 " + year + "," + month + ",'" & transdate & "','" + char_month + "'"
        'Dim ls_WesternUnionSendout As String = "select dbo.spCashRRBeginning_CF_Ver3 ('" + year + "','" + month + "','" & transdate & "','" + char_month + "')"
        Dim c1 As New ClsMindanao
        Dim rdr1 As SqlClient.SqlDataReader = Nothing
        If c1.Error_Inititalize_INI Then Exit Sub
        If c1.ErrorConnectionReading(False) Then Exit Sub
        If Not c1.Error_SetRdr(ls_WesternUnionSendout, rdr, sqlmsg) Then
            If rdr.Read Then
                'ld_begbal = Trim(rdr(0).ToString) 'area beginning balance
                ls_amount = Trim(rdr(0).ToString) 'area beginning balance
                If ls_amount = "" Then
                    ld_westernunionsendout = 0.0
                Else
                    ld_westernunionsendout = CDbl(ls_amount)
                End If
            End If
        End If
        c1.DisposeR()
        'If c.ErrorConnectionReading(False) Then Exit Sub
        'If Not c.Error_SetRdr(ls_WesternUnionSendout, rdr, sqlmsg) Then
        '    If rdr.Read Then
        '        ld_westernunionsendout = Trim(rdr(0)) 'area kp sendout
        '    End If
        'End If
        'c.DisposeR()
        '--------------------------------------------------------------
        Dim ls_FundTransferDebit As String = "select sum(FundTransferDebit) from CF_region_vismin where class_02 = 'Mindanao' and " & _
   " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_FundTransferDebit, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_fundtransferdebit = 0.0
                Else
                    ld_fundtransferdebit = Trim(rdr(0)) 'area kp sendout comm
                End If
            End If
        End If
        c.DisposeR()
        '----------------------------------------------------------
        Dim ls_FundTransferCredit As String = "select sum(FundTransferCredit) from CF_region_vismin where class_02 = 'Mindanao' and " & _
      " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_FundTransferCredit, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_fundtransfercredit = 0.0
                Else
                    ld_fundtransfercredit = Trim(rdr(0)) 'area lukat
                End If
            End If
        End If
        c.DisposeR()
        '--------------------------------------------------------------
        Dim ls_BranchExpense As String = "select sum(BranchExpense) from CF_region_vismin where class_02 = 'Mindanao' and " & _
      " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_BranchExpense, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_branchexpense = 0.0
                Else
                    ld_branchexpense = Trim(rdr(0)) 'area otherincome
                End If
            End If
        End If
        c.DisposeR()
        '----------------------------------------------------------
        Dim ls_OtherExpense As String = "select sum(OtherExpense) from CF_region_vismin where class_02 = 'Mindanao' and " & _
       " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_OtherExpense, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_otherexpense = 0.0
                Else
                    ld_otherexpense = Trim(rdr(0)) 'area prenda
                End If
            End If
        End If
        c.DisposeR()
        '----------------------------------------------------------

        '/-added last 2/9/2011
        '----------------------------------------------------------
        Dim ls_nso As String = "select sum(NSO) from CF_region_vismin where class_02 = 'Mindanao' and " & _
       " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_nso, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_nso = 0.0
                Else
                    ld_nso = Trim(rdr(0)) 'area prenda
                End If
            End If
        End If
        c.DisposeR()
        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_mccr As String = "select sum(MCCashReceipts) from CF_region_vismin where class_02 = 'Mindanao' and " & _
       " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_mccr, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_MCCR = 0.0
                Else
                    ld_MCCR = Trim(rdr(0)) 'area prenda
                End If
            End If
        End If
        c.DisposeR()
        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_mccd As String = "select sum(MCCashDisbursements) from CF_region_vismin where class_02 = 'Mindanao' and " & _
       " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_mccd, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_MCCD = 0.0
                Else
                    ld_MCCD = Trim(rdr(0)) 'area prenda
                End If
            End If
        End If
        c.DisposeR()
        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_racr As String = "select sum(RACashReceipts) from CF_region_vismin where class_02 = 'Mindanao' and " & _
       " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_racr, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_RACR = 0.0
                Else
                    ld_RACR = Trim(rdr(0)) 'area prenda
                End If
            End If
        End If
        c.DisposeR()
        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_racd As String = "select sum(RACashDisbursements) from CF_region_vismin where class_02 = 'Mindanao' and " & _
       " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_racd, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_RACD = 0.0
                Else
                    ld_RACD = Trim(rdr(0)) 'area prenda
                End If
            End If
        End If
        c.DisposeR()
        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_dfb As String = "select sum(DepositFromBank) from CF_region_vismin where class_02 = 'Mindanao' and " & _
       " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_dfb, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_Depositfrombank = 0.0
                Else
                    ld_Depositfrombank = Trim(rdr(0)) 'area prenda
                End If
            End If
        End If
        c.DisposeR()
        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_wfb As String = "select sum(WithdrawalFromBank) from CF_region_vismin where class_02 = 'Mindanao' and " & _
       " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_wfb, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_withdrawalfrombank = 0.0
                Else
                    ld_withdrawalfrombank = Trim(rdr(0)) 'area prenda
                End If
            End If
        End If
        c.DisposeR()
        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_rts As String = "select sum(returntosender) from CF_region_vismin where class_02 = 'Mindanao' and " & _
       " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_rts, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_returntosender = 0.0
                Else
                    ld_returntosender = Trim(rdr(0)) 'area prenda
                End If
            End If
        End If
        c.DisposeR()
        '----------------------------------------------------------
        '\-added last 2/9/2011

        '/-added last 7/26/2011
        '----------------------------------------------------------
        Dim ls_cashover As String = "select isnull(sum(cashover),0) from CF_region_vismin where class_02 = 'Mindanao' and " & _
       " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_cashover, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_CashOver = 0.0
                Else
                    ld_CashOver = Trim(rdr(0)) 'area prenda
                End If
            End If
        End If
        c.DisposeR()
        '----------------------------------------------------------
        '----------------------------------------------------------
        Dim ls_cashshort As String = "select isnull(sum(cashshort),0) from CF_region_vismin where class_02 = 'Mindanao' and " & _
       " transdate = '" + transdate + "'"
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_cashshort, rdr, sqlmsg) Then
            If rdr.Read Then
                If IsDBNull(rdr(0)) Then '---Arthur
                    ld_CashShort = 0.0
                Else
                    ld_CashShort = Trim(rdr(0)) 'area prenda
                End If
            End If
        End If
        c.DisposeR()
        '----------------------------------------------------------
        '\-added last 7/26/2011


        ld_begbal_lv = ListViewRegionInsertion.Items.Add(ld_begbal).Text ' area beginning balance
        ld_endingbal_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_endingbal)).Text 'area ending balance
        ld_foodproducts_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_foodproducts)).Text 'area food products
        ld_insurance_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_insurance)).Text 'area outrightsales
        ld_outrightsales_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_outrightsales)).Text 'area outrightsales
        ld_layaway_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_layaway)).Text 'area layaway
        ld_salesreturn_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_salesreturn)).Text 'area salesreturn
        ld_layawaycancel_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_layawaycancel)).Text 'area layawaycancel
        ld_interest_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_interest)).Text 'area interest
        ld_kppayout_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_kppayout)).Text 'area kppayout
        ld_kpsendout_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_kpsendout)).Text 'area kpsendout
        ld_kpsendoutcomm_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_kpsendoutcomm)).Text 'area sendoutcomm
        ld_lukat_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_lukat)).Text 'area lukat
        ld_otherincome_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_otherincome)).Text 'area otheri1ncome
        ld_prenda_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_prenda)).Text 'area prenda
        ld_telecomms_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_telecomms)).Text 'area telecomms
        ld_souvenirs_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_souvenirs)).Text 'area souvenirs
        ld_corpsendout_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_corpsendout)).Text 'area corp sendout
        ld_corppayout_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_corppayout)).Text 'area corp payout
        ld_corpcomm_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_corpcomm)).Text 'area corp comm
        ld_westernUnionComm_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_westernUnionComm)).Text 'area wuc
        ld_westernunionPayout_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_westernunionPayout)).Text 'area wup
        ld_westernunionsendout_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_westernunionsendout)).Text 'area wus
        ld_fundtransferdebit_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_fundtransferdebit)).Text 'area fund transfer debit
        ld_fundtransfercredit_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_fundtransfercredit)).Text 'area fund transfer credit
        ld_branchexpense_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_branchexpense)).Text 'area branch expense
        ld_otherexpense_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_otherexpense)).Text 'area ohter expense



        '/added last 2/8/2011
        ld_nso_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_nso)).Text 'NSO
        ld_MCCR_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_MCCR)).Text 'Money Changer Cash Receipts
        ld_MCCD_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_MCCD)).Text 'Money Changer Cash Disbursements
        ld_RACR_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_RACR)).Text 'Renewal anywhere Cash Receipts
        ld_RACD_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_RACD)).Text 'Renewal Anywhere Cash Disbursements
        ld_Depositfrombank_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_Depositfrombank)).Text 'Deposit From Bank
        ld_withdrawalfrombank_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_withdrawalfrombank)).Text 'Withdrawal from Bank
        ld_returntosender_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_returntosender)).Text 'Return to sender
        '\added last 2/8/2011
        '/added last 7/26/2011
        ld_CashOver_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_CashOver)).Text 'Withdrawal from Bank
        ld_CashShort_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_CashShort)).Text 'Return to sender

        '\added last 7/26/2011




        vismin_transdate_exist()

        If gb_vismin_update = True Then
            Dim ls_update As String = "UPDATE [CF_VisMin_wide]" & _
           " SET [BeginningBalance]=" & ld_begbal_lv & ", [EndingBalance]=" & ld_endingbal_lv & ", [FoodProducts]=" & ld_foodproducts_lv & ",[Insurance]=" & ld_insurance_lv & ", [outrightsales]=" & ld_outrightsales_lv & ", [layaway]=" & ld_layaway_lv & ", [salesreturn]=" & ld_salesreturn_lv & ", [layawaycancel]=" & ld_layawaycancel_lv & ", [Interest]=" & ld_interest_lv & ",  " & _
           " [KP_Payout]= " & ld_kppayout_lv & ", [KP_Sendout]=" & ld_kpsendout_lv & ", [KP_Sendout_Comm]=" & ld_kpsendoutcomm_lv & " , [Lukat]= " & ld_lukat_lv & ", [OtherIncome]= " & ld_otherincome_lv & "  , [Prenda]=" & ld_prenda_lv & " , " & _
           " [Telecomms] = " & ld_telecomms_lv & ", [Souvenirs]=" & ld_souvenirs_lv & ", [Corp_Sendout]=" & ld_corpsendout_lv & ", [Corp_Payout]=" & ld_corppayout_lv & ",[Corp_Comm]=" & ld_corpcomm_lv & " ,[WesternUnionComm]=" & ld_westernUnionComm_lv & ", " & _
           " [WesternUnionPayout]=" & ld_westernunionPayout_lv & ", [WesternUnionSendout]=" & ld_westernunionsendout_lv & ", [FundTransferDebit]=" & ld_fundtransferdebit_lv & ", [FundTransferCredit]=" & ld_fundtransfercredit_lv & ", " & _
           " [BranchExpense]=" & ld_branchexpense_lv & ",[OtherExpense] = " & ld_otherexpense_lv & " ,[NSO] = " & ld_nso_lv & " ,[MCCashReceipts] = " & ld_MCCR_lv & ",[MCCashDisbursements] = " & ld_MCCD_lv & ",[RACashReceipts] = " & ld_RACR_lv & ",[RACashDisbursements] = " & ld_RACD_lv & ",[DepositFromBank] = " & ld_Depositfrombank_lv & " ,[WithdrawalFromBank] = " & ld_withdrawalfrombank_lv & " ,[ReturnToSender] = " & ld_returntosender & " ,[cashover] = " & ld_CashOver & " ,[cashshort] = " & ld_CashShort & "  ,[dategenerated] = '" + Now.Date + "' WHERE class_02 = 'Mindanao' and transdate = '" + transdate + "' "
            Log("Update Vismin Wide-- " & " " & transdate & " " & Now.TimeOfDay.ToString)
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_update, rdr, sqlmsg) Then
            End If
            c.DisposeR()

            '/ email functionality added 08-30-2012
            gs_transdate = transdate
            gs_transdatereport_title = Convert.ToDateTime(DtTransdate.Text).ToString("MM-dd-yyyy")
            gs_HO_Email_Info = "Head_Office_CashFlowReport_" & gs_transdatereport_title
            genwide()
            'Button3.PerformClick()
            '\ email functionality added 08-30-2012
        Else
            Dim ls_s As String = "INSERT INTO CF_vismin_wide ([Id],[BeginningBalance], [EndingBalance], [FoodProducts],   [Insurance],  [outrightsales],  [layaway],  [salesreturn],  [layawaycancel],  [Interest], [KP_Payout], [KP_Sendout], [KP_Sendout_Comm], [Lukat], [OtherIncome],[Prenda], [Telecomms], [Souvenirs], [Corp_Sendout], [Corp_Payout], [Corp_Comm], [WesternUnionComm],[WesternUnionPayout],[WesternUnionSendout],[FundTransferDebit], [FundTransferCredit], [BranchExpense], [OtherExpense],[NSO],[MCCashReceipts],[MCCashDisbursements],[RACashReceipts],[RACashDisbursements],[DepositFromBank],[WithdrawalFromBank],[returntosender],[cashover],[cashshort],[Transdate], [DateGenerated], [class_02])" & _
                 " VALUES(" & li_idcount & "," & ld_begbal_lv & "," & ld_endingbal_lv & "," & ld_foodproducts_lv & "," & ld_insurance_lv & "," & ld_outrightsales_lv & "," & ld_layaway_lv & "," & ld_salesreturn_lv & "," & ld_layawaycancel_lv & "," & ld_interest_lv & "," & ld_kppayout_lv & "," & ld_kpsendout_lv & ", " & ld_kpsendoutcomm_lv & "," & ld_lukat_lv & ", " & ld_otherincome_lv & ", " & ld_prenda_lv & ", " & _
                 " " & ld_telecomms_lv & ", " & ld_souvenirs_lv & ", " & ld_corpsendout_lv & "," & _
                 " " & ld_corppayout_lv & ", " & ld_corpcomm_lv & ", " & ld_westernUnionComm_lv & ", " & _
                 " " & ld_westernunionPayout_lv & "," & ld_westernunionsendout_lv & "," & ld_fundtransferdebit_lv & "," & ld_fundtransfercredit_lv & "," & ld_branchexpense_lv & ", " & _
                 " " & ld_otherexpense_lv & "," & ld_nso_lv & "," & ld_MCCR_lv & "," & ld_MCCD_lv & "," & ld_RACR_lv & "," & ld_RACD_lv & "," & ld_Depositfrombank_lv & "," & ld_withdrawalfrombank_lv & "," & ld_returntosender & ", " & ld_CashOver_lv & " , " & ld_CashShort_lv & "  ,'" + transdate + "','" + Now.Date + "', 'Mindanao') "
            Log("Insert Vismin Wide-- " & " " & transdate & " " & Now.TimeOfDay.ToString)
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_s, rdr, sqlmsg) Then
            End If
            c.DisposeR()

            '/ email functionality added 08-30-2012
            gs_transdate = transdate
            gs_transdatereport_title = Convert.ToDateTime(DtTransdate.Text).ToString("MM-dd-yyyy")
            gs_HO_Email_Info = "Head_Office_CashFlowReport_" & gs_transdatereport_title
            'Button3.PerformClick()
            genwide()
            '\ email functionality added 08-30-2012

            'Next
        End If


        Exit Sub

    End Sub
    Private Sub insert_to_CF_region_vismin()
        Dim li_i As Integer = Nothing
        Dim li_idcount As Integer = Nothing
        Dim li_listviewcount As Integer = Nothing
        li_listviewcount = ListViewRegion.Items.Count - 1
        For li_i = 0 To li_listviewcount
            Dim ls_class03 As String = Nothing
            ls_class03 = Trim(ListViewRegion.Items(li_i).Text.ToString)
            Dim class03 As String = Replace(ls_class03, ",", " ")
            Call id_count_region_vismin()
            li_idcount = gi_idcount_region
            li_idcount = li_idcount + 1
            Dim ld_begbal As Double = Nothing 'beginning balance
            Dim ld_endingbal As Double = Nothing 'ending balance
            Dim ld_foodproducts As Double = Nothing 'food products
            Dim ld_insurance As Double = Nothing 'insurance
            Dim ld_outrightsales As Double = Nothing 'outright sales-----------------jen 
            Dim ld_layaway As Double = Nothing 'layaway
            Dim ld_salesreturn As Double = Nothing 'sales return 
            Dim ld_layawaycancel As Double = Nothing 'layaway cancel
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

            '/-------------------added last 2/9/2011 due to judith findings
            Dim ld_nso As Double = Nothing 'NSO
            Dim ld_MCCR As Double = Nothing 'Money Changer Cash Receipts
            Dim ld_MCCD As Double = Nothing 'Money Changer Cash Disbursements
            Dim ld_RACR As Double = Nothing 'Renewal anywhere Cash Receipts
            Dim ld_RACD As Double = Nothing ' Renewal Anywhere Cash Disbursements
            Dim ld_Depositfrombank As Double = Nothing ' 'Deposit From Bank
            Dim ld_withdrawalfrombank As Double = Nothing ' Withdrawal from Bank
            Dim ld_returntosender As Double = Nothing ' Returntosender 2/10/2011
            '\-------------------added last 2/9/2011 due to judith findings

            '/-------------------added last 7/26/2011 due to judith findings
            Dim ld_cashover As Double = Nothing 'Cash OVer
            Dim ld_cashshort As Double = Nothing 'Cash Short
            '\-------------------added last 7/26/2011 due to judith findings


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


            '/-------------------added last 2/8/2011 due to judith findings
            Dim ld_nso_lv As Double = Nothing 'NSO
            Dim ld_MCCR_lv As Double = Nothing 'Money Changer Cash Receipts
            Dim ld_MCCD_lv As Double = Nothing 'Money Changer Cash Disbursements
            Dim ld_RACR_lv As Double = Nothing 'Renewal anywhere Cash Receipts
            Dim ld_RACD_lv As Double = Nothing ' Renewal Anywhere Cash Disbursements
            Dim ld_Depositfrombank_lv As Double = Nothing ' 'Deposit From Bank
            Dim ld_withdrawalfrombank_lv As Double = Nothing ' Withdrawal from Bank
            Dim ld_returntosender_lv As Double = Nothing ' Returntosender 2/10/2011
            '\-------------------added last 2/8/2011 due to judith findings


            '/-------------------added last 7/26/2011 due to judith findings
            Dim ld_cashover_lv As Double = Nothing 'Cash OVer 
            Dim ld_cashshort_lv As Double = Nothing 'Cash Short
            '\-------------------added last 7/26/2011 due to judith findings

            '/-------------------added last 7/26/2011 due to judith findings


            '\-------------------added last 7/26/2011 due to judith findings




            'code starts here-------------------------------------------------------------------
            'update by Arthur 6/17/2013
            Dim ls_begbal As String = "select isnull(sum(beginningbalance),0) from CF_area_vismin where class_02 = 'Mindanao' and " & _
            "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"

            Dim c As New clsData
            Dim rdr As SqlClient.SqlDataReader = Nothing
            If c.Error_Inititalize_INI Then Exit Sub
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_begbal, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_begbal = 0.0
                    Else
                        ld_begbal = CDbl(Trim(rdr(0))) 'area beginning balance
                    End If
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_endingbal As String = "select isnull (sum(EndingBalance),0) from CF_area_vismin where class_02 = 'Mindanao' and " & _
              "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_endingbal, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_endingbal = 0.0
                    Else
                        ld_endingbal = Trim(rdr(0)) 'area ending balance
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            Dim ls_foodproducts As String = "select sum(Foodproducts) from CF_area_vismin where class_02 = 'Mindanao' and " & _
     "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_foodproducts, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_foodproducts = 0.0
                    Else
                        ld_foodproducts = Trim(rdr(0)) 'area food products
                    End If
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_insurance As String = "select sum(Insurance) from CF_area_vismin where class_02 = 'Mindanao' and " & _
     "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_insurance, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_insurance = 0.0
                    Else
                        ld_insurance = Trim(rdr(0)) 'area Insurance
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------jen
            'Dim ls_outrightsales As String = "select sum(outrightsales) from CF_area_vismin where class_02 = 'Mindanao' and transdate =  '" & transdate & "','" & "" & "','" & "" & "','" & "" & "'"
            Dim ls_outrightsales As String = "select sum(outrightsales) from CF_area_vismin where class_02 = 'Mindanao' and transdate =  '" & transdate & "' and  class_03 = '" & ls_class03 & "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_outrightsales, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_outrightsales = 0.0
                    Else
                        ld_outrightsales = Trim(rdr(0)) 'area outrightsales
                    End If
                End If
            End If
            c.DisposeR()

            '----------------------------------------------------------
            'Dim ls_layaway As String = "select sum(layaway) from CF_area_vismin where class_02 = 'Mindanao' and transdate =  '" & transdate & "','" & "" & "','" & "" & "','" & "" & "'"
            Dim ls_layaway As String = "select sum(layaway) from CF_area_vismin where class_02 = 'Mindanao' and transdate =  '" & transdate & " 'and  class_03 = '" & ls_class03 & "'"

            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_layaway, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_layaway = 0.0
                    Else
                        ld_layaway = Trim(rdr(0)) 'area layaway
                    End If
                End If
            End If
            c.DisposeR()

            '----------------------------------------------------------
            'Dim ls_salesreturn As String = "select sum(salesreturn) from CF_area_vismin where class_02 = 'Mindanao' and transdate =  '" & transdate & "','" & "" & "','" & "" & "','" & "" & "'"
            Dim ls_salesreturn As String = "select sum(salesreturn) from CF_area_vismin where class_02 = 'Mindanao' and transdate =  '" & transdate & " 'and  class_03 = '" & ls_class03 & "'"

            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_salesreturn, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_salesreturn = 0.0
                    Else
                        ld_salesreturn = Trim(rdr(0)) 'area salesreturn
                    End If
                End If
            End If
            c.DisposeR()



            '----------------------------------------------------------------------------------------------------
            'Dim ls_layawaycancel As String = "select sum(layawaycancel) from CF_area_vismin where class_02 = 'Mindanao' and transdate =  '" & transdate & "','" & "" & "','" & "" & "','" & "" & "'"
            Dim ls_layawaycancel As String = "select sum(layawaycancel) from CF_area_vismin where class_02 = 'Mindanao' and transdate =  '" & transdate & " 'and  class_03 = '" & ls_class03 & "'"

            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_layawaycancel, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_layawaycancel = 0.0
                    Else
                        ld_layawaycancel = Trim(rdr(0)) 'area layawaycancel
                    End If
                End If
            End If
            c.DisposeR()

            '----------------------------------------------------------

            Dim ls_interest As String = "select sum(interest) from CF_area_vismin where class_02 = 'Mindanao' and " & _
     "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_interest, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_interest = 0.0
                    Else
                        ld_interest = Trim(rdr(0)) 'area interest
                    End If
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_kppayout As String = "select sum(kp_payout) from CF_area_vismin where class_02 = 'Mindanao' and " & _
     "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_kppayout, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_kppayout = 0.0
                    Else
                        ld_kppayout = Trim(rdr(0)) 'area kp payout
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            Dim ls_kpsendout As String = "select sum(kp_sendout) from CF_area_vismin where class_02 = 'Mindanao' and " & _
     "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_kpsendout, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_kpsendout = 0.0
                    Else
                        ld_kpsendout = Trim(rdr(0)) 'area kp sendout
                    End If
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_kpsendoutcomm As String = "select sum(KP_Sendout_Comm) from CF_area_vismin where class_02 = 'Mindanao' and " & _
     "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_kpsendoutcomm, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_kpsendoutcomm = 0.0
                    Else
                        ld_kpsendoutcomm = Trim(rdr(0)) 'area kp sendout comm
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            Dim ls_lukat As String = "select sum(Lukat) from CF_area_vismin where class_02 = 'Mindanao' and " & _
     "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_lukat, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_lukat = 0.0
                    Else
                        ld_lukat = Trim(rdr(0)) 'area lukat
                    End If
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_otherincome As String = "select sum(OtherIncome) from CF_area_vismin where class_02 = 'Mindanao' and " & _
     "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_otherincome, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_otherincome = 0.0
                    Else
                        ld_otherincome = Trim(rdr(0)) 'area otherincome
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            Dim ls_Prenda As String = "select sum(prenda) from CF_area_vismin where class_02 = 'Mindanao' and " & _
      "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_Prenda, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_prenda = 0.0
                    Else
                        ld_prenda = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_telecomms As String = "select sum(telecomms) from CF_area_vismin where class_02 = 'Mindanao' and " & _
         "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_telecomms, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_telecomms = 0.0
                    Else
                        ld_telecomms = Trim(rdr(0)) 'area telecomms
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            Dim ls_souvenirs As String = "select sum(Souvenirs) from CF_area_vismin where class_02 = 'Mindanao' and " & _
        "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_souvenirs, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_souvenirs = 0.0
                    Else
                        ld_souvenirs = Trim(rdr(0)) 'area souvenirs
                    End If
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_Corp_Sendout As String = "select sum(Corp_Sendout) from CF_area_vismin where class_02 = 'Mindanao' and " & _
           "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_Corp_Sendout, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_corpsendout = 0.0
                    Else
                        ld_corpsendout = Trim(rdr(0)) 'area ending balance
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            Dim ls_Corp_Payout As String = "select sum(Corp_Payout) from CF_area_vismin where class_02 = 'Mindanao' and " & _
     "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_Corp_Payout, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_corppayout = 0.0
                    Else
                        ld_corppayout = Trim(rdr(0)) 'area food products
                    End If
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_Corp_Comm As String = "select sum(Corp_Comm) from CF_area_vismin where class_02 = 'Mindanao' and " & _
      "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_Corp_Comm, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_corpcomm = 0.0
                    Else
                        ld_corpcomm = Trim(rdr(0)) 'area Insurance
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            Dim ls_WesternUnionComm As String = "select sum(WesternUnionComm) from CF_area_vismin where class_02 = 'Mindanao' and " & _
     "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_WesternUnionComm, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_westernUnionComm = 0.0
                    Else
                        ld_westernUnionComm = Trim(rdr(0)) 'area interest
                    End If
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_WesternUnionPayout As String = "select sum(WesternUnionPayout) from CF_area_vismin where class_02 = 'Mindanao' and " & _
     "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_WesternUnionPayout, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_westernunionPayout = 0.0
                    Else
                        ld_westernunionPayout = Trim(rdr(0)) 'area kp payout
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            Dim ls_WesternUnionSendout As String = "select sum(WesternUnionSendout) from CF_area_vismin where class_02 = 'Mindanao' and " & _
     "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_WesternUnionSendout, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_westernunionsendout = 0.0
                    Else
                        ld_westernunionsendout = Trim(rdr(0)) 'area kp sendout
                    End If
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_FundTransferDebit As String = "select sum(FundTransferDebit) from CF_area_vismin where class_02 = 'Mindanao' and " & _
            "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_FundTransferDebit, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_fundtransferdebit = 0.0
                    Else
                        ld_fundtransferdebit = Trim(rdr(0)) 'area kp sendout comm
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            Dim ls_FundTransferCredit As String = "select sum(FundTransferCredit) from CF_area_vismin where class_02 = 'Mindanao' and " & _
           "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_FundTransferCredit, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_fundtransfercredit = 0.0
                    Else
                        ld_fundtransfercredit = Trim(rdr(0)) 'area lukat
                    End If
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_BranchExpense As String = "select sum(BranchExpense) from CF_area_vismin where class_02 = 'Mindanao' and " & _
     "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_BranchExpense, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_branchexpense = 0.0
                    Else
                        ld_branchexpense = Trim(rdr(0)) 'area otherincome
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            Dim ls_OtherExpense As String = "select sum(OtherExpense) from CF_area_vismin where class_02 = 'Mindanao' and " & _
        "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_OtherExpense, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_otherexpense = 0.0
                    Else
                        ld_otherexpense = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '/-added last 2/9/2011
            '----------------------------------------------------------
            Dim ls_NSO As String = "select sum(NSO) from CF_area_vismin where class_02 = 'Mindanao' and " & _
        "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_NSO, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_nso = 0.0
                    Else
                        ld_nso = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '----------------------------------------------------------
            Dim ls_MCCR As String = "select sum(MCCashReceipts) from CF_area_vismin where class_02 = 'Mindanao' and " & _
        "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_MCCR, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_MCCR = 0.0
                    Else
                        ld_MCCR = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '----------------------------------------------------------
            Dim ls_MCCD As String = "select sum(MCCashDisbursements) from CF_area_vismin where class_02 = 'Mindanao' and " & _
        "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_MCCD, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_MCCD = 0.0
                    Else
                        ld_MCCD = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '----------------------------------------------------------
            Dim ls_RACR As String = "select sum(RACashReceipts) from CF_area_vismin where class_02 = 'Mindanao' and " & _
        "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_RACR, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_RACR = 0.0
                    Else
                        ld_RACR = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '----------------------------------------------------------
            Dim ls_RACD As String = "select sum(RACashDisbursements) from CF_area_vismin where class_02 = 'Mindanao' and " & _
        "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_RACD, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_RACD = 0.0
                    Else
                        ld_RACD = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '----------------------------------------------------------
            Dim ls_DFB As String = "select sum(DepositFromBank) from CF_area_vismin where class_02 = 'Mindanao' and " & _
        "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_DFB, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_Depositfrombank = 0.0
                    Else
                        ld_Depositfrombank = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '----------------------------------------------------------
            Dim ls_WFB As String = "select sum(WithdrawalFromBank) from CF_area_vismin where class_02 = 'Mindanao' and " & _
        "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_WFB, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_withdrawalfrombank = 0.0
                    Else
                        ld_withdrawalfrombank = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '----------------------------------------------------------
            Dim ls_rts As String = "select sum(returntosender) from CF_area_vismin where class_02 = 'Mindanao' and " & _
        "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_rts, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_returntosender = 0.0
                    Else
                        ld_returntosender = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '\-added last 2/9/2011
            '/-added last 7/26/2011
            Dim ls_cashover As String = "select sum(cashover) from CF_area_vismin where class_02 = 'Mindanao' and " & _
   "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_cashover, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_cashover = 0.0
                    Else
                        ld_cashover = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '-----------------------------------------------------------
            Dim ls_cashshort As String = "select sum(cashshort) from CF_area_vismin where class_02 = 'Mindanao' and " & _
   "class_03 = '" + ls_class03 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_cashshort, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_cashshort = 0.0
                    Else
                        ld_cashshort = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '\-added last 7/26/2011




            ld_begbal_lv = ListViewRegionInsertion.Items.Add(ld_begbal).Text ' area beginning balance
            ld_endingbal_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_endingbal)).Text 'area ending balance
            ld_foodproducts_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_foodproducts)).Text 'area food products
            ld_insurance_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_insurance)).Text 'area insurance
            ld_outrightsales_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_outrightsales)).Text 'area outrightsales
            ld_layaway_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_layaway)).Text 'area layaway
            ld_salesreturn_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_salesreturn)).Text 'area salesreturn
            ld_layawaycancel_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_layawaycancel)).Text 'area layawaycancel
            ld_interest_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_interest)).Text 'area interest
            ld_kppayout_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_kppayout)).Text 'area kppayout
            ld_kpsendout_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_kpsendout)).Text 'area kpsendout
            ld_kpsendoutcomm_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_kpsendoutcomm)).Text 'area sendoutcomm
            ld_lukat_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_lukat)).Text 'area lukat
            ld_otherincome_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_otherincome)).Text 'area otheri1ncome
            ld_prenda_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_prenda)).Text 'area prenda
            ld_telecomms_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_telecomms)).Text 'area telecomms
            ld_souvenirs_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_souvenirs)).Text 'area souvenirs
            ld_corpsendout_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_corpsendout)).Text 'area corp sendout
            ld_corppayout_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_corppayout)).Text 'area corp payout
            ld_corpcomm_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_corpcomm)).Text 'area corp comm
            ld_westernUnionComm_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_westernUnionComm)).Text 'area wuc
            ld_westernunionPayout_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_westernunionPayout)).Text 'area wup
            ld_westernunionsendout_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_westernunionsendout)).Text 'area wus
            ld_fundtransferdebit_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_fundtransferdebit)).Text 'area fund transfer debit
            ld_fundtransfercredit_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_fundtransfercredit)).Text 'area fund transfer credit
            ld_branchexpense_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_branchexpense)).Text 'area branch expense
            ld_otherexpense_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_otherexpense)).Text 'area ohter expense

            '/added last 2/9/2011
            ld_nso_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_nso)).Text 'NSO
            ld_MCCR_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_MCCR)).Text 'Money Changer Cash Receipts
            ld_MCCD_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_MCCD)).Text 'Money Changer Cash Disbursements
            ld_RACR_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_RACR)).Text 'Renewal anywhere Cash Receipts
            ld_RACD_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_RACD)).Text 'Renewal Anywhere Cash Disbursements
            ld_Depositfrombank_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_Depositfrombank)).Text 'Deposit From Bank
            ld_withdrawalfrombank_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_withdrawalfrombank)).Text 'Withdrawal from Bank
            ld_returntosender_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_returntosender)).Text 'Return To sender
            '\added last 2/9/2011

            '/added last 7/26/2011
            ld_cashover_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_cashover)).Text 'Withdrawal from Bank
            ld_cashshort_lv = ListViewRegionInsertion.Items(ListViewRegionInsertion.Items.Count - 1).SubItems.Add(Trim(ld_cashshort)).Text 'Return To sender
            '\added last 7/26/2011


            ''diri naku


            region_transdate_exist()

            If gb_region_update = True Then
                Dim ls_update As String = "UPDATE [CF_region_VisMin]" & _
                " SET [BeginningBalance]=" & ld_begbal_lv & ", [EndingBalance]=" & ld_endingbal_lv & ", [FoodProducts]=" & ld_foodproducts_lv & ",[Insurance]=" & ld_insurance_lv & ",[outrightsales]=" & ld_outrightsales_lv & ",[layaway]=" & ld_layaway_lv & ",[salesreturn]=" & ld_salesreturn_lv & ",[layawaycancel]=" & ld_layawaycancel_lv & ", [Interest]=" & ld_interest_lv & ",  " & _
                " [KP_Payout]= " & ld_kppayout_lv & ", [KP_Sendout]=" & ld_kpsendout_lv & ", [KP_Sendout_Comm]=" & ld_kpsendoutcomm_lv & " , [Lukat]= " & ld_lukat_lv & ", [OtherIncome]= " & ld_otherincome_lv & "  , [Prenda]=" & ld_prenda_lv & " , " & _
                " [Telecomms] = " & ld_telecomms_lv & ", [Souvenirs]=" & ld_souvenirs_lv & ", [Corp_Sendout]=" & ld_corpsendout_lv & ", [Corp_Payout]=" & ld_corppayout_lv & ",[Corp_Comm]=" & ld_corpcomm_lv & " ,[WesternUnionComm]=" & ld_westernUnionComm_lv & ", " & _
                " [WesternUnionPayout]=" & ld_westernunionPayout_lv & ", [WesternUnionSendout]=" & ld_westernunionsendout_lv & ", [FundTransferDebit]=" & ld_fundtransferdebit_lv & ", [FundTransferCredit]=" & ld_fundtransfercredit_lv & ", " & _
                " [BranchExpense]=" & ld_branchexpense_lv & ",[OtherExpense] = " & ld_otherexpense_lv & " ,[NSO] = " & ld_nso_lv & " ,[MCCashReceipts] = " & ld_MCCR_lv & ",[MCCashDisbursements] = " & ld_MCCD_lv & ",[RACashReceipts] = " & ld_RACR_lv & ",[RACashDisbursements] = " & ld_RACD_lv & ",[DepositFromBank] = " & ld_Depositfrombank_lv & " ,[WithdrawalFromBank] = " & ld_withdrawalfrombank_lv & ",[returntosender] = " & ld_returntosender_lv & "  ,[cashover] = " & ld_cashover_lv & "  ,[cashshort] = " & ld_cashshort_lv & "  ,[dategenerated] = '" + Now.Date + "' WHERE class_02 = 'Mindanao' and transdate = '" + transdate + "' and class_03 = '" + class03 + "'"
                Log("Update region--" & class03 & " " & transdate & " " & Now.TimeOfDay.ToString)
                If c.ErrorConnectionReading(False) Then Exit Sub
                If Not c.Error_SetRdr(ls_update, rdr, sqlmsg) Then
                End If
                c.DisposeR()



            Else
                Dim ls_s As String = "INSERT INTO CF_region_vismin ([Id],[Class_03], [BeginningBalance], [EndingBalance], [FoodProducts],   [Insurance] ,  [outrightsales],  [layaway],  [salesreturn],  [layawaycancel], [Interest], [KP_Payout], [KP_Sendout], [KP_Sendout_Comm], [Lukat], [OtherIncome],[Prenda], [Telecomms], [Souvenirs], [Corp_Sendout], [Corp_Payout], [Corp_Comm], [WesternUnionComm],[WesternUnionPayout],[WesternUnionSendout],[FundTransferDebit], [FundTransferCredit], [BranchExpense], [OtherExpense],[NSO],[MCCashReceipts],[MCCashDisbursements],[RACashReceipts],[RACashDisbursements],[DepositFromBank],[WithdrawalFromBank],[returntosender],[cashover],[cashshort],[Transdate], [DateGenerated], [class_02])" & _
                     " VALUES(" & li_idcount & ",'" + class03 + "'," & _
                     " " & ld_begbal_lv & "," & ld_endingbal_lv & "," & ld_foodproducts_lv & "," & ld_insurance_lv & "," & ld_outrightsales_lv & "," & ld_layaway_lv & "," & ld_salesreturn_lv & "," & ld_layawaycancel_lv & "," & ld_interest_lv & "," & ld_kppayout_lv & "," & ld_kpsendout_lv & ", " & ld_kpsendoutcomm_lv & "," & ld_lukat_lv & ", " & ld_otherincome_lv & ", " & ld_prenda_lv & ", " & _
                     " " & ld_telecomms_lv & ", " & ld_souvenirs_lv & ", " & ld_corpsendout_lv & "," & _
                     " " & ld_corppayout_lv & ", " & ld_corpcomm_lv & ", " & ld_westernUnionComm_lv & ", " & _
                     " " & ld_westernunionPayout_lv & "," & ld_westernunionsendout_lv & "," & ld_fundtransferdebit_lv & "," & ld_fundtransfercredit_lv & "," & ld_branchexpense_lv & ", " & _
                     " " & ld_otherexpense_lv & " ," & ld_nso_lv & "," & ld_MCCR_lv & "," & ld_MCCD_lv & "," & ld_RACR_lv & "," & ld_RACD_lv & "," & ld_Depositfrombank_lv & "," & ld_withdrawalfrombank_lv & "," & ld_returntosender_lv & "," & ld_cashover_lv & ", " & ld_cashshort_lv & "  ,'" + transdate + "','" + Date.Now + "', 'Mindanao')"
                'Dim c25 As New clsData"
                'Dim rdr25 As SqlClient.SqlDataReader = Nothing
                'If c25.Error_Inititalize_INI Then Exit Sub
                Log("Insert region--" & class03 & " " & transdate & " " & Now.TimeOfDay.ToString)
                If c.ErrorConnectionReading(False) Then Exit Sub
                If Not c.Error_SetRdr(ls_s, rdr, sqlmsg) Then
                End If
                c.DisposeR()
            End If
        Next
        Exit Sub
    End Sub
    Private Sub insertion_to_cf_pb_vismin()
        Dim li_i As Integer = Nothing
        Dim li_idcount As Integer = Nothing
        Dim li_listviewcount As Integer = Nothing

        li_listviewcount = ListViewPb.Items.Count - 1


        For li_i = 0 To li_listviewcount
            Dim bcode As String = Trim(ListViewPb.Items(li_i).Text)
            'dim bcode As String = "005"
            Dim branchname As String = Trim(ListViewPb.Items(li_i).SubItems.Item(1).Text)
            Dim ls_class03 As String = Trim(ListViewPb.Items(li_i).SubItems.Item(2).Text)
            Dim class03 As String = Replace(ls_class03, ",", " ")
            Dim ls_class04 As String = ListViewPb.Items(li_i).SubItems.Item(3).Text

            Call id_count_pb_vsimin()
            li_idcount = gi_idcount_pb
            li_idcount = li_idcount + 1

            gs_bcode = bcode
            gs_class04 = ls_class04 'area class
            gs_class03 = class03

            Dim ld_begbal As Double = Nothing 'beginning balance
            Dim ld_endingbal As Double = Nothing 'ending balance
            Dim ld_foodproducts As Double = Nothing 'food products
            Dim ld_insurance As Double = Nothing 'insurance
            Dim ld_interest As Double = Nothing 'interest
            Dim ld_outrightsales As Double = Nothing 'outright sales--------------------------------jen
            Dim ld_layaway As Double = Nothing 'layaway
            Dim ld_salesreturn As Double = Nothing 'sales return 
            Dim ld_layawaycancel As Double = Nothing 'layaway cancel
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
            Dim ld_westernunionsendout As Double = Nothing 'running Receivable added 9-3-2012
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
            '\-------------------added last 7/26/2011 due to judith findings


            '\------additional amounts
            Dim ld_totalreceipts As Double = Nothing
            Dim ld_totaldisbursements As Double = Nothing

            Dim ld_NP_lukat As Double = Nothing 'notpure lukat
            Dim ld_rematado As Double = Nothing
            '--------------------------/

            Dim ld_withdrawlFromBankfundtransfercredit As Double = Nothing  '8-16-2011
            Dim ld_depositFromBankfundtransferdebit As Double = Nothing '8-16-2011
            Dim ld_OtherexpenseNotRMbase As Double = Nothing 'added 8/25/2011'


            Dim ld_begbal_lv As Double = Nothing 'beginning balance
            Dim ld_endingbal_lv As Double = Nothing 'ending balance
            Dim ld_foodproducts_lv As Double = Nothing 'food products
            Dim ld_insurance_lv As Double = Nothing 'insurance
            Dim ld_outrightsales_lv As Double = Nothing 'outright sales-----------------------------jen
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
            Dim ld_westernUnionComm_lv As Double = Nothing 'travel and tours
            Dim ld_westernunionPayout_lv As Double = Nothing ' health care
            Dim ld_westernunionsendout_lv As Double = Nothing 'western union sendout
            Dim ld_fundtransferdebit_lv As Double = Nothing 'fundtransferdebit
            Dim ld_fundtransfercredit_lv As Double = Nothing 'fund transfer Credit
            Dim ld_branchexpense_lv As Double = Nothing 'branch expense
            Dim ld_otherexpense_lv As Double = Nothing 'otherexpense
            '/-------------------added last 2/8/2011 due to judith findings
            Dim ld_nso_lv As Double = Nothing 'NSO
            Dim ld_MCCR_lv As Double = Nothing 'Money Changer Cash Receipts
            Dim ld_MCCD_lv As Double = Nothing 'Money Changer Cash Disbursements
            Dim ld_RACR_lv As Double = Nothing 'Renewal anywhere Cash Receipts
            Dim ld_RACD_lv As Double = Nothing ' Renewal Anywhere Cash Disbursements
            Dim ld_Depositfrombank_lv As Double = Nothing ' 'Deposit From Bank
            Dim ld_withdrawalfrombank_lv As Double = Nothing ' Withdrawal from Bank
            Dim ld_returntosender_lv As Double = Nothing ' Returntosender 2/10/2011
            '\-------------------added last 2/8/2011 due to judith findings

            '/-------------------added last 7/26/2011 due to judith findings
            Dim ld_CashShort_lv As Double = Nothing 'Cash Short
            Dim ld_CashOver_lv As Double = Nothing 'Cash Over
            '\-------------------added last 7/26/2011 due to judith findings

            'code starts here-------------------------------------------------------------------
            Dim li_month As Integer = CInt(transmonth)
            Dim li_year As Integer = CInt(transyear)
            Dim ls_month As String = CStr(li_month)
            Dim ld_1000006 As Double = Nothing
            Dim ld_1000004 As Double = Nothing
            If Len(ls_month) = 1 Then
                ls_month = "  " & CStr(li_month)
            Else
                ls_month = " " & CStr(li_month)
            End If



            Dim ls_begbal As String = "select dbo.EXEC_SF_1000006CASHBALANCEPESO_CF_V3 ('" + transdate + "','" + bcode + "')"
            Dim c As New ClsMindanao
            Dim rdr As SqlClient.SqlDataReader = Nothing
            If c.Error_Inititalize_INI Then Exit Sub
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_begbal, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_begbal = Trim(rdr(0).ToString) 'area beginning balance
                    ls_amount = Trim(rdr(0).ToString) 'area beginning balance
                    If ls_amount = "" Then
                        ld_1000006 = 0
                    Else
                        ld_1000006 = CDbl(ls_amount)
                    End If
                End If
            End If
            c.DisposeR()



            Dim ls_begbal2 As String = "select dbo.EXEC_SF_1000004CASHBALANCEPESO_CF_V3 ('" + transdate + "','" + bcode + "')"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_begbal2, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_begbal = Trim(rdr(0).ToString) 'area beginning balance
                    ls_amount = Trim(rdr(0).ToString) 'area beginning balance
                    If ls_amount = "" Then
                        ld_1000004 = 0
                    Else
                        ld_1000004 = CDbl(ls_amount)
                    End If
                End If
            End If
            c.DisposeR()
            ld_begbal = ld_1000006 + ld_1000004 ' beginning balance

            '--------------------------------------------------------------
            Dim ls_foodproducts As String = "select -1 * sum(totalfoodproducts) as totalfoodproducts from vwCashFlowFoodProducts_CF_ver3 " & _
            "where transdate = '" + transdate + "' and branchcode = '" + bcode + "' group by branchcode"

            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_foodproducts, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_foodproducts = Trim(rdr(0)) 'area food products
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_foodproducts = 0
                    Else
                        ld_foodproducts = CDbl(ls_amount)
                    End If

                Else
                    ld_foodproducts = 0
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_insurance As String = "select -1 * sum(InsuranceAmt) from " & _
            " vwCashFlowInsurance_CF_ver3 where branchcode = '" + bcode + "' and transdate = '" + transdate + "'"

            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_insurance, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_insurance = Trim(rdr(0).ToString) 'area Insurance
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_insurance = 0
                    Else
                        ld_insurance = CDbl(ls_amount)
                    End If
                Else
                    ld_insurance = 0
                End If
            End If
            c.DisposeR()
            '   '--------------------------------------------------------------------------jen

            Dim ls_outrightsales As String = "select dbo.SF_OUTRIGHTSALES_SHOWROOM ( '" & transdate & "', '" & "" & "','" & bcode & "','" & "" & "')"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_outrightsales, rdr, sqlmsg) Then
                If rdr.Read Then

                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_outrightsales = 0
                    Else
                        'ld_outrightsales = CDbl(ls_amount)
                        If ls_amount.Contains("-") Then
                            ld_outrightsales = CDbl(ls_amount) * -1
                        Else
                            ld_outrightsales = CDbl(ls_amount)
                        End If



                    End If
                Else
                    ld_outrightsales = 0
                End If
            End If
            c.DisposeR()

            '-------------------------------------------------------------------------------------------------------------------------------------------
            Dim ls_layaway As String = "select dbo.SF_LAYAWAY_SHOWROOM ( '" & transdate & "', '" & "" & "','" & bcode & "','" & "" & "')"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_layaway, rdr, sqlmsg) Then
                If rdr.Read Then

                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_layaway = 0
                    Else
                        'ld_layaway = CDbl(ls_amount)
                        If ls_amount.Contains("-") Then
                            ld_layaway = CDbl(ls_amount) * -1
                        Else
                            ld_layaway = CDbl(ls_amount)
                        End If
                    End If
                Else
                    ld_layaway = 0
                End If
            End If
            c.DisposeR()
            '-------------------------------------------------------------------------------------------------------------------------------------------------------------------    

            Dim ls_salesreturn As String = "select dbo.SF_SALESRETURN_SHOWROOM ( '" & transdate & "', '" & "" & "','" & bcode & "','" & "" & "')"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_salesreturn, rdr, sqlmsg) Then
                If rdr.Read Then

                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_salesreturn = 0
                    Else
                        'ld_salesreturn = CDbl(ls_amount)
                        If ls_amount.Contains("-") Then
                            ld_salesreturn = CDbl(ls_amount) * -1
                        Else
                            ld_salesreturn = CDbl(ls_amount)
                        End If
                    End If
                Else
                    ld_salesreturn = 0
                End If
            End If
            c.DisposeR()

            '---------------------------------------------------------------------------------------------------------------------------------------------------     
            Dim ls_layawaycancel As String = "select dbo.SF_LAYAWAYCANCEL_SHOWROOM( '" & transdate & "', '" & "" & "','" & bcode & "','" & "" & "')"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_layawaycancel, rdr, sqlmsg) Then
                If rdr.Read Then

                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_layawaycancel = 0
                    Else
                        'ld_layawaycancel = CDbl(ls_amount)
                        If ls_amount.Contains("-") Then
                            ld_layawaycancel = CDbl(ls_amount) * -1
                        Else
                            ld_layawaycancel = CDbl(ls_amount)
                        End If
                    End If
                Else
                    ld_layawaycancel = 0
                End If
            End If
            c.DisposeR()



            '   '----------------------------------------------------------
            Dim ls_interest As String = "select -1 *  sum(interestamt) from" & _
            " vwcashflowinterest_CF_ver3 where branchcode = '" + bcode + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_interest, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_interest = Trim(rdr(0)) 'area interest
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_interest = 0
                    Else
                        ld_interest = CDbl(ls_amount)
                    End If
                Else
                    ld_interest = 0

                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_kppayout As String = "select sum(kppayoutamt) from " & _
            " vwCashFlowKPPayout_CF_ver3 where branchcode = '" + bcode + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_kppayout, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_kppayout = Trim(rdr(0)) 'area kp payout
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_kppayout = 0
                    Else
                        ld_kppayout = CDbl(ls_amount)
                    End If
                Else
                    ld_kppayout = 0
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            Dim ls_kpsendout As String = "select -1 * sum(kpsendoutamt) " & _
            " from vwCashFlowKPSendout_CF_ver3 where branchcode = '" + bcode + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_kpsendout, rdr, sqlmsg) Then
                If rdr.Read Then
                    ' ld_kpsendout = Trim(rdr(0).ToString) 'area kp sendout
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_kpsendout = 0
                    Else
                        ld_kpsendout = CDbl(ls_amount)
                    End If
                Else
                    ld_kpsendout = 0
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_kpsendoutcomm As String = "select -1 * sum(KPCOMMISSIONAMT) " & _
            " from vwCashFlowKPCommission_CF_ver3 where branchcode ='" + bcode + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_kpsendoutcomm, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_kpsendoutcomm = Trim(rdr(0)) 'area kp sendout comm
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_kpsendoutcomm = 0
                    Else
                        ld_kpsendoutcomm = CDbl(ls_amount)
                    End If
                Else
                    ld_kpsendoutcomm = 0
                End If
            End If
            c.DisposeR()
            '   '----------------------------------------------------------
            Dim ls_lukat As String = "select -1 * sum(lukatamt) " & _
            " from vwCashFlowLukat_CF_Ver3 where branchcode = '" + bcode + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_lukat, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_kpsendoutcomm = Trim(rdr(0)) 'area kp sendout comm
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_NP_lukat = 0
                    Else
                        ld_NP_lukat = CDbl(ls_amount)
                    End If
                Else
                    ld_NP_lukat = 0
                End If
            End If
            c.DisposeR()

            Dim ls_rematado As String = "select sum(totrematado) from" & _
            " vwCashFlowRematado_CF_ver3 where branchcode = '" + bcode + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_rematado, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_kpsendoutcomm = Trim(rdr(0)) 'area kp sendout comm
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_rematado = 0
                    Else
                        ld_rematado = CDbl(ls_amount)
                    End If
                Else
                    ld_rematado = 0
                End If
            End If
            c.DisposeR()
            '   '--------------------------------------------------------------
            Dim ls_otherincome As String = "select -1 * sum(OtherIncomeAmt) " & _
"from vwCashFlowOtherIncome_CF_V3 where branchcode = '" + bcode + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_otherincome, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_otherincome = 0
                    Else
                        ld_otherincome = CDbl(ls_amount)
                    End If
                Else
                    ld_otherincome = 0
                End If
            End If
            c.DisposeR()
            '   '----------------------------------------------------------
            Dim ls_Prenda As String = "select sum(PrendaAmt) from " & _
            " vwCashFlowPrenda_CF_ver3 where branchcode = '" + bcode + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_Prenda, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_prenda = 0
                    Else
                        ld_prenda = CDbl(ls_amount)
                    End If
                Else
                    ld_prenda = 0
                End If
            End If
            c.DisposeR()
            '   '--------------------------------------------------------------
            Dim ls_telecomms As String = "select -1 * sum (totaltelecomms) " & _
            " from vwCashFlowTelecomms_CF_ver3 where branchcode = '" + bcode + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_telecomms, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_telecomms = 0
                    Else
                        ld_telecomms = CDbl(ls_amount)
                    End If
                Else
                    ld_telecomms = 0
                End If
            End If
            c.DisposeR()
            '   '----------------------------------------------------------
            Dim ls_souvenirs As String = "select -1 * sum(totalSouvenirs) from " & _
            " vwCashFlowSouvenirs_CF_ver3 where branchcode = '" + bcode + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_souvenirs, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_souvenirs = 0
                    Else
                        ld_souvenirs = CDbl(ls_amount)
                    End If
                Else
                    ld_souvenirs = 0
                End If
            End If
            c.DisposeR()
            '   '--------------------------------------------------------------
            Dim ls_Corp_Sendout As String = "SELECT -1 * sum(amountcentral) " & _
 " FROM vwCashFlowCorpPartnersSendout_CF_VER3 WHERE branchcode = '" + bcode + "' and TRANSDATE = '" + transdate + "' "
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_Corp_Sendout, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_corpsendout = 0
                    Else
                        ld_corpsendout = CDbl(ls_amount)
                    End If
                Else
                    ld_corpsendout = 0
                End If
            End If
            c.DisposeR()
            '   '----------------------------------------------------------
            Dim ls_Corp_Payout As String = "SELECT sum(amountcentral) FROM " & _
            " vwCashFlowCorpPartnersPayout_CF_Ver3 WHERE branchcode = '" + bcode + "' and TRANSDATE = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_Corp_Payout, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_corppayout = 0
                    Else
                        ld_corppayout = CDbl(ls_amount)
                    End If
                Else
                    ld_corppayout = 0
                End If
            End If
            c.DisposeR()
            '   '--------------------------------------------------------------
            Dim ls_Corp_Comm As String = "select -1 * sum(amountcentral) from " & _
            " vwCashFlowCorpPartnersCommision_CF_Ver3 where branchcode = '" + bcode + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_Corp_Comm, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_corpcomm = 0
                    Else
                        ld_corpcomm = CDbl(ls_amount)
                    End If
                Else
                    ld_corpcomm = 0
                End If
            End If
            c.DisposeR()
            ''------Travel and Tours
            '/added last 9-3-2012
            Dim ls_WesternUnionComm As String = "select isnull(-1 * sum(TravelAndToursAmt),0) from vwCashFlowTravelandTours_CF_ver3 where branchcode = '" + bcode + "' and transdate = '" + transdate + "'"
            'Dim ls_WesternUnionComm As String = "select -1 * sum(TravelAndToursAmt) from vwCashFlowTravelandTours_CF_ver3 where branchcode = '109' and transdate = '2012-07-02'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_WesternUnionComm, rdr, sqlmsg) Then
                If rdr.Read Then
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_westernUnionComm = 0
                    Else
                        ld_westernUnionComm = CDbl(ls_amount)
                    End If
                Else
                    ld_westernUnionComm = 0
                End If
            End If
            c.DisposeR()
            '\added last 9-3-2012
            ''------Travel and Tours
            '/added last 9-3-2012
            ''------Health Care
            Dim ls_WesternUnionPayout As String = "select isnull(-1 * sum(HealthCareAmt),0) from vwCashFlowHealthCare_CF_ver3 where branchcode = '" + bcode + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_WesternUnionPayout, rdr, sqlmsg) Then
                If rdr.Read Then
                    ls_amount = Trim(rdr(0)) 'area kp payout
                    If ls_amount = "" Then
                        ld_westernunionPayout = 0
                    Else
                        ld_westernunionPayout = CDbl(ls_amount)
                    End If
                Else
                    ld_westernunionPayout = 0
                End If
            End If
            c.DisposeR()
            '\added last 9-3-2012
            ''------Health Care

            '\added last 9-3-2012
            ''------Running Receivable Beginning

            Dim ls_WesternUnionSendout As String = "select dbo.EXEC_SF_1020001CASHBALANCEPESO_CF_V3 ('" + transdate + "','" + bcode + "')"
            'Dim ls_WesternUnionSendout As String = "exec spCashRRBeginning_CF_Ver3 '" + bcode + "'," + Convert.ToDateTime(DtTransdate.Text).ToString("yyyy") + "," + Convert.ToDateTime(DtTransdate.Text).ToString("MM") + " "
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_WesternUnionSendout, rdr, sqlmsg) Then
                If rdr.Read Then
                    ls_amount = Trim(rdr(0))
                    If ls_amount = "" Then
                        ld_westernunionsendout = 0
                    Else
                        ld_westernunionsendout = CDbl(ls_amount)
                    End If
                Else
                    ld_westernunionsendout = 0
                End If
            End If
            c.DisposeR()
            '\added last 9-3-2012
            ''------Running Receivable Beginning



            Dim ls_fundtransferdepositfrombank As String = "select isnull( sum(TotalWithDrawFromBankFundTransferDebit),0) from vwCashFlowDepositFromBankFundTransferDebit_CF_ver3 where branchcode = '" + bcode + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_fundtransferdepositfrombank, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_depositFromBankfundtransferdebit = 0
                    Else
                        ld_depositFromBankfundtransferdebit = CDbl(ls_amount)
                    End If
                Else
                    ld_depositFromBankfundtransferdebit = 0
                End If
            End If
            c.DisposeR()

            Dim ls_FundTransferDebit As String = "select sum(totalfundtransferdebit) from vwCashFlowFundTransferDebit_CF_Ver3" & _
            " where branchcode = '" + bcode + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_FundTransferDebit, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_fundtransferdebit = 0
                    Else
                        ld_fundtransferdebit = CDbl(ls_amount)
                    End If
                Else
                    ld_fundtransferdebit = 0
                End If
            End If
            ld_fundtransferdebit = ld_fundtransferdebit + ld_depositFromBankfundtransferdebit
            c.DisposeR()
            '----------------------------------------------------------

            Dim ls_fundtransferWithDrawalfrombankCredit As String = "select isnull(-1 * sum(TotalWithDrawFromBankFundTransferCredit),0) from vwCashFlowWithdrawalFromBankFundTransferCredit_CF_ver3 where branchcode = '" + bcode + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_fundtransferWithDrawalfrombankCredit, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_withdrawlFromBankfundtransfercredit = 0
                    Else
                        ld_withdrawlFromBankfundtransfercredit = CDbl(ls_amount)
                    End If
                Else
                    ld_withdrawlFromBankfundtransfercredit = 0
                End If
            End If
            c.DisposeR()

            'Dim ls_FundTransferCredit As String = "select -1 * sum(totalfundtransfercredit) " & _
            '" from vwCashFlowFundTransferCredit_CF_ver3 where branchcode = '" + bcode + "' " & _
            '" and transdate = '" + transdate + "'"
            Dim ls_FundTransferCredit As String = "select isnull(sum(totalfundtransfercredit) * -1,0) " & _
            " from vwCashFlowFundTransferCredit_CF_ver3 where branchcode = '" + bcode + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_FundTransferCredit, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_fundtransfercredit = 0
                    Else
                        ld_fundtransfercredit = CDbl(ls_amount)
                    End If
                Else
                    ld_fundtransfercredit = 0
                End If
            End If
            c.DisposeR()

            ld_fundtransfercredit = ld_fundtransfercredit + ld_withdrawlFromBankfundtransfercredit

            '   '--------------------------------------------------------------
            Dim ls_BranchExpense As String = "select sum(totalbranchexpenses) " & _
" from vwCashFlowBranchExpenses_CF_ver3" & _
" where branchcode = '" + bcode + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_BranchExpense, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_branchexpense = 0
                    Else
                        ld_branchexpense = CDbl(ls_amount)
                    End If
                Else
                    ld_branchexpense = 0
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '///added findings by ms. judith 8/25/2011
            Dim ls_OtherExpenseNotRMBase As String = "select  sum(totalotherexpensenotRMBase) " & _
" from vwCashFlowOtherExpenseNotRMBase_CF_ver3 " & _
" where branchcode = '" + bcode + "' and transdate ='" + transdate + "' and rmbase_CostCenter = '0" + bcode + "-" + bcode + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_OtherExpenseNotRMBase, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_OtherexpenseNotRMbase = 0
                    Else
                        ld_OtherexpenseNotRMbase = CDbl(ls_amount)
                    End If
                Else
                    ld_OtherexpenseNotRMbase = 0
                End If
            End If
            c.DisposeR()
            '///added findings by ms. judith 8/25/2011

            Dim ls_OtherExpense As String = "select sum(totalotherexpense) " & _
" from vwCashFlowOtherExpense_CF_ver3" & _
" where branchcode = '" + bcode + "' and transdate ='" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_OtherExpense, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_otherexpense = 0
                    Else
                        ld_otherexpense = CDbl(ls_amount)
                    End If
                Else
                    ld_otherexpense = 0
                End If
            End If
            c.DisposeR()

            ld_otherexpense = ld_otherexpense + ld_OtherexpenseNotRMbase

            '----------------------------------------------------------
            '/-----additional entry for pre alpha findings by judith
            '----------------------------------------------------------
            Dim ls_NSO As String = "select -1 * sum(totalNSo) " & _
            " from vwCashFlowNSO_CF_ver3" & _
            " where branchcode = '" + bcode + "' and transdate ='" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_NSO, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_nso = 0
                    Else
                        ld_nso = CDbl(ls_amount)
                    End If
                Else
                    ld_nso = 0
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '----------------------------------------------------------
            Dim ls_mccr As String = "select sum(TotalMCCashReceipt) " & _
            " from vwCashFlowMCCashReceipt_CF_ver3" & _
            " where branchcode = '" + bcode + "' and transdate ='" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_mccr, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_MCCR = 0
                    Else
                        ld_MCCR = CDbl(ls_amount)
                    End If
                Else
                    ld_MCCR = 0
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '----------------------------------------------------------
            'Dim ls_mccd As String = "select sum(TotalCashDisbursements) " & _
            '" from vwCashFlowMCCashDisbursement_CF_ver3" & _
            '" where branchcode = '" + bcode + "' and transdate ='" + transdate + "'"
            Dim ls_mccd As String = "select sum(totalcashdisbursements) from (select * from vwCashFlowMCCashDisbursement_CF_ver3 where transdate = '" + transdate + "' and branchcode = '" + bcode + "')x where totalcashdisbursements > 0"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_mccd, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_MCCD = 0
                    Else
                        ld_MCCD = CDbl(ls_amount)
                    End If
                Else
                    ld_MCCD = 0
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '----------------------------------------------------------
            Dim ls_racr As String = "select -1 * sum(TotalRACashReceipts) " & _
            " from vwCashflowRACashReceipts_CF_ver3" & _
            " where branchcode = '" + bcode + "' and transdate ='" + transdate + "' and TotalRACashReceipts < 0 "
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_racr, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_RACR = 0
                    Else
                        ld_RACR = CDbl(ls_amount)
                    End If
                Else
                    ld_RACR = 0
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '----------------------------------------------------------
            Dim ls_racd As String = "select sum(TotalRACashDisbursements) " & _
            " from vwCashFlowRACashDisbursements_CF_ver3" & _
            " where branchcode = '" + bcode + "' and transdate ='" + transdate + "' and TotalRACashDisbursements > 0"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_racd, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_RACD = 0
                    Else
                        ld_RACD = CDbl(ls_amount)
                    End If
                Else
                    ld_RACD = 0
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '----------------------------------------------------------
            Dim ls_dfb As String = "select -1 * sum(TotalDeposit) " & _
            " from vwCashFlowDeposit_CF_ver3" & _
            " where branchcode = '" + bcode + "' and transdate ='" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_dfb, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_Depositfrombank = 0
                    Else
                        ld_Depositfrombank = CDbl(ls_amount)
                    End If
                Else
                    ld_Depositfrombank = 0
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '----------------------------------------------------------
            Dim ls_wfb As String = "select sum(TotalWithDrawal) " & _
            " from vwCashFlowWithDrawal_CF_ver3" & _
            " where branchcode = '" + bcode + "' and transdate ='" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_wfb, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_withdrawalfrombank = 0
                    Else
                        ld_withdrawalfrombank = CDbl(ls_amount)
                    End If
                Else
                    ld_withdrawalfrombank = 0
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '----------------------------------------------------------
            Dim ls_rts As String = "select sum(TotalReturnToSender) " & _
            " from vwCashFlowReturnToSender_CF_ver3" & _
            " where branchcode = '" + bcode + "' and transdate ='" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_rts, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_returntosender = 0
                    Else
                        ld_returntosender = CDbl(ls_amount)
                    End If
                Else
                    ld_returntosender = 0
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '\-----additional entry for pre alpha findings by judith


            '/---- 7/26 /2011 additional entry for second pre alpha findings by judith
            '----------------------------------------------------------
            Dim ls_cashover As String = "select isnull(sum(CASHOVERAmt)* -1,0) from vwCashFlowCashOver_CF_Ver3 " & _
         " where branchcode = '" + bcode + "' and transdate ='" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_cashover, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_CashOver = 0
                    Else
                        ld_CashOver = CDbl(ls_amount)
                    End If
                Else
                    ld_CashOver = 0
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            Dim ls_cashshort As String = "select isnull(sum(CASHSHORTAmt),0) from vwCashFlowCashShort_CF_Ver3  " & _
            " where branchcode = '" + bcode + "' and transdate ='" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_cashshort, rdr, sqlmsg) Then
                If rdr.Read Then
                    'ld_otherincome = Trim(rdr(0)) 'area otherincome
                    ls_amount = Trim(rdr(0).ToString)
                    If ls_amount = "" Then
                        ld_CashShort = 0
                    Else
                        ld_CashShort = CDbl(ls_amount)
                    End If
                Else
                    ld_CashShort = 0
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '\---- 7/26 /2011  additional entry for second pre alpha findings by judith




            '\----Not pure lukat amount
            ld_totalreceipts = ld_begbal + ld_NP_lukat + ld_interest + ld_otherincome + ld_kpsendout + ld_kpsendoutcomm + ld_corpsendout + ld_corpcomm + ld_insurance + ld_outrightsales + ld_layaway + ld_foodproducts + ld_telecomms + ld_souvenirs + ld_fundtransfercredit
            'Not pure lukat amount------------/

            ld_totaldisbursements = ld_prenda + ld_kppayout + ld_corppayout + ld_fundtransferdebit + ld_branchexpense + ld_salesreturn + ld_layawaycancel + ld_otherexpense
            'ld_totaldisbursements = ld_prenda + ld_kppayout + ld_corpcomm + ld_fundtransferdebit + ld_branchexpense + ld_otherexpense ' commented  last 9/20/2010 because instead of corp payout t'was corp commission being inputted
            ld_endingbal = ld_totalreceipts - ld_totaldisbursements
            '\----------pure lukat
            'ld_lukat = ld_NP_lukat ' adjusted last 8/24/2011 findings by ms judith
            ld_lukat = ld_NP_lukat - ld_rematado
            '--------------/pure lukat

            ld_begbal_lv = ListViewAreaInsertion.Items.Add(ld_begbal).Text ' area beginning balance
            ld_endingbal_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_endingbal)).Text 'area ending balance
            ld_foodproducts_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_foodproducts)).Text 'area food products
            ld_insurance_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_insurance)).Text 'area insurance
            ld_outrightsales_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_outrightsales)).Text 'area outrightsales
            ld_layaway_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_layaway)).Text 'area layaway
            ld_salesreturn_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_salesreturn)).Text 'area salesreturn
            ld_layawaycancel_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_layawaycancel)).Text 'area layawaycancel
            ld_interest_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_interest)).Text 'area interest
            ld_kppayout_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_kppayout)).Text 'area kppayout
            ld_kpsendout_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_kpsendout)).Text 'area kpsendout
            ld_kpsendoutcomm_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_kpsendoutcomm)).Text 'area sendoutcomm
            ld_lukat_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_lukat)).Text 'area lukat
            ld_otherincome_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_otherincome)).Text 'area otheri1ncome
            ld_prenda_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_prenda)).Text 'area prenda
            ld_telecomms_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_telecomms)).Text 'area telecomms
            ld_souvenirs_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_souvenirs)).Text 'area souvenirs
            ld_corpsendout_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_corpsendout)).Text 'area corp sendout
            ld_corppayout_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_corppayout)).Text 'area corp payout
            ld_corpcomm_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_corpcomm)).Text 'area corp comm

            '/update last 9-3-2012 for 3.1 cashflow version
            ld_westernUnionComm_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_westernUnionComm)).Text 'travel and tours
            ld_westernunionPayout_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_westernunionPayout)).Text 'health care
            ld_westernunionsendout_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_westernunionsendout)).Text 'running receivable beginning balance
            '\update last 9-3-2012 for 3.1 cashflow version




            ld_fundtransferdebit_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_fundtransferdebit)).Text 'area fund transfer debit
            ld_fundtransfercredit_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_fundtransfercredit)).Text 'area fund transfer credit
            ld_branchexpense_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_branchexpense)).Text 'area branch expense
            ld_otherexpense_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_otherexpense)).Text 'area ohter expense
            '/added last 2/8/2011
            ld_nso_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_nso)).Text 'NSO
            ld_MCCR_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_MCCR)).Text 'Money Changer Cash Receipts
            ld_MCCD_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_MCCD)).Text 'Money Changer Cash Disbursements
            ld_RACR_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_RACR)).Text 'Renewal anywhere Cash Receipts
            ld_RACD_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_RACD)).Text 'Renewal Anywhere Cash Disbursements
            ld_Depositfrombank_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_Depositfrombank)).Text 'Deposit From Bank
            ld_withdrawalfrombank_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_withdrawalfrombank)).Text 'Withdrawal from Bank
            ld_returntosender_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_returntosender)).Text 'Withdrawal from Bank
            '\added last 2/8/2011
            '/added last 7/26/2011
            ld_CashOver_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_CashOver)).Text 'Withdrawal from Bank
            ld_CashShort_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_CashShort)).Text 'Withdrawal from Bank
            '\added last 7/26/2011
            '/added last 9/3/2012

            '\added last 9/3/2012




            pb_transdate_exist() 'call transdate
            Try
                If gb_pb_update = True Then
                    Dim ls_update As String = "UPDATE CF_PB_VisMin" & _
                    " SET [BeginningBalance]=" & ld_begbal_lv & ", [EndingBalance]=" & ld_endingbal_lv & ", [FoodProducts]=" & ld_foodproducts_lv & ",[Insurance]=" & ld_insurance_lv & ", [outrightsales]=" & ld_outrightsales_lv & ", [layaway]=" & ld_layaway_lv & ", [salesreturn]=" & ld_salesreturn_lv & ", [layawaycancel]=" & ld_layawaycancel_lv & ", [Interest]=" & ld_interest_lv & ",  " & _
                    " [KP_Payout]= " & ld_kppayout_lv & ", [KP_Sendout]=" & ld_kpsendout_lv & ", [KP_Sendout_Comm]=" & ld_kpsendoutcomm_lv & " , [Lukat]= " & ld_lukat_lv & ", [OtherIncome]= " & ld_otherincome_lv & "  , [Prenda]=" & ld_prenda_lv & " , " & _
                    " [Telecomms] = " & ld_telecomms_lv & ", [Souvenirs]=" & ld_souvenirs_lv & ", [Corp_Sendout]=" & ld_corpsendout_lv & ", [Corp_Payout]=" & ld_corppayout_lv & ",[Corp_Comm]=" & ld_corpcomm_lv & " ,[WesternUnionComm]=" & ld_westernUnionComm_lv & ", " & _
                    " [WesternUnionPayout]=" & ld_westernunionPayout_lv & ", [WesternUnionSendout]=" & ld_westernunionsendout_lv & ", [FundTransferDebit]=" & ld_fundtransferdebit_lv & ", [FundTransferCredit]=" & ld_fundtransfercredit_lv & ", " & _
                    " [BranchExpense]=" & ld_branchexpense_lv & ",[OtherExpense] = " & ld_otherexpense_lv & " ,[NSO] = " & ld_nso_lv & " ,[MCCashReceipts] = " & ld_MCCR_lv & ",[MCCashDisbursements] = " & ld_MCCD_lv & ",[RACashReceipts] = " & ld_RACR_lv & ",[RACashDisbursements] = " & ld_RACD_lv & ",[DepositFromBank] = " & ld_Depositfrombank_lv & " ,[WithdrawalFromBank] = " & ld_withdrawalfrombank_lv & ",[ReturnToSender] = " & ld_returntosender_lv & " , [Cashover] = " & ld_CashOver_lv & " , [cashshort] = " & ld_CashShort_lv & "  ,[dategenerated] = '" + Now.Date + "' WHERE class_02 = 'Mindanao' and branchcode = '" + bcode + "' and transdate = '" + transdate + "' "
                    Log("Update--" & bcode & " " & transdate & " " & Now.TimeOfDay.ToString)
                    Dim c26 As New clsData
                    Dim rdr26 As SqlClient.SqlDataReader = Nothing
                    If c26.Error_Inititalize_INI Then Exit Sub
                    If c26.ErrorConnectionReading(False) Then Exit Sub
                    If Not c26.Error_SetRdr(ls_update, rdr26, sqlmsg) Then
                    End If
                    c26.DisposeR()
                Else
                    Dim ls_s As String = "INSERT INTO CF_PB_vismin ([Id],[branchcode],[branchname],[Class_03], [Class_04], [BeginningBalance], [EndingBalance], [FoodProducts],   [Insurance],  [outrightsales],  [layaway],  [salesreturn],  [layawaycancel], [Interest], [KP_Payout], [KP_Sendout], [KP_Sendout_Comm], [Lukat], [OtherIncome],[Prenda], [Telecomms], [Souvenirs], [Corp_Sendout], [Corp_Payout], [Corp_Comm], [WesternUnionComm],[WesternUnionPayout],[WesternUnionSendout],[FundTransferDebit], [FundTransferCredit], [BranchExpense], [OtherExpense],[NSO],[MCCashReceipts],[MCCashDisbursements],[RACashReceipts],[RACashDisbursements],[DepositFromBank],[WithdrawalFromBank],[ReturnToSender],[cashover],[cashshort] ,[Transdate], [DateGenerated], [class_02])" & _
                        " VALUES(" & li_idcount & ",'" + bcode + "','" + branchname + "','" + class03 + "', '" + ls_class04 + "', " & ld_begbal_lv & "," & ld_endingbal_lv & "," & ld_foodproducts_lv & "," & ld_insurance_lv & "," & ld_outrightsales_lv & ", " & ld_layaway_lv & ", " & ld_salesreturn_lv & ", " & ld_layawaycancel_lv & ", " & ld_interest_lv & "," & ld_kppayout_lv & "," & ld_kpsendout_lv & ", " & ld_kpsendoutcomm_lv & "," & ld_lukat_lv & ", " & ld_otherincome_lv & ", " & ld_prenda_lv & ", " & _
                        " " & ld_telecomms_lv & ", " & ld_souvenirs_lv & ", " & ld_corpsendout_lv & "," & _
                        " " & ld_corppayout_lv & ", " & ld_corpcomm_lv & ", " & ld_westernUnionComm_lv & ", " & _
                        " " & ld_westernunionPayout_lv & "," & ld_westernunionsendout_lv & "," & ld_fundtransferdebit_lv & "," & ld_fundtransfercredit_lv & "," & ld_branchexpense_lv & ", " & _
                        " " & ld_otherexpense_lv & "," & ld_nso_lv & "," & ld_MCCR_lv & "," & ld_MCCD_lv & "," & ld_RACR_lv & "," & ld_RACD_lv & "," & ld_Depositfrombank_lv & "," & ld_withdrawalfrombank_lv & "," & ld_returntosender_lv & ", " & ld_CashOver_lv & " ," & ld_CashShort_lv & " ,'" + transdate + "','" + Now.Date + "', 'Mindanao') "
                    Log(bcode & " " & transdate & " " & Now.TimeOfDay.ToString)
                    Dim c25 As New clsData
                    Dim rdr25 As SqlClient.SqlDataReader = Nothing
                    If c25.Error_Inititalize_INI Then Exit Sub
                    If c25.ErrorConnectionReading(False) Then Exit Sub
                    If Not c25.Error_SetRdr(ls_s, rdr25, sqlmsg) Then
                    End If
                    c25.DisposeR()
                End If
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try

        Next
        Exit Sub
    End Sub
    Private Sub insert_to_CF_area_vismin()
        Dim li_i As Integer = Nothing
        Dim li_idcount As Integer = Nothing
        Dim li_listviewcount As Integer = Nothing

        li_listviewcount = ListviewArea.Items.Count - 1


        For li_i = 0 To li_listviewcount
            Dim ls_class03 As String = Nothing
            ls_class03 = Trim(ListviewArea.Items(li_i).Text.ToString)

            Dim class03 As String = Replace(ls_class03, ",", " ")
            Dim ls_class04 As String = ListviewArea.Items(li_i).SubItems.Item(1).Text

            Call id_count_area_vismin()
            li_idcount = gi_idcount_area
            li_idcount = li_idcount + 1
            Dim ld_begbal As Double = Nothing 'beginning balance
            Dim ld_endingbal As Double = Nothing 'ending balance
            Dim ld_foodproducts As Double = Nothing 'food products
            Dim ld_insurance As Double = Nothing 'insurance
            Dim ld_outrightsales As Double = Nothing 'outright sales---------------------------jen
            Dim ld_layaway As Double = Nothing 'layaway
            Dim ld_salesreturn As Double = Nothing 'sales return 
            Dim ld_layawaycancel As Double = Nothing 'layaway cancel
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
            '/-------------------added last 2/9/2011 due to judith findings
            Dim ld_nso As Double = Nothing 'NSO
            Dim ld_MCCR As Double = Nothing 'Money Changer Cash Receipts
            Dim ld_MCCD As Double = Nothing 'Money Changer Cash Disbursements
            Dim ld_RACR As Double = Nothing 'Renewal anywhere Cash Receipts
            Dim ld_RACD As Double = Nothing ' Renewal Anywhere Cash Disbursements
            Dim ld_Depositfrombank As Double = Nothing ' 'Deposit From Bank
            Dim ld_withdrawalfrombank As Double = Nothing ' Withdrawal from Bank
            Dim ld_returntosender As Double = Nothing ' Returntosender 2/10/2011

            '\-------------------added last 2/9/2011 due to judith findings

            '/-------------------added last 7/26/2011 due to judith findings
            Dim ld_cashover As Double = Nothing ' CashOver 7/26/2011
            Dim ld_cashshort As Double = Nothing ' CashShort 7/26/2011
            '\-------------------added last 7/26/2011 due to judith findings


            Dim ld_begbal_lv As Double = Nothing 'beginning balance
            Dim ld_endingbal_lv As Double = Nothing 'ending balance
            Dim ld_foodproducts_lv As Double = Nothing 'food products
            Dim ld_insurance_lv As Double = Nothing 'insurance
            Dim ld_outrightsales_lv As Double = Nothing 'outright sales--------------------------jen
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

            '/-------------------added last 2/8/2011 due to judith findings
            Dim ld_nso_lv As Double = Nothing 'NSO
            Dim ld_MCCR_lv As Double = Nothing 'Money Changer Cash Receipts
            Dim ld_MCCD_lv As Double = Nothing 'Money Changer Cash Disbursements
            Dim ld_RACR_lv As Double = Nothing 'Renewal anywhere Cash Receipts
            Dim ld_RACD_lv As Double = Nothing ' Renewal Anywhere Cash Disbursements
            Dim ld_Depositfrombank_lv As Double = Nothing ' 'Deposit From Bank
            Dim ld_withdrawalfrombank_lv As Double = Nothing ' Withdrawal from Bank
            Dim ld_returntosender_lv As Double = Nothing ' Returntosender 2/10/2011
            '\-------------------added last 2/8/2011 due to judith findings

            '/-------------------added last 7/26/2011 due to judith findings
            Dim ld_cashover_lv As Double = Nothing ' CashOver 7/26/2011
            Dim ld_cashshort_lv As Double = Nothing ' CashShort 7/26/2011
            '\-------------------added last 7/26/2011 due to judith findings


            'code starts here-------------------------------------------------------------------
            Dim ls_begbal As String = "select sum(beginningbalance) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            Dim c As New clsData
            Dim rdr As SqlClient.SqlDataReader = Nothing
            If c.Error_Inititalize_INI Then Exit Sub
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_begbal, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_begbal = 0.0
                    Else
                        ld_begbal = Trim(rdr(0)) 'area ending balance
                    End If
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_endingbal As String = "select sum(EndingBalance) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_endingbal, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_endingbal = 0.0
                    Else
                        ld_endingbal = Trim(rdr(0)) 'area ending balance
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            Dim ls_foodproducts As String = "select sum(Foodproducts) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
           "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_foodproducts, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_foodproducts = 0.0
                    Else
                        ld_foodproducts = Trim(rdr(0)) 'area food products
                    End If
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_insurance As String = "select sum(Insurance) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_insurance, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_insurance = 0.0
                    Else
                        ld_insurance = Trim(rdr(0)) 'area Insurance
                    End If
                End If
            End If
            c.DisposeR()

            '----------------------------------------------------------jen

            Dim ls_outrightsales As String = "select sum(outrightsales) from CF_pb_vismin where class_02 = 'Mindanao' and transdate =  '" & transdate & "'and class_04 = '" & ls_class04 & "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_outrightsales, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_outrightsales = 0.0
                    Else
                        ld_outrightsales = Trim(rdr(0)) 'area outrightsales
                    End If
                End If
            End If
            c.DisposeR()
            '-------------------------------- 
            Dim ls_layaway As String = "select sum(layaway) from CF_pb_vismin where class_02 = 'Mindanao' and transdate =  '" & transdate & "'and class_04 = '" & ls_class04 & "'"

            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_layaway, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_layaway = 0.0
                    Else
                        ld_layaway = Trim(rdr(0)) 'area layaway
                    End If
                End If
            End If
            c.DisposeR()

            '--------------------------------
            Dim ls_salesreturn As String = "select sum(salesreturn) from CF_pb_vismin where class_02 = 'Mindanao' and transdate =  '" & transdate & "'and class_04 = '" & ls_class04 & "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_salesreturn, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_salesreturn = 0.0
                    Else
                        ld_salesreturn = Trim(rdr(0)) 'area salesreturn
                    End If
                End If
            End If
            c.DisposeR()


            '----------------------------------------------------
            Dim ls_layawaycancel As String = "select sum(layawaycancel) from CF_pb_vismin  where class_02 = 'Mindanao' and transdate =  '" & transdate & "'and class_04 = '" & ls_class04 & "'"

            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_layawaycancel, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_layawaycancel = 0.0
                    Else
                        ld_layawaycancel = Trim(rdr(0)) 'area layawaycancel
                    End If
                End If
            End If
            c.DisposeR()
            '---------------------------------------------------------------------------------------

            Dim ls_interest As String = "select sum(interest) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
         "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_interest, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_interest = 0.0
                    Else
                        ld_interest = Trim(rdr(0)) 'area interest
                    End If
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_kppayout As String = "select sum(kp_payout) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"

            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_kppayout, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_kppayout = 0.0
                    Else
                        ld_kppayout = Trim(rdr(0)) 'area kp payout
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            Dim ls_kpsendout As String = "select sum(kp_sendout) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
          "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_kpsendout, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_kpsendout = 0.0
                    Else
                        ld_kpsendout = Trim(rdr(0)) 'area kp sendout
                    End If
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_kpsendoutcomm As String = "select sum(KP_Sendout_Comm) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_kpsendoutcomm, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_kpsendoutcomm = 0.0
                    Else
                        ld_kpsendoutcomm = Trim(rdr(0)) 'area kp sendout comm
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            Dim ls_lukat As String = "select sum(lukat) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
           "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_lukat, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_lukat = 0.0
                    Else
                        ld_lukat = Trim(rdr(0)) 'area lukat
                    End If
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_otherincome As String = "select sum(OtherIncome) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_otherincome, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_otherincome = 0.0
                    Else
                        ld_otherincome = Trim(rdr(0)) 'area otherincome
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            Dim ls_Prenda As String = "select sum(prenda) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
         "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_Prenda, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_prenda = 0.0
                    Else
                        ld_prenda = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_telecomms As String = "select sum(telecomms) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_telecomms, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_telecomms = 0.0
                    Else
                        ld_telecomms = Trim(rdr(0)) 'area telecomms
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            Dim ls_souvenirs As String = "select sum(Souvenirs) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_souvenirs, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_souvenirs = 0.0
                    Else
                        ld_souvenirs = Trim(rdr(0)) 'area souvenirs
                    End If
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_Corp_Sendout As String = "select sum(Corp_Sendout) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_Corp_Sendout, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_corpsendout = 0.0
                    Else
                        ld_corpsendout = Trim(rdr(0)) 'area ending balance
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            Dim ls_Corp_Payout As String = "select sum(Corp_Payout) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
           "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_Corp_Payout, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_corppayout = 0.0
                    Else
                        ld_corppayout = Trim(rdr(0)) 'area food products
                    End If
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_Corp_Comm As String = "select sum(Corp_Comm) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_Corp_Comm, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_corpcomm = 0.0
                    Else
                        ld_corpcomm = Trim(rdr(0)) 'area Insurance
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            Dim ls_WesternUnionComm As String = "select sum(WesternUnionComm) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
         "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_WesternUnionComm, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_westernUnionComm = 0.0
                    Else
                        ld_westernUnionComm = Trim(rdr(0)) 'area interest
                    End If
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_WesternUnionPayout As String = "select sum(WesternUnionPayout) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_WesternUnionPayout, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_westernunionPayout = 0.0
                    Else
                        ld_westernunionPayout = Trim(rdr(0)) 'area kp payout
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            Dim ls_WesternUnionSendout As String = "select sum(WesternUnionSendout) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
          "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_WesternUnionSendout, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_westernunionsendout = 0.0
                    Else
                        ld_westernunionsendout = Trim(rdr(0)) 'area kp sendout
                    End If
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_FundTransferDebit As String = "select sum(FundTransferDebit) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_FundTransferDebit, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_fundtransferdebit = 0.0
                    Else
                        ld_fundtransferdebit = Trim(rdr(0)) 'area kp sendout comm
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            Dim ls_FundTransferCredit As String = "select sum(FundTransferCredit) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
           "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_FundTransferCredit, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_fundtransfercredit = 0.0
                    Else
                        ld_fundtransfercredit = Trim(rdr(0)) 'area lukat
                    End If
                End If
            End If
            c.DisposeR()
            '--------------------------------------------------------------
            Dim ls_BranchExpense As String = "select sum(BranchExpense) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_BranchExpense, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_branchexpense = 0.0
                    Else
                        ld_branchexpense = Trim(rdr(0)) 'area otherincome
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            Dim ls_OtherExpense As String = "select sum(OtherExpense) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_OtherExpense, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_otherexpense = 0.0
                    Else
                        ld_otherexpense = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------

            '/-added last 2/9/2011

            '----------------------------------------------------------
            Dim ls_nso As String = "select sum(NSO) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_nso, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_nso = 0.0
                    Else
                        ld_nso = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '----------------------------------------------------------
            Dim ls_mccr As String = "select sum(MCCashReceipts) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_mccr, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_MCCR = 0.0
                    Else
                        ld_MCCR = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '----------------------------------------------------------
            Dim ls_mccd As String = "select sum(MCCashDisbursements) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_mccd, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_MCCD = 0.0
                    Else
                        ld_MCCD = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '----------------------------------------------------------
            Dim ls_racr As String = "select sum(RACashReceipts) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_racr, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_RACR = 0.0
                    Else
                        ld_RACR = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '----------------------------------------------------------
            Dim ls_racd As String = "select sum(RACashDisbursements) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_racd, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_RACD = 0.0
                    Else
                        ld_RACD = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '----------------------------------------------------------
            Dim ls_dfb As String = "select sum(DepositFromBank) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_dfb, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_Depositfrombank = 0.0
                    Else
                        ld_Depositfrombank = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '----------------------------------------------------------
            Dim ls_wfb As String = "select sum(WithdrawalFromBank) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_wfb, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_withdrawalfrombank = 0.0
                    Else
                        ld_withdrawalfrombank = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '----------------------------------------------------------
            Dim ls_rts As String = "select sum(ReturnToSender) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
            "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_rts, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_returntosender = 0.0
                    Else
                        ld_returntosender = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '\-added last 2/9/2011

            '\-added last 7/26/2011
            '----------------------------------------------------------
            Dim ls_cashover As String = "select sum(cashover) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
         "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_cashover, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_cashover = 0.0
                    Else
                        ld_cashover = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()

            '----------------------------------------------------------
            Dim ls_cashshort As String = "select sum(cashshort) from CF_pb_vismin where class_02 = 'Mindanao' and " & _
       "class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "'"
            If c.ErrorConnectionReading(False) Then Exit Sub
            If Not c.Error_SetRdr(ls_cashshort, rdr, sqlmsg) Then
                If rdr.Read Then
                    If IsDBNull(rdr(0)) Then '---Arthur
                        ld_cashshort = 0.0
                    Else
                        ld_cashshort = Trim(rdr(0)) 'area prenda
                    End If
                End If
            End If
            c.DisposeR()
            '----------------------------------------------------------
            '\-added last 7/26/2011

            ld_begbal_lv = ListViewAreaInsertion.Items.Add(ld_begbal).Text ' area beginning balance
            ld_endingbal_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_endingbal)).Text 'area ending balance
            ld_foodproducts_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_foodproducts)).Text 'area food products
            ld_insurance_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_insurance)).Text 'area insurance
            ld_outrightsales_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_outrightsales)).Text 'area outrightsales
            ld_layaway_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_layaway)).Text 'area layaway
            ld_salesreturn_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_salesreturn)).Text 'area salesreturn
            ld_layawaycancel_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_layawaycancel)).Text 'area layawaycancel
            ld_interest_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_interest)).Text 'area interest
            ld_kppayout_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_kppayout)).Text 'area kppayout
            ld_kpsendout_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_kpsendout)).Text 'area kpsendout
            ld_kpsendoutcomm_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_kpsendoutcomm)).Text 'area sendoutcomm
            ld_lukat_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_lukat)).Text 'area lukat
            ld_otherincome_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_otherincome)).Text 'area otheri1ncome
            ld_prenda_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_prenda)).Text 'area prenda
            ld_telecomms_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_telecomms)).Text 'area telecomms
            ld_souvenirs_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_souvenirs)).Text 'area souvenirs
            ld_corpsendout_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_corpsendout)).Text 'area corp sendout
            ld_corppayout_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_corppayout)).Text 'area corp payout
            ld_corpcomm_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_corpcomm)).Text 'area corp comm
            ld_westernUnionComm_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_westernUnionComm)).Text 'area wuc
            ld_westernunionPayout_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_westernunionPayout)).Text 'area wup
            ld_westernunionsendout_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_westernunionsendout)).Text 'area wus
            ld_fundtransferdebit_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_fundtransferdebit)).Text 'area fund transfer debit
            ld_fundtransfercredit_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_fundtransfercredit)).Text 'area fund transfer credit
            ld_branchexpense_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_branchexpense)).Text 'area branch expense
            ld_otherexpense_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_otherexpense)).Text 'area ohter expense

            '/-added last 2/9/2011
            ld_nso_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_nso)).Text 'NSO
            ld_MCCR_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_MCCR)).Text 'Money Changer Cash Receipts
            ld_MCCD_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_MCCD)).Text 'Money Changer Cash Disbursements
            ld_RACR_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_RACR)).Text 'Renewal anywhere Cash Receipts
            ld_RACD_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_RACD)).Text 'Renewal Anywhere Cash Disbursements
            ld_Depositfrombank_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_Depositfrombank)).Text 'Deposit From Bank
            ld_withdrawalfrombank_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_withdrawalfrombank)).Text 'Withdrawal from Bank
            ld_returntosender_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_returntosender)).Text 'Return to Sender

            '\-added last 2/9/2011
            '/-added last 7/26/2011
            ld_cashover_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_cashover)).Text 'Return to Sender
            ld_cashshort_lv = ListViewAreaInsertion.Items(ListViewAreaInsertion.Items.Count - 1).SubItems.Add(Trim(ld_cashshort)).Text 'Return to Sender

            '\-added last 7/26/2011


            area_transdate_exist() 'call transdate

            If gb_area_update = True Then
                Dim ls_update As String = "UPDATE [CF_area_VisMin]" & _
" SET [BeginningBalance]=" & ld_begbal_lv & ", [EndingBalance]=" & ld_endingbal_lv & ", [FoodProducts]=" & ld_foodproducts_lv & ",[Insurance]=" & ld_insurance_lv & ",[outrightsales]=" & ld_outrightsales & ",[layaway]=" & ld_layaway & ",[salesreturn]=" & ld_salesreturn & ",[layawaycancel]=" & ld_layawaycancel & ", [Interest]=" & ld_interest_lv & ",  " & _
" [KP_Payout]= " & ld_kppayout_lv & ", [KP_Sendout]=" & ld_kpsendout_lv & ", [KP_Sendout_Comm]=" & ld_kpsendoutcomm_lv & " , [Lukat]= " & ld_lukat_lv & ", [OtherIncome]= " & ld_otherincome_lv & "  , [Prenda]=" & ld_prenda_lv & " , " & _
" [Telecomms] = " & ld_telecomms_lv & ", [Souvenirs]=" & ld_souvenirs_lv & ", [Corp_Sendout]=" & ld_corpsendout_lv & ", [Corp_Payout]=" & ld_corppayout_lv & ",[Corp_Comm]=" & ld_corpcomm_lv & " ,[WesternUnionComm]=" & ld_westernUnionComm_lv & ", " & _
" [WesternUnionPayout]=" & ld_westernunionPayout_lv & ", [WesternUnionSendout]=" & ld_westernunionsendout_lv & ", [FundTransferDebit]=" & ld_fundtransferdebit_lv & ", [FundTransferCredit]=" & ld_fundtransfercredit_lv & ", " & _
"[BranchExpense]=" & ld_branchexpense_lv & ",[OtherExpense] = " & ld_otherexpense_lv & " ,[NSO] = " & ld_nso_lv & " ,[MCCashReceipts] = " & ld_MCCR_lv & ",[MCCashDisbursements] = " & ld_MCCD_lv & ",[RACashReceipts] = " & ld_RACR_lv & ",[RACashDisbursements] = " & ld_RACD_lv & ",[DepositFromBank] = " & ld_Depositfrombank_lv & " ,[WithdrawalFromBank] = " & ld_withdrawalfrombank_lv & ",[ReturnToSender] = " & ld_returntosender_lv & " ,[cashover] = " & ld_cashover_lv & "  ,[cashshort] = " & ld_cashshort_lv & "  ,[dategenerated] = '" + Now.Date + "' WHERE class_02 = 'Mindanao' and class_04 = '" + ls_class04 + "' and transdate = '" + transdate + "' "
                Log("Update Area--" & ls_class04 & " " & transdate & " " & Now.TimeOfDay.ToString)
                If c.ErrorConnectionReading(False) Then Exit Sub
                If Not c.Error_SetRdr(ls_update, rdr, sqlmsg) Then
                End If
                c.DisposeR()

            Else
                Dim ls_s As String = "INSERT INTO CF_AREA_vismin ([Id],[Class_03], [Class_04], [BeginningBalance], [EndingBalance], [FoodProducts],   [Insurance],  [outrightsales],  [layaway],  [salesreturn],  [layawaycancel],  [Interest], [KP_Payout], [KP_Sendout], [KP_Sendout_Comm], [Lukat], [OtherIncome],[Prenda], [Telecomms], [Souvenirs], [Corp_Sendout], [Corp_Payout], [Corp_Comm], [WesternUnionComm],[WesternUnionPayout],[WesternUnionSendout],[FundTransferDebit], [FundTransferCredit], [BranchExpense], [OtherExpense],[NSO],[MCCashReceipts],[MCCashDisbursements],[RACashReceipts],[RACashDisbursements],[DepositFromBank],[WithdrawalFromBank],[ReturnToSender],[cashover],[cashshort], [Transdate], [DateGenerated], [class_02])" & _
                     " VALUES(" & li_idcount & ",'" + class03 + "', '" + ls_class04 + "'," & _
                     " " & ld_begbal_lv & "," & ld_endingbal_lv & "," & ld_foodproducts_lv & "," & ld_insurance_lv & "," & ld_outrightsales & "," & ld_layaway & "," & ld_salesreturn & "," & ld_layawaycancel & "," & ld_interest_lv & "," & ld_kppayout_lv & "," & ld_kpsendout_lv & ", " & ld_kpsendoutcomm_lv & "," & ld_lukat_lv & ", " & ld_otherincome_lv & ", " & ld_prenda_lv & ", " & _
                     " " & ld_telecomms_lv & ", " & ld_souvenirs_lv & ", " & ld_corpsendout_lv & "," & _
                     " " & ld_corppayout_lv & ", " & ld_corpcomm_lv & ", " & ld_westernUnionComm_lv & ", " & _
                     " " & ld_westernunionPayout_lv & "," & ld_westernunionsendout_lv & "," & ld_fundtransferdebit_lv & "," & ld_fundtransfercredit_lv & "," & ld_branchexpense_lv & ", " & _
                     " " & ld_otherexpense_lv & "," & ld_nso_lv & "," & ld_MCCR_lv & "," & ld_MCCD_lv & "," & ld_RACR_lv & "," & ld_RACD_lv & "," & ld_Depositfrombank_lv & "," & ld_withdrawalfrombank_lv & "," & ld_returntosender_lv & "," & ld_cashover_lv & ", " & ld_cashshort_lv & " ,'" + transdate + "','" + Now.Date + "', 'Mindanao') "
                Log("Insert Area--" & ls_class04 & " " & transdate & " " & Now.TimeOfDay.ToString)
                If c.ErrorConnectionReading(False) Then Exit Sub
                If Not c.Error_SetRdr(ls_s, rdr, sqlmsg) Then
                End If
                c.DisposeR()
            End If
        Next
        Exit Sub
    End Sub
    Private Sub select_per_branch()
        Dim rdr As SqlClient.SqlDataReader = Nothing
        Dim fullname As String = Nothing
        Dim idno As String = Nothing
        Dim username As String = Nothing
        Dim jobtitle As String = Nothing
        'updated by Arthur 6/17/13
        Dim ls_s As String = "select bedrnr, bedrnm, class_03, class_04, class_02 from bedryf where class_02 = 'Mindanao' order by class_04 asc"
        Dim c As New ClsMindanao
        If c.Error_Inititalize_INI Then Exit Sub
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_s, rdr, sqlmsg) Then
            While rdr.Read
                ListViewPb.Items.Add(Trim(rdr(0)))
                ListViewPb.Items(ListViewPb.Items.Count - 1).SubItems.Add(Trim(rdr(1)))
                ListViewPb.Items(ListViewPb.Items.Count - 1).SubItems.Add(Trim(rdr(2)))
                ListViewPb.Items(ListViewPb.Items.Count - 1).SubItems.Add(Trim(rdr(3)))
            End While
            c.DisposeR()
        End If
    End Sub
    Private Sub select_region()
        Dim rdr As SqlClient.SqlDataReader = Nothing
        Dim fullname As String = Nothing
        Dim idno As String = Nothing
        Dim username As String = Nothing
        Dim jobtitle As String = Nothing
        'updated by Arthur 6/17/13
        Dim ls_s As String = "select distinct class_03, class_02 from bedryf where class_02 = 'Mindanao' order by class_03 asc"
        Dim c As New ClsMindanao
        If c.Error_Inititalize_INI Then Exit Sub
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_s, rdr, sqlmsg) Then
            While rdr.Read
                ListViewRegion.Items.Add(Trim(rdr(0)))
            End While
            c.DisposeR()
        End If
    End Sub
    Private Sub select_area()
        Dim rdr As SqlClient.SqlDataReader = Nothing
        Dim fullname As String = Nothing
        Dim idno As String = Nothing
        Dim username As String = Nothing
        Dim jobtitle As String = Nothing
        'updated by Arthur 6/17/13
        Dim ls_s As String = "select distinct class_03, class_04, class_02 from bedryf where class_02 = 'Mindanao' order by class_04 asc"
        Dim c As New ClsMindanao
        If c.Error_Inititalize_INI Then Exit Sub
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_s, rdr, sqlmsg) Then
            While rdr.Read
                ListviewArea.Items.Add(Trim(rdr(0)))
                ListviewArea.Items(ListviewArea.Items.Count - 1).SubItems.Add(Trim(rdr(1)))
            End While
            c.DisposeR()
        End If
    End Sub
    Private Sub id_count_vismin_wide()
        Dim rdr As SqlClient.SqlDataReader = Nothing
        Dim fullname As String = Nothing
        Dim idno As String = Nothing
        Dim username As String = Nothing
        Dim jobtitle As String = Nothing
        Dim ls_s As String = "select count(id) from cf_vismin_wide where class_02 = 'Mindanao'"
        Dim c As New clsData
        If c.Error_Inititalize_INI Then Exit Sub
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_s, rdr, sqlmsg) Then
            If rdr.Read Then
                ls_amount = rdr(0).ToString
                If ls_amount = "" Then
                    gi_idcount_visminwide = 0
                Else
                    gi_idcount_visminwide = CDbl(ls_amount)
                End If
            Else
                gi_idcount_visminwide = 0
            End If
            c.DisposeR()
        End If
    End Sub
    Private Sub id_count_region_vismin()
        Dim rdr As SqlClient.SqlDataReader = Nothing
        Dim fullname As String = Nothing
        Dim idno As String = Nothing
        Dim username As String = Nothing
        Dim jobtitle As String = Nothing
        Dim ls_s As String = "select count(id) from cf_region_vismin where class_02 = 'Mindanao'"
        Dim c As New clsData
        If c.Error_Inititalize_INI Then Exit Sub
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_s, rdr, sqlmsg) Then
            If rdr.Read Then
                ls_amount = rdr(0).ToString
                If ls_amount = "" Then
                    gi_idcount_region = 0
                Else
                    gi_idcount_region = CDbl(ls_amount)
                End If
            Else
                gi_idcount_region = 0
            End If
            c.DisposeR()
        End If
    End Sub
    Private Sub id_count_area_vismin()
        Dim rdr As SqlClient.SqlDataReader = Nothing
        Dim fullname As String = Nothing
        Dim idno As String = Nothing
        Dim username As String = Nothing
        Dim jobtitle As String = Nothing
        Dim ls_s As String = "select count(id) from cf_area_vismin where class_02 = 'Mindanao'"
        Dim c As New clsData
        If c.Error_Inititalize_INI Then Exit Sub
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_s, rdr, sqlmsg) Then
            If rdr.Read Then
                ls_amount = rdr(0).ToString
                If ls_amount = "" Then
                    gi_idcount_area = 0
                Else
                    gi_idcount_area = CDbl(ls_amount)
                End If
            Else
                gi_idcount_area = 0
            End If
            c.DisposeR()
        End If
    End Sub
    Private Sub id_count_pb_vsimin()
        Dim rdr As SqlClient.SqlDataReader = Nothing
        Dim fullname As String = Nothing
        Dim idno As String = Nothing
        Dim username As String = Nothing
        Dim jobtitle As String = Nothing
        Dim ls_s As String = "select count(id) from cf_pb_vismin where class_02 = 'Mindanao'"
        Dim c As New clsData
        If c.Error_Inititalize_INI Then Exit Sub
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_s, rdr, sqlmsg) Then
            If rdr.Read Then
                ls_amount = rdr(0).ToString
                If ls_amount = "" Then
                    gi_idcount_pb = 0
                Else
                    gi_idcount_pb = CDbl(ls_amount)
                End If
            Else
                gi_idcount_pb = 0
            End If
            c.DisposeR()
        End If
    End Sub
    Private Sub pb_transdate_exist()
        Dim rdr As SqlClient.SqlDataReader = Nothing
        Dim fullname As String = Nothing
        Dim idno As String = Nothing
        Dim username As String = Nothing
        Dim jobtitle As String = Nothing
        Dim ls_s As String = "select distinct transdate from cf_pb_vismin " & _
" where class_02 = 'Mindanao' and transdate = '" + transdate + "' and branchcode = '" + gs_bcode + "'"
        Dim c As New clsData
        If c.Error_Inititalize_INI Then Exit Sub
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_s, rdr, sqlmsg) Then
            If rdr.Read Then
                gb_pb_update = True
            Else
                gb_pb_update = False
            End If
        Else
            gb_pb_update = False
        End If
        c.DisposeR()
    End Sub
    Private Sub area_transdate_exist()
        Dim rdr As SqlClient.SqlDataReader = Nothing
        Dim fullname As String = Nothing
        Dim idno As String = Nothing
        Dim username As String = Nothing
        Dim jobtitle As String = Nothing
        '        Dim ls_s As String = "select distinct transdate from cf_area_vismin " & _
        '" where transdate = '" + transdate + "' and class_04 = '" + gs_class04 + "'"
        Dim ls_s As String = "select distinct transdate from cf_area_vismin " & _
" where class_02 = 'Mindanao' and transdate = '" + transdate + "' and class_04 = '" + gs_class04 + "'"
        Dim c As New clsData
        If c.Error_Inititalize_INI Then Exit Sub
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_s, rdr, sqlmsg) Then
            If rdr.Read Then
                gb_area_update = True
            Else
                gb_area_update = False
            End If
        Else
            gb_area_update = False
        End If
        c.DisposeR()
    End Sub
    Private Sub region_transdate_exist()
        Dim rdr As SqlClient.SqlDataReader = Nothing
        Dim fullname As String = Nothing
        Dim idno As String = Nothing
        Dim username As String = Nothing
        Dim jobtitle As String = Nothing
        Dim ls_s As String = "select distinct transdate from cf_region_vismin " & _
" where class_02 = 'Mindanao' and transdate = '" + transdate + "' and class_03 = '" + gs_class03 + "'"
        Dim c As New clsData
        If c.Error_Inititalize_INI Then Exit Sub
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_s, rdr, sqlmsg) Then
            If rdr.Read Then
                gb_region_update = True
            Else
                gb_region_update = False
            End If
        Else
            gb_region_update = False
        End If
        c.DisposeR()
    End Sub
    Private Sub vismin_transdate_exist()
        Dim rdr As SqlClient.SqlDataReader = Nothing
        Dim ls_s As String = "select distinct transdate from cf_vismin_wide " & _
" where class_02 = 'Mindanao' and transdate = '" + transdate + "' "
        Dim c As New clsData
        If c.Error_Inititalize_INI Then Exit Sub
        If c.ErrorConnectionReading(False) Then Exit Sub
        If Not c.Error_SetRdr(ls_s, rdr, sqlmsg) Then
            If rdr.Read Then
                gb_vismin_update = True
            Else
                gb_vismin_update = False
            End If
        Else
            gb_vismin_update = False
        End If
        c.DisposeR()
    End Sub

    Private Sub Tmr_CashFlow_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Tmr_CashFlow.Tick
        calltime()
        Dim alltime As DateTime
        Dim starttime As DateTime


        starttime = Now.ToShortTimeString
        alltime = Now.ToShortTimeString

        If starttime >= "4:00:00 AM" Then
            If alltime >= ls_timestart And alltime <= ls_timeend Then
                Button1_Click(Button1, EventArgs.Empty)
                Button2_Click(Button2, EventArgs.Empty)
                'Button1.PerformClick()
                'Button2.PerformClick()

            End If
        End If

        'If alltime >= ls_timestart And alltime <= ls_timeend Then
        '    Button1.PerformClick()
        'End If

    End Sub
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        select_per_branch()
        select_area()
        select_region()
        Tmr_CashFlow.Start()
        Button2.Enabled = False
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        genwide()
    End Sub
    Private Sub genwide()
        Dim Dt As Date = DtTransdate.Text
        gs_transdate = DtTransdate.Text '---> only use for short cut purposes debugging
        gs_transdatereport_title = Convert.ToDateTime(Dt.AddDays(-1)).ToString("MM-dd-yyyy")

        gs_HO_Email_Info = "Executive Management Report " & gs_transdatereport_title
        Dim genrep As New GenerateReport
        genrep.Show()
    End Sub

    Private Sub DtTransdate_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DtTransdate.ValueChanged

    End Sub
End Class


