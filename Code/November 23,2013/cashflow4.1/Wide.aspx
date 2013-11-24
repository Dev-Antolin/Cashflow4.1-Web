﻿<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Wide.aspx.vb" Inherits="Report" %>

<%@ Register assembly="CrystalDecisions.Web, Version=10.5.3700.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Wide Report</title>
</head>
<body>
    <form id="form1" runat="server">
    <div align="center" style="height: 1136px">
    
        <CR:CrystalReportSource ID="CrystalReportSource1" runat="server">
            <Report FileName="Wide.rpt">
            </Report>
        </CR:CrystalReportSource>
    <table style="width:100%;">
            <tr>
                <td align="left">
                    <asp:Button ID="Button1" runat="server" Text="Back" />
                </td>
            </tr>
        </table>
        <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
            AutoDataBind="true" ReportSourceID="CrystalReportSource1" 
            BorderStyle="None" DisplayGroupTree="False" HasCrystalLogo="False" 
            HasDrillUpButton="False" HasExportButton="False" HasGotoPageButton="False" 
            HasPageNavigationButtons="False" HasSearchButton="False" 
            HasToggleGroupTreeButton="False" HasViewList="False" 
            HasZoomFactorList="False" />
    
    </div>
    <table style="width: 111%;">
        <tr>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
    </table>
    </form>
</body>
</html>
