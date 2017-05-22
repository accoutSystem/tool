<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="testcache.aspx.cs" Inherits="CaptchaServerCache.testcache" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
        <asp:Button ID="Button1" runat="server" Text="Set Cache" OnClick="Button1_Click" /><asp:Button ID="Button2" runat="server" Text="Read Cache" OnClick="Button2_Click" />
    </div>
    </form>
</body>
</html>
