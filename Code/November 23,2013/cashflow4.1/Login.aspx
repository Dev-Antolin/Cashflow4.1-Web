<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="Login" %>
<%@ Register Assembly="FlashControl" Namespace="Bewise.Web.UI.WebControls" TagPrefix="Bewise" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="aspx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<link rel="shortcut icon" href="Images/application.ico"/>
    <link rel="stylesheet" type="text/css" href="sinorca-screen.css" media="screen" title="Sinorca (screen)" />
    <link rel="stylesheet alternative" type="text/css" href="sinorca-screen-alt.css" media="screen" title="Sinorca (alternative)" />
    <link rel="stylesheet" type="text/css" href="sinorca-print.css" media="print" />

    <script type = "text/javascript" >
    function disableBackButton()
        {
        window.history.forward();
        }
        setTimeout("disableBackButton()", -1500);
    </script>
    <title>Cash Flow v4.1 Login</title>
    </head>
<body onload ="disableBackButton()">
    <form id="form1" runat="server">
    <ajaxToolkit:ToolkitScriptManager runat="Server" ID="ScriptManager1" EnablePageMethods="true" />
    <div id="wrapper">
    <div class="superHeader">
<%--          <div class="rightbelow" 
              style="text-align: left; height: 13px; width: 142px; color: #0000FF;">
          &nbsp;<b>GMO Web Version 1.5</b>
          &nbsp;&nbsp;&nbsp;&nbsp;
          </div>--%>
     </div>
        
    <div id="header">
        <div class="midHeader">
            
                <img alt="M. Lhuillier" src="Images/ReviseML.jpg" />
          
              <div style =" float: right; margin-top: 40px; width: 400px;">
               
                  <asp:TextBox ID="txtUserName" style="margin-right: -5px;" runat="server" MaxLength="15"></asp:TextBox>
                <ajaxToolkit:TextBoxWatermarkExtender ID="txtUserName_TextBoxWatermarkExtender" 
                    runat="server" Enabled="True" TargetControlID="txtUserName" 
                    WatermarkCssClass="watermarked" WatermarkText="Username">
                </ajaxToolkit:TextBoxWatermarkExtender>&nbsp
            
                <asp:TextBox ID="txtPassword" style="margin-right: -5px;" runat="server" TextMode="Password" MaxLength="10"></asp:TextBox>
                <ajaxToolkit:TextBoxWatermarkExtender ID="txtPassword_TextBoxWatermarkExtender" 
                    runat="server" Enabled="True" TargetControlID="txtPassword" 
                    WatermarkCssClass="watermarked" WatermarkText="8037624">
                </ajaxToolkit:TextBoxWatermarkExtender>
                <asp:Button ID="btnLogIn" runat="server" Text="Log In" BackColor="White" 
                    BorderColor="White" BorderStyle="None" Font-Bold="True" Font-Size="Small" 
                    Width="50px" />
             </div>
        </div> 
      <div class="subHeader">
           
            <span class="doNotDisplay">Navigation:</span>
            Cash Flow Version 4.1
            <!-- ##### Display Link ##### -->
             <div style ="float: right; width: 355px">
             <asp:Label ID="lblMsg" runat="server" 
                        style="font-style: italic; color: #FF0000;"></asp:Label>
             </div>
      </div>
    </div>
    </div>
    <div id="Log_menu" style = " text-align: center;">
        <br />
        <br />
        <br />
        <br />
    <br />
  
        <div class =" navbar " style ="height: 400px;">
            <Bewise:FlashControl ID="FlashControl1" runat="server" Height="208px" 
                                MovieUrl="~/Animation/WebLoginAnimation.swf" Width="339px" Loop="True" />
        </div>    
    </div>
  
  <div id="footer">
      <div class="left">
          M. Lhuillier Philippines Inc.c.<a href="" class="doNotPrint"></a>
      </div>

      <br class="doNotDisplay doNotPrint" />

      <div class="right">
        All Rights Reserved. Copyright © 2013
        <br />
        <a href="" class="doNotPrint"></a>
      </div>
    </div>
    </form>
</body>
</html>
