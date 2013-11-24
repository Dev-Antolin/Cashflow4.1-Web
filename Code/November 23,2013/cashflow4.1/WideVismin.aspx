<%@ Page Language="VB" AutoEventWireup="false" CodeFile="WideVismin.aspx.vb" Inherits="WideVismin" %>

<%@ Register assembly="CrystalDecisions.Web, Version=10.5.3700.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<link rel="shortcut icon" href="Images/application.ico"/>
    <title>CashFlow v4.0 report</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="Button1" runat="server" Text="Button" />
    </div>
    <div align="center">
    
        <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
            AutoDataBind="True" DisplayGroupTree="False" 
            ReportSourceID="CrystalReportSource2" Height="1039px" Width="773px" 
            filename="WideV.rpt" />
    
        <CR:CrystalReportSource ID="CrystalReportSource2" runat="server">
            <report filename="CFRVW.rpt">
            </report>
        </CR:CrystalReportSource>
    
    </div>
    </form>
</body>
</html>
