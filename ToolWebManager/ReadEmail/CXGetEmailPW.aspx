<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CXGetEmailPW.aspx.cs" Inherits="ToolWebManager.ReadEmail.CXGetEmailPW" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Label ID="Label1" runat="server" ForeColor="Red" Text="由于生产过程中数据没有同步导致邮箱密码不对，对此感到抱歉，&lt;/br&gt;请通过该页面找回邮箱密码"></asp:Label>
        <br />
        <br />
        邮箱名称:<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="查询" />
        <br />
        <br />
        邮箱密码:<asp:Label ID="lbPW" runat="server"></asp:Label>
    
    </div>
    </form>
</body>
</html>
