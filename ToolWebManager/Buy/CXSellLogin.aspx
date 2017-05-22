<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CXSellLogin.aspx.cs" Inherits="ToolWebManager.Buy.CXSellLogin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" style="height: 100%" runat="server">
        <table align="center" style="height: 100%; vertical-align: middle">
            <tr>
                <td> 
                    <asp:Login ID="Login1" runat="server" BackColor="#F7F7DE" BorderColor="#CCCC99" BorderStyle="Solid" BorderWidth="1px" Font-Names="Verdana" Font-Size="10pt" OnAuthenticate="Login1_Authenticate" TitleText="畅行内部销售系统" Height="304px" Width="254px">
                        <TitleTextStyle BackColor="#6B696B" Font-Bold="True" ForeColor="#FFFFFF" />
                    </asp:Login>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
