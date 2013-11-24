<%@ Page Language="VB" MasterPageFile="~/LeftPageMaster.master" AutoEventWireup="false" CodeFile="MainMenu.aspx.vb" Inherits="MainMenu" title="CashFlow v4.1" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>


<asp:Content ID="Content1" ContentPlaceHolderID="LeftContentPlaceHolder" Runat="Server">
    <link rel="shortcut icon" href="Images/application.ico"/>

    <div id="content2">
    <br />
    <table style="width: 100%">
        <tr>
            <td align="right" style="width: 61px; height: 31px;">
                <asp:Label ID="Label1" runat="server" Text="Date"></asp:Label>
            </td>
            <td style="width: 12px; height: 31px;">
                <asp:Label ID="Label7" runat="server" Text=":"></asp:Label>
            </td>
            <td style="height: 31px">
                <asp:TextBox ID="txtStartDate" runat="server" Height="16px" Width="99px" 
                    AutoComplete="off" TabIndex="1" EnableTheming="True" MaxLength="10"></asp:TextBox>
                <asp:FilteredTextBoxExtender ID="txtStartDate_FilteredTextBoxExtender" 
                    runat="server" Enabled="True" FilterType="Custom, Numbers" 
                    TargetControlID="txtStartDate" ValidChars="/-">
                </asp:FilteredTextBoxExtender>
                <asp:Image ID="Image1" runat="server" ImageUrl="images/Calendar.png" 
                    Height="16px" Width="20px" />&nbsp;
                <asp:CalendarExtender ID="txtStartDate_CalendarExtender" runat="server" 
                    Enabled="True" TargetControlID="txtStartDate" PopupPosition ="Left" PopupButtonID = "image1"  >
                </asp:CalendarExtender>
                <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
                </asp:ToolkitScriptManager>
            </td>
        </tr>
        <tr>
            <td align="right" style="width: 61px; height: 31px;">
                <asp:Label ID="Label3" runat="server" Text="Management"></asp:Label>
            </td>
            <td style="width: 12px; height: 31px;">
                <asp:Label ID="Label12" runat="server" Text=":"></asp:Label>
            </td>
             <td style="height: 31px">
            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                        <ContentTemplate>
                <asp:DropDownList ID="dplSummary" runat="server" AppendDataBoundItems="True" 
                                AutoPostBack="True" Width="103px" Height="22px">
                </asp:DropDownList>
                </ContentTemplate>
                    </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td align="right" style="width: 61px; height: 31px;">
                <asp:Label ID="Label2" runat="server" Text="H.O."></asp:Label>
            </td>
            <td style="width: 12px; height: 31px;">
                <asp:Label ID="Label8" runat="server" Text=":"></asp:Label>
            </td>
            <td style="height: 31px">
            <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                        <ContentTemplate>
                <asp:DropDownList ID="dplWide" runat="server" AppendDataBoundItems="True" 
                                AutoPostBack="True" Width="103px" Height="22px">
                </asp:DropDownList>
                </ContentTemplate>
                    </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td align="right" style="width: 61px; height: 31px;">
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="Label9" runat="server" Text="Region"></asp:Label>
            </td>
            <td style="width: 12px; height: 31px;">
                <asp:Label ID="Label4" runat="server" Text=":"></asp:Label>
            </td>
            <td style="height: 31px">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                <asp:DropDownList ID="dplRegion" runat="server" AppendDataBoundItems="True" 
                    AutoPostBack="True" style="height: 22px" Height="22px" Width="265px">
                </asp:DropDownList>
                 </ContentTemplate>
                    </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td align="right" style="width: 61px; height: 31px;">
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;
                <asp:Label ID="Label10" runat="server" Text="Area"></asp:Label>
            </td>
            <td style="width: 12px; height: 31px;">
                <asp:Label ID="Label5" runat="server" Text=":"></asp:Label>
            </td>
            <td style="height: 31px">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                <asp:DropDownList ID="dplArea" runat="server" AppendDataBoundItems="True" 
                    AutoPostBack="True" Height="22px" Width="265px">
                </asp:DropDownList>
                </ContentTemplate>
                    </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td align="right" style="width: 61px; ">
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="Label11" runat="server" Text="Branch"></asp:Label>
            </td>
            <td style="width: 12px; ">
                <asp:Label ID="Label6" runat="server" Text=":"></asp:Label>
            </td>
            <td>
            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                <asp:DropDownList ID="dplBranch" runat="server" AppendDataBoundItems="True" 
                                AutoPostBack="True" Height="22px" Width="265px">
                </asp:DropDownList>
                </ContentTemplate>
                    </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td style="width: 61px">
                &nbsp;</td>
            <td style="width: 12px">
                &nbsp;</td>
            <td>
                <asp:RadioButtonList ID="rbselection" runat="server" Height="47px" 
                    Visible="False">
                    <asp:ListItem Selected="True">Default</asp:ListItem>
                    <asp:ListItem>PDF</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td style="width: 61px; height: 38px;">
                </td>
            <td style="width: 12px; height: 38px;">
                </td>
            <td style="height: 38px">
                <asp:Button ID="btnGenerate" runat="server" Height="34px" Text="Generate" 
                    Width="117px" />
            </td>
        </tr>
        <tr>
            <td style="width: 61px">
                &nbsp;</td>
            <td style="width: 12px">
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
    </table>
</div>
</asp:Content>

