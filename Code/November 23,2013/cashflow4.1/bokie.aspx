<%@ Page Language="VB" AutoEventWireup="false" CodeFile="bokie.aspx.vb" Inherits="bokie" %>

<%@ Register assembly="CrystalDecisions.Web, Version=10.5.3700.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<link rel="shortcut icon" href="Images/application.ico"/>
    <title>CashFlow v 3.2</title>
    <style type="text/css">

	.adb2ead668-54d3-4535-8878-49b9785f7045-0 {border-color:#000000;border-left-width:0;border-right-width:0;border-top-width:0;border-bottom-width:0;}
	.fcd50a9cc8-9968-4f01-bd85-b37b4dbc59fc-0 {font-size:9pt;color:#000000;font-family:Arial;font-weight:normal;}
	.fcd50a9cc8-9968-4f01-bd85-b37b4dbc59fc-1 {font-size:9pt;color:#000000;font-family:Arial;font-weight:bold;}
	</style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
        AutoDataBind="false" />
    </form>
</body>
</html>
