<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetUser.aspx.cs" Inherits="ToolWebManager.Free.GetUser" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style type="text/css">
        .auto-style1 {
            font-size: x-large;
        }
        .auto-style2 {
            font-size: xx-large;
        }

        body {
                font-size: 12px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <strong><span class="auto-style2">12306牛牛梦想群福利</span></strong><br />
        <br />
        每天不定期赠送一定数量的12306账号信息<br />
        <br />
        群中人数越多赠送的账号越多，大家帮忙多拉朋友进群：<strong><span class="auto-style1">514896515</span></strong><br />
        <br />
        提取码只能使用一次，提取完成后一定要保存好,<strong><span class="auto-style1">该福利不记名请领到福利的亲们去群里秀一下，谢谢牛牛们</span></strong><br />
        <br />
        <asp:Label ID="Label1" runat="server" Text="输入提取码："></asp:Label>
        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
&nbsp;<asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="提取" />
        <br />
        <br />
        <asp:Label ID="Label2" runat="server" Text="提取信息:"></asp:Label>
        <asp:Label ID="lbInfo" runat="server" Text="无信息" Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" ForeColor="Red"></asp:Label>
    
    </div>
    </form>
</body>
</html>
