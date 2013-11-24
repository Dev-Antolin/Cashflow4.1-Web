<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Branch.aspx.vb" Inherits="Branch" %>

<%@ Register assembly="CrystalDecisions.Web, Version=10.5.3700.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<link rel="shortcut icon" href="Images/application.ico"/>
    <title>Per Branch Report</title>
    <style type="text/css">

    div.crystalstyle div {position:absolute; z-index:25}
    .ad9ef97973-740f-47e9-8ff7-66186cfd1aa8-0 {border-color:#000000;border-left-width:0;border-right-width:0;border-top-width:0;border-bottom-width:0;}
	.fc64361137-4d28-45ea-bbe6-3cf845e51f0b-0 {font-size:9pt;color:#000000;font-family:Arial;font-weight:normal;}
	.fc64361137-4d28-45ea-bbe6-3cf845e51f0b-1 {font-size:9pt;color:#000000;font-family:Arial;font-weight:bold;}
	</style>
</head>
<body>
    <form id="form1" runat="server">
    <table style="width:100%;">
            <tr>
                <td>
                    <asp:Button ID="Button1" runat="server" Text="Back" />
    <div align="center">
        <CR:CrystalReportSource ID="CrystalReportSource1" runat="server">
            <Report FileName="E:\Back up\Silvher's Vb.net 08\VisMinCashflowVer3\Branch.rpt">
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
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
