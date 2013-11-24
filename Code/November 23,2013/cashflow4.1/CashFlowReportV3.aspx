<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CashFlowReportV3.aspx.vb" Inherits="CashFlowReportV3" %>

<%@ Register assembly="CrystalDecisions.Web, Version=10.5.3700.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<link rel="shortcut icon" href="Images/application.ico"/>
    <title>Cash Flow v3.1 Report</title>
</head>
<body>
    <form id="form1" runat="server">
    <div align="center">
    
        <asp:Button ID="Button1" runat="server" Text="Button" />
        <CR:CrystalReportSource ID="CrystalReportSource1" runat="server">
        </CR:CrystalReportSource>
        <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
            AutoDataBind="true" DisplayGroupTree="False" 
            ReportSourceID="CrystalReportSource1" />
    
    </div>
    </form>
</body>
</html>
