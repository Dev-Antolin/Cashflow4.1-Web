<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CashFlowReport.aspx.vb" Inherits="CashFlowReport" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<%@ Register assembly="CrystalDecisions.Web, Version=10.5.3700.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<link href="sinorca-screen-alt.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" language="javascript">
       
         function pageLoad(sender, args) {
            var sm = Sys.WebForms.PageRequestManager.getInstance();
            if (!sm.get_isInAsyncPostBack()) {
                sm.add_beginRequest(onBeginRequest);
                sm.add_endRequest(onRequestDone);
            }
        }
        
        function onBeginRequest(sender, args) {
            var send = args.get_postBackElement().value;
            if (displayWait(send) == "yes") {
                $find('PleaseWaitPopup').show();
            } 
        }
        
        function onRequestDone() {
             $find('PleaseWaitPopup').hide();
        }
        
        function displayWait(send) {
            switch (send) {
                case "Update":
                    return ("yes");
                    break;
                default:
                    return ("no");
                    break;
            }            
        }
    </script>
<head runat="server">
    <link rel="shortcut icon" href="Images/application.ico"/>
    <title>CashFlow v4.1 Report</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" AsyncPostBackTimeout = "360000">
    </asp:ToolkitScriptManager>
   
    <div>
          <asp:UpdatePanel ID="PleaseWaitPanel" runat="server" RenderMode="Inline">
            <ContentTemplate>
                    <asp:Button ID="Button1" runat="server" Text="Back" />&nbsp&nbsp
                    <asp:Button ID="UpdateBtn" runat="server" Text="Update" style="height: 26px" Onlick = "Update_Click"/>
                </ContentTemplate>
            </asp:UpdatePanel>

    </div>
    
     
    <div align="center">
    
        <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
            AutoDataBind="True" DisplayGroupTree="False" 
            ReportSourceID="CrystalReportSource2" Height="1039px" Width="773px" 
            filename="WideV.rpt" HasCrystalLogo="False" HasDrillUpButton="False" 
            HasExportButton="False" HasGotoPageButton="False" 
            HasPageNavigationButtons="False" HasSearchButton="False" 
            HasToggleGroupTreeButton="False" HasViewList="False" 
            HasZoomFactorList="False" />
    
        <CR:CrystalReportSource ID="CrystalReportSource2" runat="server">
            <report filename="CFRVW.rpt">
            </report>
        </CR:CrystalReportSource>
    
    </div>
    
    <asp:Panel ID="PleaseWaitMessagePanel" runat="server" CssClass="modalPopup" Height="150px"
            Width="400px">
            <div style=" font-size:large; margin-top: 30px; ">One Moment Please....</div>
            <div style =" margin-top: 20px;"><img src="image/ajax-loader.gif" alt="Please wait" 
                    style="height: 21px; width: 20px" /></div>
            </asp:Panel>
     <asp:Button ID="HiddenButton" runat="server" CssClass="hidden" Text="Hidden Button"
            ToolTip="Necessary for Modal Popup Extender" />
        <asp:ModalPopupExtender ID="PleaseWaitPopup" BehaviorID="PleaseWaitPopup"
            runat="server" TargetControlID="HiddenButton" PopupControlID="PleaseWaitMessagePanel"
            BackgroundCssClass="modalBackground">
        </asp:ModalPopupExtender>
   
    </form>
   
     
   
   
    
  
    
</body>
</html>
