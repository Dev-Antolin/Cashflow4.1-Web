﻿<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AreaReport.aspx.vb" Inherits="AreaReport" %>

<%@ Register assembly="CrystalDecisions.Web, Version=10.5.3700.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
<link rel="shortcut icon" href="Images/application.ico"/>
    <title>Area Report</title>
</head>
<body>
    <form id="form1" runat="server">
    <table style="width:100%;">
            <tr>
                <td>
                    <asp:Button ID="Button1" runat="server" Text="Back" />
                </td>
            </tr>
        </table>
    <div align="center">
    
        <CR:CrystalReportSource ID="CrystalReportSource1" runat="server">
            <Report FileName="E:\Back up\Silvher's Vb.net 08\VisMinCashflowVer3\Area.rpt">
            </Report>
        </CR:CrystalReportSource>
        <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
            AutoDataBind="true" DisplayGroupTree="False" 
            ReportSourceID="CrystalReportSource1" HasCrystalLogo="False" 
            HasDrillUpButton="False" HasExportButton="False" HasGotoPageButton="False" 
            HasPageNavigationButtons="False" HasSearchButton="False" 
            HasToggleGroupTreeButton="False" HasViewList="False" 
            HasZoomFactorList="False" />
    
    </div>
    </form>
</body>
</html>
